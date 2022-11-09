Shader "Ron/PhongShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    //處理平行光渲染
    SubShader
    {
        // No culling or depth
        //Cull Off ZWrite Off ZTest Always
        Tags { "LightMode" = "ForwardBase" }//平行光
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // 向前渲染?
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"

            //取_LightColor0 參數(燈光原色)
            #include "Lighting.cginc"  
            //#include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                float3 normal:NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

                float3 worldNormal:TEXCOORD1; //uv座標
                float3 worldPos:TEXCOORD2;  //世界座標
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                //use phong shaderring... I = IaKa + IiKd(L.N) + IiKs(R.V)n
                fixed4 col = tex2D(_MainTex, i.uv);
                //col.rgb*= _LightColor0.rgb;//測試：乘上光源顏色，調整光色會跟著變。
                fixed3 lightStrength = _LightColor0.rgb;

                //計算光源入射方向
                #ifndef USING_DIRECTIONAL_LIGHT
                  fixed3 lightDirection = normalize(UnityWorldSpaceLightDir(i.worldPos));
                #else
                  fixed3 lightDirection = _WorldSpaceLightPos0.xyz;
                #endif
                //col.rgb = lightDirection;//測試：光源入射方向，調整光源入射角度會變顏色。
                fixed3 diff =col.rgb * lightStrength * (max (0, dot (i.worldNormal, lightDirection))); //計算漫反射 = 材質顏色 * 光強 * cos入射角

                //計算高光
  float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz -  i.worldPos.xyz);
  float3 H = (lightDirection+worldViewDir)/2 ;
  fixed3 spec = lightStrength *pow (max (0, dot (H, i.worldNormal)),20)*1.5;


                //環境光純色，請參考下面程式碼         
                //col.rgb = unity_AmbientSky.rgb;//測試：環境光參數引用 //unity_AmbientSky, unity_AmbientEquator, unity_AmbientGround
                float ratio = dot(i.worldNormal,float3 (0,1,0)); //跟天空(正上方)做一個比值，越面向地板，就反射地板顏色；越面向天空就反射天空顏色。
                fixed3 amb = lerp(unity_AmbientGround.rgb, unity_AmbientSky.rgb, ratio);
                //col.rgb = spec; //測試：高光 scene view 要跟 camera 在一起!!


                col.rgb = amb +diff+spec; //加上漫反射

  //如果要用skybox，請參考下面程式碼
   //col.rgb = diff; //加上漫反射
  // Setup lighting environment
  //float3 worldViewDir = normalize(UnityWorldSpaceLightDir(i.worldPos));程式碼上提
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  //gi.indirect.diffuse = 0;
  //gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDirection;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = i.worldPos;
  giInput.worldViewDir = worldViewDir;
  //giInput.atten = atten; //origin
  giInput.atten = 1; //edit
  //#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
  //  giInput.lightmapUV = IN.lmap;
  //#else
    giInput.lightmapUV = 0.0;
  //#endif
  //#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
  //  giInput.ambient = IN.sh;
  //#else
    giInput.ambient.rgb = 0.0;
  //#endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
   UnityGI ogi = UnityGlobalIllumination (giInput, 1.0, i.worldNormal);
   fixed4 albedo = tex2D(_MainTex, i.uv);
   
   
   col.rgb += albedo.rgb * ogi.indirect.diffuse; //請在前面宣告texture albedo。

   //高光


                return col;
            }
            ENDCG
        }
    }
      FallBack "Diffuse"  //影子
}
