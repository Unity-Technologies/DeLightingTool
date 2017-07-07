#pragma warning disable 414
namespace UnityEditor.Experimental.DelightingInternal
{
    using UnityEditor.Experimental.ViewModel;
    using System.Collections.Generic;
    using UnityEngine;

    internal partial class DelightingViewModel : SerializedViewModelBase
    {
        internal static class Properties
        {

            public static readonly ClassProperty<bool> kFoldInputParameters = new StaticClassProperty<bool, DelightingViewModel>("foldInputParameters");

            public static readonly ClassProperty<bool> kFoldOptionalParameters = new StaticClassProperty<bool, DelightingViewModel>("foldOptionalParameters");

            public static readonly ClassProperty<bool> kSwitchYZaxes = new StaticClassProperty<bool, DelightingViewModel>("switchYZaxes");

            public static readonly ClassProperty<Texture2D> kBaseTexture = new StaticClassProperty<Texture2D, DelightingViewModel>("baseTexture");

            public static readonly ClassProperty<Texture2D> kNormalsTexture = new StaticClassProperty<Texture2D, DelightingViewModel>("normalsTexture");

            public static readonly ClassProperty<Texture2D> kBentNormalsTexture = new StaticClassProperty<Texture2D, DelightingViewModel>("bentNormalsTexture");

            public static readonly ClassProperty<Texture2D> kAmbientOcclusionTexture = new StaticClassProperty<Texture2D, DelightingViewModel>("ambientOcclusionTexture");

            public static readonly ClassProperty<Texture2D> kPositionTexture = new StaticClassProperty<Texture2D, DelightingViewModel>("positionTexture");

            public static readonly ClassProperty<bool> kFoldDelightingParameters = new StaticClassProperty<bool, DelightingViewModel>("foldDelightingParameters");

            public static readonly ClassProperty<float> kSmoothNormals = new StaticClassProperty<float, DelightingViewModel>("smoothNormals");

            public static readonly ClassProperty<float> kRemoveHighlights = new StaticClassProperty<float, DelightingViewModel>("removeHighlights");

            public static readonly ClassProperty<float> kRemoveDarkNoise = new StaticClassProperty<float, DelightingViewModel>("removeDarkNoise");

            public static readonly ClassProperty<float> kSeparateDarkAreas = new StaticClassProperty<float, DelightingViewModel>("separateDarkAreas");

            public static readonly ClassProperty<float> kForceLocalDelighting = new StaticClassProperty<float, DelightingViewModel>("forceLocalDelighting");

            public static readonly ClassProperty<Texture2D> kMaskTexture = new StaticClassProperty<Texture2D, DelightingViewModel>("maskTexture");

            public static readonly ClassProperty<float> kCompareViewLerp = new StaticClassProperty<float, DelightingViewModel>("compareViewLerp");

            public static readonly ClassProperty<DelightingViewMode> kLeftViewMode = new StaticClassProperty<DelightingViewMode, DelightingViewModel>("leftViewMode");

            public static readonly ClassProperty<DelightingViewMode> kRightViewMode = new StaticClassProperty<DelightingViewMode, DelightingViewModel>("rightViewMode");

            public static readonly ClassProperty<DelightingUI.Mode> kDisplayedUIMode = new StaticClassProperty<DelightingUI.Mode, DelightingViewModel>("displayedUIMode");

            public static readonly ClassProperty<DelightingUI.Display> kDisplayedUI = new StaticClassProperty<DelightingUI.Display, DelightingViewModel>("displayedUI");

            public static readonly ClassProperty<bool> kAutoCompute = new StaticClassProperty<bool, DelightingViewModel>("autoCompute");

            public static readonly ClassProperty<bool> kHasValidInput = new StaticClassProperty<bool, DelightingViewModel>("hasValidInput");

            public static readonly ClassProperty<Delighting.ErrorCode> kInputErrorCode = new StaticClassProperty<Delighting.ErrorCode, DelightingViewModel>("inputErrorCode");

            public static readonly ClassProperty<bool> kLoadingShow = new StaticClassProperty<bool, DelightingViewModel>("loadingShow");

