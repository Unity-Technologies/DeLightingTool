Shader "Hidden/Delighter/Preview"
{
    CGINCLUDE
            
        #include "UnityCG.cginc"
        #pragma target 5.0

        sampler2D _ViewLeft;
        sampler2D _ViewRight;
        float _ComparePos;
        float _AlphaDisplayLeft;
        float _AlphaDisplayRight;

        float4 frag(v2f_img i) : SV_Target
        {
            float4 viewLeft = tex2D(_ViewLeft, i.uv);
            float4 viewRight = tex2D(_ViewRight, i.uv);

            viewLeft = lerp(viewLeft, float4(viewLeft.aaa, 1.0), _AlphaDisplayLeft);
            viewRight = lerp(viewRight, float4(viewRight.aaa, 1.0), _AlphaDisplayRight);

            float pos = saturate(sign(i.uv.x - _ComparePos));
            float4 compareResult = lerp(viewLeft, viewRight, pos);

            return float4(compareResult.xyz, 1.0);
        }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
                #pragma vertex vert_img
                #pragma fragment frag
            ENDCG
        }
    }
}
