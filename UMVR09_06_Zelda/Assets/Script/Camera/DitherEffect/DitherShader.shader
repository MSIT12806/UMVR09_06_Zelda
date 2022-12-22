Shader "Ron/DitherShader"
{
  //show values to edit in inspector
  Properties{
    _Color ("Tint", Color) = (0, 0, 0, 1)
    _MainTex ("Texture", 2D) = "white" {}
    //Shader Property
    _DitherPattern ("Dithering Pattern", 2D) = "white" {}
        _MinDistance ("Minimum Fade Distance", Float) = 0
  }

  SubShader{
    //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
    Tags{ "RenderType"="Transparent" "Queue"="Transparent" }

    Pass{
      CGPROGRAM

      //include useful shader functions
      #include "UnityCG.cginc"
                  //取_LightColor0 參數(燈光原色)
            #include "Lighting.cginc"  
      //define vertex and fragment shader functions
      #pragma vertex vert
      #pragma fragment frag

      //texture and transforms of the texture
      sampler2D _MainTex;
      float4 _MainTex_ST;
        float _MinDistance;

      //tint of the texture
      fixed4 _Color;

      //the mesh data thats read by the vertex shader
      struct appdata{
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
      };
      //The dithering pattern
      sampler2D _DitherPattern;
      float4 _DitherPattern_TexelSize;

      //the data thats passed from the vertex to the fragment shader and interpolated by the rasterizer
      struct v2f{
        float4 position : SV_POSITION;
        float2 uv : TEXCOORD0;
        float4 screenPosition : TEXCOORD1;

                float3 worldNormal:TEXCOORD2; //uv座標
                float3 worldPos:TEXCOORD3;  //世界座標
      };

      //the vertex shader function
      v2f vert(appdata v){
        v2f o;
        //convert the vertex positions from object space to clip space so they can be rendered correctly
        o.position = UnityObjectToClipPos(v.vertex);
        //apply the texture transforms to the UV coordinates and pass them to the v2f struct
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        o.screenPosition = ComputeScreenPos(o.position);
        return o;
      }

      //the fragment shader function
      fixed4 frag(v2f i) : SV_TARGET{
        //texture value the dithering is based on
        float4 texColor = tex2D(_MainTex, i.uv);
        float4 col = texColor;
                col.rgb*= _LightColor0.rgb;//測試：乘上光源顏色，調整光色會跟著變。
       
        //value from the dither pattern
        float2 screenPos = i.screenPosition.xy / i.screenPosition.w;
        float2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
        float ditherValue = tex2D(_DitherPattern, ditherCoordinate).r;
  //  return ditherValue;
        //combine dither pattern with texture value to get final result
            clip(_MinDistance - ditherValue.r);

        return col;
          }

      ENDCG
    }
  }
  Fallback "VertexLit"
}
