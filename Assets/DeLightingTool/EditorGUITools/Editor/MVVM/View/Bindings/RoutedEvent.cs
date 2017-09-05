using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.Experimental.VisualElements;

namespace UnityEditor.Experimental.MVVM
{
    public abstract class RoutedEvent
    {
        public abstract void Invoke(object arg);

        public void Raise(IVisualElement elt, DependencyProperty dp)
        {
            throw new NotImplementedException();
            //if (elt.panel != null && elt.panel.propertyChangedService != null)
            //    elt.panel.propertyChangedService.DispatchValueChanged(elt.panel, dp, elt);
        }
    }

    public class RoutedEvent<TOwner> : RoutedEvent
        where TOwner : IVisualElement
    {
        readonly Action<TOwner, Action> _add;
        readonly Action<TOwner, Action> _remove;

        public void Add(TOwner elt, Action handler)
        {
            _add(elt, handler);
        }

        public void Remove(TOwner elt, Action handler)
        {
            _remove(elt, handler);
        }

        public RoutedEvent(Action<TOwner, Action> add, Action<TOwner, Action> remove)
        {
            _add = add;
            _remove = remove;
        }

        public override void Invoke(object arg)
        {
        }
    }

    public class RoutedEvent<TOwner, TArg> : RoutedEvent
        where TOwner : IVisualElement
    {
        readonly Action<TOwner, Action<TArg>> _add;
        readonly Action<TOwner, Action<TArg>> _remove;

        public void Add(TOwner elt, Action<TArg> handler)
        {
            _add(elt, handler);
        }

        public void Remove(TOwner elt, Action<TArg> handler)
        {
            _remove(elt, handler);
        }

        public RoutedEvent(Action<TOwner, Action<TArg>> add, Action<TOwner, Action<TArg>> remove)
        {
            _add = add;
            _remove = remove;
        }

        public override void Invoke(object arg)
        {
        }
    }
}
