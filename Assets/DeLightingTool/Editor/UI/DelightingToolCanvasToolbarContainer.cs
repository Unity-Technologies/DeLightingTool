using System;
using UnityEditor.Experimental.VisualElements;
using UnityEngine;

namespace UnityEditor.Experimental.DelightingInternal
{
    partial class DelightingToolCanvasToolbarContainer : IMGUIVisualContainer
    {
        static class Content
        {
            public static GUIContent fitToWindowLabel { get { return EditorGUIUtility.IconContent("EditorGUITools/maximize.png", "Fit To Window"); } }
            public static GUIContent exposureGizmoLabel { get { return EditorGUIUtility.IconContent("EditorGUITools/pick.png", "Reference Gizmo"); } }
            public static GUIContent resetExposureLabel { get { return EditorGUIUtility.IconContent("EditorGUITools/reset.png", "Reset Reference"); } }
        }

        public override void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            var hasPreviewTexture = GetValue(kPreviewTexture) != null;
            GUI.enabled = hasPreviewTexture;
            SetValue(kOverrideReferenceZone, GUILayout.Toggle(GetValue(kOverrideReferenceZone), Content.exposureGizmoLabel, EditorStyles.toolbarButton));

            if (GUILayout.Button(Content.resetExposureLabel, EditorStyles.toolbarButton))
                ExecuteCommand(kCmdResetExposureZone);

            if (GUILayout.Button(Content.fitToWindowLabel, EditorStyles.toolbarButton))
                SetValue(kFitCanvasToWindow, true);

            GUILayout.FlexibleSpace();

            var shouldRenderPreview = false;
            var showViewModes = (GetValue(kDisplayedUI) & DelightingUI.Display.FieldViewMode) != 0;
            if (showViewModes)
                shouldRenderPreview |= SetValue(kLeftViewMode, (DelightingViewMode)EditorGUILayout.EnumPopup(GetValue(kLeftViewMode), EditorStyles.toolbarPopup, GUILayout.Width(130f)));
            shouldRenderPreview |= SetValue(kCompareViewLerp, GUILayout.HorizontalSlider(GetValue(kCompareViewLerp), 0f, 1f, DelightingStyles.preSlider, DelightingStyles.preSliderThumb, GUILayout.Width(140f)));
            if (showViewModes)
                shouldRenderPreview |= SetValue(kRightViewMode, (DelightingViewMode)EditorGUILayout.EnumPopup(GetValue(kRightViewMode), EditorStyles.toolbarPopup, GUILayout.Width(130f)));

            if (shouldRenderPreview)
                ExecuteCommand(kCmdRenderPreview);
            GUI.enabled = true;

            GUILayout.FlexibleSpace();

            SetValue(kDisplayedUIMode, (DelightingUI.Mode)EditorGUILayout.EnumPopup(GetValue(kDisplayedUIMode), EditorStyles.toolbarDropDown));

            GUILayout.EndHorizontal();
        }
    }
}
