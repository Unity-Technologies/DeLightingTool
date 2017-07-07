using UnityEngine;

namespace UnityEditor.Experimental.DelightingInternal
{
    static class DelightingStyles
    {
        internal static readonly GUIStyle preSlider;
        internal static readonly GUIStyle preSliderThumb;
        internal static readonly GUIStyle sectionTitle;

        static DelightingStyles()
        {
            preSlider = new GUIStyle("PreSlider");
            preSliderThumb = new GUIStyle("PreSliderThumb");
            sectionTitle = new GUIStyle("IN Title");
        }
    }
}
