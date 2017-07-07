using System;
using UnityEditor.Experimental.VisualElements;
using UnityEngine;

namespace UnityEditor.Experimental.DelightingInternal
{
    partial class DelightingToolInspectorToolbarContainer : IMGUIVisualContainer
    {
        static class Content
        {
            public static GUIContent autoComputeLabel { get { return EditorGUIUtility.IconContent("EditorGUITools/automatic.png", "Auto Compute"); } }
        }

        public override void OnGUI()
        {
            var displays = GetValue(kDisplayedUI);
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            if ((displays & DelightingUI.Display.ButtonAutoCompute) != 0)
                SetValue(kAutoCompute, GUILayout.Toggle(GetValue(kAutoCompute), Content.autoComputeLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)));
            GUILayout.EndHorizontal();
        }
    }
}
