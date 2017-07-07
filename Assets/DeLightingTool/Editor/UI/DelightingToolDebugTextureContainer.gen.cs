#pragma warning disable 414
namespace UnityEditor.Experimental.DelightingInternal
{
    using UnityEditor.Experimental.VisualElements;
    using System.Collections.Generic;
    using UnityEngine;

    internal partial class DelightingToolDebugTextureContainer : IMGUIVisualContainer
    {

        private static readonly IClassMethod kCmdResetExposureZone = new DynamicClassMethod("CmdResetExposureZone");

        private static readonly IClassMethod kCmdExport = new DynamicClassMethod("CmdExport");

        private static readonly IClassMethod kCmdExportMainEnvMap = new DynamicClassMethod("CmdExportMainEnvMap");

        private static readonly IClassMethod kCmdProcessFromColorCorrection = new DynamicClassMethod("CmdProcessFromColorCorrection");

        private static readonly IClassMethod kCmdProcessFromGather = new DynamicClassMethod("CmdProcessFromGather");

        private static readonly IClassMethod kCmdRenderPreview = new DynamicClassMethod("CmdRenderPreview");

        private static readonly IClassMethod kCmdLoadInputFolder = new DynamicClassMethod("CmdLoadInputFolder");

        private static readonly ClassProperty<bool> kFoldInputParameters = new DynamicClassProperty<bool>("foldInputParameters");

        private static readonly ClassProperty<bool> kFoldOptionalParameters = new DynamicClassProperty<bool>("foldOptionalParameters");

        private static readonly ClassProperty<bool> kSwitchYZaxes = new DynamicClassProperty<bool>("switchYZaxes");

        private static readonly ClassProperty<Texture2D> kBaseTexture = new DynamicClassProperty<Texture2D>("baseTexture");

        private static readonly ClassProperty<Texture2D> kNormalsTexture = new DynamicClassProperty<Texture2D>("normalsTexture");

        private static readonly ClassProperty<Texture2D> kBentNormalsTexture = new DynamicClassProperty<Texture2D>("bentNormalsTexture");

        private static readonly ClassProperty<Texture2D> kAmbientOcclusionTexture = new DynamicClassProperty<Texture2D>("ambientOcclusionTexture");

        private static readonly ClassProperty<Texture2D> kPositionTexture = new DynamicClassProperty<Texture2D>("positionTexture");

        private static readonly ClassProperty<bool> kFoldDelightingParameters = new DynamicClassProperty<bool>("foldDelightingParameters");

        private static readonly ClassProperty<float> kSmoothNormals = new DynamicClassProperty<float>("smoothNormals");

        private static readonly ClassProperty<float> kRemoveHighlights = new DynamicClassProperty<float>("removeHighlights");

        private static readonly ClassProperty<float> kRemoveDarkNoise = new DynamicClassProperty<float>("removeDarkNoise");

        private static readonly ClassProperty<float> kSeparateDarkAreas = new DynamicClassProperty<float>("separateDarkAreas");

        private static readonly ClassProperty<float> kForceLocalDelighting = new DynamicClassProperty<float>("forceLocalDelighting");

        private static readonly ClassProperty<Texture2D> kMaskTexture = new DynamicClassProperty<Texture2D>("maskTexture");

        private static readonly ClassProperty<float> kCompareViewLerp = new DynamicClassProperty<float>("compareViewLerp");

        private static readonly ClassProperty<DelightingViewMode> kLeftViewMode = new DynamicClassProperty<DelightingViewMode>("leftViewMode");

        private static readonly ClassProperty<DelightingViewMode> kRightViewMode = new DynamicClassProperty<DelightingViewMode>("rightViewMode");

        private static readonly ClassProperty<DelightingUI.Mode> kDisplayedUIMode = new DynamicClassProperty<DelightingUI.Mode>("displayedUIMode");

