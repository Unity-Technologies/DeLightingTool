using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UnityEditor.Experimental.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        bool m_RegisteredForUpdate = false;
        HashSet<ClassProperty> m_ChangedProperties = new HashSet<ClassProperty>();

        public void SetPropertyChanged(params ClassProperty[] properties)
        {
            if (!m_RegisteredForUpdate)
            {
                m_RegisteredForUpdate = true;
                EditorApplication.update += DoPropertyChanged;
            }
            m_ChangedProperties.UnionWith(properties);
        }

        void DoPropertyChanged()
        {
            EditorApplication.update -= DoPropertyChanged;
            m_RegisteredForUpdate = false;

            if (m_ChangedProperties.Count > 0)
            {
                if (PropertyChanged != null)
                {
                    if (m_ChangedProperties.Contains(null))
                        PropertyChanged(this, new PropertyChangedEventArgs(null));
                    else
                    {
                        foreach (var changedProperty in m_ChangedProperties)
                            PropertyChanged(this, new PropertyChangedEventArgs(changedProperty.propertyName));
                    }
                }
                m_ChangedProperties.Clear();
            }
        }
    }
}
