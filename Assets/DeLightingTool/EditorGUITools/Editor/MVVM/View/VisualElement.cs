using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.Experimental.MVVM;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace UnityEditor.Experimental.VisualElements
{
    public class VisualElement : IVisualElement
    {
        public static readonly DependencyProperty<float> propertyWidthOverride = new DependencyProperty<VisualElement, float>(
            "widthOverride",
            elt => elt.widthOverride ?? 0,
            (elt, v) => elt.widthOverride = v,
            v => (float)v);

        public static readonly DependencyProperty<float> propertyHeightOverride = new DependencyProperty<VisualElement, float>(
            "heightOverride",
            elt => elt.heightOverride ?? 0,
            (elt, v) => elt.heightOverride = v,
            v => (float)v);

        public float? widthOverride { get; set; }
        public float? heightOverride { get; set; }

        IVisualElement m_Parent = null;
        public IVisualElement parent
        {
            get { return m_Parent; }
            set
            {
                var previous = m_Parent;
                m_Parent = value;
                OnParentChanged(previous, value);
            }
        }

        object m_DataContext = null;
        public object dataContext
        {
            get
            {
                return m_DataContext == null && m_Parent != null
                    ? m_Parent.dataContext
                    : m_DataContext;
            }
            set
            {
                if (m_DataContext is INotifyPropertyChanged)
                    ((INotifyPropertyChanged)m_DataContext).PropertyChanged -= OnPropertyChanged;

                m_DataContext = value;

                if (m_DataContext is INotifyPropertyChanged)
                    ((INotifyPropertyChanged)m_DataContext).PropertyChanged += OnPropertyChanged;
            }
        }
        internal object actualDataContext { get { return m_DataContext; } }

        List<Binding> m_Bindings = new List<Binding>();
        HashSet<string> m_Classes = new HashSet<string>();
        public string[] classes { get { return m_Classes.ToArray(); } }

        public bool HasClass(string @class)
        {
            return m_Classes.Contains(@class);
        }

        public void AddClass(params string[] classes)
        {
            m_Classes.UnionWith(classes);
        }

        public void RemoveClass(params string[] classes)
        {
            m_Classes.ExceptWith(classes);
        }

        public virtual void Dispose()
        {
            
        }

        public Binding AddBinding<TProp>(DependencyProperty<TProp> vProperty, ClassProperty<TProp> vmProperty)
        {
            var result = new Binding<TProp>(this, vProperty, vmProperty);
            m_Bindings.Add(result);
            return result;
        }

        public Binding AddBinding<TProp>(DependencyProperty<TProp> vProperty, IClassMethod<TProp> vmProperty)
        {
            var result = new Binding<TProp>(this, vProperty, vmProperty);
            m_Bindings.Add(result);
            return result;
        }

        public Binding AddBinding<TOwner>(ActionDependencyProperty<TOwner> vProperty, IClassMethod vmProperty)
            where TOwner : IVisualElement
        {
            var result = new ActionBinding<TOwner>(this, vProperty, vmProperty);
            m_Bindings.Add(result);
            return result;
        }

        public void RemoveBinding(Binding binding)
        {
            m_Bindings.Remove(binding);
        }

        internal void ExecuteBindings(PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var resolvedDataContext = dataContext;
            for (var i = 0; i < m_Bindings.Count; i++)
            {
                if (m_Bindings[i].ShouldTrigger(propertyChangedEventArgs.PropertyName))
                    m_Bindings[i].SetViewPropertyValueFromContext(resolvedDataContext);
            }
        }

        protected internal virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            ExecuteBindings(propertyChangedEventArgs);
        }

        protected void OnParentChanged(IVisualElement previousParent, IVisualElement nextParent)
        {
            OnDetach(previousParent);
            OnAttach(nextParent);
        }

        protected virtual void OnAttach(IVisualElement parent)
        {
            
        }

        protected virtual void OnDetach(IVisualElement parent)
        {
            
        }

        protected TPropertyType GetValue<TPropertyType>(ClassProperty<TPropertyType> property, TPropertyType @default = default(TPropertyType))
        {
            var resolvedDataContext = dataContext;
            if (resolvedDataContext == null)
            {
                UnityDebug.LogWarning("SetValue without data context");
                return @default;
            }

            try
            {
                return property.GetValue(resolvedDataContext);
            }
            catch (Exception)
            {
                UnityDebug.LogWarningFormat("Property {0} in data context {1} is not of a valid {2}", property.propertyName, m_DataContext, typeof(TPropertyType));
                return @default;
            }
        }

        protected bool SetValue<TPropertyType>(ClassProperty<TPropertyType> property, TPropertyType value)
        {
            var resolvedDataContext = dataContext;
            if (resolvedDataContext == null)
            {
                UnityDebug.LogWarning("SetValue without data context");
                return false;
            }

            try
            {
                if (!property.ValueEquals(resolvedDataContext, value))
                {
                    property.SetValue(resolvedDataContext, value);
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                UnityDebug.LogWarningFormat("Could not set property {0} in data context {1} with {2} ({3})", property.propertyName, m_DataContext, value, e);
                return false;
            }
        }

        protected void ExecuteCommand(IClassMethod classMethod)
        {
            var resolvedDataContext = dataContext;
            if (resolvedDataContext == null)
            {
                UnityDebug.LogWarning("ExecuteCommand without data context");
                return;
            }

            classMethod.Execute(resolvedDataContext);
        }
    }
}
