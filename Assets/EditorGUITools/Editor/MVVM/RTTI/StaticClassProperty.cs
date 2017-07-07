using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental
{
    public class StaticClassProperty<TPropertyType, TOwnerType> : ClassProperty<TPropertyType>
    {
        readonly PropertyInfo m_PropertyInfo;

        public StaticClassProperty(string propertyName)
            : base(propertyName)
        {
            var type = typeof(TOwnerType);
            m_PropertyInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (m_PropertyInfo == null)
                Debug.LogWarningFormat("Property {0} was not found in type {1}", propertyName, type);
        }

        public override int GetHashCode()
        {
            return typeof(TOwnerType).GetHashCode() ^ typeof(TPropertyType).GetHashCode() ^ (m_PropertyInfo != null ? m_PropertyInfo.GetHashCode() : 0);
        }

        public override TPropertyType GetValue(object context)
        {
            if (context == null)
            {
                Debug.LogError("Given context was null");
                return default(TPropertyType);
            }

            return m_PropertyInfo != null
                ? (TPropertyType)Convert.ChangeType(m_PropertyInfo.GetValue(context, null), typeof(TPropertyType))
                : default(TPropertyType);
        }

        public override void SetValue(object context, TPropertyType value)
        {
            if (context == null)
            {
                Debug.LogError("Given context was null");
                return;
            }

            Assert.IsNotNull(context);
            if (m_PropertyInfo != null)
                m_PropertyInfo.SetValue(context, value, null);
        }
    }
}