            public static readonly ClassProperty<float> kLoadingProgress = new StaticClassProperty<float, DelightingViewModel>("loadingProgress");

            public static readonly ClassProperty<string> kLoadingContent = new StaticClassProperty<string, DelightingViewModel>("loadingContent");

            public static readonly ClassProperty<List<string>> kInspectorErrorMessages = new StaticClassProperty<List<string>, DelightingViewModel>("inspectorErrorMessages");

            public static readonly ClassProperty<bool> kOverrideReferenceZone = new StaticClassProperty<bool, DelightingViewModel>("overrideReferenceZone");

            public static readonly ClassProperty<ComputeBuffer> kLatLongAverage = new StaticClassProperty<ComputeBuffer, DelightingViewModel>("latLongAverage");

            public static readonly ClassProperty<RenderTexture> kPreviewTexture = new StaticClassProperty<RenderTexture, DelightingViewModel>("previewTexture");

            public static readonly ClassProperty<Vector2> kCameraPosition = new StaticClassProperty<Vector2, DelightingViewModel>("cameraPosition");

            public static readonly ClassProperty<float> kZoom = new StaticClassProperty<float, DelightingViewModel>("zoom");

            public static readonly ClassProperty<Vector2> kExposureGizmoPosition = new StaticClassProperty<Vector2, DelightingViewModel>("exposureGizmoPosition");

            public static readonly ClassProperty<float> kExposureGizmoRadius = new StaticClassProperty<float, DelightingViewModel>("exposureGizmoRadius");

            public static readonly ClassProperty<bool> kFitCanvasToWindow = new StaticClassProperty<bool, DelightingViewModel>("fitCanvasToWindow");

            public static readonly ClassProperty<bool> kFoldDebugParameters = new StaticClassProperty<bool, DelightingViewModel>("foldDebugParameters");

            public static readonly ClassProperty<RenderTexture> kLatLongA = new StaticClassProperty<RenderTexture, DelightingViewModel>("latLongA");

            public static readonly ClassProperty<RenderTexture> kBakedLUT = new StaticClassProperty<RenderTexture, DelightingViewModel>("bakedLUT");

            public static readonly ClassProperty<float> kLatLongExposure = new StaticClassProperty<float, DelightingViewModel>("latLongExposure");

            public static readonly ClassProperty<Vector4> kSafetyZoneParams = new StaticClassProperty<Vector4, DelightingViewModel>("safetyZoneParams");

            public static readonly ClassProperty<bool> kFoldImportParameters = new StaticClassProperty<bool, DelightingViewModel>("foldImportParameters");

            public static readonly ClassProperty<string> kBaseTextureSuffix = new StaticClassProperty<string, DelightingViewModel>("baseTextureSuffix");

            public static readonly ClassProperty<string> kNormalsTextureSuffix = new StaticClassProperty<string, DelightingViewModel>("normalsTextureSuffix");

            public static readonly ClassProperty<string> kBentNormalsTextureSuffix = new StaticClassProperty<string, DelightingViewModel>("bentNormalsTextureSuffix");

            public static readonly ClassProperty<string> kAmbientOcclusionTextureSuffix = new StaticClassProperty<string, DelightingViewModel>("ambientOcclusionTextureSuffix");

            public static readonly ClassProperty<string> kPositionsTextureSuffix = new StaticClassProperty<string, DelightingViewModel>("positionsTextureSuffix");

            public static readonly ClassProperty<string> kMaskTextureSuffix = new StaticClassProperty<string, DelightingViewModel>("maskTextureSuffix");

            public static readonly IClassMethod kCmdResetExposureZone = new StaticClassMethod<DelightingViewModel>("CmdResetExposureZone");

            public static readonly IClassMethod kCmdExport = new StaticClassMethod<DelightingViewModel>("CmdExport");

            public static readonly IClassMethod kCmdExportMainEnvMap = new StaticClassMethod<DelightingViewModel>("CmdExportMainEnvMap");

