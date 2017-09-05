using System;
using System.Xml.Serialization;

namespace UnityEditor.Experimental.VMXMLInternal
{
    [Serializable]
    public class DOMCommand : DOMMember
    {
        [Serializable]
        public class DOMArgument
        {
            [XmlAttribute]
            public string type;
        }

        [XmlAttribute]
        public string type;
        [XmlAttribute]
        public string name;

        [XmlElement("arg")]
        public DOMArgument[] args;
    }
}
