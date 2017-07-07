using UnityEditor.Experimental.MVVM;

namespace UnityEditor.Experimental.VisualElements
{
    public class VisualElementRoot : IVisualElement
    {
        public void Dispose()
        {
        }

        public Binding AddBinding<TProp>(DependencyProperty<TProp> vProperty, ClassProperty<TProp> vmProperty)
        {
            return null;
        }

        public void RemoveBinding(Binding binding)
        {
        }

        public IVisualElement parent { get; set; }
        public object dataContext { get; set; }
        public string[] classes { get; private set; }
        public bool HasClass(string @class)
        {
            return false;
        }

        public void AddClass(params string[] classes)
        {
        }

        public void RemoveClass(params string[] classes)
        {
        }
    }
}
