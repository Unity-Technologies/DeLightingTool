using System;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    [DOMTag(kTag)]
    [Serializable]
    public class DOMLabel : DOMElement
    {
        internal const string kTag = "label";
        internal const string kClass = "Label";
        
        [XmlAttribute]
        public string tooltip;
        [XmlAttribute]
        public string image;
        [XmlText]
        public string text;
    }

    public partial class VXMLDOMElementVisitor
    {
        void VisitIn(DOMLabel label)
        {
            Assert.IsNotNull(label);

            var fieldName = WriteChild(DOMLabel.kTag, GetCSharpType(label, DOMLabel.kClass), label);

            WriteClasses(fieldName, label.@class);
            WriteSetOrBind(fieldName, DOMLabel.kClass, "text", label.text, "{0}.{1} = \"{2}\";");
            WriteSetOrBind(fieldName, DOMLabel.kClass, "tooltip", label.tooltip, "{0}.{1} = \"{2}\";");
            WriteSetOrBindTexture(fieldName, DOMLabel.kClass, "image", label.image);
        }
    }
}
