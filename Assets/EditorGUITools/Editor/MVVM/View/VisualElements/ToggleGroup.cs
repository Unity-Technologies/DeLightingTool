using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.MVVM;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VisualElements
{
    public class ToggleGroup : IMGUIVisualContainer
    {
        public static readonly DependencyProperty<string> propertyFirstClass = new DependencyProperty<ToggleGroup, string>(
            "firstClass",
            elt => elt.firstClass,
            (elt, v) => elt.firstClass = v,
            v => (string)v);

        public static readonly DependencyProperty<string> propertyMiddleClass = new DependencyProperty<ToggleGroup, string>(
            "middleClass",
            elt => elt.middleClass,
            (elt, v) => elt.middleClass = v,
            v => (string)v);

        public static readonly DependencyProperty<string> propertyLastClass = new DependencyProperty<ToggleGroup, string>(
            "lastClass",
            elt => elt.lastClass,
            (elt, v) => elt.lastClass = v,
            v => (string)v);

        public static readonly DependencyProperty<string> propertySingleClass = new DependencyProperty<ToggleGroup, string>(
            "singleClass",
            elt => elt.singleClass,
            (elt, v) => elt.singleClass = v,
            v => (string)v);

        public static readonly DependencyProperty<bool> propertyAllowNoneSelected = new DependencyProperty<ToggleGroup, bool>(
            "allowNoneSelected",
            elt => elt.allowNoneSelected,
            (elt, v) => elt.allowNoneSelected = v,
            v => (bool)v);

        public static readonly DependencyProperty<int> propertyActiveIndex = new DependencyProperty<ToggleGroup, int>(
            "activeIndex",
            elt => elt.activeIndex,
            (elt, v) => elt.activeIndex = v,
            v => (int)v,
            new RoutedEvent<ToggleGroup, int>((el, c) => el.ActiveIndexChanged += c, (el, c) => el.ActiveIndexChanged -= c));

        public static readonly DependencyProperty<int[]> propertyActiveIndexes = new DependencyProperty<ToggleGroup, int[]>(
            "activeIndexes",
            elt => elt.activeIndexes,
            (elt, v) => elt.activeIndexes = v,
            v => (int[])v,
            new RoutedEvent<ToggleGroup, int[]>((el, c) => el.ActiveIndexesChanged += c, (el, c) => el.ActiveIndexesChanged -= c));

        public event Action<int> ActiveIndexChanged;
        public event Action<int[]> ActiveIndexesChanged;

        string m_FirstClass = string.Empty;
        public string firstClass
        {
            get { return m_FirstClass; }
            set { m_FirstClass = value; }
        }

        string m_MiddleClass = string.Empty;
        public string middleClass
        {
            get { return m_MiddleClass; }
            set { m_MiddleClass = value; }
        }

        string m_LastClass = string.Empty;
        public string lastClass
        {
            get { return m_LastClass; }
            set { m_LastClass = value; }
        }

        string m_SingleClass = string.Empty;
        public string singleClass
        {
            get { return m_SingleClass; }
            set { m_SingleClass = value; }
        }

        bool m_AllowNoneSelected = false;
        public bool allowNoneSelected
        {
            get { return m_AllowNoneSelected; }
            set { m_AllowNoneSelected = value; }
        }

        bool m_AllowMultiple = false;

        public bool allowMultiple
        {
            get { return m_AllowMultiple; }
            set { m_AllowMultiple = value; }
        }

        HashSet<int> m_ActiveIndexes = new HashSet<int>();
        public int[] activeIndexes
        {
            get { return m_ActiveIndexes.ToArray(); }
            set
            {
                if (!m_ActiveIndexes.IsSubsetOf(value)
                    || m_ActiveIndexes.IsSupersetOf(value))
                {
                    m_ActiveIndexes.Clear();
                    m_ActiveIndexes.UnionWith(value);
                    TriggerActiveIndexesChanged();
                    UpdateToggleValueFromActiveIndex();
                }
            }
        }

        public int activeIndex
        {
            get { return m_ActiveIndexes.Count > 0 ? m_ActiveIndexes.First() : -1; }
            set
            {
                if (!m_ActiveIndexes.Contains(value) || m_ActiveIndexes.Count != 1)
                {
                    m_ActiveIndexes.Clear();
                    m_ActiveIndexes.Add(value);
                    TriggerActiveIndexesChanged();
                    UpdateToggleValueFromActiveIndex();
                }
            }
        }

        Dictionary<Toggle, Action<bool>> m_Callbacks = new Dictionary<Toggle, Action<bool>>();

        public override void AddChild(IVisualElement element)
        {
            base.AddChild(element);
            var toggle = element as Toggle;
            if (toggle != null)
            {
                var callback = CreateToggleCallback(toggle);
                m_Callbacks[toggle] = callback;
                toggle.ValueChanged += callback;
            }
        }

        public override void RemoveChild(IVisualElement element)
        {
            base.RemoveChild(element);
            var toggle = element as Toggle;
            if (toggle != null)
            {
                Assert.IsTrue(m_Callbacks.ContainsKey(toggle));
                var callback = m_Callbacks[toggle];
                toggle.ValueChanged -= callback;
                m_Callbacks.Remove(toggle);
            }
        }

        public override void OnGUI()
        {
            var childrenCount = this.childrenCount;
            var firstIndex = -1;
            var lastIndex = -1;
            for (int i = 0; i < childrenCount; i++)
            {
                var child = GetChildAt(i) as Toggle;
                if (child == null)
                    continue;

                if (firstIndex == -1)
                    firstIndex = i;
                lastIndex = i;
            }

            if (firstIndex == -1)
                return;

            BeginAlignment();
            BeginGUILayout(style.guiStyle, guiLayoutOptions);
            if (firstIndex == lastIndex)
            {
                var toggle = GetChildAt(firstIndex) as Toggle;
                Assert.IsNotNull(toggle);

                toggle.RemoveClass(firstClass);
                toggle.RemoveClass(middleClass);
                toggle.RemoveClass(lastClass);
                toggle.AddClass(singleClass);

                toggle.OnGUI();
            }
            else
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = GetChildAt(i) as Toggle;
                    if (child == null)
                        continue;

                    child.RemoveClass(firstClass);
                    child.RemoveClass(middleClass);
                    child.RemoveClass(lastClass);
                    child.RemoveClass(singleClass);

                    if (i == firstIndex)
                        child.AddClass(firstClass);
                    else if (i != firstIndex && i != lastIndex)
                        child.AddClass(middleClass);
                    else if (i == lastIndex)
                        child.AddClass(lastClass);

                    child.OnGUI();
                }
            }
            EndGUILayout();
            EndAlignment();
        }

        Action<bool> CreateToggleCallback(Toggle toggle)
        {
            return b => OnToggleValueChanged(toggle, b);
        }

        void TriggerActiveIndexesChanged()
        {
            if (ActiveIndexesChanged != null)
                ActiveIndexesChanged(m_ActiveIndexes.ToArray());
            if (ActiveIndexChanged != null)
                ActiveIndexChanged(activeIndex);
        }

        void OnToggleValueChanged(Toggle toggle, bool value)
        {
            var index = 0;
            for (int i = 0; i < childrenCount; i++)
            {
                var child = GetChildAt(i) as Toggle;
                if (child == null)
                    continue;

                if (child == toggle)
                {
                    if (toggle.value && !m_ActiveIndexes.Contains(index))
                    {
                        if (m_ActiveIndexes.Count > 0 && !allowMultiple)
                            m_ActiveIndexes.Clear();

                        m_ActiveIndexes.Add(index);
                        TriggerActiveIndexesChanged();
                        UpdateToggleValueFromActiveIndex();
                    }
                    else if (!toggle.value && m_ActiveIndexes.Contains(index))
                    {
                        m_ActiveIndexes.Remove(index);
                        TriggerActiveIndexesChanged();
                        UpdateToggleValueFromActiveIndex();
                    }
                    break;
                }

                ++index;
            }
        }

        void UpdateToggleValueFromActiveIndex()
        {
            var index = 0;
            for (int i = 0; i < childrenCount; i++)
            {
                var child = GetChildAt(i) as Toggle;
                if (child == null)
                    continue;

                child.value = m_ActiveIndexes.Contains(index);
                ++index;
            }
        }
    }
}
