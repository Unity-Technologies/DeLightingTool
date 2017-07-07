using System;
using UnityEditor.Experimental.MVVM;
using UnityEditor.Experimental.VisualElements;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental
{
    public abstract class Binding : IDisposable
    {
        public abstract IVisualElement element { get; }

        public abstract bool ShouldTrigger(string propertyName);
        public abstract void SetViewPropertyValueFromContext(object dataContext);
        public abstract void Dispose();
    }

    public class ActionBinding<TOwner> : Binding
        where TOwner : IVisualElement
    {
        public static int GetHashCodeFor(ActionDependencyProperty<TOwner> vProperty, IClassMethod vmProperty)
        {
            return vProperty.GetHashCode() ^ vmProperty.GetHashCode();
        }

        readonly IClassMethod m_ViewModelCommand;
        readonly ActionDependencyProperty<TOwner> m_ViewProperty;
        readonly IVisualElement m_VisualElement;
        public override IVisualElement element { get { return m_VisualElement; } }

        public ActionBinding(IVisualElement elt, ActionDependencyProperty<TOwner> vProperty, IClassMethod vmProperty)
        {
            Assert.IsNotNull(elt);
            Assert.IsNotNull(vProperty);
            Assert.IsNotNull(vmProperty);
            m_ViewProperty = vProperty;
            m_VisualElement = elt;
            vProperty.Hook(elt, vmProperty);
        }

        public override bool ShouldTrigger(string propertyName)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return GetHashCodeFor(m_ViewProperty, m_ViewModelCommand);
        }

        public override void SetViewPropertyValueFromContext(object dataContext)
        {
            
        }

        public override void Dispose()
        {
            if (m_ViewModelCommand != null)
                m_ViewProperty.Unhook(element, m_ViewModelCommand);
        }
    }

    public class Binding<TBindingType> : Binding
    {
        public static int GetHashCodeFor(DependencyProperty<TBindingType> vProperty, ClassProperty<TBindingType> vmProperty)
        {
            return vProperty.GetHashCode() ^ vmProperty.GetHashCode();
        }

        public static int GetHashCodeFor(DependencyProperty<TBindingType> vProperty, IClassMethod<TBindingType> vmProperty)
        {
            return vProperty.GetHashCode() ^ vmProperty.GetHashCode();
        }

        readonly ClassProperty<TBindingType> m_ViewModelProperty;
        readonly IClassMethod<TBindingType> m_ViewModelCommand;
        readonly DependencyProperty<TBindingType> m_ViewProperty;
        readonly IVisualElement m_VisualElement;
        public override IVisualElement element { get { return m_VisualElement;} }

        public Binding(IVisualElement elt, DependencyProperty<TBindingType> vProperty, ClassProperty<TBindingType> vmProperty)
        {
            Assert.IsNotNull(elt);
            Assert.IsNotNull(vProperty);
            Assert.IsNotNull(vmProperty);
            m_ViewModelProperty = vmProperty;
            m_ViewProperty = vProperty;
            m_VisualElement = elt;
            vProperty.Hook(elt, vmProperty);
        }

        public Binding(IVisualElement elt, DependencyProperty<TBindingType> vProperty, IClassMethod<TBindingType> vmProperty)
        {
            Assert.IsNotNull(elt);
            Assert.IsNotNull(vProperty);
            Assert.IsNotNull(vmProperty);
            m_ViewModelCommand = vmProperty;
            m_ViewProperty = vProperty;
            m_VisualElement = elt;
            vProperty.Hook(elt, vmProperty);
        }

        public override bool ShouldTrigger(string propertyName)
        {
            return m_ViewModelProperty != null 
                && (m_ViewModelProperty.propertyName == propertyName || propertyName == null);
        }

        public override int GetHashCode()
        {
            return m_ViewModelProperty != null
                ? GetHashCodeFor(m_ViewProperty, m_ViewModelProperty)
                : GetHashCodeFor(m_ViewProperty, m_ViewModelCommand);
        }

        public override void SetViewPropertyValueFromContext(object dataContext)
        {
            if (m_ViewModelProperty != null)
                m_ViewProperty.SetValue(m_VisualElement, m_ViewModelProperty.GetValue(dataContext));
        }

        public override void Dispose()
        {
            if (m_ViewProperty != null)
                m_ViewProperty.Unhook(element, m_ViewModelProperty);
            if (m_ViewModelCommand != null)
                m_ViewProperty.Unhook(element, m_ViewModelCommand);
        }
    }
}
