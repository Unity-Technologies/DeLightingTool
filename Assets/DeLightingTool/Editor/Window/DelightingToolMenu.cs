namespace UnityEditor.DelightingInternal
{
    static class DelightingToolMenu
    {
        [MenuItem("Window/Experimental/Delighting Tool")]
        static void Open()
        {
            Delighting.FocusWindow();
        }
    }
}
