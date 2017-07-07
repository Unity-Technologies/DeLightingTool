using System;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    [DOMTag(kTag)]
    [Serializable]
    public class DOMToggle : DOMElement
    {
        internal const string kTag = "toggle";
        internal const string kClass = "Toggle";

        [XmlAttribute]
        public string value;
        [XmlAttribute]
        public string label;
        [XmlAttribute("label-image")]
        public string labelImage;
        [XmlAttribute("label-tooltip")]
        public string labelTooltip;
        [XmlText]
        public string labelText;
    }

    public partial class VXMLDOMElementVisitor
    {
        void VisitIn(DOMToggle toggle)
        {
            Assert.IsNotNull(toggle);

            var fieldName = WriteChild(DOMToggle.kTag, GetCSharpType(toggle, DOMToggle.kClass), toggle);

            WriteClasses(fieldName, toggle.@class);
            WriteSetOrBind(fieldName, DOMToggle.kClass, "value", toggle.value);
            WriteSetOrBind(fieldName, DOMToggle.kClass, "labelTooltip", toggle.labelTooltip, "{0}.{1} = \"{2}\";");
            WriteSetOrBind(fieldName, DOMToggle.kClass, "labelText", toggle.labelText, "{0}.{1} = \"{2}\";");
            WriteSetOrBind(fieldName, DOMToggle.kClass, "label", toggle.label);
            WriteSetOrBindTexture(fieldName, DOMToggle.kClass, "labelImage", toggle.labelImage);
        }
    }
}
