using System;
using System.Xml.Serialization;

namespace UnityEditor.Experimental.WXMLInternal
{
    [Serializable]
    public class DOMEditorPrefsProperty
    {
        public enum Type
        {
            [XmlEnum("")] None,
            [XmlEnum("int")] Int,
            [XmlEnum("string")] String,
            [XmlEnum("bool")] Bool,
            [XmlEnum("enum")] Enum
        }

        [XmlAttribute]
        public Type type;
        [XmlAttribute]
        public string customType;
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public string vmPropertyName;
        [XmlAttribute("default")]
        public string @default;
    }
}
