using System;
using UnityEngine;

namespace UnityEditor.Experimental.VisualElements
{
    public class IMGUIVisualContainer : VisualContainer, IIMGUIVisualElement
    {
        StyleSheet m_StyleSheet = null;
        internal StyleSheet styleSheet
        {
            get
            {
                if (m_StyleSheet != null)
                    return m_StyleSheet;

                IVisualElement cursor = this;
                while (cursor.parent != null)
                {
                    cursor = cursor.parent;
                    var elt = cursor as IMGUIVisualContainer;
                    if (elt != null && elt.styleSheet != null)
                        return elt.styleSheet;
                }

                return null;
            }
        }

        protected GUILayoutOption[] guiLayoutOptions
        {
            get { return style.ToGUILayoutOptionArray(widthOverride, heightOverride); }
        }

        protected StyleSheet.Style style
        {
            get
            {
                var resolvedStylesheet = styleSheet;
                return resolvedStylesheet != null
                        ? resolvedStylesheet.GetStyleFor(classes, GetType())
                        : new StyleSheet.Style();
            }
        }

        public event Action RepaintRequested;

        public void AddStyleSheet(StyleSheet styleSheet)
        {
            m_StyleSheet = styleSheet;
        }

        public virtual void OnGUI()
        {
            BeginAlignment();
            BeginGUILayout(style.guiStyle, guiLayoutOptions);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = GetChildAt(i) as IIMGUIVisualElement;
                if (child != null)
                    child.OnGUI();
            }
            EndGUILayout();
            EndAlignment();
        }

        public void Repaint()
        {
            if (RepaintRequested != null)
                RepaintRequested();
            if (parent is IIMGUIVisualElement)
                ((IIMGUIVisualElement)parent).Repaint();
        }

        public override void Dispose()
        {
            base.Dispose();
            RepaintRequested = null;
        }

        protected void BeginGUILayout(GUIStyle guiStyle, params GUILayoutOption[] options)
        {
            switch (style.layout)
            {
                case StyleSheet.GUILayoutType.Horizontal:
                    GUILayout.BeginHorizontal(guiStyle, options);
                    break;
                default:
                    GUILayout.BeginVertical(guiStyle, options);
                    break;
            }
        }

        protected void EndGUILayout()
        {
            switch (style.layout)
            {
                case StyleSheet.GUILayoutType.Horizontal:
                    GUILayout.EndHorizontal();
                    break;
                default:
                    GUILayout.EndVertical();
                    break;
            }
        }

        protected void BeginAlignment()
        {
            switch (style.alignment)
            {
                case StyleSheet.Alignment.HorizontalCenter:
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    break;
                case StyleSheet.Alignment.VerticalCenter:
                    GUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();
                    break;
                case StyleSheet.Alignment.BothCenter:
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();
                    break;
            }
        }

        protected void EndAlignment()
        {
            switch (style.alignment)
            {
                case StyleSheet.Alignment.HorizontalCenter:
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    break;
                case StyleSheet.Alignment.VerticalCenter:
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();
                    break;
                case StyleSheet.Alignment.BothCenter:
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    break;
            }
        }
    }
}
