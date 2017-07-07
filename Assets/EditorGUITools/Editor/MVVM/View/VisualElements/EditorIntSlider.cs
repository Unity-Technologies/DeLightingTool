using System;
using UnityEditor.Experimental.MVVM;
using UnityEngine;

namespace UnityEditor.Experimental.VisualElements
{
    public class EditorIntSlider : IMGUIVisualContainer
    {
        public static readonly DependencyProperty<int> propertyValue = new DependencyProperty<EditorIntSlider, int>(
            "value",
            elt => elt.value,
            (elt, v) => elt.value = v,
            v => (int)v,
            new RoutedEvent<EditorIntSlider, int>((slider, action) => slider.ValueChanged += action, (slider, action) => slider.ValueChanged -= action));

        public static readonly DependencyProperty<GUIContent> propertyLabel = new DependencyProperty<EditorIntSlider, GUIContent>(
            "label",
            elt => elt.label,
            (elt, v) => elt.label = v,
            v => (GUIContent)v);

        public static readonly DependencyProperty<int> propertyMinValue = new DependencyProperty<EditorIntSlider, int>(
            "minValue",
            elt => elt.minValue,
            (elt, v) => elt.minValue = v,
            v => (int)v);

        public static readonly DependencyProperty<int> propertyMaxValue = new DependencyProperty<EditorIntSlider, int>(
            "maxValue",
            elt => elt.maxValue,
            (elt, v) => elt.maxValue = v,
            v => (int)v);

        public event Action<int> ValueChanged;

        GUIContent m_Label = GUIContent.none;

        public GUIContent label
        {
            get { return m_Label; }
            set { m_Label = value; }
        }

        int m_MinValue = 0;
        public int minValue
        {
            get { return m_MinValue; }
            set { m_MinValue = value; }
        }

        int m_MaxValue = 0;
        public int maxValue
        {
            get { return m_MaxValue; }
            set { m_MaxValue = value; }
        }

        int m_Value = 0;
        public int value
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
            value = EditorGUILayout.IntSlider(label, value, minValue, maxValue, guiLayoutOptions);
            EndAlignment();
        }
    }
}
