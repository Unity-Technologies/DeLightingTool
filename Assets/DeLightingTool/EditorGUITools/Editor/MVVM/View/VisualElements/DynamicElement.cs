using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.Experimental.MVVM;
using UnityEngine;

namespace UnityEditor.Experimental.VisualElements
{
    public class DynamicElement : IMGUIVisualContainer
    {
        public static readonly DependencyProperty<int> propertyActiveIndex = new DependencyProperty<DynamicElement, int>(
            "activeIndex",
            elt => elt.activeIndex,
            (elt, v) => elt.activeIndex = v,
            v => (int)v);

        int m_ActiveIndex = 0;
        public int activeIndex
        {
            get { return m_ActiveIndex; }
            set
            {
                if (m_ActiveIndex >= 0 && m_ActiveIndex < m_DynamicChildren.Count)
                    RemoveChild(m_DynamicChildren[m_ActiveIndex]);
                m_ActiveIndex = value;
                if (m_ActiveIndex >= 0 && m_ActiveIndex < m_DynamicChildren.Count)
                    AddChild(m_DynamicChildren[m_ActiveIndex]);
            }
        }

        List<IVisualElement> m_DynamicChildren = new List<IVisualElement>();

        public void AddDynamicChild(IVisualElement element)
        {
            m_DynamicChildren.Add(element);
            if (activeIndex == m_DynamicChildren.Count - 1)
                AddChild(m_DynamicChildren[activeIndex]);
        }

        public void RemoveDynamicChild(IVisualElement element)
        {
            var index = m_DynamicChildren.IndexOf(element);
            if (index != -1)
            {
                m_DynamicChildren.Remove(element);
                if (index >= activeIndex)
                    --activeIndex;
            }
        }

        public override void OnGUI()
        {
            if (childrenCount == 0)
                return;

            var child = GetChildAt(0) as IIMGUIVisualElement;
            if (child == null)
                return;

            BeginGUILayout(style.guiStyle, guiLayoutOptions);
            child.OnGUI();
            EndGUILayout();
        }
    }
}
