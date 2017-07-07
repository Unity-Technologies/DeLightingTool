using System;

namespace UnityEditor.Experimental
{
    public static partial class Delighting
    {
        [Flags]
        public enum ErrorCode
        {
            NoErrors = 0,
            MissingBaseTexture = 1 << 0,
            MissingNormalsTexture = 1 << 1,
            MissingBentNormalsTexture = 1 << 2,
            MissingAmbientOcclusionTexture = 1 << 3,
            PreviewNotAvailable = 1 << 4,
            WrongSizeNormalsTexture = 1 << 5,
            WrongSizeBentNormalsTexture = 1 << 6,
            WrongSizeAmbientOcclusionTexture = 1 << 7,
            WrongSizePositionTexture = 1 << 8,
            WrongSizeMaskTexture = 1 << 9,
            ColorSpaceNotLinear = 1 << 10
        }
    }
}
