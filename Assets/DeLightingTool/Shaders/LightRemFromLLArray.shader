Shader "Hidden/Delighter/LightRemFromLLArray"
{
    CGINCLUDE
            
        #include "UnityCG.cginc"
        #pragma target 5.0


		StructuredBuffer<uint4> _LatLongArray;
		uint2 _LLOutputSize;

        sampler2D _BaseText;		
		sampler2D _AO;
        sampler2D _NM;
		sampler2D _Mask;
		sampler2D _Position;

        float _Blur;
		float _ForceLocalDL;

        int _Width;
        int _Height;
        int _SwitchAxes;
		int _ValidMask;
		int _ValidPosition;


		float4 DecLLBuf(StructuredBuffer<uint4> LLBuffer, uint2 OutputSize, int gridRes,float2 UV, uint3 clusterPos, int clusterOffset)
		{

			uint ClusterSize = OutputSize.x * OutputSize.y;
			uint ClusterIndex = clusterPos.x + (clusterPos.y * gridRes) + (clusterPos.z * gridRes * gridRes);
			ClusterIndex += clusterOffset;

			uint2 pos = floor(UV * OutputSize);
			uint bufferIndex = pos.y * OutputSize.x + pos.x;
			bufferIndex += ClusterIndex * ClusterSize;


			uint4 tempBuffer = LLBuffer[bufferIndex];
			return (float4(tempBuffer)/255);
		}


		float4 voxelSampling(float3 positionNorm, uint gridRes, float2 LLCoorNM, float4 defaultColor)
		{
			float3 position = positionNorm * gridRes;

			float3 ClusterPos01 = floor(position);

			float3 ClusterOffset = sign(position - (ClusterPos01 + 0.5));

			float3 ClusterPos02 = ClusterPos01 + float3(ClusterOffset.x, 0, 0);
			ClusterPos02.x = clamp(ClusterPos02.x, 0, gridRes - 1);

			float3 ClusterPos03 = ClusterPos01 + float3(0, ClusterOffset.y, 0);
			ClusterPos03.y = clamp(ClusterPos03.y, 0, gridRes - 1);

			float3 ClusterPos04 = float3(ClusterPos02.x, ClusterPos03.y, ClusterPos01.z);

			float3 ClusterPos05 = ClusterPos01 + float3(0, 0, ClusterOffset.z);
			ClusterPos05.z = clamp(ClusterPos05.z, 0, gridRes - 1);

			float3 ClusterPos06 = float3(ClusterPos02.x, ClusterPos05.y, ClusterPos05.z);
			float3 ClusterPos07 = float3(ClusterPos05.x, ClusterPos03.y, ClusterPos05.z);
			float3 ClusterPos08 = float3(ClusterPos02.x, ClusterPos03.y, ClusterPos05.z);


			float dist01 = length(position - (ClusterPos01 + 0.5));
			float dist02 = length(position - (ClusterPos02 + 0.5));
			float dist03 = length(position - (ClusterPos03 + 0.5));
			float dist04 = length(position - (ClusterPos04 + 0.5));
			float dist05 = length(position - (ClusterPos05 + 0.5));
			float dist06 = length(position - (ClusterPos06 + 0.5));
			float dist07 = length(position - (ClusterPos07 + 0.5));
			float dist08 = length(position - (ClusterPos08 + 0.5));

			float allDist = dist01 + dist02 + dist03 + dist04 + dist05 + dist06 + dist07 + dist08;

			float weight01 = 1 - (dist01 / allDist);
			float weight02 = 1 - (dist02 / allDist);
			float weight03 = 1 - (dist03 / allDist);
			float weight04 = 1 - (dist04 / allDist);
			float weight05 = 1 - (dist05 / allDist);
			float weight06 = 1 - (dist06 / allDist);
			float weight07 = 1 - (dist07 / allDist);
			float weight08 = 1 - (dist08 / allDist);

			uint offset;

			if (gridRes == 1) offset = 0 ;
			if (gridRes == 2) offset = 1;
			if (gridRes == 4) offset = 1 + (2 * 2 * 2);
			if (gridRes == 8) offset = 1 + (2 * 2 * 2) + (4 * 4 * 4);
			if (gridRes == 16) offset = 1 + (2 * 2 * 2) + (4 * 4 * 4) + (8 * 8 * 8);


			float4 sample01 = DecLLBuf(_LatLongArray, _LLOutputSize, gridRes, LLCoorNM, (uint3)ClusterPos01, offset);
			float4 sample02 = DecLLBuf(_LatLongArray, _LLOutputSize, gridRes, LLCoorNM, (uint3)ClusterPos02, offset);
			float4 sample03 = DecLLBuf(_LatLongArray, _LLOutputSize, gridRes, LLCoorNM, (uint3)ClusterPos03, offset);
			float4 sample04 = DecLLBuf(_LatLongArray, _LLOutputSize, gridRes, LLCoorNM, (uint3)ClusterPos04, offset);
			float4 sample05 = DecLLBuf(_LatLongArray, _LLOutputSize, gridRes, LLCoorNM, (uint3)ClusterPos05, offset);
			float4 sample06 = DecLLBuf(_LatLongArray, _LLOutputSize, gridRes, LLCoorNM, (uint3)ClusterPos06, offset);
			float4 sample07 = DecLLBuf(_LatLongArray, _LLOutputSize, gridRes, LLCoorNM, (uint3)ClusterPos07, offset);
			float4 sample08 = DecLLBuf(_LatLongArray, _LLOutputSize, gridRes, LLCoorNM, (uint3)ClusterPos08, offset);


			float4 samples =    sample01 * weight01 +
								sample02 * weight02 +
								sample03 * weight03 +
								sample04 * weight04 +
								sample05 * weight05 +
								sample06 * weight06 +
								sample07 * weight07 +
								sample08 * weight08 + 
								defaultColor * 0.05;
				

			return float4(samples.rgb / samples.a,1);

		}


        float3 NormalFromNM(float3 nm)
        {
            return normalize(nm * 2.0 - 1.0);
        }

        float2 LLCoordFromVector(float3 v)
        {
            float Y = acos(v.z) / UNITY_PI;
            float X = atan2(v.y, v.x) / (2.0 * UNITY_PI);
            return float2(X + 0.5, saturate(1.0 - Y));
        }

        float4 frag(v2f_img i) : SV_Target
        {

            float4 BaseColor = tex2D(_BaseText,i.uv);
            float4 AO = saturate(tex2D(_AO, i.uv));
			float4 Mask = (_ValidMask == 1) ? tex2D(_Mask, i.uv) : float4(0, 0, 0, 0);
			float3 positionNorm = (_ValidPosition == 1) ? tex2D(_Position, i.uv).rgb : float3(i.uv.x, i.uv.y, 0);
			float4 NM = tex2D(_NM, i.uv);
			if (_SwitchAxes > 0)
			{
				NM = float4(NM.x, 1 - NM.z, NM.y, NM.w);
			}
			float3 Normal = NormalFromNM(NM.xyz);
			float2 finalLLCoords = LLCoordFromVector(Normal); // Compute LatLong coords from the normal vector

			//------------------------------------------------------------------------------------------


			float mipComp = (pow(1 - (AO),0.5)) * 4; // Use AO to interp between 2 LatLongArray mipmap
			mipComp += (Mask.g + _ForceLocalDL) * 4;// Use mask.g value to shift the LatLongArray mipmap.
			mipComp = min(4.0001, mipComp); // Force maximum Mip to be 4

			uint mipLevel = floor(mipComp); // First Mip Level 
			uint mipLevelUp = min(4, mipLevel + 1); // Upper Mip Level

			float4 LLcolor0 = DecLLBuf(_LatLongArray, _LLOutputSize, 0, finalLLCoords, uint3(0,0,0), 0);
			LLcolor0 = float4(LLcolor0.rgb / LLcolor0.a, 1); // Default color for missing areas

			float4 LLcolor1 = voxelSampling(positionNorm, pow(2,mipLevel), finalLLCoords, LLcolor0); // First mip level color
			float4 LLcolor2 = voxelSampling(positionNorm, pow(2, mipLevelUp), finalLLCoords, LLcolor0); // Second mip level color		

			float4 LLcolor = lerp(LLcolor1, LLcolor2, frac(mipComp)) ; // Interpolation between the 2 LatLongArray mipmap
			LLcolor.a = 1;
			
			LLcolor = max(0.05,LLcolor);
			LLcolor.rgb = GammaToLinearSpace(LLcolor.rgb);
			


			float4 finalColor = (1 / LLcolor) * BaseColor; // inverse the lighting and apply it to the base color
			finalColor = lerp(BaseColor, finalColor, BaseColor.a);
            finalColor.a = BaseColor.a;

			return finalColor;



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
