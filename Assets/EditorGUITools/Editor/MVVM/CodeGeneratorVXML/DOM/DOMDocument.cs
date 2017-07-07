using System;
using System.Xml.Serialization;

namespace UnityEditor.Experimental.VXMLInternal
{
    [Serializable]
    [XmlRoot("view")]
    public class DOMDocument
    {
        [XmlAttribute("namespace")]
        public string @namespace;

        [XmlElement("command", typeof(DOMCommand))]
        [XmlElement("import", typeof(DOMImport))]
        [XmlElement("property", typeof(DOMProperty))]
        [XmlElement("using", typeof(DOMUsing))]
        public DOMNode[] nodes;

        [XmlElement("root")]
        public DOMRoot root;

        [XmlElement]
        public DOMStyleSheet stylesheet;
    }
}
