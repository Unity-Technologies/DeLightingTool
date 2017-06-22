using System;
using System.Xml.Serialization;

namespace UnityEditor.VMXMLInternal
{
    [Serializable]
    public class DOMField : DOMMember
    {
        [XmlAttribute]
        public string type;
        [XmlAttribute]
        public string name;
        [XmlAttribute("default")]
        public string @default;
    }
}
