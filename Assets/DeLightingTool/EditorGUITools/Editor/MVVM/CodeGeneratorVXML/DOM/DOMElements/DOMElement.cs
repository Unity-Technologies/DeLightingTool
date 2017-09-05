using System;
using System.Xml.Serialization;

namespace UnityEditor.Experimental.VXMLInternal
{
    [Serializable]
    public class DOMElement
    {
        [XmlAttribute("type")]
        public string type;

        [XmlAttribute("class")]
        public string @class;

        [XmlAttribute("width")]
        public string widthOverride;

        [XmlAttribute("height")]
        public string heightOverride;
    }
}
