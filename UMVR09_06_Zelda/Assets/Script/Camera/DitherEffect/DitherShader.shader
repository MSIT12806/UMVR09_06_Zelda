Shader "Ron/DitherShader"
{
  //show values to edit in inspector
  Properties{
    _Color ("Tint", Color) = (0, 0, 0, 1)
    _MainTex ("Texture", 2D) = "white" {}
    //Shader Property
    _DitherPattern ("Dithering Pattern", 2D) = "white" {}
  }

  SubShader{
    //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
    Tags{ "RenderType"="Opaque" "Queue"="Geometry" }

    Pass{
      CGPROGRAM

      //include useful shader functions
      #include "UnityCG.cginc"

      //define vertex and fragment shader functions
      #pragma vertex vert
      #pragma fragment frag

      //texture and transforms of the texture
      sampler2D _MainTex;
      float4 _MainTex_ST;

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
      };

      //the vertex shader function
      v2f vert(appdata v){
        v2f o;
        //convert the vertex positions from object space to clip space so they can be rendered correctly
        o.position = UnityObjectToClipPos(v.vertex);
        //apply the texture transforms to the UV coordinates and pass them to the v2f struct
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        return o;
      }

      //the fragment shader function
      fixed4 frag(v2f i) : SV_TARGET{
    //texture value the dithering is based on
    float texColor = tex2D(_MainTex, i.uv).r;

    //value from the dither pattern
    float2 screenPos = i.screenPosition.xy / i.screenPosition.w;
    float2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
    float ditherValue = tex2D(_DitherPattern, ditherCoordinate).r;

    //combine dither pattern with texture value to get final result
    float col = step(ditherValue, texColor);
    return col;
      }

      ENDCG
    }
  }
  Fallback "VertexLit"
}
