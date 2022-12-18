Shader "Custom/RadialBlurShader1"
{
    Properties
    {
        _MainTex("Base(RGB)",2D) = "white"{}
    }

    CGINCLUDE
    uniform sampler2D _MainTex;
    uniform float _BlurFactor; //模糊強度(0-0.05)
    uniform float4 _BlurCenter; //模糊中心點xy值(0-1)螢幕空間
    #include "UnityCG.cginc"
    #define SAMPLE_COUNT 6 

    fixed4 frag(v2f_img i):SV_Target
    {
        //模糊方向為模糊中點指向邊緣，越邊力道越大越模糊
        float2 dir = i.uv - _BlurCenter.xy;
        float4 outColor = 0;
        //採樣SAMPLE_COUNT次
        for(int j = 0;j<SAMPLE_COUNT;++j)
        {
            //計算採樣uv值：正常uv值+從中間向邊緣增加的採樣距離
            float2 uv = i.uv+_BlurFactor*dir*j;
            outColor += tex2D(_MainTex,uv);
        }
        //取平均值
        outColor /= SAMPLE_COUNT;
        return outColor;
    }
    ENDCG

    SubShader
    {
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off
            Fog{ Mode off}

            //調用CG函數
            CGPROGRAM
            //使效率更高的編譯宏
            #pragma fragmentoption ARB_precision_hint_fastest

            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
    Fallback off
}
