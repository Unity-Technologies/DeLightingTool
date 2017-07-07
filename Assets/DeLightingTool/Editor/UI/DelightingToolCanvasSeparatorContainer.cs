using System;
using UnityEditor.Experimental.VisualElements;
using UnityEngine;

namespace UnityEditor.Experimental.DelightingInternal
{
    partial class DelightingToolCanvasSeparatorContainer : IMGUIVisualContainer
    {
        [Flags]
        enum DrawCondition
        {
            None = 0,
            Hot = 1 << 0,
            NotHot = 1 << 1
        }

        static class Styles
        {
            internal static readonly Color separatorColor;
            internal static readonly Color separatorHighlightColor;
            internal const float kSeparatorWidth = 1;

            static Styles()
            {
                if (EditorGUIUtility.isProSkin)
                {
                    separatorColor = new Color(32f / 255f, 32f / 255f, 32f / 255f);
                    separatorHighlightColor = new Color(20f / 255f, 20f / 255f, 20f / 255f);
                }
                else
                {
                    separatorColor = new Color(32f / 255f, 32f / 255f, 32f / 255f);
                    separatorHighlightColor = new Color(20f / 255f, 20f / 255f, 20f / 255f);
                }
            }
        }

        static readonly int kSeparatorHash = "SeparatorHash".GetHashCode();

        DrawCondition m_DrawCondition = DrawCondition.NotHot | DrawCondition.Hot;

        public override void OnGUI()
        {
            var controlId = EditorGUIUtility.GetControlID(kSeparatorHash, FocusType.Passive);
            var evt = Event.current;

            var texture = GetValue(kPreviewTexture);
            var width = texture != null ? texture.width : 256;
            var height = texture != null ? texture.height : 256;

            var cameraPosition = GetValue(kCameraPosition);
            var zoom = GetValue(kZoom);
            var separator = GetValue(kCompareViewLerp);
            var separatorRect = new Rect(cameraPosition.x + width * zoom * separator, cameraPosition.y, Styles.kSeparatorWidth, height * zoom);

            switch (evt.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    if (evt.button == 0)
                    {
                        EditorGUIUtility.hotControl = controlId;
                        EditorGUIUtility.keyboardControl = 0;

                        if (SetValue(kCompareViewLerp, Mathf.Clamp01((evt.mousePosition.x - cameraPosition.x) / (width * zoom))))
                            ExecuteCommand(kCmdRenderPreview);
                    }
                    break;
                case EventType.MouseDrag:
                    {
                        if (evt.button == 0 && EditorGUIUtility.hotControl == controlId)
                        {
                            if (SetValue(kCompareViewLerp, Mathf.Clamp01((evt.mousePosition.x - cameraPosition.x) / (width * zoom))))
                                ExecuteCommand(kCmdRenderPreview);
                        }
                        break;
                    }
                case EventType.mouseUp:
                    if (evt.button == 0 && EditorGUIUtility.hotControl == controlId)
                    {
                        EditorGUIUtility.hotControl = 0;
                        Repaint();
                    }
                    break;
                case EventType.Repaint:
                    {
                        if ((m_DrawCondition & DrawCondition.NotHot) != 0 && EditorGUIUtility.hotControl != controlId
                            || (m_DrawCondition & DrawCondition.Hot) != 0 && EditorGUIUtility.hotControl == controlId)
                        {
                            var tmpCol = GUI.color;
                            GUI.color = Styles.separatorColor;
                            GUI.DrawTexture(separatorRect, Texture2D.whiteTexture, ScaleMode.StretchToFill);
                            GUI.color = tmpCol;
                        }
                        break;
                    }
            }
        }
    }
}