            public static readonly IClassMethod kCmdProcessFromColorCorrection = new StaticClassMethod<DelightingViewModel>("CmdProcessFromColorCorrection");

            public static readonly IClassMethod kCmdProcessFromGather = new StaticClassMethod<DelightingViewModel>("CmdProcessFromGather");

            public static readonly IClassMethod kCmdRenderPreview = new StaticClassMethod<DelightingViewModel>("CmdRenderPreview");

            public static readonly ClassProperty<string> kInputFolderPath = new StaticClassProperty<string, DelightingViewModel>("inputFolderPath");

            public static readonly IClassMethod kCmdLoadInputFolder = new StaticClassMethod<DelightingViewModel>("CmdLoadInputFolder");



        }

        private bool m_FoldInputParameters = true;

        private bool m_FoldOptionalParameters = true;

        private bool m_SwitchYZaxes;

        private Texture2D m_BaseTexture;

        private Texture2D m_NormalsTexture;

        private Texture2D m_BentNormalsTexture;

        private Texture2D m_AmbientOcclusionTexture;

        private Texture2D m_PositionTexture;

        private bool m_FoldDelightingParameters = true;

        private float m_SmoothNormals = 0;

        private float m_RemoveHighlights = 0.3f;

        private float m_RemoveDarkNoise = 0.0f;

        private float m_SeparateDarkAreas = 0.0f;

        private float m_ForceLocalDelighting = 0.0f;

        private Texture2D m_MaskTexture;

        private float m_CompareViewLerp = 0.5f;

        private DelightingViewMode m_LeftViewMode = DelightingViewMode.Base;

        private DelightingViewMode m_RightViewMode = DelightingViewMode.BaseUnlit;

        private DelightingUI.Mode m_DisplayedUIMode;

        private DelightingUI.Display m_DisplayedUI;

        private bool m_AutoCompute;

        private bool m_HasValidInput;

        private Delighting.ErrorCode m_InputErrorCode;

        private bool m_LoadingShow;

        private float m_LoadingProgress;

        private string m_LoadingContent;

        private List<string> m_InspectorErrorMessages = new List<string>();

        private bool m_OverrideReferenceZone;

        private ComputeBuffer m_LatLongAverage;

        private RenderTexture m_PreviewTexture;

        private Vector2 m_CameraPosition;

        private float m_Zoom = 1;

        private Vector2 m_ExposureGizmoPosition;

        private float m_ExposureGizmoRadius = 256;

        private bool m_FitCanvasToWindow;

        private bool m_FoldDebugParameters = true;

        private RenderTexture m_LatLongA;

        private RenderTexture m_BakedLUT;

        private float m_LatLongExposure = 1;

        private Vector4 m_SafetyZoneParams;

        private bool m_FoldImportParameters;

        private string m_BaseTextureSuffix = DelightingService.kDefaultBaseTextureSuffix;

        private string m_NormalsTextureSuffix = DelightingService.kDefaultNormalsTextureSuffix;

        private string m_BentNormalsTextureSuffix = DelightingService.kDefaultBentNormalsTextureSuffix;

        private string m_AmbientOcclusionTextureSuffix = DelightingService.kDefaultAmbientOcclusionTextureSuffix;

        private string m_PositionsTextureSuffix = DelightingService.kDefaultPositionTextureSuffix;

        private string m_MaskTextureSuffix = DelightingService.kDefaultMaskTextureSuffix;

        private string m_InputFolderPath;


        public  bool foldInputParameters
        {
            get
            {
                return (bool)m_FoldInputParameters;
            }
            set
            {
                if (SetFoldInputParameters(value)) { SetPropertyChanged(Properties.kFoldInputParameters); }
            }
        }

        public  bool foldOptionalParameters
        {
            get
            {
                return (bool)m_FoldOptionalParameters;
            }
            set
            {
                if (SetFoldOptionalParameters(value)) { SetPropertyChanged(Properties.kFoldOptionalParameters); }
            }
        }

