using System;
using UnityEditor.Experimental.MVVM;
using UnityEngine;

namespace UnityEditor.Experimental.VisualElements
{
    public abstract class ResizeBase : IMGUIVisualContainer
    {
        public static readonly DependencyProperty<float> propertyValue = new DependencyProperty<ResizeBase, float>(
            "value",
            elt => elt.value,
            (elt, v) => elt.value = v,
            v => (float)v,
            new RoutedEvent<ResizeBase, float>((el, a) => el.ValueChanged += a, (el, a) => el.ValueChanged -= a));

        public static readonly DependencyProperty<float> propertyMinValue = new DependencyProperty<ResizeBase, float>(
            "minValue",
            elt => elt.minValue,
            (elt, v) => elt.minValue = v,
            v => (float)v);

        public static readonly DependencyProperty<float> propertyMaxValue = new DependencyProperty<ResizeBase, float>(
            "maxValue",
            elt => elt.maxValue,
            (elt, v) => elt.maxValue = v,
            v => (float)v);

        public event Action<float> ValueChanged;

        protected int m_HintID = 0;

        float m_Value;
        public float value
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

        public float minValue { get; set; }
        public float maxValue { get; set; }

        public ResizeBase()
        {
            m_HintID = Mathf.FloorToInt(UnityEngine.Random.value * 10000000);
            minValue = float.MinValue;
            maxValue = float.MaxValue;
        }
    }
}
