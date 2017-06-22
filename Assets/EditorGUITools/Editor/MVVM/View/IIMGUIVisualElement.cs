using UnityEngine;

namespace UnityEditor.VisualElements
{
    public interface IIMGUIVisualElement : IVisualElement
    {
        void OnGUI();
        void Repaint();
    }
}
