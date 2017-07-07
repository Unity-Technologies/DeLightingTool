using System;
using System.Collections.Generic;

namespace UnityEditor.Experimental.ViewModel
{
    public class SerializedViewModelBase : ViewModelBase
    {
        SerializedProperty m_Root = null;
        Dictionary<string, SerializedProperty> m_CachedProperties = new Dictionary<string, SerializedProperty>();

        protected SerializedProperty root { get { return m_Root; } }

        public void SetRoot(SerializedProperty root)
        {
            m_Root = root;
        }

        protected SerializedProperty this[string path]
        {
            get
            {
                SerializedProperty target = null;
                if (!m_CachedProperties.TryGetValue(path, out target))
                {
                    target = m_Root.FindPropertyRelative(path);
                    m_CachedProperties[path] = target;
                }
                return target;
            }
        }
    }
}
