namespace UnityEditor.Experimental.DelightingInternal
{
    class DelightingTexturePostProcessor : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;

            // Game Assets --------------------------------------------------------------------
            if (DelightingHelpers.IsPathSuffixed(assetPath, DelightingToolWindow.prefsBaseTextureSuffix))
            {
                textureImporter.textureType = TextureImporterType.Default;
                textureImporter.sRGBTexture = true;
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.maxTextureSize = 8192;
            }
            else if (DelightingHelpers.IsPathSuffixed(assetPath, DelightingToolWindow.prefsMaskTextureSuffix)
                || DelightingHelpers.IsPathSuffixed(assetPath, DelightingToolWindow.prefsPositionsTextureSuffix)
                || DelightingHelpers.IsPathSuffixed(assetPath, DelightingToolWindow.prefsAmbientOcclusionTextureSuffix)
                || DelightingHelpers.IsPathSuffixed(assetPath, DelightingToolWindow.prefsNormalsTextureSuffix)
                || DelightingHelpers.IsPathSuffixed(assetPath, DelightingToolWindow.prefsBentNormalsTextureSuffix))
            {
                textureImporter.textureType = TextureImporterType.Default;
                textureImporter.sRGBTexture = false;
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.maxTextureSize = 8192;
            }
        }
    }
}
