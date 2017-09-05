using System;
using UnityEditor.Experimental.MVVM;
using UnityEngine;

namespace UnityEditor.Experimental.VisualElements
{
    public class Button : IMGUIVisualContainer
    {
        public static readonly ActionDependencyProperty<Button> propertyOnClick = new ActionDependencyProperty<Button>(
            "onClick",
            new RoutedEvent<Button>((d, a) => d.Click += a, (d, a) => d.Click -= a));

        public static readonly DependencyProperty<GUIContent> propertyLabel = new DependencyProperty<Button, GUIContent>(
            "label",
            elt => elt.label,
            (elt, v) => elt.label = v,
            v => (GUIContent)v);

        public static readonly DependencyProperty<string> propertyLabelText = new DependencyProperty<Button, string>(
            "labelText",
            elt => elt.labelText,
            (elt, v) => elt.labelText = v,
            v => (string)v);

        public static readonly DependencyProperty<string> propertyLabelTooltip = new DependencyProperty<Button, string>(
            "labelTooltip",
            elt => elt.labelTooltip,
            (elt, v) => elt.labelTooltip = v,
            v => (string)v);

        public static readonly DependencyProperty<Texture> propertyLabelImage = new DependencyProperty<Button, Texture>(
            "labelImage",
            elt => elt.labelImage,
            (elt, v) => elt.labelImage = v,
            v => (Texture)v);

        public event Action Click;

        GUIContent m_Label = new GUIContent();
        public GUIContent label
        {
            get { return m_Label; }
            set { m_Label = value; }
        }
        public string labelText
        {
            get { return m_Label.text; }
            set { m_Label.text = value; }
        }
        public Texture labelImage
        {
            get { return m_Label.image; }
            set { m_Label.image = value; }
        }
        public string labelTooltip
        {
            get { return m_Label.tooltip; }
            set { m_Label.tooltip = value; }
        }

        public override void OnGUI()
        {
            if (GUILayout.Button(label, style.guiStyle, guiLayoutOptions) && Click != null)
                Click();
        }
    }
}
