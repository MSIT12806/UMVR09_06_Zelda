Shader "Custom/RadialBlurShader1"
{
    Properties
    {
        _MainTex("Base(RGB)",2D) = "white"{}
    }

    CGINCLUDE
    uniform sampler2D _MainTex;
    uniform float _BlurFactor; //�ҽk�j��(0-0.05)
    uniform float4 _BlurCenter; //�ҽk�����Ixy��(0-1)�ù��Ŷ�
    #include "UnityCG.cginc"
    #define SAMPLE_COUNT 6 

    fixed4 frag(v2f_img i):SV_Target
    {
        //�ҽk��V���ҽk���I���V��t�A�V��O�D�V�j�V�ҽk
        float2 dir = i.uv - _BlurCenter.xy;
        float4 outColor = 0;
        //�ļ�SAMPLE_COUNT��
        for(int j = 0;j<SAMPLE_COUNT;++j)
        {
            //�p��ļ�uv�ȡG���`uv��+�q�����V��t�W�[���ļ˶Z��
            float2 uv = i.uv+_BlurFactor*dir*j;
            outColor += tex2D(_MainTex,uv);
        }
        //��������
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

            //�ե�CG���
            CGPROGRAM
            //�ϮĲv�󰪪��sĶ��
            #pragma fragmentoption ARB_precision_hint_fastest

            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
    Fallback off
}
