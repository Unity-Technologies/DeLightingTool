Shader "Hidden/EditorGUITools/Grid"
{
    Properties
    {
        _BackgroundColor("Background Color", Color) = (0.76, 0.76, 0.76, 1)
        _GridMajorColor("Grid Major Color", Color) = (0.33, 0.33, 0.33, 1)
        _GridMinorColor("Grid Minor Color", Color) = (0.41, 0.41, 0.41, 1)
        _GridMajorSize("Grid Major Size", Float) = 32
        _GridMinorSize("Grid Minor Size", Float) = 8
        _Zoom("Zoom", Float) = 1
        _CameraX("CameraX", Float) = 0
        _CameraY("CameraY", Float) = 0
    }
    SubShader
    {
        Tags{ "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag

#include "UnityCG.cginc"

            float4 _BackgroundColor;
            float4 _GridMajorColor;
            float4 _GridMinorColor;
            float _GridMajorSize;
            float _GridMinorSize;
            float _Zoom;
            float _CameraX;
            float _CameraY;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float minorSize = _GridMinorSize * _Zoom;
                float majorSize = _GridMajorSize * _Zoom;
                float x = i.pos.x + _CameraX;
                x = x > 0 ? x : -x + 1;
                float y = i.pos.y + _CameraY;
                y = y > 0 ? y : -y + 1;

                float4 col = _BackgroundColor;
                if ((x % minorSize) < 1)
					col = lerp(_BackgroundColor, _GridMinorColor, saturate(_Zoom));
				if ((y % minorSize) < 1)
					col = lerp(_BackgroundColor, _GridMinorColor, saturate(_Zoom));
                    

                if ((x % majorSize) < 1)
                    col = _GridMajorColor;
                if ((y % majorSize) < 1)
                    col = _GridMajorColor;
                return col;
            }
            ENDCG
        }
    }
}
