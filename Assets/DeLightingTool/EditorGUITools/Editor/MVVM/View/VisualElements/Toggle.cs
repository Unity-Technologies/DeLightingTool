using System;
using UnityEditor.Experimental.MVVM;
using UnityEngine;

namespace UnityEditor.Experimental.VisualElements
{
    public class Toggle : IMGUIVisualContainer
    {
        public static readonly DependencyProperty<bool> propertyValue = new DependencyProperty<Toggle, bool>(
            "value",
            elt => elt.value,
            (elt, v) => elt.value = v,
            v => (bool)v,
            new RoutedEvent<Toggle, bool>((el, c) => el.ValueChanged += c, (el, c) => el.ValueChanged-= c));

        public static readonly DependencyProperty<GUIContent> propertyLabel = new DependencyProperty<Toggle, GUIContent>(
            "label",
            elt => elt.label,
            (elt, v) => elt.label = v,
            v => (GUIContent)v);

        public static readonly DependencyProperty<string> propertyLabelText = new DependencyProperty<Toggle, string>(
            "labelText",
            elt => elt.labelText,
            (elt, v) => elt.labelText = v,
            v => (string)v);

        public static readonly DependencyProperty<string> propertyLabelTooltip = new DependencyProperty<Toggle, string>(
            "labelTooltip",
            elt => elt.labelTooltip,
            (elt, v) => elt.labelTooltip = v,
            v => (string)v);

        public static readonly DependencyProperty<Texture> propertyLabelImage = new DependencyProperty<Toggle, Texture>(
            "labelImage",
            elt => elt.labelImage,
            (elt, v) => elt.labelImage = v,
            v => (Texture)v);

        public event Action<bool> ValueChanged;

        bool m_Value;

        public bool value
        {
            get { return m_Value; }
            set
            {
                m_Value = value;
                if (ValueChanged != null)
                    ValueChanged(m_Value);
            }
        }

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
            BeginAlignment();
            value = GUILayout.Toggle(value, label, style.guiStyle, guiLayoutOptions);
            EndAlignment();
        }
    }
}
