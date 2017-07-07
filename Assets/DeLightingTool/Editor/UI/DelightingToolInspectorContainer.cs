using System;
using UnityEditor.Experimental.VisualElements;
using UnityEngine;

namespace UnityEditor.Experimental.DelightingInternal
{
    partial class DelightingToolInspectorContainer : IMGUIVisualContainer, IDisposable
    {
        static class Styles
        {
            internal static readonly GUIStyle largeButton = new GUIStyle("LargeButton") { padding = new RectOffset(10, 10, 10, 10), margin = new RectOffset(10, 10, 20, 20) };
            internal static readonly GUIStyle largeButtonLeft = new GUIStyle("LargeButtonLeft") { padding = new RectOffset(10, 10, 10, 10), margin = new RectOffset(10, 0, 20, 20) };
            internal static readonly GUIStyle largeButtonMid = new GUIStyle("LargeButtonMid") { padding = new RectOffset(10, 10, 10, 10), margin = new RectOffset(0, 0, 20, 20) };
            internal static readonly GUIStyle largeButtonRight = new GUIStyle("LargeButtonRight") { padding = new RectOffset(10, 10, 10, 10), margin = new RectOffset(0, 10, 20, 20) };
            internal static readonly GUIStyle separator = new GUIStyle("IN Title") { fixedHeight = 4 };
        }

        static class Content
        {
            internal static readonly GUIContent inputParametersTitle = new GUIContent("Input Parameters");
            internal static readonly GUIContent optionalParametersTitle = new GUIContent("Optional Parameters");
            internal static readonly GUIContent delightingParametersTitle = new GUIContent("Delighting Parameters");
            internal static readonly GUIContent debugPanelTitle = new GUIContent("Debug Panel");
            internal static readonly GUIContent importPanelTitle = new GUIContent("Import Settings");
            internal static readonly GUIContent computeLabel = new GUIContent("Compute");
            internal static readonly GUIContent exportLabel = new GUIContent("Export");

            internal static readonly GUIContent switchYZLabel = new GUIContent("Switch Y/Z Axes");
            internal static readonly GUIContent baseTextureLabel = new GUIContent("Base");
            internal static readonly GUIContent normalsTextureLabel = new GUIContent("Normals");
            internal static readonly GUIContent bentNormalsTextureLabel = new GUIContent("Bent Normals");
            internal static readonly GUIContent ambientOcclusionTextureLabel = new GUIContent("Ambient Occlusion");
            internal static readonly GUIContent positionTextureLabel = new GUIContent("Position");
            internal static readonly GUIContent maskLabel = new GUIContent("Mask");
            internal static readonly GUIContent smoothNormalsLabel = new GUIContent("Smooth Normals");
            internal static readonly GUIContent removeHighlightsLabel = new GUIContent("Remove Highlights");
            internal static readonly GUIContent removeDarkNoiseLabel = new GUIContent("Remove Dark Noise");
            internal static readonly GUIContent separateDarkAreasLabel = new GUIContent("Separate Dark Areas");
            internal static readonly GUIContent forceLocalDelightingLabel = new GUIContent("Force Local Delighting");
        }

        const Delighting.ErrorCode kBaseValueError = Delighting.ErrorCode.MissingBaseTexture;
        const Delighting.ErrorCode kNormalsValueError = Delighting.ErrorCode.MissingNormalsTexture | Delighting.ErrorCode.WrongSizeNormalsTexture;
        const Delighting.ErrorCode kBentNormalsValueError = Delighting.ErrorCode.MissingBentNormalsTexture | Delighting.ErrorCode.WrongSizeBentNormalsTexture;
        const Delighting.ErrorCode kAmbientOcclusionValueError = Delighting.ErrorCode.MissingAmbientOcclusionTexture | Delighting.ErrorCode.WrongSizeAmbientOcclusionTexture;
        const Delighting.ErrorCode kPositionValueError = Delighting.ErrorCode.WrongSizePositionTexture;
        const Delighting.ErrorCode kMaskValueError = Delighting.ErrorCode.WrongSizeMaskTexture;

        const Delighting.ErrorCode kInputError = kBaseValueError | kNormalsValueError | kBentNormalsValueError | kAmbientOcclusionValueError;
        const Delighting.ErrorCode kOptionalError = kMaskValueError | kPositionValueError;

        DelightingToolDebugTextureContainer m_DebugTexture = new DelightingToolDebugTextureContainer();