        public  bool switchYZaxes
        {
            get
            {
                return (bool)m_SwitchYZaxes;
            }
            set
            {
                if (SetSwitchYZaxes(value)) { SetPropertyChanged(Properties.kSwitchYZaxes, Properties.kSafetyZoneParams); }
            }
        }

        public  Texture2D baseTexture
        {
            get
            {
                return (Texture2D)m_BaseTexture;
            }
            set
            {
                if (SetBaseTexture(value)) { SetPropertyChanged(Properties.kBaseTexture); }
            }
        }

        public  Texture2D normalsTexture
        {
            get
            {
                return (Texture2D)m_NormalsTexture;
            }
            set
            {
                if (SetNormalsTexture(value)) { SetPropertyChanged(Properties.kNormalsTexture); }
            }
        }

        public  Texture2D bentNormalsTexture
        {
            get
            {
                return (Texture2D)m_BentNormalsTexture;
            }
            set
            {
                if (SetBentNormalsTexture(value)) { SetPropertyChanged(Properties.kBentNormalsTexture, Properties.kSafetyZoneParams); }
            }
        }

        public  Texture2D ambientOcclusionTexture
        {
            get
            {
                return (Texture2D)m_AmbientOcclusionTexture;
            }
            set
            {
                if (SetAmbientOcclusionTexture(value)) { SetPropertyChanged(Properties.kAmbientOcclusionTexture); }
            }
        }

        public  Texture2D positionTexture
        {
            get
            {
                return (Texture2D)m_PositionTexture;
            }
            set
            {
                if (SetPositionTexture(value)) { SetPropertyChanged(Properties.kPositionTexture); }
            }
        }

        public  bool foldDelightingParameters
        {
            get
            {
                return (bool)m_FoldDelightingParameters;
            }
            set
            {
                if (SetFoldDelightingParameters(value)) { SetPropertyChanged(Properties.kFoldDelightingParameters); }
            }
        }

        public  float smoothNormals
        {
            get
            {
                return (float)m_SmoothNormals;
            }
            set
            {
                if (SetSmoothNormals(value)) { SetPropertyChanged(Properties.kSmoothNormals); }
            }
        }

        public  float removeHighlights
        {
            get
            {
                return (float)m_RemoveHighlights;
            }
            set
            {
                if (SetRemoveHighlights(value)) { SetPropertyChanged(Properties.kRemoveHighlights); }
            }
        }

        public  float removeDarkNoise
        {
            get
            {
                return (float)m_RemoveDarkNoise;
            }
            set
            {
                if (SetRemoveDarkNoise(value)) { SetPropertyChanged(Properties.kRemoveDarkNoise); }
            }
        }

        public  float separateDarkAreas
        {
            get
            {
                return (float)m_SeparateDarkAreas;
            }
            set
            {
                if (SetSeparateDarkAreas(value)) { SetPropertyChanged(Properties.kSeparateDarkAreas); }
            }
        }

        public  float forceLocalDelighting
        {
            get
            {
                return (float)m_ForceLocalDelighting;
            }
            set
            {
                if (SetForceLocalDelighting(value)) { SetPropertyChanged(Properties.kForceLocalDelighting); }
            }
        }

        public  Texture2D maskTexture
        {
            get
            {
                return (Texture2D)m_MaskTexture;
            }
            set
            {
                if (SetMaskTexture(value)) { SetPropertyChanged(Properties.kMaskTexture); }
            }
        }

        public  float compareViewLerp
        {
            get
            {
                return (float)m_CompareViewLerp;
            }
            set
            {
                if (SetCompareViewLerp(value)) { SetPropertyChanged(Properties.kCompareViewLerp); }
            }
        }

        public  DelightingViewMode leftViewMode
        {
            get
            {
                return (DelightingViewMode)m_LeftViewMode;
            }
            set
            {
                if (SetLeftViewMode(value)) { SetPropertyChanged(Properties.kLeftViewMode); }
            }
        }

        public  DelightingViewMode rightViewMode
        {
            get
            {
                return (DelightingViewMode)m_RightViewMode;
            }
            set
            {
                if (SetRightViewMode(value)) { SetPropertyChanged(Properties.kRightViewMode); }
            }
        }

