using System;
using UnityEditor.Experimental.MVVM;
using UnityEngine;

namespace UnityEditor.Experimental.VisualElements
{
    public class EditorProgressBar : IMGUIVisualContainer
    {
        public static readonly DependencyProperty<float> propertyValue = new DependencyProperty<EditorProgressBar, float>(
            "value",
            elt => elt.value,
            (elt, v) => elt.value = v,
            v => (float)v);

        public static readonly DependencyProperty<string> propertyContent = new DependencyProperty<EditorProgressBar, string>(
            "content",
            elt => elt.content,
            (elt, v) => elt.content = v,
            v => (string)v);

        float m_Value;
        public float value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        string m_Content = string.Empty;
        public string content
        {
            get { return m_Content; }
            set { m_Content = value; }
        }

        public override void OnGUI()
        {
            var rect = GUILayoutUtility.GetRect(0, float.MaxValue, 0, float.MaxValue, style.guiStyle, guiLayoutOptions);
            EditorGUI.ProgressBar(rect, value, content);
        }
    }
}
