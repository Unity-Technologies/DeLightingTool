using System;
using UnityEditor.Experimental.MVVM;
using UnityEngine;

namespace UnityEditor.Experimental.VisualElements
{
    public class Label : IMGUIVisualContainer
    {
        public static readonly DependencyProperty<GUIContent> propertyContent = new DependencyProperty<Label, GUIContent>(
            "content",
            elt => elt.content,
            (elt, v) => elt.content = v,
            v => (GUIContent)v);

        public static readonly DependencyProperty<string> propertyText = new DependencyProperty<Label, string>(
            "text",
            elt => elt.text,
            (elt, v) => elt.text = v,
            v => (string)v);

        public static readonly DependencyProperty<Texture> propertyImage = new DependencyProperty<Label, Texture>(
            "image",
            elt => elt.image,
            (elt, v) => elt.image = v,
            v => (Texture)v);

        public static readonly DependencyProperty<string> propertyTooltip = new DependencyProperty<Label, string>(
            "tooltip",
            elt => elt.tooltip,
            (elt, v) => elt.tooltip = v,
            v => (string)v);

        GUIContent m_Content = new GUIContent();
        public GUIContent content
        {
            get { return m_Content; }
            set { m_Content = value; }
        }
        public string text
        {
            get { return m_Content.text; }
            set { m_Content.text = value; }
        }
        public Texture image
        {
            get { return m_Content.image; }
            set { m_Content.image = value; }
        }
        public string tooltip
        {
            get { return m_Content.tooltip; }
            set { m_Content.tooltip = value; }
        }

        public override void OnGUI()
        {
            BeginAlignment();
            GUILayout.Label(m_Content, style.guiStyle, guiLayoutOptions);
            EndAlignment();
        }
    }
}
