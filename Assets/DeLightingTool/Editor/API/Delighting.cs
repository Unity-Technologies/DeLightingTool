using UnityEditor.Experimental.DelightingInternal;

namespace UnityEditor.Experimental
{
    public static partial class Delighting
    {
        static DelightingToolWindow s_MainDelightingToolWindow = null;

        public static void FocusWindow()
        {
            if (s_MainDelightingToolWindow == null)
                s_MainDelightingToolWindow = DelightingToolWindow.CreateWindow();

            s_MainDelightingToolWindow.Focus();
        }

        public static IDelighting NewService()
        {
            return new DelightingService();
        }
    }
}
