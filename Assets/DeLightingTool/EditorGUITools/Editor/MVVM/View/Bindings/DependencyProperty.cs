using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.VisualElements;
using UnityEngine;

namespace UnityEditor.Experimental.MVVM
{
    public abstract class DependencyProperty
    {
        protected string m_name;

        protected DependencyProperty(string name)
        {
            m_name = name;
        }

        public static DependencyProperty Create<TOwner, TProp>(string name, Func<TOwner, TProp> getter, Action<TOwner, TProp> setter,
            Func<object, TProp> coerce = null,
            RoutedEvent<TOwner, TProp> hook = null)
            where TOwner : IVisualElement
        {
            return new DependencyProperty<TOwner, TProp>(name, getter, setter, coerce, hook);
        }

        public abstract object GetValue(IVisualElement e);
        public abstract void SetValue(IVisualElement e, object v);
        public abstract object CoerceValue(object v);
    }

    public abstract class DependencyProperty<TProp> : DependencyProperty
    {
        protected DependencyProperty(string name)
            : base(name) {}

        public abstract void Hook(IVisualElement elt, ClassProperty<TProp> vmProperty);
        public abstract void Unhook(IVisualElement element, ClassProperty<TProp> vmProperty);
        public abstract void Hook(IVisualElement elt, IClassMethod<TProp> vmMethod);
        public abstract void Unhook(IVisualElement element, IClassMethod<TProp> vmProperty);
    }

