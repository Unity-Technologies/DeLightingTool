using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Experimental.VXMLInternal
{
    class VXMLCodeGeneratorState : ScriptableObject
    {
        const string kStatePath = "Assets/VXMLCodeGeneratorState.asset";

        static VXMLCodeGeneratorState s_Instance = null;
        internal static VXMLCodeGeneratorState state
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = AssetDatabase.LoadAssetAtPath<VXMLCodeGeneratorState>(kStatePath);

                    if (s_Instance == null)
                    {
                        s_Instance = CreateInstance<VXMLCodeGeneratorState>();
                        //s_Instance.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable;
                        AssetDatabase.CreateAsset(s_Instance, kStatePath);
                    }
                }
                return s_Instance;
            }
        }

        [Serializable]
        struct FileDependency
        {
            [SerializeField]
            internal string declaringFile;
            [SerializeField]
            internal string referencedFile;
        }

        [SerializeField]
        List<FileDependency> m_FileDependency = new List<FileDependency>();

        HashSet<string> m_SetDependencies_Tmp = new HashSet<string>();
        public void SetDependencies(string assetPath, IEnumerable<string> fileDependencies)
        {
            m_SetDependencies_Tmp.Clear();
            foreach (var dep in m_FileDependency)
                if (dep.declaringFile == assetPath)
                    m_SetDependencies_Tmp.Add(dep.referencedFile);

            if (!m_SetDependencies_Tmp.SetEquals(fileDependencies))
            {
                m_FileDependency.RemoveAll(dep => dep.declaringFile == assetPath);
                foreach (var dep in fileDependencies)
                    m_FileDependency.Add(new FileDependency { referencedFile = dep, declaringFile = assetPath });

                EditorUtility.SetDirty(this);
            }
        }

        public IEnumerable<string> GetFilesThatDependsOn(string assetPath)
        {
            return m_FileDependency.Where(dep => dep.referencedFile == assetPath).Select(dep => dep.declaringFile);
        }

        public void MoveDependency(string fromPath, string toPath)
        {
            for (int i = 0; i < m_FileDependency.Count; i++)
            {
                var dep = m_FileDependency[i];

                if (dep.declaringFile == fromPath)
                    dep.declaringFile = toPath;

                if (dep.referencedFile == fromPath)
                    dep.referencedFile = toPath;

                m_FileDependency[i] = dep;
            }

            EditorUtility.SetDirty(this);
        }

        public void DeleteDependency(string assetPath)
        {
            for (int i = m_FileDependency.Count - 1; i >= 0; --i)
            {
                var dep = m_FileDependency[i];

                if (dep.declaringFile == assetPath || dep.referencedFile == assetPath)
                    m_FileDependency.RemoveAt(i);
            }

            EditorUtility.SetDirty(this);
        }
    }
}
