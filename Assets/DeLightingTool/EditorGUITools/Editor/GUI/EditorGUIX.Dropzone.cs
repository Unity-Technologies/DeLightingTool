using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Experimental
{
    using UnityObject = UnityEngine.Object;

    public static partial class EditorGUIXLayout
    {
        public static bool DropZone(int controlId, Func<UnityObject[], string[], DragAndDropVisualMode> canAcceptCallback)
        {
            var rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            return EditorGUIX.DropZone(controlId, rect, canAcceptCallback);
        }

        public static void DropZoneHint(GUIContent text = null, GUIContent icon = null)
        {
            var rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUIX.DropZoneHint(rect, text, icon);
        }
    }

    public static partial class EditorGUIX
    {
        public static partial class Styles
        {
            public static readonly GUIStyle dropzoneInfoBackgroundStyle = new GUIStyle("NotificationBackground");
            public static readonly GUIStyle dropzoneInfoLabelStyle = new GUIStyle("NotificationText");
            public static readonly GUIStyle dropzoneInfoIconStyle = new GUIStyle() { fixedHeight = 100, stretchWidth = true, alignment = TextAnchor.MiddleCenter };

            static readonly Color kDropZoneColorPersonal = new Color(0, 0, 0, 0.05f);
            static readonly Color kDropZoneColorProfessional = new Color(1, 1, 1, 0.05f);
            public static Color dropZoneColor { get { return EditorGUIUtility.isProSkin ? kDropZoneColorProfessional : kDropZoneColorPersonal; } }
        }

        public static partial class Content
        {
            public static readonly GUIContent dropZoneLabel = new GUIContent("Drop assets here");
            static GUIContent s_DropIcon = null;
            public static GUIContent dropIcon { get { return s_DropIcon ?? (s_DropIcon = new GUIContent((Texture2D)EditorGUIUtility.Load("EditorGUITools/DropFileIcon.png"))); } }
        }

        static Dictionary<int, bool> s_ShowFeedback = new Dictionary<int, bool>();

        public static bool DropZone(int controlId, Rect rect, Func<UnityObject[], string[], DragAndDropVisualMode> canAcceptCallback)
        {
            var result = false;
            var evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                {
                    DragAndDrop.visualMode = canAcceptCallback(DragAndDrop.objectReferences, DragAndDrop.paths);
                    var canAccept = DragAndDrop.visualMode != DragAndDropVisualMode.Rejected
                        && DragAndDrop.visualMode != DragAndDropVisualMode.None;
                    s_ShowFeedback[controlId] = canAccept;
                    if (canAccept)
                        evt.Use();

                    break;
                }
                case EventType.DragPerform:
                {
                    s_ShowFeedback[controlId] = false;
                    DragAndDrop.AcceptDrag();
                    result = true;
                    break;
                }
                case EventType.DragExited:
                {
                    s_ShowFeedback[controlId] = false;
                    break;
                }
                case EventType.Repaint:
                {
                    bool draw;
                    if (s_ShowFeedback.TryGetValue(controlId, out draw) && draw)
                    {
                        var tmpColor = GUI.color;
                        GUI.color = Styles.dropZoneColor;
                        GUI.DrawTexture(rect, Texture2D.whiteTexture);
                        GUI.color = tmpColor;
                    }
                    break;
                }
            }
            return result;
        }

        public static void DropZoneHint(Rect rect, GUIContent text = null, GUIContent icon = null)
        {
            text = text ?? Content.dropZoneLabel;
            icon = icon ?? Content.dropIcon;

            var textSize = Styles.dropzoneInfoBackgroundStyle.CalcSize(text);
            var textRect = new Rect(rect.position + (rect.size - textSize) * 0.5f, textSize);
            var backgroundRect = textRect;
            var iconRect = new Rect();
            if (icon != null)
            {
                var iconSize = Styles.dropzoneInfoIconStyle.CalcSize(icon);
                iconRect = new Rect(rect.position + (rect.size - textSize) * 0.5f, iconSize);
            }

            if (iconRect.size != Vector2.zero)
            {
                var backgroundOffset = Vector2.zero;
                if (iconRect.size.x > backgroundRect.width)
                    backgroundOffset.x += backgroundRect.width - iconRect.size.x;
                backgroundOffset.y += iconRect.size.y;

                backgroundOffset.y += 50;

                backgroundRect.size += backgroundOffset;
                backgroundRect.position -= backgroundOffset * 0.5f;

                textRect.position -= Vector2.up * backgroundOffset.y * 0.5f;
                iconRect.y = textRect.yMax;
                iconRect.x = backgroundRect.x;
                iconRect.width = backgroundRect.width;
            }

            GUI.Box(backgroundRect, GUIContent.none, Styles.dropzoneInfoBackgroundStyle);
            EditorGUI.LabelField(textRect, text, Styles.dropzoneInfoLabelStyle);
            GUI.Box(iconRect, icon, Styles.dropzoneInfoIconStyle);
        }
    }
}
