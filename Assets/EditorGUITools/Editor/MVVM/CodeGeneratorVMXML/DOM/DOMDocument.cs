using System;
using System.Xml.Serialization;

namespace UnityEditor.Experimental.VMXMLInternal
{
    [Serializable]
    [XmlRoot("view-model")]
    public class DOMDocument
    {
        [XmlAttribute("namespace")]
        public string @namespace;

        [XmlElement("command", typeof(DOMCommand))]
        [XmlElement("field", typeof(DOMField))]
        [XmlElement("property", typeof(DOMProperty))]
        [XmlElement("serialized-property", typeof(DOMSerializedProperty))]
        [XmlElement("using", typeof(DOMUsing))]
        public DOMMember[] members;
    }
}