        Vector2 m_ScrollPosition = Vector2.zero;

        public DelightingToolInspectorContainer()
        {
            AddChild(m_DebugTexture);
        }

        public override void OnGUI()
        {
            var displays = GetValue(kDisplayedUI);
            var errorCode = GetValue(kInputErrorCode);

            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            if ((displays & DelightingUI.Display.SectionInput) != 0)
            {
                EditorGUILayout.Space();
                SetValue(kFoldInputParameters, EditorGUIXLayout.InspectorFoldout(GetValue(kFoldInputParameters), Content.inputParametersTitle, DelightingHelpers.GetIcon(errorCode, kInputError)));
                if (GetValue(kFoldInputParameters))
                {
                    ++EditorGUI.indentLevel;
                    if ((displays & DelightingUI.Display.FieldBaseTexture) != 0)
                        SetValue(kBaseTexture, EditorGUIXLayout.InlineObjectField(Content.baseTextureLabel, GetValue(kBaseTexture), DelightingHelpers.GetIcon(errorCode, kBaseValueError)));
                    if ((displays & DelightingUI.Display.FieldNormalsTexture) != 0)
                        SetValue(kNormalsTexture, EditorGUIXLayout.InlineObjectField(Content.normalsTextureLabel, GetValue(kNormalsTexture), DelightingHelpers.GetIcon(errorCode, kNormalsValueError)));
                    if ((displays & DelightingUI.Display.FieldBentNormalsTexture) != 0)
                        SetValue(kBentNormalsTexture, EditorGUIXLayout.InlineObjectField(Content.bentNormalsTextureLabel, GetValue(kBentNormalsTexture), DelightingHelpers.GetIcon(errorCode, kBentNormalsValueError)));
                    if ((displays & DelightingUI.Display.FieldAmbientOcclusionTexture) != 0)
                        SetValue(kAmbientOcclusionTexture, EditorGUIXLayout.InlineObjectField(Content.ambientOcclusionTextureLabel, GetValue(kAmbientOcclusionTexture), DelightingHelpers.GetIcon(errorCode, kAmbientOcclusionValueError)));
                    --EditorGUI.indentLevel;
                }

                EditorGUILayout.Space();
                SetValue(kFoldOptionalParameters, EditorGUIXLayout.InspectorFoldout(GetValue(kFoldOptionalParameters), Content.optionalParametersTitle, DelightingHelpers.GetIcon(errorCode, kOptionalError)));
                if (GetValue(kFoldOptionalParameters))
                {
                    ++EditorGUI.indentLevel;
                    if ((displays & DelightingUI.Display.FieldPositionTexture) != 0)
                        SetValue(kPositionTexture, EditorGUIXLayout.InlineObjectField(Content.positionTextureLabel, GetValue(kPositionTexture), DelightingHelpers.GetIcon(errorCode, kPositionValueError)));
                    if ((displays & DelightingUI.Display.FieldMaskTexture) != 0)
                        SetValue(kMaskTexture, EditorGUIXLayout.InlineObjectField(Content.maskLabel, GetValue(kMaskTexture), DelightingHelpers.GetIcon(errorCode, kMaskValueError)));
                    if ((displays & DelightingUI.Display.FieldSwitchYZAxes) != 0)
                        SetValue(kSwitchYZaxes, EditorGUILayout.Toggle(Content.switchYZLabel, GetValue(kSwitchYZaxes)));
                    --EditorGUI.indentLevel;
                }
            }

            if ((displays & DelightingUI.Display.SectionDelighting) != 0)
            {
                EditorGUILayout.Space();
                SetValue(kFoldDelightingParameters, EditorGUIXLayout.InspectorFoldout(GetValue(kFoldDelightingParameters), Content.delightingParametersTitle));
                if (GetValue(kFoldDelightingParameters))
                {
                    ++EditorGUI.indentLevel;
                    GUI.enabled = GetValue(kPreviewTexture) != null;

                    //if ((displays & DelightingUI.Display.FieldNoiseReduction) != 0)
                       // SetValue(kSmoothNormals, EditorGUILayout.Slider(Content.smoothNormalsLabel, GetValue(kSmoothNormals), 0, 2));
                    if ((displays & DelightingUI.Display.FieldRemoveHighlights) != 0)
                        SetValue(kRemoveHighlights, EditorGUILayout.Slider(Content.removeHighlightsLabel, GetValue(kRemoveHighlights), 0, 1));
                    if ((displays & DelightingUI.Display.FieldRemoveDarkNoise) != 0)
                        SetValue(kRemoveDarkNoise, EditorGUILayout.Slider(Content.removeDarkNoiseLabel, GetValue(kRemoveDarkNoise), 0, 1));
                    if ((displays & DelightingUI.Display.FieldSeparateDarkAreas) != 0)
                        SetValue(kSeparateDarkAreas, EditorGUILayout.Slider(Content.separateDarkAreasLabel, GetValue(kSeparateDarkAreas), 0, 1));
                    if ((displays & DelightingUI.Display.FieldForceLocalDelighting) != 0)
                        SetValue(kForceLocalDelighting, EditorGUILayout.Slider(Content.forceLocalDelightingLabel, GetValue(kForceLocalDelighting), 0, 1));

                    GUI.enabled = true;
                    --EditorGUI.indentLevel;
                }
            }

            if ((displays & DelightingUI.Display.SectionImport) != 0)
            {
                EditorGUILayout.Space();
                SetValue(kFoldImportParameters, EditorGUIXLayout.InspectorFoldout(GetValue(kFoldImportParameters), Content.importPanelTitle));
                if (GetValue(kFoldImportParameters))
                {
                    ++EditorGUI.indentLevel;
                    SetValue(kBaseTextureSuffix, EditorGUILayout.TextField(Content.baseTextureLabel, GetValue(kBaseTextureSuffix)));
                    SetValue(kNormalsTextureSuffix, EditorGUILayout.TextField(Content.normalsTextureLabel, GetValue(kNormalsTextureSuffix)));
                    SetValue(kBentNormalsTextureSuffix, EditorGUILayout.TextField(Content.bentNormalsTextureLabel, GetValue(kBentNormalsTextureSuffix)));
                    SetValue(kAmbientOcclusionTextureSuffix, EditorGUILayout.TextField(Content.ambientOcclusionTextureLabel, GetValue(kAmbientOcclusionTextureSuffix)));
                    SetValue(kPositionsTextureSuffix, EditorGUILayout.TextField(Content.positionTextureLabel, GetValue(kPositionsTextureSuffix)));
                    SetValue(kMaskTextureSuffix, EditorGUILayout.TextField(Content.maskLabel, GetValue(kMaskTextureSuffix)));
                    --EditorGUI.indentLevel;
                }
            }

            var messages = GetValue(kInspectorErrorMessages);
            if (messages != null)
            {
                EditorGUILayout.Space();
                for (int i = 0; i < messages.Count; i++)
                    EditorGUILayout.HelpBox(messages[i], MessageType.Error, true);
            }

            if ((displays & (DelightingUI.Display.ButtonCompute | DelightingUI.Display.ButtonExport)) != 0)
            {
                var computeStyle = Styles.largeButton;
                var exportStyle = Styles.largeButton;

                if ((displays & (DelightingUI.Display.ButtonCompute | DelightingUI.Display.ButtonExport)) == 
                    (DelightingUI.Display.ButtonCompute | DelightingUI.Display.ButtonExport))
                {
                    computeStyle = Styles.largeButtonLeft;
                    exportStyle = Styles.largeButtonRight;
                }

                GUILayout.Box(GUIContent.none, Styles.separator);
                GUI.enabled = GetValue(kHasValidInput);
                GUILayout.BeginHorizontal();
                if ((displays & DelightingUI.Display.ButtonCompute) != 0
                    && GUILayout.Button(Content.computeLabel, computeStyle))
                    ExecuteCommand(kCmdProcessFromGather);
                if ((displays & DelightingUI.Display.ButtonExport) != 0
                    && GUILayout.Button(Content.exportLabel, exportStyle))
                    ExecuteCommand(kCmdExport);
                GUILayout.EndHorizontal();
            }
            GUI.enabled = true;

            if ((displays & DelightingUI.Display.SectionDebug) != 0)
            {
                EditorGUILayout.Space();
                SetValue(kFoldDebugParameters, EditorGUIXLayout.InspectorFoldout(GetValue(kFoldDebugParameters), Content.debugPanelTitle));
                if (GetValue(kFoldDebugParameters))
                {
                    ++EditorGUI.indentLevel;
                    m_DebugTexture.OnGUI();
                    --EditorGUI.indentLevel;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        public override void Dispose()
        {
            base.Dispose();
            RemoveChild(m_DebugTexture);
            m_DebugTexture.Dispose();
            m_DebugTexture = null;
        }
    }
}
