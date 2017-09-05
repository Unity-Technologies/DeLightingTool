using System;
using UnityEditor.Experimental.MVVM;

namespace UnityEditor.Experimental.VisualElements
{
    public interface IVisualElement : IDisposable
    {
        string[] classes { get; }
        bool HasClass(string @class);
        void AddClass(params string[] classes);
        void RemoveClass(params string[] classes);
        Binding AddBinding<TProp>(DependencyProperty<TProp> vProperty, ClassProperty<TProp> vmProperty);
        void RemoveBinding(Binding binding);
        IVisualElement parent { get; set; }
        object dataContext { get; set; }
    }
}
