using System;

namespace UnityEditor.Experimental.DelightingInternal
{
    internal static class DelightingUI
    {
        [Flags]
        internal enum Display
        {
            Normal = 0,
            SectionInput = 1 << 0,
            SectionDelighting = 1 << 1,
            SectionDebug = 1 << 2,
            SectionImport = 1 << 3,
            FieldBaseTexture = 1 << 4,
            FieldNormalsTexture = 1 << 5,
            FieldBentNormalsTexture = 1 << 6,
            FieldAmbientOcclusionTexture = 1 << 7,
            FieldPositionTexture = 1 << 8,
            FieldSwitchYZAxes = 1 << 9,
            FieldNoiseReduction = 1 << 10,
            FieldMaskTexture = 1 << 11,
            FieldViewMode = 1 << 12,
            FieldRemoveHighlights = 1 << 13,
            ButtonCompute = 1 << 14,
            ButtonAutoCompute = 1 << 15,
            ButtonExport = 1 << 16,
            ButtonFitToWindow = 1 << 17,
            FieldRemoveDarkNoise = 1 << 18,
            FieldSeparateDarkAreas = 1 << 19,
            FieldForceLocalDelighting = 1 << 20
        }

        internal enum Mode
        {
            Normal,
            Advanced,
            Debug
        }
    }
}