        public  DelightingUI.Mode displayedUIMode
        {
            get
            {
                return (DelightingUI.Mode)m_DisplayedUIMode;
            }
            set
            {
                if (SetDisplayedUIMode(value)) { SetPropertyChanged(Properties.kDisplayedUIMode, Properties.kDisplayedUI); }
            }
        }

        public  DelightingUI.Display displayedUI
        {
            get
            {
                return GetDisplaysFor(displayedUIMode);
            }
        }

        public  bool autoCompute
        {
            get
            {
                return (bool)m_AutoCompute;
            }
            set
            {
                if (SetAutoCompute(value)) { SetPropertyChanged(Properties.kAutoCompute); }
            }
        }

        public  bool hasValidInput
        {
            get
            {
                return m_Service.ValidateInputs() == 0;;
            }
        }

        public  Delighting.ErrorCode inputErrorCode
        {
            get
            {
                return m_Service.ValidateInputs();
            }
        }

        public  bool loadingShow
        {
            get
            {
                return (bool)m_LoadingShow;
            }
            set
            {
                if (SetLoadingShow(value)) { SetPropertyChanged(Properties.kLoadingShow); }
            }
        }

        public  float loadingProgress
        {
            get
            {
                return (float)m_LoadingProgress;
            }
            set
            {
                if (SetLoadingProgress(value)) { SetPropertyChanged(Properties.kLoadingProgress); }
            }
        }

        public  string loadingContent
        {
            get
            {
                return (string)m_LoadingContent;
            }
            set
            {
                if (SetLoadingContent(value)) { SetPropertyChanged(Properties.kLoadingContent); }
            }
        }

        public  List<string> inspectorErrorMessages
        {
            get
            {
                return GetErrorMessagesFrom(inputErrorCode);;
            }
        }

        public  bool overrideReferenceZone
        {
            get
            {
                return (bool)m_OverrideReferenceZone;
            }
            set
            {
                if (SetOverrideReferenceZone(value)) { SetPropertyChanged(Properties.kOverrideReferenceZone); }
            }
        }

        public  ComputeBuffer latLongAverage
        {
            get
            {
                return (ComputeBuffer)m_LatLongAverage;
            }
            set
            {
                if (SetLatLongAverage(value)) { SetPropertyChanged(Properties.kLatLongAverage); }
            }
        }

        public  RenderTexture previewTexture
        {
            get
            {
                return (RenderTexture)m_PreviewTexture;
            }
            set
            {
                if (SetPreviewTexture(value)) { SetPropertyChanged(Properties.kPreviewTexture); }
            }
        }

        public  Vector2 cameraPosition
        {
            get
            {
                return (Vector2)m_CameraPosition;
            }
            set
            {
                if (SetCameraPosition(value)) { SetPropertyChanged(Properties.kCameraPosition); }
            }
        }

        public  float zoom
        {
            get
            {
                return (float)m_Zoom;
            }
            set
            {
                if (SetZoom(value)) { SetPropertyChanged(Properties.kZoom); }
            }
        }

        public  Vector2 exposureGizmoPosition
        {
            get
            {
                return (Vector2)m_ExposureGizmoPosition;
            }
            set
            {
                if (SetExposureGizmoPosition(value)) { SetPropertyChanged(Properties.kExposureGizmoPosition, Properties.kSafetyZoneParams); }
            }
        }

        public  float exposureGizmoRadius
        {
            get
            {
                return (float)m_ExposureGizmoRadius;
            }
            set
            {
                if (SetExposureGizmoRadius(value)) { SetPropertyChanged(Properties.kExposureGizmoRadius, Properties.kSafetyZoneParams); }
            }
        }

        public  bool fitCanvasToWindow
        {
            get
            {
                return (bool)m_FitCanvasToWindow;
            }
            set
            {
                if (SetFitCanvasToWindow(value)) { SetPropertyChanged(Properties.kFitCanvasToWindow); }
            }
        }

