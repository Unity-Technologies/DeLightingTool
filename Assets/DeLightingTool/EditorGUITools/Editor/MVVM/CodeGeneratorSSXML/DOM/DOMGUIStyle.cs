using System;
using System.Xml.Serialization;
using UnityEngine;

namespace UnityEditor.Experimental.SSXMLInternal
{
    [Serializable]
    public class DOMGUIStyle
    {
        [Serializable]
        public class DOMGUIStyleState
        {
            [XmlAttribute("text-color-r")]
            public float textColorR = -1;
            [XmlAttribute("text-color-g")]
            public float textColorG = -1;
            [XmlAttribute("text-color-b")]
            public float textColorB = -1;
            [XmlAttribute("text-color-a")]
            public float textColorA = -1;
        }

        [XmlAttribute("image-position")]
        public ImagePosition imagePosition;
        [XmlAttribute("alignment")]
        public TextAnchor alignment;
        [XmlAttribute("word-wrap")]
        public bool wordWrap;
        [XmlAttribute("fixed-width")]
        public int fixedWidth = -1;
        [XmlAttribute("fixed-height")]
        public int fixedHeight = -1;
        [XmlAttribute("stretch-width")]
        public bool stretchWidth;
        [XmlAttribute("stretch-height")]
        public bool stretchHeight;
        [XmlAttribute("font-size")]
        public int fontSize = -1;
        [XmlAttribute("font-style")]
        public FontStyle fontStyle;
        [XmlAttribute("rich-text")]
        public bool richText;
        [XmlAttribute("content-offset-x")]
        public float contentOffsetX = float.MinValue;
        [XmlAttribute("content-offset-y")]
        public float contentOffsetY = float.MinValue;
        [XmlAttribute("border-top")]
        public int borderTop = int.MinValue;
        [XmlAttribute("border-right")]
        public int borderRight = int.MinValue;
        [XmlAttribute("border-bottom")]
        public int borderBottom = int.MinValue;
        [XmlAttribute("border-left")]
        public int borderLeft = int.MinValue;
        [XmlAttribute("margin-top")]
        public int marginTop = int.MinValue;
        [XmlAttribute("margin-right")]
        public int marginRight = int.MinValue;
        [XmlAttribute("margin-bottom")]
        public int marginBottom = int.MinValue;
        [XmlAttribute("margin-left")]
        public int marginLeft = int.MinValue;
        [XmlAttribute("padding-top")]
        public int paddingTop = int.MinValue;
        [XmlAttribute("padding-right")]
        public int paddingRight = int.MinValue;
        [XmlAttribute("padding-bottom")]
        public int paddingBottom = int.MinValue;
        [XmlAttribute("padding-left")]
        public int paddingLeft = int.MinValue;
        [XmlAttribute("overflow-top")]
        public int overflowTop = int.MinValue;
        [XmlAttribute("overflow-right")]
        public int overflowRight = int.MinValue;
        [XmlAttribute("overflow-bottom")]
        public int overflowBottom = int.MinValue;
        [XmlAttribute("overflow-left")]
        public int overflowLeft = int.MinValue;

        [XmlElement]
        public DOMGUIStyleState normal;
        [XmlElement]
        public DOMGUIStyleState hover;
        [XmlElement]
        public DOMGUIStyleState active;
        [XmlElement]
        public DOMGUIStyleState onNormal;
        [XmlElement]
        public DOMGUIStyleState onHover;
        [XmlElement]
        public DOMGUIStyleState onActive;
        [XmlElement]
        public DOMGUIStyleState focused;
        [XmlElement]
        public DOMGUIStyleState onFocused;
    }
}
