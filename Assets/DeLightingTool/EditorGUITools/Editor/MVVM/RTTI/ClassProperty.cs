using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Experimental
{
    public abstract class ClassProperty
    {
        public readonly string propertyName;

        public ClassProperty(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public override int GetHashCode()
        {
            return ("" + propertyName).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as ClassProperty;
            if (other == null)
                return false;

            return other.propertyName == propertyName;
        }
    }

    public abstract class ClassProperty<TPropertyType> : ClassProperty
    {
        public abstract TPropertyType GetValue(object context);
        public abstract void SetValue(object context, TPropertyType value);

        public ClassProperty(string propertyName)
            : base(propertyName) { }

        public bool ValueEquals(object context, TPropertyType otherValue)
        {
            var myValue = GetValue(context);
            return myValue == null && otherValue == null || myValue != null && myValue.Equals(otherValue);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ typeof(TPropertyType).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            var other = obj as ClassProperty<TPropertyType>;
            if (other == null)
                return false;

            return other.propertyName == propertyName;
        }
    }
}
