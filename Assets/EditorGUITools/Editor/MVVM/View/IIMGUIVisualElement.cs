using UnityEngine;

namespace UnityEditor.Experimental.VisualElements
{
    public interface IIMGUIVisualElement : IVisualElement
    {
        void OnGUI();
        void Repaint();
    }
}
