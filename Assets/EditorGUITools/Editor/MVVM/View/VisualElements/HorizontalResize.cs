using System;
using UnityEditor.Experimental.MVVM;
using UnityEngine;

namespace UnityEditor.Experimental.VisualElements
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