    public class ActionDependencyProperty<TOwner> : DependencyProperty
        where TOwner : IVisualElement
    {
        RoutedEvent<TOwner> m_Hook = null;

        Dictionary<CallbackTuple, Action> m_CallbackRegistered = null;

        struct CallbackTuple
        {
            public readonly IVisualElement element;
            public readonly IClassMethod vmMethod;

            public CallbackTuple(IVisualElement element, IClassMethod vmMethod)
            {
                this.element = element;
                this.vmMethod = vmMethod;
            }

            public override int GetHashCode()
            {
                return (element != null ? element.GetHashCode() : 0)
                    ^ (vmMethod != null ? vmMethod.GetHashCode() : 0);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is CallbackTuple))
                    return false;

                var other = (CallbackTuple)obj;

                return other.element == element && other.vmMethod == vmMethod;
            }
        }

        public ActionDependencyProperty(string name, RoutedEvent<TOwner> hook)
            : base(name)
        {
            m_Hook = hook;
        }

        public override object GetValue(IVisualElement e)
        {
            throw new Exception("Property has no value");
        }

        public override void SetValue(IVisualElement e, object v)
        {
            throw new Exception("Property has no value");
        }

        public override object CoerceValue(object v)
        {
            return v;
        }

        public void Hook(IVisualElement elt, IClassMethod vmMethod)
        {
            if (m_Hook != null)
            {
                var key = new CallbackTuple(elt, vmMethod);
                if (m_CallbackRegistered == null)
                    m_CallbackRegistered = new Dictionary<CallbackTuple, Action>();
                if (!m_CallbackRegistered.ContainsKey(key))
                {
                    var callback = GetCallbackFor(elt, vmMethod);
                    m_CallbackRegistered[key] = callback;
                    m_Hook.Add((TOwner)elt, callback);
                }
            }
        }

        public void Unhook(IVisualElement elt, IClassMethod vmMethod)
        {
            if (m_Hook != null)
            {
                var key = new CallbackTuple(elt, vmMethod);
                Action callback;
                if (m_CallbackRegistered != null && m_CallbackRegistered.TryGetValue(key, out callback))
                    m_Hook.Remove((TOwner)elt, callback);
            }
        }

        static Action GetCallbackFor(IVisualElement elt, IClassMethod vmMethod)
        {
            return () => vmMethod.Execute(elt.dataContext);
        }
    }


    public class DependencyProperty<TOwner, TProp> : DependencyProperty<TProp> where TOwner : IVisualElement
    {
        readonly Func<TOwner, TProp> m_Getter;
        readonly Action<TOwner, TProp> m_Setter;
        readonly Func<object, TProp> m_Coerce;
        readonly RoutedEvent<TOwner, TProp> m_Hook;

        Dictionary<CallbackTuple, Action<TProp>> m_CallbackRegistered = null;

        struct CallbackTuple
        {
            public readonly IVisualElement element;
            public readonly ClassProperty<TProp> vmProperty;
            public readonly IClassMethod<TProp> vmMethod;

            public CallbackTuple(IVisualElement element, ClassProperty<TProp> vmProperty)
            {
                this.element = element;
                this.vmProperty = vmProperty;
                vmMethod = null;
            }

            public CallbackTuple(IVisualElement element, IClassMethod<TProp> vmMethod)
            {
                this.element = element;
                this.vmMethod = vmMethod;
                vmProperty = null;
            }

            public override int GetHashCode()
            {
                return (element != null ? element.GetHashCode() : 0)
                    ^ (vmProperty != null ? vmProperty.GetHashCode() : 0)
                    ^ (vmMethod != null ? vmMethod.GetHashCode() : 0);
            }

            public override bool Equals(object obj)
            {
                if(!(obj is CallbackTuple))
                    return false;

                var other = (CallbackTuple)obj;

                return other.element == element 
                    && other.vmProperty == vmProperty 
                    && other.vmMethod == vmMethod;
            }
        }

        public DependencyProperty(string name) : base(name)
        {
        }

        public DependencyProperty(string name, Func<TOwner, TProp> getter, Action<TOwner, TProp> setter, Func<object, TProp> coerce = null, RoutedEvent<TOwner, TProp> hook = null)
            : base(name)
        {
            m_Getter = getter;
            m_Setter = setter;
            m_Coerce = coerce;
            m_Hook = hook;
        }

        public DependencyProperty(string name, RoutedEvent<TOwner, TProp> hook)
            : base(name)
        {
            m_Getter = null;
            m_Setter = null;
            m_Coerce = null;
            m_Hook = hook;
        }

        public override object GetValue(IVisualElement e)
        {
            return m_Getter((TOwner)e);
        }

        public override void SetValue(IVisualElement e, object v)
        {
            var coerced = CoerceValue(v);
            if (!(coerced is TProp) && v != null)
            {
                Debug.LogErrorFormat("Cannot cast value {0} to {1}", coerced ?? "<NULL>", typeof(TProp).Name);
            }
            if (!(e is TOwner))
                Debug.LogErrorFormat("Cannot cast value {0} to {1}", e == null ? "<null>" : (e.GetType().Name), typeof(TOwner).Name);

            m_Setter((TOwner)e, (TProp)coerced);
        }

        public override object CoerceValue(object v)
        {
            if (m_Coerce == null)
                return v;
            return m_Coerce(v);
        }

        public override void Hook(IVisualElement elt, ClassProperty<TProp> vmProperty)
        {
            if (m_Hook != null)
            {
                var key = new CallbackTuple(elt, vmProperty);
                if (m_CallbackRegistered == null)
                    m_CallbackRegistered = new Dictionary<CallbackTuple, Action<TProp>>();
                if (!m_CallbackRegistered.ContainsKey(key))
                {
                    var callback = GetCallbackFor(elt, vmProperty);
                    m_CallbackRegistered[key] = callback;
                    m_Hook.Add((TOwner)elt, callback);
                }
            }
        }

        public override void Unhook(IVisualElement element, ClassProperty<TProp> vmProperty)
        {
            if (m_Hook != null)
            {
                var key = new CallbackTuple(element, vmProperty);
                Action<TProp> callback;
                if (m_CallbackRegistered != null && m_CallbackRegistered.TryGetValue(key, out callback))
                    m_Hook.Remove((TOwner)element, callback);
            }
        }

        public override void Hook(IVisualElement elt, IClassMethod<TProp> vmMethod)
        {
            if (m_Hook != null)
            {
                var key = new CallbackTuple(elt, vmMethod);
                if (m_CallbackRegistered == null)
                    m_CallbackRegistered = new Dictionary<CallbackTuple, Action<TProp>>();
                if (!m_CallbackRegistered.ContainsKey(key))
                {
                    var callback = GetCallbackFor(elt, vmMethod);
                    m_CallbackRegistered[key] = callback;
                    m_Hook.Add((TOwner)elt, callback);
                }
            }
        }

        public override void Unhook(IVisualElement element, IClassMethod<TProp> vmProperty)
        {
            if (m_Hook != null)
            {
                var key = new CallbackTuple(element, vmProperty);
                Action<TProp> callback;
                if (m_CallbackRegistered != null && m_CallbackRegistered.TryGetValue(key, out callback))
                    m_Hook.Remove((TOwner)element, callback);
            }
        }

        Action<TProp> GetCallbackFor(IVisualElement elt, ClassProperty<TProp> vmProperty)
        {
            return _ => vmProperty.SetValue(elt.dataContext, (TProp)GetValue(elt));
        }

        Action<TProp> GetCallbackFor(IVisualElement elt, IClassMethod<TProp> vmMethod)
        {
            return _ => vmMethod.Execute(elt.dataContext, _);
        }
    }
}
