Shader "Hidden/Delighter/LatLongNorm"
{
    CGINCLUDE
            
        #include "UnityCG.cginc"
        #pragma target 5.0
        
        StructuredBuffer<uint4> _LatLong;
        uint2 _LLOutputSize;


        float4 DecLLBuf(StructuredBuffer<uint4> LLBuffer, uint2 OutputSize, float2 UV, uint offset)
        {
            uint2 pos = floor(UV * OutputSize);
			uint bufferIndex = (pos.y * OutputSize.x + pos.x) + offset;
            uint4 tempBuffer = LLBuffer[bufferIndex];
            return float4(tempBuffer);
        }

        float4 NormAlpha(float4 Col)
        {
            float4 tempCol = Col / 255.0;

            if (tempCol.a > 1)
            {
                tempCol /= tempCol.a;
                tempCol.a = 1.0;
            }
            else
            {
                tempCol = float4(0, 0, 0, 0);
            }

            return float4(tempCol);
        }

        float4 frag(v2f_img i) : SV_Target
        {
            float4 LLcolor = DecLLBuf(_LatLong, _LLOutputSize, i.uv, 0);
            LLcolor = NormAlpha(LLcolor);
			//LLcolor = float4(i.uv.x, i.uv.y,0,1);
            return LLcolor;
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
