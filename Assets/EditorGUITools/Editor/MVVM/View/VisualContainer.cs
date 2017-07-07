using System.Collections.Generic;
using System.ComponentModel;

namespace UnityEditor.Experimental.VisualElements
{
    public class VisualContainer : VisualElement
    {
        List<IVisualElement> m_Children = new List<IVisualElement>();
        public int childrenCount { get { return m_Children.Count; } }

        List<Binding> m_Bindings = new List<Binding>();

        public VisualContainer()
        {
            Build();
        }

        public virtual void AddChild(IVisualElement element)
        {
            element.parent = this;
            m_Children.Add(element);
        }

        public virtual void RemoveChild(IVisualElement element)
        {
            m_Children.Remove(element);
            element.parent = null;
        }

        public IVisualElement GetChildAt(int index)
        {
            return index >= 0 && index < childrenCount
                ? m_Children[index]
                : null;
        }

        protected internal override void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnPropertyChanged(sender, propertyChangedEventArgs);

            for (int i = 0; i < m_Children.Count; i++)
            {
                var child = m_Children[i] as VisualElement;
                if (child != null && child.actualDataContext == null)
                    child.OnPropertyChanged(sender, propertyChangedEventArgs);
            }
        }

        protected void RegisterBinding(Binding binding)
        {
            m_Bindings.Add(binding);
        }

        protected void UnregisterBinding(Binding binding)
        {
            m_Bindings.Remove(binding);
        }

        protected virtual void Build()
        {
            if (parent != null)
                Bind();
        }

        protected virtual void Bind()
        {
            ExecuteBindings(new PropertyChangedEventArgs(null));
        }

        protected void Unbind()
        {
            for (int i = 0; i < m_Bindings.Count; i++)
                m_Bindings[i].element.RemoveBinding(m_Bindings[i]);
            m_Bindings.Clear();
        }

        protected override void OnAttach(IVisualElement parent)
        {
            base.OnAttach(parent);
            if (parent != null)
                Bind();
        }

        protected override void OnDetach(IVisualElement parent)
        {
            base.OnDetach(parent);
            if (parent != null)
                Unbind();
        }
    }
}
