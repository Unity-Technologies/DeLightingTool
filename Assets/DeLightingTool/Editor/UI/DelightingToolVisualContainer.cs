using System;
using UnityEditor.Experimental.VisualElements;
using UnityEngine;

namespace UnityEditor.Experimental.DelightingInternal
{
    partial class DelightingToolVisualContainer : IMGUIVisualContainer, IDisposable
    {
        static readonly int kResizeHandle = "ResizeHandleHash".GetHashCode();
        static RectOffset _kPadding = null;
        static RectOffset kPadding { get { return _kPadding ?? (_kPadding = new RectOffset(8, 8, 8, 8)); } }

        const float kMinSplitterPosition = 300;
        const float kMaxSplitterPosition = 500;

        DelightingToolCanvasToolbarContainer m_CanvasToolbar = new DelightingToolCanvasToolbarContainer();
        DelightingToolInspectorToolbarContainer m_InspectorToolbar = new DelightingToolInspectorToolbarContainer();
        DelightingToolInspectorContainer m_Inspector = new DelightingToolInspectorContainer();
        DelightingToolCanvasContainer m_Canvas = new DelightingToolCanvasContainer();

        float m_SpliterPosition = 300;

        public DelightingToolVisualContainer()
        {
            AddChild(m_CanvasToolbar);
            AddChild(m_InspectorToolbar);
            AddChild(m_Inspector);
            AddChild(m_Canvas);
        }

        public override void OnGUI()
        {
            var resizeHandleId = EditorGUIUtility.GetControlID(kResizeHandle, FocusType.Passive);

            GUILayout.BeginHorizontal();
            GUILayout.BeginHorizontal(GUILayout.Width(m_SpliterPosition));
            m_InspectorToolbar.OnGUI();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            m_CanvasToolbar.OnGUI();
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(m_SpliterPosition));
            m_Inspector.OnGUI();
            GUILayout.EndVertical();

            m_SpliterPosition = EditorGUIXLayout.HorizontalHandle(resizeHandleId, m_SpliterPosition, kMinSplitterPosition, kMaxSplitterPosition);

            m_Canvas.OnGUI();
            GUILayout.EndHorizontal();

            var progressRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (GetValue(kLoadingShow))
                EditorGUI.ProgressBar(progressRect, GetValue(kLoadingProgress), GetValue(kLoadingContent));
            else
                EditorGUI.ProgressBar(progressRect, 0, string.Empty);
        }

        public override void Dispose()
        {
            base.Dispose();
            RemoveChild(m_Inspector);
            m_Inspector.Dispose();
            m_Inspector = null;
            RemoveChild(m_CanvasToolbar);
            m_Canvas.Dispose();
            m_Canvas = null;
            RemoveChild(m_InspectorToolbar);
            m_InspectorToolbar.Dispose();
            m_InspectorToolbar = null;
            RemoveChild(m_Canvas);
            m_Canvas.Dispose();
            m_Canvas = null;
        }
    }
}