        private static readonly ClassProperty<DelightingUI.Display> kDisplayedUI = new DynamicClassProperty<DelightingUI.Display>("displayedUI");

        private static readonly ClassProperty<bool> kAutoCompute = new DynamicClassProperty<bool>("autoCompute");

        private static readonly ClassProperty<bool> kHasValidInput = new DynamicClassProperty<bool>("hasValidInput");

        private static readonly ClassProperty<Delighting.ErrorCode> kInputErrorCode = new DynamicClassProperty<Delighting.ErrorCode>("inputErrorCode");

        private static readonly ClassProperty<bool> kLoadingShow = new DynamicClassProperty<bool>("loadingShow");

        private static readonly ClassProperty<float> kLoadingProgress = new DynamicClassProperty<float>("loadingProgress");

        private static readonly ClassProperty<string> kLoadingContent = new DynamicClassProperty<string>("loadingContent");

        private static readonly ClassProperty<List<string>> kInspectorErrorMessages = new DynamicClassProperty<List<string>>("inspectorErrorMessages");

        private static readonly ClassProperty<bool> kOverrideReferenceZone = new DynamicClassProperty<bool>("overrideReferenceZone");

        private static readonly ClassProperty<ComputeBuffer> kLatLongAverage = new DynamicClassProperty<ComputeBuffer>("latLongAverage");

        private static readonly ClassProperty<RenderTexture> kPreviewTexture = new DynamicClassProperty<RenderTexture>("previewTexture");

        private static readonly ClassProperty<Vector2> kCameraPosition = new DynamicClassProperty<Vector2>("cameraPosition");

        private static readonly ClassProperty<float> kZoom = new DynamicClassProperty<float>("zoom");

        private static readonly ClassProperty<Vector2> kExposureGizmoPosition = new DynamicClassProperty<Vector2>("exposureGizmoPosition");

        private static readonly ClassProperty<float> kExposureGizmoRadius = new DynamicClassProperty<float>("exposureGizmoRadius");

        private static readonly ClassProperty<bool> kFitCanvasToWindow = new DynamicClassProperty<bool>("fitCanvasToWindow");

        private static readonly ClassProperty<bool> kFoldDebugParameters = new DynamicClassProperty<bool>("foldDebugParameters");

        private static readonly ClassProperty<RenderTexture> kLatLongA = new DynamicClassProperty<RenderTexture>("latLongA");

        private static readonly ClassProperty<RenderTexture> kBakedLUT = new DynamicClassProperty<RenderTexture>("bakedLUT");

        private static readonly ClassProperty<float> kLatLongExposure = new DynamicClassProperty<float>("latLongExposure");

        private static readonly ClassProperty<Vector4> kSafetyZoneParams = new DynamicClassProperty<Vector4>("safetyZoneParams");

        private static readonly ClassProperty<bool> kFoldImportParameters = new DynamicClassProperty<bool>("foldImportParameters");

        private static readonly ClassProperty<string> kBaseTextureSuffix = new DynamicClassProperty<string>("baseTextureSuffix");

        private static readonly ClassProperty<string> kNormalsTextureSuffix = new DynamicClassProperty<string>("normalsTextureSuffix");

        private static readonly ClassProperty<string> kBentNormalsTextureSuffix = new DynamicClassProperty<string>("bentNormalsTextureSuffix");

        private static readonly ClassProperty<string> kAmbientOcclusionTextureSuffix = new DynamicClassProperty<string>("ambientOcclusionTextureSuffix");

        private static readonly ClassProperty<string> kPositionsTextureSuffix = new DynamicClassProperty<string>("positionsTextureSuffix");

        private static readonly ClassProperty<string> kMaskTextureSuffix = new DynamicClassProperty<string>("maskTextureSuffix");

        private static readonly ClassProperty<string> kInputFolderPath = new DynamicClassProperty<string>("inputFolderPath");



    }

}
#pragma warning restore 414
