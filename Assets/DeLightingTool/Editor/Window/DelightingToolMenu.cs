namespace UnityEditor.DelightingInternal
{
    static class DelightingToolMenu
    {
        [MenuItem("Window/Delighting Tool")]
        static void Open()
        {
            Delighting.FocusWindow();
        }
    }
}
