using UnityEngine;

namespace UnityEditor.Experimental
{
    public static partial class Delighting
    {
        public interface IInput
        {
            bool hasBaseTexture { get; }
            Texture2D baseTexture { get; }
            bool hasNormalsTexture { get; }
            Texture2D normalsTexture { get; }
            bool hasBentNormalsTexture { get; }
            Texture2D bentNormalsTexture { get; }
            bool hasAmbientOcclusionTexture { get; }
            Texture2D ambientOcclusionTexture { get; }
            bool hasPositionTexture { get; }
            Texture2D positionTexture { get; }
            bool hasMaskTexture { get; }
            Texture2D maskTexture { get; }
            bool? switchYZ { get; }
            float? separateDarkAreas { get; }
            float? forceLocalDelighting { get; }
            float? removeHighlights { get; }
            float? removeDarkNoise { get; }
        }
    }
}
