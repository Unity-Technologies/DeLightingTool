using System;
using System.Xml.Serialization;

namespace UnityEditor.Experimental.VMXMLInternal
{
    [Serializable]
    public class DOMProperty : DOMMember
    {
        [Serializable]
        public class DOMAccessor
        {
            [XmlText]
            public string textContent;
        }

        [XmlAttribute]
        public string type;
        [XmlAttribute]
        public string name;
        [XmlAttribute("default")]
        public string @default;

        [XmlElement]
        public DOMAccessor getter;
        [XmlElement]
        public DOMAccessor setter;
    }
}
