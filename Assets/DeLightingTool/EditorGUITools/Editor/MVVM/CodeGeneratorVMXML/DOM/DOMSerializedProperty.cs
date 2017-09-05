using System;
using System.Xml.Serialization;

namespace UnityEditor.Experimental.VMXMLInternal
{
    [Serializable]
    public class DOMSerializedProperty : DOMMember
    {
        public enum Type
        {
            [XmlEnum("int")]
            Int,
            [XmlEnum("long")]
            Long,
            [XmlEnum("float")]
            Float,
            [XmlEnum("double")]
            Double,
            [XmlEnum("bool")]
            Bool,
            [XmlEnum("string")]
            String,
            [XmlEnum("Color")]
            Color,
            [XmlEnum("AnimationCurve")]
            AnimationCurve,
            [XmlEnum("Vector2")]
            Vector2,
            [XmlEnum("Vector3")]
            Vector3,
            [XmlEnum("Vector4")]
            Vector4,
            [XmlEnum("Quaternion")]
            Quaternion,
            [XmlEnum("Rect")]
            Rect,
            [XmlEnum("Bounds")]
            Bounds,
            [XmlEnum("enum")]
            Enum,
            [XmlEnum("object")]
            Object
        }

        [XmlAttribute]
        public Type type;
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public string path;
        [XmlAttribute]
        public string customType;
    }
}
