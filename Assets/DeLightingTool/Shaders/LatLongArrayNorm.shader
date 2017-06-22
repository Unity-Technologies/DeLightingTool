Shader "Hidden/Delighter/LatLongArrayNorm"
{
    CGINCLUDE
            
        #include "UnityCG.cginc"
        #pragma target 5.0
        
        StructuredBuffer<uint4> _LatLong;
        uint3 _LLOutputSize;


        float4 DecLLBuf(StructuredBuffer<uint4> LLBuffer, uint3 OutputSize, float2 UV, uint3 clusterPos)
        {
			uint ClusterSize = OutputSize.x * OutputSize.y;
			uint ClusterIndex = clusterPos.x + (clusterPos.y * OutputSize.z)+ (clusterPos.z * OutputSize.z * OutputSize.z);
			ClusterIndex += 1; //shift 1 slice. Slice 0 will be the global LL
			//ClusterIndex = 50;

            uint2 pos = floor(UV * OutputSize);
			uint bufferIndex = pos.y * OutputSize.x + pos.x;
			bufferIndex += ClusterIndex * ClusterSize;
			

            uint4 tempBuffer = LLBuffer[bufferIndex];
            return float4(tempBuffer);
        }

        float4 NormAlpha(float4 Col)
        {
            float4 tempCol = Col / 255.0;

			
            if (tempCol.a > 0)
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
			uint3 ClusterPos = uint3(2,2,4);

			float4 LLcolor = DecLLBuf(_LatLong, _LLOutputSize, i.uv, ClusterPos);
			LLcolor = NormAlpha(LLcolor);


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
