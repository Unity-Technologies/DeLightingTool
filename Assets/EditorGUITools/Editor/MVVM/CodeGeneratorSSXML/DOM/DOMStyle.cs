using System;
using System.Xml.Serialization;

namespace UnityEditor.Experimental.SSXMLInternal
{
    [Serializable]
    public class DOMStyle
    {
        [XmlAttribute]
        public string name;
        [XmlAttribute("element-type")]
        public string elementType;

        [XmlAttribute("expand-width")]
        public bool expandWidth;
        [XmlAttribute("expand-height")]
        public bool expandHeight;
        [XmlAttribute("min-width")]
        public float minWidth = -1;
        [XmlAttribute("min-height")]
        public float minHeight = -1;
        [XmlAttribute("width")]
        public float width = -1;
        [XmlAttribute("height")]
        public float height = -1;
        [XmlAttribute("max-width")]
        public float maxWidth = -1;
        [XmlAttribute("max-height")]
        public float maxHeight = -1;

        [XmlAttribute("layout")]
        public StyleSheet.GUILayoutType layout = StyleSheet.GUILayoutType.None;
        [XmlAttribute("alignment")]
        public StyleSheet.Alignment alignment = StyleSheet.Alignment.None;

        [XmlAttribute("gui-style")]
        public string guiStyle;

        [XmlElement("gui-style")]
        public DOMGUIStyle guiStyleOverrides;
    }
}
