using System.IO;
using UnityEngine;

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

            var editorResources = Directory.GetFiles("Assets/EditorGUITools/Editor Default Resources");
            for (int i = 0; i < editorResources.Length; i++)
            {
                var from = editorResources[i].Replace(@"\\", "/");
                if (from.EndsWith(".meta"))  
                    continue;

                var to = Path.Combine(targetDir, Path.GetFileName(from));

                AssetDatabase.CopyAsset(from, to);
            }
        }
    }
}
