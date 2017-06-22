using System;
using System.Xml.Serialization;

namespace UnityEditor.VXMLInternal
{
    [Serializable]
    public class DOMStyleSheet
    {
        [XmlAttribute]
        public string path;
        [XmlAttribute]
        public string name;
    }
}
