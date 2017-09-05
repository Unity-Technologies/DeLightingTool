using System.IO;

namespace UnityEditor.Experimental.EditorGUITools
{
    static class Install
    {
        [InitializeOnLoadMethod]
        static void Init()
        {
            var targetDir = "Assets/Editor Default Resources/EditorGUITools";
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            var editorResources = Directory.GetFiles("Assets/DeLightingTool/EditorGUITools/Editor Default Resources");
            for (int i = 0; i < editorResources.Length; i++)
            {
                var from = editorResources[i].Replace(@"\\", "/");
                if (from.EndsWith(".meta"))  
                    continue;

                var to = Path.Combine(targetDir, Path.GetFileName(from));

                if (!File.Exists(to))
                    AssetDatabase.CopyAsset(from, to);
            }
        }
    }
}
