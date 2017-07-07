using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.DelightingInternal
{
    using UnityObject = UnityEngine.Object;

    internal class DelightingService : IDelighting
    {
        internal static string kDefaultBaseTextureSuffix = "_DLBC";
        internal static string kDefaultNormalsTextureSuffix = "_DLN";
        internal static string kDefaultBentNormalsTextureSuffix = "_DLBN";
        internal static string kDefaultAmbientOcclusionTextureSuffix = "_DLAO";
        internal static string kDefaultPositionTextureSuffix = "_position";
        internal static string kDefaultMaskTextureSuffix = "_mask";


        static ComputeShader s_LUTGatherArrayComputeShader = null;
        static ComputeShader kLUTGatherArrayComputeShader { get { return s_LUTGatherArrayComputeShader ?? (s_LUTGatherArrayComputeShader = (ComputeShader)EditorGUIUtility.LoadRequired("Delighter/LUTArrayGather.compute")); } }
        static ComputeShader s_LatLongArrayGatherComputeShader = null;
        static ComputeShader kLatLongArrayGatherComputeShader { get { return s_LatLongArrayGatherComputeShader ?? (s_LatLongArrayGatherComputeShader = (ComputeShader)EditorGUIUtility.LoadRequired("Delighter/LatLongArrayGather.compute")); } }
        static ComputeShader s_LatLongAverageComputeShader = null;
        static ComputeShader kLatLongAverageComputeShader { get { return s_LatLongAverageComputeShader ?? (s_LatLongAverageComputeShader ?? (ComputeShader)EditorGUIUtility.LoadRequired("Delighter/LatLongAverage.compute")); } }
        static ComputeShader s_CleanBufferShader = null;
        static ComputeShader kCleanBufferShader { get { return s_CleanBufferShader ?? (s_CleanBufferShader ?? (ComputeShader)EditorGUIUtility.LoadRequired("Delighter/CleanBuffer.compute")); } }


        static Material s_PreviewMaterial = null;
        static Material kPreviewMaterial { get { return s_PreviewMaterial ?? (s_PreviewMaterial = new Material(Shader.Find("Hidden/Delighter/Preview")) { hideFlags = HideFlags.DontSave }); } }
        static Material s_ApplyLUTArrayMaterial = null;
        static Material kApplyLUTArrayMaterial { get { return s_ApplyLUTArrayMaterial ?? (s_ApplyLUTArrayMaterial = new Material(Shader.Find("Hidden/Delighter/ApplyLUTArray")) { hideFlags = HideFlags.DontSave }); } }
        static Material s_CleanLutMaterial = null;
        static Material kNormLutMaterial { get { return s_CleanLutMaterial ?? (s_CleanLutMaterial = new Material(Shader.Find("Hidden/Delighter/NormalizeLut")) { hideFlags = HideFlags.DontSave }); } }
        static Material s_LatLongNormMaterial = null;
        static Material kLatLongNormMaterial { get { return s_LatLongNormMaterial ?? (s_LatLongNormMaterial = new Material(Shader.Find("Hidden/Delighter/LatLongNorm")) { hideFlags = HideFlags.DontSave }); } }
        static Material s_LightRemFromLLArrayMaterial = null;
        static Material kLightRemFromLLArrayMaterial { get { return s_LightRemFromLLArrayMaterial ?? (s_LightRemFromLLArrayMaterial = new Material(Shader.Find("Hidden/Delighter/LightRemFromLLArray")) { hideFlags = HideFlags.DontSave }); } }
        static Material s_ColorCorrectionMaterial = null;
        static Material kColorCorrectionMaterial { get { return s_ColorCorrectionMaterial ?? (s_ColorCorrectionMaterial = new Material(Shader.Find("Hidden/Delighter/ColorCorrection")) { hideFlags = HideFlags.DontSave }); } }
        static Material s_RecoverAO = null;
        static Material kRecoverAO { get { return s_RecoverAO ?? (s_RecoverAO = new Material(Shader.Find("Hidden/Delighter/AOrecover")) { hideFlags = HideFlags.DontSave }); } }


        static Material s_LatLongPreviewMaterial = null;
        static Material latLongPreviewMaterial { get { return s_LatLongPreviewMaterial ?? (s_LatLongPreviewMaterial = new Material(Shader.Find("Hidden/Delighter/LatLongPreview")) { hideFlags = HideFlags.DontSave }); } }

        internal static Material GetLatLongMaterial(
            Texture latLongA, 
            Texture normal,
            ComputeBuffer averageColor,
            bool useExposure,
            float exposure,
            Vector4 exposureParams,
            bool exportMode)
        {
            latLongPreviewMaterial.SetTexture("_latLong", latLongA);
            latLongPreviewMaterial.SetTexture("_SourceNM", normal);
            latLongPreviewMaterial.SetFloat("_Exposure", exposure);
            latLongPreviewMaterial.SetFloat("_Export", exportMode ? 1 : 0);
            latLongPreviewMaterial.SetVector("_SafetyZoneParams", exposureParams);
            latLongPreviewMaterial.SetInt("_UseSaftyZone", useExposure ? 1 : 0);
            latLongPreviewMaterial.SetBuffer("_AverageColor", averageColor);

            return latLongPreviewMaterial;
        }

        const int kGridSize = 1;
        const int kEnvSize = 64;

        internal DelightingViewModel vm { get { return m_ViewModel; } }
        DelightingViewModel m_ViewModel = null;

        RenderTexture m_Result = null;
        RenderTexture m_ColorCorrect = null;
        RenderTexture m_AOCorrected = null;
        public Texture result { get { return m_ColorCorrect; } }
        private RenderTexture m_DefaultMask;
        public ILogger log { get; set; }

        ComputeBuffer m_LatLongArray = null;
        ComputeBuffer m_LutArrayGather = null;

        public DelightingService()
        {
            m_ViewModel = new DelightingViewModel(this);
            log = UnityEngine.Debug.logger;
        }

        public void Dispose()
        {
            m_ViewModel.Dispose();
            DelightingHelpers.DeleteRTIfRequired(m_Result);
            m_Result = null;
            DelightingHelpers.DeleteRTIfRequired(m_AOCorrected);
            m_AOCorrected = null;
            DelightingHelpers.DeleteRTIfRequired(m_ColorCorrect);
            m_ColorCorrect = null;
            DelightingHelpers.DeleteRTIfRequired(m_DefaultMask);
            m_DefaultMask = null;


            if (m_LatLongArray != null)
            {
                m_LatLongArray.Release();
                m_LatLongArray = null;
            }
            if (vm.latLongAverage != null)
            {
                vm.latLongAverage.Release();
                vm.latLongAverage = null;
            }
            if (m_LutArrayGather != null)
            {
                m_LutArrayGather.Release();
                m_LutArrayGather = null;
            }
        }

        #region API
        public Delighting.ErrorCode ValidateInputs()
        {
            var result = Delighting.ErrorCode.NoErrors;
            if (vm.baseTexture == null)
                result |= Delighting.ErrorCode.MissingBaseTexture;
            if (vm.normalsTexture == null)
                result |= Delighting.ErrorCode.MissingNormalsTexture;
            if (vm.bentNormalsTexture == null)
                result |= Delighting.ErrorCode.MissingBentNormalsTexture;
            if (vm.ambientOcclusionTexture == null)
                result |= Delighting.ErrorCode.MissingAmbientOcclusionTexture;

            var checkSize = vm.baseTexture != null;
            if (checkSize)
            {
                var width = vm.baseTexture.width;
                var height = vm.baseTexture.height;

                if (vm.normalsTexture != null
                    && (vm.normalsTexture.width != width || vm.normalsTexture.height != height))
                    result |= Delighting.ErrorCode.WrongSizeNormalsTexture;

                if (vm.bentNormalsTexture != null
                    && (vm.bentNormalsTexture.width != width || vm.bentNormalsTexture.height != height))
                    result |= Delighting.ErrorCode.WrongSizeBentNormalsTexture;

                if (vm.ambientOcclusionTexture != null
                    && (vm.ambientOcclusionTexture.width != width || vm.ambientOcclusionTexture.height != height))
                    result |= Delighting.ErrorCode.WrongSizeAmbientOcclusionTexture;

                if (vm.positionTexture != null
                    && (vm.positionTexture.width != width || vm.positionTexture.height != height))
                    result |= Delighting.ErrorCode.WrongSizePositionTexture;

                if (vm.maskTexture != null
                    && (vm.maskTexture.width != width || vm.maskTexture.height != height))
                    result |= Delighting.ErrorCode.WrongSizeMaskTexture;
            }

            if (PlayerSettings.colorSpace != ColorSpace.Linear)
                result |= Delighting.ErrorCode.ColorSpaceNotLinear;

            return result;
        }

        public AsyncOperationEnumerator<Delighting.IProcessOperation> ProcessAsync(Delighting.ProcessArgs args)
        {
            var error = ValidateInputs();
            if (error != Delighting.ErrorCode.NoErrors)
            {
                var result = new AsyncOperationEnumerator<Delighting.IProcessOperation>(null, 0);
                var width = 0;
                var height = 0;
                if (vm.baseTexture != null)
                {
                    width = vm.baseTexture.width;
                    height = vm.baseTexture.height;
                }
                result.SetError(new Delighting.ProcessException(error, "Error during process", width, height));
                return result;
            }

            var e = DoProcess(args, true);
            var stepCount = 0;
            while (e.MoveNext()) ++stepCount;

            return new AsyncOperationEnumerator<Delighting.IProcessOperation>(DoProcess(args, false), stepCount);
        }

        public Delighting.ErrorCode RenderPreview()
        {
            var error = ValidateInputs();
            if (result == null)
                error |= Delighting.ErrorCode.PreviewNotAvailable;
            if (error != Delighting.ErrorCode.NoErrors)
                return error;

            // Prepare RT target
            vm.previewTexture = DelightingHelpers.InstantiateRTIfRequired(vm.previewTexture, result.width, result.height, false, TextureWrapMode.Clamp);

            // Store local variables
            var previewTexture = vm.previewTexture;
            var delighted = result;
            var leftViewMode = vm.leftViewMode;
            var rightViewMode = vm.rightViewMode;
            var compareViewLerp = vm.compareViewLerp;
            var baseTexture = vm.baseTexture;
            var normalsTexture = vm.normalsTexture;
            var bentNormalsTexture = vm.bentNormalsTexture;
            var ambientOcclusionTexture = vm.ambientOcclusionTexture;
            var maskTexture = vm.maskTexture;

            // Material setup
            kPreviewMaterial.SetFloat("_ComparePos", compareViewLerp);
            SetupPreviewMaterial("Left", leftViewMode, baseTexture, delighted, normalsTexture, bentNormalsTexture, ambientOcclusionTexture, maskTexture);
            SetupPreviewMaterial("Right", rightViewMode, baseTexture, delighted, normalsTexture, bentNormalsTexture, ambientOcclusionTexture, maskTexture);

            var oldRt = RenderTexture.active;
            Graphics.Blit(null, previewTexture, kPreviewMaterial, 0);
            RenderTexture.active = oldRt;

            vm.SetPropertyChanged(DelightingViewModel.Properties.kPreviewTexture);

            return Delighting.ErrorCode.NoErrors;
        }

        public AsyncOperationEnumerator<Delighting.ILoadAssetFolderOperation> LoadInputFolderAsync(string inputFolderPath)
        {
            if (!AssetDatabase.IsValidFolder(inputFolderPath))
                throw new ArgumentException("inputFolderPath is not a valid asset folder");

            var assetPaths = Directory.GetFiles(inputFolderPath, "*", SearchOption.TopDirectoryOnly);

            var result = new AsyncOperationEnumerator<Delighting.ILoadAssetFolderOperation>(LoadAssets(assetPaths), assetPaths.Length);
            return result;
        }

        public void SetInput(Delighting.IInput input)
        {
            if (input.hasBaseTexture)
                vm.baseTexture = input.baseTexture;
            if (input.hasNormalsTexture)
                vm.normalsTexture = input.normalsTexture;
            if (input.hasBentNormalsTexture)
                vm.bentNormalsTexture = input.bentNormalsTexture;
            if (input.hasAmbientOcclusionTexture)
                vm.ambientOcclusionTexture = input.ambientOcclusionTexture;
            if (input.hasMaskTexture)
                vm.maskTexture = input.maskTexture;
            if (input.hasPositionTexture)
                vm.positionTexture = input.positionTexture;
            if (input.switchYZ.HasValue)
                vm.switchYZaxes = input.switchYZ.Value;
            if (input.removeHighlights.HasValue)
                vm.removeHighlights = input.removeHighlights.Value;
            if (input.removeDarkNoise.HasValue)
                vm.removeDarkNoise = input.removeDarkNoise.Value;
            if (input.separateDarkAreas.HasValue)
                vm.separateDarkAreas = input.separateDarkAreas.Value;
            if (input.forceLocalDelighting.HasValue)
                vm.forceLocalDelighting = input.forceLocalDelighting.Value;

            ResetReferenceGizmo();
        }

        public void Reset()
        {
            vm.baseTexture = null;
            vm.normalsTexture = null;
            vm.bentNormalsTexture = null;
            vm.ambientOcclusionTexture = null;
            vm.positionTexture = null;
            vm.maskTexture = null;

            vm.smoothNormals = 0;
            vm.removeDarkNoise = 0.0f;
            vm.removeHighlights = 0.3f;
            vm.separateDarkAreas = 0.0f;
            vm.forceLocalDelighting = 0.0f;

            vm.exposureGizmoRadius = 256;
            vm.exposureGizmoPosition = Vector2.zero;

            vm.loadingShow = false;
            vm.compareViewLerp = 0.5f;
            vm.fitCanvasToWindow = true;
        }

        public void ResetReferenceGizmo()
        {
            if (vm.baseTexture != null)
            {
                vm.exposureGizmoPosition = new Vector2(vm.baseTexture.width * 0.5f, vm.baseTexture.height * 0.5f);
                vm.exposureGizmoRadius = Mathf.Min(vm.baseTexture.width * 0.1f, vm.baseTexture.height * 0.1f);
            }
            else
            {
                vm.exposureGizmoPosition = Vector2.zero;
                vm.exposureGizmoRadius = 256;
            }
        }

        IEnumerator DoProcess(Delighting.ProcessArgs args, bool dryRun)
        {
            // ------------------------------------------------------------------------------------
            // Render loop goes here

            ///
            if (args.fromStep == Delighting.ProcessStep.Gather)
            {
                // Prepare outputs
                if (!dryRun)
                {
                    m_Result = DelightingHelpers.InstantiateRTIfRequired(m_Result, vm.baseTexture.width, vm.baseTexture.height, true, TextureWrapMode.Clamp);
                    m_AOCorrected = DelightingHelpers.InstantiateRTIfRequired(m_AOCorrected, vm.ambientOcclusionTexture.width, vm.ambientOcclusionTexture.height, true, TextureWrapMode.Clamp);
                    vm.latLongA = DelightingHelpers.InstantiateRTIfRequired(vm.latLongA, kEnvSize, kEnvSize / 2, false, TextureWrapMode.Clamp);
                    vm.bakedLUT = DelightingHelpers.InstantiateRTIfRequired(vm.bakedLUT, 64, 64, false, TextureWrapMode.Clamp);
                }
                yield return null;

                RenderTexture GlobalLatLong = null;
                if (!dryRun)
                {
                    GlobalLatLong = vm.latLongA;
                    if (m_LatLongArray != null)
                    {
                        m_LatLongArray.Release();
                        m_LatLongArray = null;
                    }

                    if (vm.latLongAverage != null)
                    {
                        vm.latLongAverage.Release();
                        vm.latLongAverage = null;
                    }

                    // Recover missing AO from BaseTex
                    processAOrecover();
                }
                yield return null;

                if (!dryRun)
                {
                    // LatLong array gather pass
                    m_LatLongArray = GatherLLArray(GlobalLatLong.width, GlobalLatLong.height); // Store colors per direction info as LatLong map 
                    CleanBuffer(m_LatLongArray, 1); // Remove small weight points
                }
                yield return null;


                if (!dryRun)
                {
                    // Store and normalize the Global LatLong
                    NormalizeLatLong(m_LatLongArray, GlobalLatLong);
                }
                yield return null;

                if (!dryRun)
                {
                    // Find the average color and use it as color reference
                    vm.latLongAverage = AverageLatLong(GlobalLatLong);
                }
                yield return null;

            }

            ///
            if (args.fromStep == Delighting.ProcessStep.Delight 
                || args.fromStep == Delighting.ProcessStep.Gather)
            {
                if (!dryRun)
                {
                    Assert.IsNotNull(m_LatLongArray);
                    Assert.IsNotNull(vm.latLongAverage);

                    // Prepare outputs
                    if (m_LutArrayGather != null)
                    {
                        m_LutArrayGather.Release();
                        m_LutArrayGather = null;
                    }

                    // Inververse IBL pass: use the LatLong array to get back the albedo. No occlusion compensation
                    DelightingArray(m_Result, m_LatLongArray, vm.latLongAverage, kEnvSize, kEnvSize / 2);
                }
                yield return null;


                if (!dryRun)
                {
                    // LUT Gathering pass - Gather Occlusion + GI and store it in a LUT
                    m_LutArrayGather = GatherLUTArray(true, vm.bakedLUT.width, vm.bakedLUT.height);

                    CleanBuffer(m_LutArrayGather, 1);
                }
                yield return null;

                if (!dryRun)
                    NormalizeLUT(m_LutArrayGather, vm.bakedLUT); // Used just to make the preview !!! Change the name
                yield return null;

                // Apply LUT
                if (!dryRun)
                    ApplyLUTArray(m_Result, m_LutArrayGather, vm.latLongAverage, vm.bakedLUT.width, vm.bakedLUT.height, kGridSize); // Apply inverse LUT for de-light Occlusion + GI
                yield return null;
            }

            ///
            if (args.fromStep == Delighting.ProcessStep.Delight
                || args.fromStep == Delighting.ProcessStep.Gather
                || args.fromStep == Delighting.ProcessStep.ColorCorrection)
            {
                if (!dryRun)
                {
                    m_ColorCorrect = DelightingHelpers.InstantiateRTIfRequired(m_ColorCorrect, vm.baseTexture.width, vm.baseTexture.height, false, TextureWrapMode.Clamp);
                    ColorCorrection(m_ColorCorrect, m_LatLongArray, vm.latLongAverage, kEnvSize, kEnvSize / 2); // Apply Color Correction using Reference/Average Color, and various filters
                }
                yield return null;
            }

            // End of render loop
            // ------------------------------------------------------------------------------------

            yield return dryRun
                ? null
                : new ProcessOperation()
            {
                error = Delighting.ErrorCode.NoErrors,
                result = args.calculateResult ? CreateDelightedTextureFromResult() : null
            };
        }

        IEnumerator LoadAssets(string[] assetPaths)
        {
            var result = new LoadAssetFolderOperation();

            for (int i = 0; i < assetPaths.Length; i++)
            {
                var assetPath = assetPaths[i];
                if (assetPath.EndsWith(".meta"))
                    continue;

                if (DelightingHelpers.IsPathSuffixed(assetPath, vm.positionsTextureSuffix))
                {
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                    if (texture != null)
                    {
                        log.LogFormat(LogType.Log, "Found Position Texture {0}", assetPath);
                        result.positionTexture = texture;
                        result.hasPositionTexture = true;
                    }
                }
                else if (DelightingHelpers.IsPathSuffixed(assetPath, vm.ambientOcclusionTextureSuffix))
                {
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                    if (texture != null)
                    {
                        log.LogFormat(LogType.Log, "Found Ambient Occlusion Texture {0}", assetPath);
                        result.ambientOcclusionTexture = texture;
                        result.hasAmbientOcclusionTexture = true;
                    }
                }
                else if (DelightingHelpers.IsPathSuffixed(assetPath, vm.baseTextureSuffix))
                {
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                    if (texture != null)
                    {
                        log.LogFormat(LogType.Log, "Found Base Texture {0}", assetPath);
                        result.baseTexture = texture;
                        result.hasBaseTexture = true;
                    }
                }
                else if (DelightingHelpers.IsPathSuffixed(assetPath, vm.bentNormalsTextureSuffix))
                {
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                    if (texture != null)
                    {
                        log.LogFormat(LogType.Log, "Found Bent Normals Texture {0}", assetPath);
                        result.bentNormalsTexture = texture;
                        result.hasBentNormalsTexture = true;
                    }
                }
                else if (DelightingHelpers.IsPathSuffixed(assetPath, vm.normalsTextureSuffix))
                {
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                    if (texture != null)
                    {
                        log.LogFormat(LogType.Log, "Found Normals Texture {0}", assetPath);
                        result.normalsTexture = texture;
                        result.hasNormalsTexture = true;
                    }
                }
                else if (DelightingHelpers.IsPathSuffixed(assetPath, vm.maskTextureSuffix))
                {
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                    if (texture != null)
                    {
                        log.LogFormat(LogType.Log, "Found Normals Texture {0}", assetPath);
                        result.maskTexture = texture;
                        result.hasMaskTexture = true;
                    }
                }

                yield return null;
            }

            yield return result;
        }
        #endregion

        public void ExportMainEnvMapDialog()
        {
            var targetPath = ShowExportPathPanel("Save main env map texture", "_MainEnvMap", "exr");
            if (!string.IsNullOrEmpty(targetPath))
            {
                var resultTexture = CreateEnvMapTextureFromResult();
                File.WriteAllBytes(targetPath, resultTexture.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat));
                UnityObject.DestroyImmediate(resultTexture);

                SetImporterForEnvMapTexture(targetPath);
            }
        }

        public void ExportDelightedTextureDialog()
        {
            var targetPath = ShowExportPathPanel("Save de-lighted texture", "_NoLight", "png");
            if (!string.IsNullOrEmpty(targetPath))
            {
                var delightedTexture = CreateDelightedTextureFromResult();
                var bytes = delightedTexture.EncodeToPNG();
                File.WriteAllBytes(targetPath, bytes);
                UnityObject.DestroyImmediate(delightedTexture);

                SetImporterForDelightedTexture(targetPath);
            }
        }

        
        Texture2D CreateDelightedTextureFromResult()
        {
            Assert.IsNotNull(m_ColorCorrect);

            var temporaryRT = RenderTexture.GetTemporary(m_ColorCorrect.width, m_ColorCorrect.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
            Graphics.Blit(m_ColorCorrect, temporaryRT);
            GL.sRGBWrite = false;

            var exportedUnlit = new Texture2D(m_ColorCorrect.width, m_ColorCorrect.height, TextureFormat.ARGB32, false, false);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = temporaryRT;
            exportedUnlit.ReadPixels(new Rect(0, 0, m_ColorCorrect.width, m_ColorCorrect.height), 0, 0, false);
            exportedUnlit.Apply();
            RenderTexture.active = previous;

            return exportedUnlit;
        }

        
        Texture2D CreateEnvMapTextureFromResult()
        {
            Assert.IsNotNull(m_ColorCorrect);

            var mat = DelightingService.GetLatLongMaterial(
            vm.latLongA,
            vm.normalsTexture,
            vm.latLongAverage,
            vm.overrideReferenceZone,
            vm.latLongExposure,
            vm.safetyZoneParams,
            true);

            var temporaryRT = RenderTexture.GetTemporary(vm.latLongA.width, vm.latLongA.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
            Graphics.Blit(null, temporaryRT, mat, 0);

            var exportedLatLong = new Texture2D(vm.latLongA.width, vm.latLongA.height, TextureFormat.RGBAFloat, false, true);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = temporaryRT;
            exportedLatLong.ReadPixels(new Rect(0, 0, vm.latLongA.width, vm.latLongA.height), 0, 0, false);
            exportedLatLong.Apply();
            RenderTexture.active = previous;
            GL.sRGBWrite = false;

            return exportedLatLong;
        }

        string ShowExportPathPanel(string description, string suffix, string ext)
        {
            Assert.IsNotNull(m_Result);

            var directory = Application.dataPath;
            var baseTextureName = "baseColor.png";
            if (vm.baseTexture != null)
            {
                var baseTexturePath = AssetDatabase.GetAssetPath(vm.baseTexture);
                if (!string.IsNullOrEmpty(baseTexturePath))
                {
                    baseTextureName = baseTexturePath;
                    directory = Path.GetDirectoryName(baseTexturePath);
                }
            }
            var defaultName = GetDefaultDelightedTextureOutputfile(baseTextureName, suffix);
            var targetPath = EditorUtility.SaveFilePanel(description, directory, defaultName, ext);
            return targetPath;
        }

        static void SetImporterForEnvMapTexture(string assetPath)
        {
            if (!assetPath.StartsWith(Application.dataPath))
                return;

            var path = "Assets" + assetPath.Substring(Application.dataPath.Length);

            AssetDatabase.Refresh();

            var textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);

            textureImporter.maxTextureSize = 8192;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.mipmapEnabled = true;
            textureImporter.SaveAndReimport();
        }

        static void SetImporterForDelightedTexture(string assetPath)
        {
            if (!assetPath.StartsWith(Application.dataPath))
                return;

            var path = "Assets" + assetPath.Substring(Application.dataPath.Length);

            AssetDatabase.Refresh();

            var textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);
            
            textureImporter.maxTextureSize = 8192;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.mipmapEnabled = true;
            textureImporter.SaveAndReimport();
            
        }

        static string GetDefaultDelightedTextureOutputfile(string path, string suffix)
        {
            return Path.GetFileNameWithoutExtension(path) + suffix;
        }

        static void SetupPreviewMaterial(
            string suffix, 
            DelightingViewMode viewMode, 
            Texture2D baseTexture, 
            Texture delighted, 
            Texture2D normalsTexture, 
            Texture2D bentNormalsTexture, 
            Texture2D ambientOcclusionTexture,
            Texture2D maskTexture)
        {
            switch (viewMode)
            {
                case DelightingViewMode.Base:
                    kPreviewMaterial.SetTexture("_View" + suffix, baseTexture);
                    kPreviewMaterial.SetFloat("_AlphaDisplay" + suffix, 0);
                    break;
                case DelightingViewMode.BaseUnlit:
                    kPreviewMaterial.SetTexture("_View" + suffix, delighted);
                    kPreviewMaterial.SetFloat("_AlphaDisplay" + suffix, 0);
                    break;
                case DelightingViewMode.Normals:
                    kPreviewMaterial.SetTexture("_View" + suffix, normalsTexture);
                    kPreviewMaterial.SetFloat("_AlphaDisplay" + suffix, 0);
                    break;
                case DelightingViewMode.BentNormals:
                    kPreviewMaterial.SetTexture("_View" + suffix, bentNormalsTexture);
                    kPreviewMaterial.SetFloat("_AlphaDisplay" + suffix, 0);
                    break;
                case DelightingViewMode.AmbientOcclusion:
                    kPreviewMaterial.SetTexture("_View" + suffix, ambientOcclusionTexture);
                    kPreviewMaterial.SetFloat("_AlphaDisplay" + suffix, 0);
                    break;
                case DelightingViewMode.BaseAlpha:
                    kPreviewMaterial.SetTexture("_View" + suffix, baseTexture);
                    kPreviewMaterial.SetFloat("_AlphaDisplay" + suffix, 1);
                    break;
                case DelightingViewMode.Mask:
                    kPreviewMaterial.SetTexture("_View" + suffix, maskTexture);
                    kPreviewMaterial.SetFloat("_AlphaDisplay" + suffix, 0);
                    break;
            }
        }

        // Renderloop Functions -----------------------------------------------------------------------------


       void processAOrecover()
        {
            var material = kRecoverAO;
            material.SetTexture("_BaseColor", vm.baseTexture);
            material.SetTexture("_AO", vm.ambientOcclusionTexture);
            material.SetFloat("_AOrecover", vm.separateDarkAreas);

            Graphics.Blit(null, m_AOCorrected, material, 0);
        }


        ComputeBuffer GatherLLArray(int outWidth, int outHeight)
        {
            int slices = 4681;
            int bufferSize = outWidth * outHeight * slices;
            int bufferStride = sizeof(uint) * 4;

            var latLongArray = new ComputeBuffer(bufferSize, bufferStride);
            latLongArray.SetData(new int[bufferSize * 4]);

            var cs = kLatLongArrayGatherComputeShader;
            int kernel = cs.FindKernel("KLatLongArrayGather");
            cs.SetBuffer(kernel, "_LatLongArray", latLongArray);
            cs.SetTexture(kernel, "_SourceBase", vm.baseTexture);
            cs.SetTexture(kernel, "_SourceNM", vm.normalsTexture);
            //cs.SetTexture(kernel, "_SourceAO", vm.ambientOcclusionTexture);
            cs.SetTexture(kernel, "_SourceAO", m_AOCorrected);
            cs.SetVector("_TextureSizes", new Vector4(vm.baseTexture.width, vm.baseTexture.height, outWidth, outHeight));
            cs.SetBool("_SwitchYZ", vm.switchYZaxes);



            if (vm.maskTexture != null)
            {
                cs.SetTexture(kernel, "_SourceMask", vm.maskTexture);
                cs.SetBool("_ValidMask", true);
            }
            else
            {
                cs.SetBool("_ValidMask", false);
            }

            if (vm.positionTexture != null)
            {
                cs.SetTexture(kernel, "_SourcePos", vm.positionTexture);
                cs.SetBool("_ValidPosition", true);
            }
            else
            {
                cs.SetBool("_ValidPosition", false);
            }


            cs.Dispatch(kernel, Mathf.CeilToInt(vm.baseTexture.width / 32f), Mathf.CeilToInt(vm.baseTexture.height / 32f), 1);

            return latLongArray;
        }


        static void CleanBuffer(ComputeBuffer _buffer, int threshold)
        {
            var cs = kCleanBufferShader;
            int kernel = cs.FindKernel("KClean");
            cs.SetBuffer(kernel, "_Buffer", _buffer);
            cs.SetInt("_Threshold", threshold);

            cs.Dispatch(kernel, _buffer.count / 32, 1, 1);
        }


        static void NormalizeLatLong(ComputeBuffer latLongGather, RenderTexture latLong)
        {
            var material = kLatLongNormMaterial;
            material.SetVector("_LLOutputSize", new Vector2(latLong.width, latLong.height));
            material.SetBuffer("_LatLong", latLongGather);
            Graphics.Blit(null, latLong, material, 0);
        }

        ComputeBuffer AverageLatLong(Texture latLong)
        {
            int bufferStride = sizeof(uint) << 2;
            var latLongAverage = new ComputeBuffer(1, bufferStride, ComputeBufferType.Default);
            latLongAverage.SetData(new int[1 << 2]);

            var cs = kLatLongAverageComputeShader;
            int kernel = cs.FindKernel("KLatLongAverage");
            cs.SetBuffer(kernel, "_AverageCol", latLongAverage);
            cs.SetTexture(kernel, "_LatLong", latLong);
            cs.SetVector("_TextureSizes", new Vector4(latLong.width, latLong.height, 1f, 1f));

            cs.Dispatch(kernel, Mathf.CeilToInt(latLong.width / 32f), Mathf.CeilToInt(latLong.height / 32f), 1);

            return latLongAverage;
        }

        void DelightingArray(RenderTexture target, ComputeBuffer latLongArray, ComputeBuffer averageCol, int LLwidth, int LLheight)
        {
            var material = kLightRemFromLLArrayMaterial;

            material.SetBuffer("_LatLongArray", latLongArray);
            material.SetBuffer("_AverageColor", averageCol);
            material.SetVector("_LLOutputSize", new Vector2(LLwidth, LLheight));

            material.SetTexture("_BaseText", vm.baseTexture);
            material.SetTexture("_AO", m_AOCorrected);
            material.SetTexture("_NM", vm.normalsTexture);
            material.SetFloat("_Blur", vm.smoothNormals);
            material.SetFloat("_ForceLocalDL", vm.forceLocalDelighting);
            material.SetInt("_Width", vm.baseTexture.width);
            material.SetInt("_Height", vm.baseTexture.height);
            material.SetInt("_SwitchAxes", vm.switchYZaxes ? 1 : 0);

            if (vm.maskTexture != null)
            {
                material.SetTexture("_Mask", vm.maskTexture);
                material.SetInt("_ValidMask", 1);
            }
            else
            {
                material.SetInt("_ValidMask", 0);
            }

            if (vm.positionTexture != null)
            {
                material.SetTexture("_Position", vm.positionTexture);
                material.SetInt("_ValidPosition", 1);
            }
            else
            {
                material.SetInt("_ValidPosition", 0);
            }

            Graphics.Blit(null, target, material, 0);
        }


        ComputeBuffer GatherLUTArray(bool mode, int outWidth, int outHeight)
        {
            int slices = 4681;

            int bufferSize = outWidth * outHeight * slices;
            int bufferStride = sizeof(uint) * 4;

            var LUT = new ComputeBuffer(bufferSize, bufferStride);
            LUT.SetData(new int[bufferSize * 4]);

            var cs = kLUTGatherArrayComputeShader;
            int kernel = cs.FindKernel("KLUTArrayGather");
            cs.SetBuffer(kernel, "_LUT", LUT);
            cs.SetTexture(kernel, "_SourceBase", m_Result);
            cs.SetTexture(kernel, "_SourceNM", vm.normalsTexture);
            cs.SetTexture(kernel, "_SourceBNM", vm.bentNormalsTexture);
            cs.SetTexture(kernel, "_SourceAO", m_AOCorrected);
            cs.SetVector("_TextureSizes", new Vector4(vm.baseTexture.width, vm.baseTexture.height, outWidth, outHeight));
            cs.SetBool("_Switch", vm.switchYZaxes);
            cs.SetBool("_Mode", mode);



            if (vm.maskTexture != null)
            {
                cs.SetTexture(kernel, "_SourceMask", vm.maskTexture);
                cs.SetBool("_ValidMask", true);
            }
            else
            {
                cs.SetBool("_ValidMask", false);
            }


            if (vm.positionTexture != null)
            {
                cs.SetTexture(kernel, "_SourcePos", vm.positionTexture);
                cs.SetBool("_ValidPosition", true);
            }
            else
            {
                cs.SetBool("_ValidPosition", false);
            }


            cs.Dispatch(kernel, Mathf.CeilToInt(vm.baseTexture.width / 32f), Mathf.CeilToInt(vm.baseTexture.height / 32f), 1);

            return LUT;
        }

        void NormalizeLUT(ComputeBuffer LutBuffer, RenderTexture resultLut)
        {
            var material = kNormLutMaterial;
            material.SetVector("_LutOutputSize", new Vector2(resultLut.width, resultLut.height));
            material.SetBuffer("_LutBuffer", LutBuffer);
            Graphics.Blit(null, resultLut, material, 0);
        }

        void ApplyLUTArray(RenderTexture color, ComputeBuffer LUTArray, ComputeBuffer latLongAverage, int LutWidth, int LutHeight, int gridRes)
        {
            var material = kApplyLUTArrayMaterial;
            var tempRt = RenderTexture.GetTemporary(color.width, color.height, 0, RenderTextureFormat.ARGBFloat);

            material.SetVector("_LutOutputSize", new Vector2(LutWidth, LutHeight));
            material.SetBuffer("_AverageColor", latLongAverage);
            material.SetBuffer("_LUTArray", LUTArray);
            material.SetTexture("_BaseText", color);
            material.SetTexture("_NM", vm.normalsTexture);
            material.SetTexture("_BNM", vm.bentNormalsTexture);
            material.SetTexture("_AO", m_AOCorrected);
            material.SetFloat("_ForceLocalDL", vm.forceLocalDelighting);


            if (vm.maskTexture != null)
            {
                material.SetTexture("_Mask", vm.maskTexture);
                material.SetInt("_ValidMask", 1);
            }
            else
            {
                material.SetInt("_ValidMask", 0);
            }


            if (vm.positionTexture != null)
            {
                material.SetTexture("_Position", vm.positionTexture);
                material.SetInt("_ValidPosition", 1);
            }
            else
            {
                material.SetInt("_ValidPosition", 0);
            }

            Graphics.Blit(color, tempRt, material, 0);
            Graphics.Blit(tempRt, color);
            RenderTexture.active = tempRt;
            GL.Clear(true, true, Color.clear);

            RenderTexture.ReleaseTemporary(tempRt);
        }

        void ColorCorrection(RenderTexture target, ComputeBuffer latLongArray, ComputeBuffer averageCol, int LLwidth, int LLheight)
        {
            var material = kColorCorrectionMaterial;


            material.SetTexture("_BaseText", vm.baseTexture);
            material.SetTexture("_colorText", m_Result);
            material.SetTexture("_latLong", vm.latLongA);
            material.SetBuffer("_AverageColor", averageCol);
            material.SetTexture("_SourceNM", vm.normalsTexture);
            material.SetFloat("_ShowDir", 1);
            material.SetVector("_SafetyZoneParams", vm.safetyZoneParams);
            material.SetInt("_UseSaftyZone", vm.overrideReferenceZone ? 1 : 0);
            material.SetFloat("_RemoveHighlights", vm.removeHighlights);
            material.SetFloat("_RemoveDarkNoise", vm.removeDarkNoise);

            Graphics.Blit(null, target, material, 0);
        }










































    }
}
