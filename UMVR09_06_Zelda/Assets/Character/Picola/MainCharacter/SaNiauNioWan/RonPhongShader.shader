Shader "Ron/PhongShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    //�B�z�������V
    SubShader
    {
        // No culling or depth
        //Cull Off ZWrite Off ZTest Always
        Tags { "LightMode" = "ForwardBase" }//�����
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // �V�e��V?
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"

            //��_LightColor0 �Ѽ�(�O�����)
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

                float3 worldNormal:TEXCOORD1; //uv�y��
                float3 worldPos:TEXCOORD2;  //�@�ɮy��
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
                //col.rgb*= _LightColor0.rgb;//���աG���W�����C��A�վ����|����ܡC
                fixed3 lightStrength = _LightColor0.rgb;

                //�p������J�g��V
                #ifndef USING_DIRECTIONAL_LIGHT
                  fixed3 lightDirection = normalize(UnityWorldSpaceLightDir(i.worldPos));
                #else
                  fixed3 lightDirection = _WorldSpaceLightPos0.xyz;
                #endif
                //col.rgb = lightDirection;//���աG�����J�g��V�A�վ�����J�g���׷|���C��C
                fixed3 diff =col.rgb * lightStrength * (max (0, dot (i.worldNormal, lightDirection))); //�p�⺩�Ϯg = �����C�� * ���j * cos�J�g��

                //�p�Ⱚ��
  float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz -  i.worldPos.xyz);
  float3 H = (lightDirection+worldViewDir)/2 ;
  fixed3 spec = lightStrength *pow (max (0, dot (H, i.worldNormal)),20)*1.5;


                //���ҥ��¦�A�аѦҤU���{���X         
                //col.rgb = unity_AmbientSky.rgb;//���աG���ҥ��ѼƤޥ� //unity_AmbientSky, unity_AmbientEquator, unity_AmbientGround
                float ratio = dot(i.worldNormal,float3 (0,1,0)); //��Ѫ�(���W��)���@�Ӥ�ȡA�V���V�a�O�A�N�Ϯg�a�O�C��F�V���V�ѪŴN�Ϯg�Ѫ��C��C
                fixed3 amb = lerp(unity_AmbientGround.rgb, unity_AmbientSky.rgb, ratio);
                //col.rgb = spec; //���աG���� scene view �n�� camera �b�@�_!!


                col.rgb = amb +diff+spec; //�[�W���Ϯg

  //�p�G�n��skybox�A�аѦҤU���{���X
   //col.rgb = diff; //�[�W���Ϯg
  // Setup lighting environment
  //float3 worldViewDir = normalize(UnityWorldSpaceLightDir(i.worldPos));�{���X�W��
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
   
   
   col.rgb += albedo.rgb * ogi.indirect.diffuse; //�Цb�e���ŧitexture albedo�C

   //����


                return col;
            }
            ENDCG
        }
    }
      FallBack "Diffuse"  //�v�l
}