        public  bool foldDebugParameters
        {
            get
            {
                return (bool)m_FoldDebugParameters;
            }
            set
            {
                if (SetFoldDebugParameters(value)) { SetPropertyChanged(Properties.kFoldDebugParameters); }
            }
        }

        public  RenderTexture latLongA
        {
            get
            {
                return (RenderTexture)m_LatLongA;
            }
            set
            {
                if (SetLatLongA(value)) { SetPropertyChanged(Properties.kLatLongA); }
            }
        }

        public  RenderTexture bakedLUT
        {
            get
            {
                return (RenderTexture)m_BakedLUT;
            }
            set
            {
                if (SetBakedLUT(value)) { SetPropertyChanged(Properties.kBakedLUT); }
            }
        }

        public  float latLongExposure
        {
            get
            {
                return (float)m_LatLongExposure;
            }
            set
            {
                if (SetLatLongExposure(value)) { SetPropertyChanged(Properties.kLatLongExposure); }
            }
        }

        public  Vector4 safetyZoneParams
        {
            get
            {
                
            return new Vector4(
                exposureGizmoPosition.x / bentNormalsTexture.width,
                1 - (exposureGizmoPosition.y / bentNormalsTexture.height),
                exposureGizmoRadius,
                switchYZaxes ? 1 : 0)
        ;
            }
        }

        public  bool foldImportParameters
        {
            get
            {
                return (bool)m_FoldImportParameters;
            }
            set
            {
                if (SetFoldImportParameters(value)) { SetPropertyChanged(Properties.kFoldImportParameters); }
            }
        }

        public  string baseTextureSuffix
        {
            get
            {
                return (string)m_BaseTextureSuffix;
            }
            set
            {
                if (SetBaseTextureSuffix(value)) { SetPropertyChanged(Properties.kBaseTextureSuffix); }
            }
        }

        public  string normalsTextureSuffix
        {
            get
            {
                return (string)m_NormalsTextureSuffix;
            }
            set
            {
                if (SetNormalsTextureSuffix(value)) { SetPropertyChanged(Properties.kNormalsTextureSuffix); }
            }
        }

        public  string bentNormalsTextureSuffix
        {
            get
            {
                return (string)m_BentNormalsTextureSuffix;
            }
            set
            {
                if (SetBentNormalsTextureSuffix(value)) { SetPropertyChanged(Properties.kBentNormalsTextureSuffix); }
            }
        }

        public  string ambientOcclusionTextureSuffix
        {
            get
            {
                return (string)m_AmbientOcclusionTextureSuffix;
            }
            set
            {
                if (SetAmbientOcclusionTextureSuffix(value)) { SetPropertyChanged(Properties.kAmbientOcclusionTextureSuffix); }
            }
        }

        public  string positionsTextureSuffix
        {
            get
            {
                return (string)m_PositionsTextureSuffix;
            }
            set
            {
                if (SetPositionsTextureSuffix(value)) { SetPropertyChanged(Properties.kPositionsTextureSuffix); }
            }
        }

        public  string maskTextureSuffix
        {
            get
            {
                return (string)m_MaskTextureSuffix;
            }
            set
            {
                if (SetMaskTextureSuffix(value)) { SetPropertyChanged(Properties.kMaskTextureSuffix); }
            }
        }

        public  string inputFolderPath
        {
            get
            {
                return (string)m_InputFolderPath;
            }
            set
            {
                if (SetInputFolderPath(value)) { SetPropertyChanged(Properties.kInputFolderPath); }
            }
        }


        private  bool SetFoldInputParameters(bool value)
        {
            if (m_FoldInputParameters != value)
            {
                m_FoldInputParameters = value;
                return true;
            }
            return false;
        }

        private  bool SetFoldOptionalParameters(bool value)
        {
            if (m_FoldOptionalParameters != value)
            {
                m_FoldOptionalParameters = value;
                return true;
            }
            return false;
        }

        private  bool SetSwitchYZaxes(bool value)
        {
            if (m_SwitchYZaxes != value)
            {
                m_SwitchYZaxes = value;
                return true;
            }
            return false;
        }

