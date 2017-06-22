using System;
using System.Xml.Serialization;

namespace UnityEditor.SSXMLInternal
{
    [Serializable]
    [XmlRoot("stylesheet")]
    public class DOMDocument
    {
        [XmlAttribute("namespace")]
        public string @namespace;

        [XmlElement("style")]
        public DOMStyle[] styles;
    }
}
