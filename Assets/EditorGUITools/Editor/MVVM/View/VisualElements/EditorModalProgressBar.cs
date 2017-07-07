using System;
using UnityEditor.Experimental.MVVM;
using UnityEngine;

namespace UnityEditor.Experimental.VisualElements
{
    public class EditorModalProgressBar : IMGUIVisualContainer
    {
        public static readonly DependencyProperty<float> propertyValue = new DependencyProperty<EditorModalProgressBar, float>(
            "value",
            elt => elt.value,
            (elt, v) => elt.value = v,
            v => (float)v);

        public static readonly DependencyProperty<string> propertyTitle = new DependencyProperty<EditorModalProgressBar, string>(
            "title",
            elt => elt.title,
            (elt, v) => elt.title = v,
            v => (string)v);

        public static readonly DependencyProperty<string> propertyInfo = new DependencyProperty<EditorModalProgressBar, string>(
            "info",
            elt => elt.info,
            (elt, v) => elt.info = v,
            v => (string)v);

        public static readonly DependencyProperty<bool> propertyCancellable = new DependencyProperty<EditorModalProgressBar, bool>(
            "cancellable",
            elt => elt.cancellable,
            (elt, v) => elt.cancellable = v,
            v => (bool)v);

        public static readonly ActionDependencyProperty<EditorModalProgressBar> propertyCancelled = new ActionDependencyProperty<EditorModalProgressBar>(
            "cancelled",
            new RoutedEvent<EditorModalProgressBar>((el, a) => el.Cancelled += a, (el, a) => el.Cancelled -= a));

        public event Action Cancelled;

        public float value { get; set; }
        public string title { get; set; }
        public string info { get; set; }
        public bool cancellable { get; set; }

        bool m_WasInProgress = false;

        public override void OnGUI()
        {
            if (m_WasInProgress && value >= 1)
            {
                EditorUtility.ClearProgressBar();
                m_WasInProgress = false;
            }
            else if (value < 1)
            {
                if (cancellable)
                {
                    if (EditorUtility.DisplayCancelableProgressBar(title, info, value) && Cancelled != null)
                        Cancelled();
                }
                else
                    EditorUtility.DisplayProgressBar(title, info, value);
                m_WasInProgress = true;
            }
        }
    }
}