        private  bool SetBaseTexture(Texture2D value)
        {
            if (m_BaseTexture != value)
            {
                m_BaseTexture = value;
                return true;
            }
            return false;
        }

        private  bool SetNormalsTexture(Texture2D value)
        {
            if (m_NormalsTexture != value)
            {
                m_NormalsTexture = value;
                return true;
            }
            return false;
        }

        private  bool SetBentNormalsTexture(Texture2D value)
        {
            if (m_BentNormalsTexture != value)
            {
                m_BentNormalsTexture = value;
                return true;
            }
            return false;
        }

        private  bool SetAmbientOcclusionTexture(Texture2D value)
        {
            if (m_AmbientOcclusionTexture != value)
            {
                m_AmbientOcclusionTexture = value;
                return true;
            }
            return false;
        }

        private  bool SetPositionTexture(Texture2D value)
        {
            if (m_PositionTexture != value)
            {
                m_PositionTexture = value;
                return true;
            }
            return false;
        }

        private  bool SetFoldDelightingParameters(bool value)
        {
            if (m_FoldDelightingParameters != value)
            {
                m_FoldDelightingParameters = value;
                return true;
            }
            return false;
        }

        private  bool SetSmoothNormals(float value)
        {
            if (m_SmoothNormals != value)
            {
                m_SmoothNormals = value;
                return true;
            }
            return false;
        }

        private  bool SetRemoveHighlights(float value)
        {
            if (m_RemoveHighlights != value)
            {
                m_RemoveHighlights = value;
                return true;
            }
            return false;
        }

        private  bool SetRemoveDarkNoise(float value)
        {
            if (m_RemoveDarkNoise != value)
            {
                m_RemoveDarkNoise = value;
                return true;
            }
            return false;
        }

        private  bool SetSeparateDarkAreas(float value)
        {
            if (m_SeparateDarkAreas != value)
            {
                m_SeparateDarkAreas = value;
                return true;
            }
            return false;
        }

        private  bool SetForceLocalDelighting(float value)
        {
            if (m_ForceLocalDelighting != value)
            {
                m_ForceLocalDelighting = value;
                return true;
            }
            return false;
        }

        private  bool SetMaskTexture(Texture2D value)
        {
            if (m_MaskTexture != value)
            {
                m_MaskTexture = value;
                return true;
            }
            return false;
        }

        private  bool SetCompareViewLerp(float value)
        {
            if (m_CompareViewLerp != value)
            {
                m_CompareViewLerp = value;
                return true;
            }
            return false;
        }

        private  bool SetLeftViewMode(DelightingViewMode value)
        {
            if (m_LeftViewMode != value)
            {
                m_LeftViewMode = value;
                return true;
            }
            return false;
        }

        private  bool SetRightViewMode(DelightingViewMode value)
        {
            if (m_RightViewMode != value)
            {
                m_RightViewMode = value;
                return true;
            }
            return false;
        }

        private  bool SetDisplayedUIMode(DelightingUI.Mode value)
        {
            if (m_DisplayedUIMode != value)
            {
                m_DisplayedUIMode = value;
                return true;
            }
            return false;
        }

        private  bool SetAutoCompute(bool value)
        {
            if (m_AutoCompute != value)
            {
                m_AutoCompute = value;
                return true;
            }
            return false;
        }

        private  bool SetLoadingShow(bool value)
        {
            if (m_LoadingShow != value)
            {
                m_LoadingShow = value;
                return true;
            }
            return false;
        }

        private  bool SetLoadingProgress(float value)
        {
            if (m_LoadingProgress != value)
            {
                m_LoadingProgress = value;
                return true;
            }
            return false;
        }

        private  bool SetLoadingContent(string value)
        {
            if (m_LoadingContent != value)
            {
                m_LoadingContent = value;
                return true;
            }
            return false;
        }

        private  bool SetOverrideReferenceZone(bool value)
        {
            if (m_OverrideReferenceZone != value)
            {
                m_OverrideReferenceZone = value;
                return true;
            }
            return false;
        }

