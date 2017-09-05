using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental
{
    public class DynamicClassProperty<TPropertyType> : ClassProperty<TPropertyType>
    {
        private Dictionary<Type, PropertyInfo> m_ResolvedProperties = new Dictionary<Type, PropertyInfo>();

        public DynamicClassProperty(string propertyName)
            : base(propertyName) {}

        public override TPropertyType GetValue(object context)
        {
            Assert.IsNotNull(context);

            var type = context.GetType();
            PropertyInfo property;
            if (!m_ResolvedProperties.TryGetValue(type, out property))
                property = ResolvePropertyFor(type);

            if (property != null)
                return (TPropertyType)Convert.ChangeType(property.GetValue(context, null), typeof(TPropertyType));

            return default(TPropertyType);
        }

        public override void SetValue(object context, TPropertyType value)
        {
            Assert.IsNotNull(context);

            var type = context.GetType();
            PropertyInfo property;
            if (!m_ResolvedProperties.TryGetValue(type, out property))
                property = ResolvePropertyFor(type);

            if (property != null)
                property.SetValue(context, value, null);
        }

        PropertyInfo ResolvePropertyFor(Type type)
        {
            var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            m_ResolvedProperties[type] = property;

            return property;
        }
    }
}
