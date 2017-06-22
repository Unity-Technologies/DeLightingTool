using System;
using UnityEditor.MVVM;
using UnityEngine;

namespace UnityEditor.VisualElements
{
    public class HorizontalResize : ResizeBase
    {
        public override void OnGUI()
        {
            var id = EditorGUIUtility.GetControlID(m_HintID, FocusType.Passive);
            value = EditorGUIXLayout.HorizontalHandle(id, value, minValue, maxValue);
        }
    }
}
