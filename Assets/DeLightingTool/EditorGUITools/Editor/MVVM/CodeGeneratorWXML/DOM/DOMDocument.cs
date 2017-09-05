using System;
using System.Xml.Serialization;

namespace UnityEditor.Experimental.WXMLInternal
{
    [Serializable]
    [XmlRoot("window")]
    public class DOMDocument
    {
        [XmlAttribute("namespace")]
        public string @namespace;

        [XmlAttribute]
        public string title;
        [XmlAttribute]
        public string rootClass;
        [XmlAttribute]
        public string serviceClass;
        [XmlAttribute]
        public string exposedServiceClass;

        [XmlElement("using")]
        public DOMUsing[] usings;

        [XmlElement("editorPrefs")]
        public DOMEditorPrefsProperty[] editorPrefsProperties;
    }
}
