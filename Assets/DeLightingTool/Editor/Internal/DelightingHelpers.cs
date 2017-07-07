using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.DelightingInternal
{
    internal static class DelightingHelpers
    {
        internal static bool IsPathSuffixed(string assetPath, string suffix = null)
        {
            Assert.IsFalse(string.IsNullOrEmpty(assetPath));
            Assert.IsFalse(string.IsNullOrEmpty(suffix));
            suffix = suffix.ToLower();

            return Path.GetFileNameWithoutExtension(assetPath).ToLower().EndsWith(suffix);
        }

        internal static RenderTexture InstantiateRTIfRequired(RenderTexture rt, int width, int heigth, bool genMips, TextureWrapMode wrap)
        {
            if (rt == null || !rt.IsCreated() || rt.width != width || rt.height != heigth)
            {
                UnityEngine.Object.DestroyImmediate(rt);
                rt = new RenderTexture(width, heigth, 0, RenderTextureFormat.ARGBFloat)
                {
                    anisoLevel = 0,
                    enableRandomWrite = true,
                    filterMode = FilterMode.Trilinear,
                    wrapMode = wrap,
                    hideFlags = HideFlags.DontSave,
                    useMipMap = genMips,
                    autoGenerateMips = genMips
                };
            }

            return rt;
        }

        internal static void DeleteRTIfRequired(RenderTexture rt)
        {
            if (rt != null)
            {
                if (RenderTexture.active == rt)
                    RenderTexture.active = null;
                UnityEngine.Object.DestroyImmediate(rt);
            }
        }

        internal static EditorGUIX.FieldIcon GetIcon(Delighting.ErrorCode errorCode, Delighting.ErrorCode comparedToErrorCode)
        {
            return (errorCode & comparedToErrorCode) != 0
                ? EditorGUIX.FieldIcon.Error
                : EditorGUIX.FieldIcon.None;
        }

        static HashSet<string> s_GetErrorMessagesFrom_MissingTextures = new HashSet<string>();
        static HashSet<string> s_GetErrorMessagesFrom_WrongSizeTextures = new HashSet<string>();
        internal static void GetErrorMessagesFrom(Delighting.ErrorCode errorCode, List<string> target, int width, int height)
        {
            s_GetErrorMessagesFrom_MissingTextures.Clear();
            s_GetErrorMessagesFrom_WrongSizeTextures.Clear();
            target.Clear();

            if ((errorCode & Delighting.ErrorCode.MissingBaseTexture) != 0)
                s_GetErrorMessagesFrom_MissingTextures.Add("Base Texture");

            if ((errorCode & Delighting.ErrorCode.MissingAmbientOcclusionTexture) != 0)
                s_GetErrorMessagesFrom_MissingTextures.Add("Ambient Occlusion");
            else if ((errorCode & Delighting.ErrorCode.WrongSizeAmbientOcclusionTexture) != 0)
                s_GetErrorMessagesFrom_WrongSizeTextures.Add("Ambient Occlusion");

            if ((errorCode & Delighting.ErrorCode.MissingNormalsTexture) != 0)
                s_GetErrorMessagesFrom_MissingTextures.Add("Normals");
            else if ((errorCode & Delighting.ErrorCode.WrongSizeNormalsTexture) != 0)
                s_GetErrorMessagesFrom_WrongSizeTextures.Add("Normals");

            if ((errorCode & Delighting.ErrorCode.MissingBentNormalsTexture) != 0)
                s_GetErrorMessagesFrom_MissingTextures.Add("Bent Normals");
            else if ((errorCode & Delighting.ErrorCode.WrongSizeBentNormalsTexture) != 0)
                s_GetErrorMessagesFrom_WrongSizeTextures.Add("Bent Normals");

            if ((errorCode & Delighting.ErrorCode.WrongSizePositionTexture) != 0)
                s_GetErrorMessagesFrom_WrongSizeTextures.Add("Positions");

            if ((errorCode & Delighting.ErrorCode.WrongSizeMaskTexture) != 0)
                s_GetErrorMessagesFrom_WrongSizeTextures.Add("Mask");

            if (s_GetErrorMessagesFrom_MissingTextures.Count > 0)
                target.Add("Missing texture(s): " + string.Join(", ", s_GetErrorMessagesFrom_MissingTextures.ToArray()));
            if (s_GetErrorMessagesFrom_WrongSizeTextures.Count > 0)
                target.Add(string.Format("This texture(s) must have the same size as Base Texture ({0}x{1}): {2}", width, height, string.Join(", ", s_GetErrorMessagesFrom_WrongSizeTextures.ToArray())));

            if ((errorCode & Delighting.ErrorCode.ColorSpaceNotLinear) != 0)
                target.Add(string.Format(@"Color Space must be linear, it is currently {0}.

Please, go to Edit > Project Settings > Player in Other Settings change Color Space to Linear.", PlayerSettings.colorSpace));
        }
    }
}
