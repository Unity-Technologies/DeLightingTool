Shader "Hidden/Delighter/LatLongPreview"
{
	CGINCLUDE

#include "UnityCG.cginc"
#pragma target 5.0


	sampler2D _latLong;
	sampler2D _SourceNM;
	float _Exposure;
	float _Export;
	float4 _SafetyZoneParams;
	int _UseSaftyZone;
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


	float4 frag(v2f_img i) : SV_Target
	{

		float LOD_Size = clamp(pow(2, _SafetyZoneParams.z / 50), 0, 8);
		float2 SafetyZoneCoords = float2(_SafetyZoneParams.x, _SafetyZoneParams.y);

		float3 WhiteLightRef_Dir = tex2Dlod(_SourceNM, float4(SafetyZoneCoords.x, SafetyZoneCoords.y, 0, LOD_Size)).xyz;
		WhiteLightRef_Dir = NormalFromNM(WhiteLightRef_Dir);

		float4 AverageColor = float4(_AverageColor[0]);
		AverageColor /= AverageColor.a;
		AverageColor.a = 1;

		float2 LatLongUV = i.uv;
		
		if (_SafetyZoneParams.w > 0.5) // SwitchYZ axis is stored in _SafetyZoneParams.w
		{
			WhiteLightRef_Dir = WhiteLightRef_Dir.rbg * float3(1, -1, 1);
			LatLongUV = i.uv + float2(0.75,0);
			LatLongUV = fmod(LatLongUV, 1);
		}
		float2 WhiteLightRef_Coords = LLCoordFromVector(WhiteLightRef_Dir);

		if (_SafetyZoneParams.w > 0.5) 
		{
			WhiteLightRef_Coords += float2(0.25, 0);
			WhiteLightRef_Coords = fmod(WhiteLightRef_Coords, 1);
		}
		

		float DistX = abs(i.uv.x - WhiteLightRef_Coords.x) - 0.015;
		float DistY = abs(i.uv.y - WhiteLightRef_Coords.y) - 0.03;


		float SafetyZoneLerp = saturate(sign(max(DistX, DistY)));
		SafetyZoneLerp = lerp(SafetyZoneLerp, 1, _Export);
        SafetyZoneLerp = (1 - _UseSaftyZone) + _UseSaftyZone * SafetyZoneLerp;

		

		float4 latLong = tex2D(_latLong, LatLongUV);
		float4 refColor = tex2D(_latLong, WhiteLightRef_Coords);
		refColor = lerp(AverageColor, refColor, _UseSaftyZone * _UseSaftyZone);

		float Exposure = lerp(_Exposure, 1, _Export);

		float4 WBresult = (latLong / refColor) * Exposure;

		float4 FinalColor = lerp(float4(1,0,0,1), WBresult,SafetyZoneLerp);


		FinalColor.rgb = lerp(FinalColor.rgb, GammaToLinearSpace(FinalColor.rgb), _Export);
		FinalColor.a = 1;


		return FinalColor;



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