        private  bool SetLatLongAverage(ComputeBuffer value)
        {
            if (m_LatLongAverage != value)
            {
                m_LatLongAverage = value;
                return true;
            }
            return false;
        }

        private  bool SetPreviewTexture(RenderTexture value)
        {
            if (m_PreviewTexture != value)
            {
                m_PreviewTexture = value;
                return true;
            }
            return false;
        }

        private  bool SetCameraPosition(Vector2 value)
        {
            if (m_CameraPosition != value)
            {
                m_CameraPosition = value;
                return true;
            }
            return false;
        }

        private  bool SetZoom(float value)
        {
            if (m_Zoom != value)
            {
                m_Zoom = value;
                return true;
            }
            return false;
        }

        private  bool SetExposureGizmoPosition(Vector2 value)
        {
            
            if (m_ExposureGizmoPosition != value)
            {
                m_ExposureGizmoPosition = value;
                if (baseTexture != null)
                {
                    m_ExposureGizmoPosition.x = Mathf.Clamp(value.x, 0, baseTexture.width);
                    m_ExposureGizmoPosition.y = Mathf.Clamp(value.y, 0, baseTexture.height);
                }
                return true;
            }
            return false;
        
        }

        private  bool SetExposureGizmoRadius(float value)
        {
            if (m_ExposureGizmoRadius != value)
            {
                m_ExposureGizmoRadius = value;
                return true;
            }
            return false;
        }

        private  bool SetFitCanvasToWindow(bool value)
        {
            if (m_FitCanvasToWindow != value)
            {
                m_FitCanvasToWindow = value;
                return true;
            }
            return false;
        }

        private  bool SetFoldDebugParameters(bool value)
        {
            if (m_FoldDebugParameters != value)
            {
                m_FoldDebugParameters = value;
                return true;
            }
            return false;
        }

        private  bool SetLatLongA(RenderTexture value)
        {
            if (m_LatLongA != value)
            {
                m_LatLongA = value;
                return true;
            }
            return false;
        }

        private  bool SetBakedLUT(RenderTexture value)
        {
            if (m_BakedLUT != value)
            {
                m_BakedLUT = value;
                return true;
            }
            return false;
        }

        private  bool SetLatLongExposure(float value)
        {
            if (m_LatLongExposure != value)
            {
                m_LatLongExposure = value;
                return true;
            }
            return false;
        }

        private  bool SetFoldImportParameters(bool value)
        {
            if (m_FoldImportParameters != value)
            {
                m_FoldImportParameters = value;
                return true;
            }
            return false;
        }

        private  bool SetBaseTextureSuffix(string value)
        {
            if (m_BaseTextureSuffix != value)
            {
                m_BaseTextureSuffix = value;
                return true;
            }
            return false;
        }

        private  bool SetNormalsTextureSuffix(string value)
        {
            if (m_NormalsTextureSuffix != value)
            {
                m_NormalsTextureSuffix = value;
                return true;
            }
            return false;
        }

        private  bool SetBentNormalsTextureSuffix(string value)
        {
            if (m_BentNormalsTextureSuffix != value)
            {
                m_BentNormalsTextureSuffix = value;
                return true;
            }
            return false;
        }

        private  bool SetAmbientOcclusionTextureSuffix(string value)
        {
            if (m_AmbientOcclusionTextureSuffix != value)
            {
                m_AmbientOcclusionTextureSuffix = value;
                return true;
            }
            return false;
        }

        private  bool SetPositionsTextureSuffix(string value)
        {
            if (m_PositionsTextureSuffix != value)
            {
                m_PositionsTextureSuffix = value;
                return true;
            }
            return false;
        }

        private  bool SetMaskTextureSuffix(string value)
        {
            if (m_MaskTextureSuffix != value)
            {
                m_MaskTextureSuffix = value;
                return true;
            }
            return false;
        }

        private  bool SetInputFolderPath(string value)
        {
            if (m_InputFolderPath != value)
            {
                m_InputFolderPath = value;
                return true;
            }
            return false;
        }

    }

}
#pragma warning restore 414
