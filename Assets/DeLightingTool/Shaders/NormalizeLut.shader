Shader "Hidden/Delighter/NormalizeLut"
{
	CGINCLUDE

#include "UnityCG.cginc"
#pragma target 5.0

	StructuredBuffer<uint4> _LutBuffer;
	uint2 _LutOutputSize;


	float4 DecLutBuf(StructuredBuffer<uint4> LutBuffer, uint2 OutputSize, float2 UV)
	{
		uint2 pos = floor(UV * (OutputSize - 1) + float2(0.0, 0.0)) ;
		uint4 tempBuffer = LutBuffer[pos.y * OutputSize.x + pos.x + (64*64*0)];
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
		
		float4 LutColor = DecLutBuf(_LutBuffer, _LutOutputSize, i.uv);
		LutColor = NormAlpha(LutColor);
		
		return LutColor;
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
