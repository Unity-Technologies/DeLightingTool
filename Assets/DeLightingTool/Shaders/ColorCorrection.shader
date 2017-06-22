Shader "Hidden/Delighter/ColorCorrection"
{
	CGINCLUDE

#include "UnityCG.cginc"
#pragma target 5.0

	sampler2D _BaseText;
	sampler2D _colorText;
	sampler2D _latLong;
	sampler2D _SourceNM;
	float _ShowDir;
	float4 _SafetyZoneParams;
	int _UseSaftyZone;
	float _RemoveHighlights;
	float _RemoveDarkNoise;

	StructuredBuffer<uint4> _AverageColor;

	float3 NormalFromNM(float3 nm)
	{
		return nm * 2.0 - 1.0;
	}

	float2 LLCoordFromVector(float3 v)
	{
		float Y = acos(v.z) / UNITY_PI;
		float X = atan2(v.y, v.x) / (2.0 * UNITY_PI);
		return float2(X + 0.5, saturate(1.0 - Y));
	}

	float getLuminance(float3 c)
	{
		return (c.x * 0.212655 + c.y * 0.715158 + c.z * 0.072187);
	}


	float4 frag(v2f_img i) : SV_Target
	{

		float HighlightsRadius = _RemoveHighlights * 8;
		float DarkNoiseRadius = _RemoveDarkNoise * 8;

		float4 UnlitText = tex2D(_colorText, i.uv);
		float4 UnlitTextLOD_Highlights = tex2Dlod(_colorText, float4(i.uv.x, i.uv.y, 0, HighlightsRadius));
		float4 UnlitTextLOD_Highlights2 = tex2Dlod(_colorText, float4(i.uv.x, i.uv.y, 0, HighlightsRadius + 1));
		float4 UnlitTextLOD_Darknoise = tex2Dlod(_colorText, float4(i.uv.x, i.uv.y, 0, DarkNoiseRadius));
		float4 UnlitTextLOD_Darknoise2 = tex2Dlod(_colorText, float4(i.uv.x, i.uv.y, 0, DarkNoiseRadius + 1));
	

		//---------------------------------------------------------
		// Color Correction => White Balance + Exposure
		//---------------------------------------------------------


		float LOD_Size = clamp(pow(2, _SafetyZoneParams.z / 50), 0, 8);
		float2 SafetyZoneCoords = float2(_SafetyZoneParams.x, _SafetyZoneParams.y);


		float3 WhiteLightRef_Dir = tex2Dlod(_SourceNM, float4(SafetyZoneCoords.x, SafetyZoneCoords.y, 0, LOD_Size)).xyz;
		WhiteLightRef_Dir = NormalFromNM(WhiteLightRef_Dir);

		if (_SafetyZoneParams.w > 0.5)
		{
			WhiteLightRef_Dir = WhiteLightRef_Dir.rbg * float3(1, -1, 1);
		}

		float2 WhiteLightRef_Coords = LLCoordFromVector(WhiteLightRef_Dir);
		float4 refColor = tex2D(_latLong, WhiteLightRef_Coords);

		float4 AverageColor = float4(_AverageColor[0]);
		AverageColor /= AverageColor.a;
		AverageColor.a = 1;

		refColor = lerp(AverageColor, refColor, _UseSaftyZone);
		refColor.rgb = GammaToLinearSpace(refColor.rgb);// => Gamma

		float4 ColorCorrect = UnlitText * refColor;

		float4 ColorCorrectLOD_Highlights = saturate(UnlitTextLOD_Highlights * refColor);
		float4 ColorCorrectLOD_Highlights2 = saturate(UnlitTextLOD_Highlights2 * refColor);

		float4 ColorCorrectLOD_DarkNoise = saturate(UnlitTextLOD_Darknoise * refColor);
		float4 ColorCorrectLOD_DarkNoise2 = saturate(UnlitTextLOD_Darknoise2 * refColor);
	

		//---------------------------------------------------------
		// Test Highlights And Dark Noise
		//---------------------------------------------------------

		float4 BaseText = tex2D(_BaseText, i.uv);
		float4 BaseTextLOD_Highlights = tex2Dlod(_BaseText, float4(i.uv.x, i.uv.y, 0, HighlightsRadius));
		float4 BaseTextLOD_DarkNoise = tex2Dlod(_BaseText, float4(i.uv.x, i.uv.y, 0, DarkNoiseRadius));
		
		float UnlitVariance_Highlights = getLuminance(ColorCorrect - ColorCorrectLOD_Highlights);
		float UnlitVariance_DarkNoise = getLuminance(ColorCorrect - ColorCorrectLOD_DarkNoise);

		float4 RecoveredLightIntensity_Hightlights = ColorCorrectLOD_Highlights2 / max(BaseTextLOD_Highlights,0.01);
		RecoveredLightIntensity_Hightlights = (RecoveredLightIntensity_Hightlights);
		float4 RecoveredLightIntensity_DarkNoise = ColorCorrectLOD_DarkNoise2 / max(BaseTextLOD_DarkNoise, 0.01);
		RecoveredLightIntensity_DarkNoise = (RecoveredLightIntensity_DarkNoise);
		
		float4 PatchColor_Hightlights = saturate(BaseText * RecoveredLightIntensity_Hightlights);
		float4 PatchColor_DarkNoise = saturate(BaseText * RecoveredLightIntensity_DarkNoise);
		
		float Unlit_Highlights = saturate(UnlitVariance_Highlights * 5);
		float Unlit_DarkNoise = saturate(UnlitVariance_DarkNoise * -5);

		float4 CleanedColor = lerp(ColorCorrect, PatchColor_Hightlights, Unlit_Highlights * BaseText.a * saturate(ColorCorrectLOD_Highlights.a * 4 - 3)); // Apply the correction mainly in Highlights
		CleanedColor = saturate(CleanedColor);
		CleanedColor = lerp(CleanedColor, PatchColor_DarkNoise , Unlit_DarkNoise * BaseText.a  * saturate(ColorCorrectLOD_DarkNoise.a * 4 - 3)); // Apply the correction mainly in Highlights

		
		
		//---------------------------------------------------------

		CleanedColor = lerp(BaseText, CleanedColor, BaseText.a);
		return float4(CleanedColor.rgb, BaseText.a);

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
