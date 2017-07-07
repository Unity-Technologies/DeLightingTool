using UnityEngine;

namespace UnityEditor.Experimental
{
    public static partial class Delighting
    {
        public interface ILoadAssetFolderOperation : IInput
        {
            string folderPath { get; }
            Texture2D baseTexture { get; }
            Texture2D normalsTexture { get; }
            Texture2D bentNormalsTexture { get; }
            Texture2D ambientOcclusionTexture { get; }
            Texture2D positionTexture { get; }
            Texture2D maskTexture { get; }
        }
    }
}
