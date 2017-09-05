using UnityEngine;

namespace UnityEditor.Experimental
{
    public static partial class EditorGUIXLayout
    {
        public static bool InspectorFoldout(bool value, GUIContent content, EditorGUIX.FieldIcon icon = EditorGUIX.FieldIcon.None)
        {
            var rect = GUILayoutUtility.GetRect(content, EditorGUIX.Styles.inspectorBackground, GUILayout.ExpandWidth(true));
            return EditorGUIX.InspectorFoldout(rect, value, content, icon);
        }

        public static T InlineObjectField<T>(GUIContent label, T obj, EditorGUIX.FieldIcon icon = EditorGUIX.FieldIcon.None)
            where T : UnityEngine.Object
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var previousImage = label.image;
                label.image = EditorGUIX.GetIcon(icon, previousImage);
                EditorGUILayout.PrefixLabel(label);
                label.image = previousImage;

                var indent = EditorGUI.indentLevel;
                if (EditorGUI.indentLevel > 0)
                    EditorGUI.indentLevel = 0;

                obj = (T)EditorGUILayout.ObjectField(obj, typeof(T), false);

                EditorGUI.indentLevel = indent;
            }

            return obj;
        }
    }

    public static partial class EditorGUIX
    {
        public static partial class Content
        {
            static GUIContent s_InfoSmallIcon = null;
            public static GUIContent infoSmallIcon { get { return s_InfoSmallIcon ?? (s_InfoSmallIcon = new GUIContent((Texture2D)EditorGUIUtility.LoadRequired("console.infoicon.sml.png"))); } }
            static GUIContent s_WarningSmallIcon = null;
            public static GUIContent warningSmallIcon { get { return s_WarningSmallIcon ?? (s_WarningSmallIcon = new GUIContent((Texture2D)EditorGUIUtility.LoadRequired("console.warnicon.sml.png"))); } }
            static GUIContent s_ErrorSmallIcon = null;
            public static GUIContent errorSmallIcon { get { return s_ErrorSmallIcon ?? (s_ErrorSmallIcon = new GUIContent((Texture2D)EditorGUIUtility.LoadRequired("console.erroricon.sml.png"))); } }
        }

        public static partial class Styles
        {
            public const float fieldIconSize = 20;
            public static GUIStyle fieldIconStyle = new GUIStyle() { fixedWidth = fieldIconSize, fixedHeight = fieldIconSize, alignment = TextAnchor.MiddleRight };
            public static readonly GUIStyle inspectorBackground = new GUIStyle("IN Title") { padding = new RectOffset(3, 3, 3, 3) };
            public static readonly GUIStyle inspectorText = new GUIStyle("IN TitleText");
        }
        public enum FieldIcon
        {
            None,
            Info,
            Warning,
            Error
        }

        public static bool InspectorFoldout(Rect rect, bool value, GUIContent content, FieldIcon icon = FieldIcon.None)
        {
            GUI.Box(rect, GUIContent.none, Styles.inspectorBackground);
            var textRect = new Rect(rect);
            textRect.x += 20;
            textRect.y += 2;
            textRect.width -= 20;
            textRect.height -= 2;
            var previousImage = content.image;
            content.image = GetIcon(icon, previousImage);
            GUI.Box(textRect, content, Styles.inspectorText);
            content.image = previousImage;
            rect.x += 5;
            rect.y += 3;
            rect.width -= 5;
            rect.height -= 3;
            return EditorGUI.Foldout(rect, value, GUIContent.none, true);
        }

        internal static Texture GetIcon(FieldIcon icon, Texture @default = null)
        {
            switch (icon)
            {
                case EditorGUIX.FieldIcon.Info: return EditorGUIX.Content.infoSmallIcon.image;
                case EditorGUIX.FieldIcon.Warning: return EditorGUIX.Content.warningSmallIcon.image;
                case EditorGUIX.FieldIcon.Error: return EditorGUIX.Content.errorSmallIcon.image;
                default: return @default;
            }
        }
    }
}
