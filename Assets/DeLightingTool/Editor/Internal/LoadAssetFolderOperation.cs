using UnityEngine;

namespace UnityEditor.Experimental.DelightingInternal
{
    class LoadAssetFolderOperation : Delighting.ILoadAssetFolderOperation
    {
        public string folderPath { get; internal set; }
        public bool hasBaseTexture { get; internal set; }
        public Texture2D baseTexture { get; internal set; }
        public bool hasNormalsTexture { get; internal set; }
        public Texture2D normalsTexture { get; internal set; }
        public bool hasBentNormalsTexture { get; internal set; }
        public Texture2D bentNormalsTexture { get; internal set; }
        public bool hasAmbientOcclusionTexture { get; internal set; }
        public Texture2D ambientOcclusionTexture { get; internal set; }
        public bool hasPositionTexture { get; internal set; }
        public Texture2D positionTexture { get; internal set; }
        public bool hasMaskTexture { get; internal set; }
        public Texture2D maskTexture { get; internal set; }
        public bool? switchYZ { get; internal set; }
        public float? separateDarkAreas { get; internal set; }
        public float? forceLocalDelighting { get; internal set; }
        public float? removeHighlights { get; internal set; }
        public float? removeDarkNoise { get; internal set; }
    }
}
