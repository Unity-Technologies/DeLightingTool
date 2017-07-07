using UnityEngine;

namespace UnityEditor.Experimental
{
    public static partial class EditorGUIXLayout
    {
        public static float VerticalHandle(int controlId, float size, float minSize = float.MinValue, float maxSize = float.MaxValue)
        {
            var rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUIX.Styles.handleSize));
            return EditorGUIX.VerticalHandle(controlId, rect, size, minSize, maxSize);
        }

        public static float HorizontalHandle(int controlId, float size, float minSize = float.MinValue, float maxSize = float.MaxValue)
        {
            var rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Width(EditorGUIX.Styles.handleSize), GUILayout.ExpandHeight(true));
            return EditorGUIX.HorizontalHandle(controlId, rect, size, minSize, maxSize);
        }
    }

    public static partial class EditorGUIX
    {
        public static partial class Styles
        {
            public const float handleSize = 2;
        }

        static float s_StartSize;
        static Vector2 s_MouseDragStartPosition;

        public static float VerticalHandle(int controlId, Rect handleRect, float size, float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            return ResizeInternal(controlId, handleRect, size, minValue, maxValue, false);
        }

        public static float HorizontalHandle(int controlId, Rect handleRect, float size, float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            return ResizeInternal(controlId, handleRect, size, minValue, maxValue, true);
        }

        internal static float ResizeInternal(int controlId, Rect handleRect, float size, float minSize, float maxSize, bool horizontal)
        {
            var axis = horizontal ? 0 : 1;
            Event evt = Event.current;
            switch (evt.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    if (GUIUtility.hotControl == 0 && handleRect.Contains(evt.mousePosition) && evt.button == 0)
                    {
                        GUIUtility.hotControl = controlId;
                        GUIUtility.keyboardControl = 0;
                        s_MouseDragStartPosition = evt.mousePosition;
                        s_StartSize = size;
                        evt.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlId)
                    {
                        evt.Use();
                        var screenPos = evt.mousePosition;
                        var delta = screenPos[axis] - s_MouseDragStartPosition[axis];
                        size = Mathf.Clamp(s_StartSize + delta, minSize, maxSize);
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlId && evt.button == 0)
                    {
                        GUIUtility.hotControl = 0;
                        evt.Use();
                    }
                    break;
                case EventType.Repaint:
                    var cursor = horizontal ? MouseCursor.SplitResizeLeftRight : MouseCursor.SplitResizeUpDown;
                    EditorGUIUtility.AddCursorRect(handleRect, cursor, controlId);

                    var orgColor = GUI.color;
                    var tintColor = (EditorGUIUtility.isProSkin) ? new Color(0.12f, 0.12f, 0.12f, 1.333f) : new Color(0.6f, 0.6f, 0.6f, 1.333f);
                    GUI.color = GUI.color * tintColor;
                    GUI.DrawTexture(handleRect, EditorGUIUtility.whiteTexture);
                    GUI.color = orgColor;
                    break;
            }

            return Mathf.Clamp(size, minSize, maxSize);
        }
    }
}
