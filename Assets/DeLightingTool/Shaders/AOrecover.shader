Shader "Hidden/Delighter/AOrecover"
{
    CGINCLUDE
            
        #include "UnityCG.cginc"
        #pragma target 5.0

        sampler2D _BaseColor;
        sampler2D _AO;
        float _AOrecover;


		float getLuminance(float3 c)
		{
			return (c.x * 0.212655 + c.y * 0.715158 + c.z * 0.072187);
		}

        float4 frag(v2f_img i) : SV_Target
        {
            float4 BaseColor = tex2D(_BaseColor, i.uv);
			BaseColor.rgb = LinearToGammaSpace(BaseColor);
            float AO = tex2D(_AO, i.uv).x;
			float correctedAO = saturate(getLuminance(BaseColor.rgb) / max(0.0001, _AOrecover));
			correctedAO *= AO;

            return correctedAO;
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
