using System;
using UnityEditor.Experimental.MVVM;
using UnityEngine;

namespace UnityEditor.Experimental.VisualElements
{
    public class EditorTextField : IMGUIVisualContainer
    {
        public static readonly DependencyProperty<string> propertyValue = new DependencyProperty<EditorTextField, string>(
            "value",
            elt => elt.value,
            (elt, v) => elt.value = v,
            v => (string)v,
            new RoutedEvent<EditorTextField, string>((slider, action) => slider.ValueChanged += action, (slider, action) => slider.ValueChanged -= action));

        public static readonly DependencyProperty<GUIContent> propertyLabel = new DependencyProperty<EditorIntSlider, GUIContent>(
            "label",
            elt => elt.label,
            (elt, v) => elt.label = v,
            v => (GUIContent)v);

        public event Action<string> ValueChanged;

        GUIContent m_Label = GUIContent.none;
        public GUIContent label
        {
            get { return m_Label; }
            set { m_Label = value; }
        }

        string m_Value = string.Empty;
        public string value
        {
            get { return m_Value; }
            set
            {
                if (m_Value != value)
                {
                    m_Value = value;
                    if (ValueChanged != null)
                        ValueChanged(m_Value);
                }
            }
        }

        public override void OnGUI()
        {
            BeginAlignment();
            value = EditorGUILayout.TextField(label, value, guiLayoutOptions);
            EndAlignment();
        }
    }
}
