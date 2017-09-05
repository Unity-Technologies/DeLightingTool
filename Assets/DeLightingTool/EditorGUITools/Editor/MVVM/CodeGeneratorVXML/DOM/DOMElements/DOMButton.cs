using System;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    [DOMTag(kTag)]
    [Serializable]
    public class DOMButton : DOMContainer
    {
        internal const string kTag = "button";
        internal const string kClass = "Button";

        [XmlAttribute]
        public string label;
        [XmlAttribute("label-image")]
        public string labelImage;
        [XmlAttribute("label-tooltip")]
        public string labelTooltip;
        [XmlAttribute("on-click")]
        public string onClick;
        [XmlText]
        public string labelText;
    }

    public partial class VXMLDOMElementVisitor
    {
        void VisitIn(DOMButton button)
        {
            Assert.IsNotNull(button);

            var fieldName = WriteChild(DOMButton.kTag, GetCSharpType(button, DOMButton.kClass), button);
            WriteClasses(fieldName, button.@class);

            WriteSetOrBind(fieldName, DOMButton.kClass, "label", button.label);
            WriteSetOrBindTexture(fieldName, DOMButton.kClass, "labelImage", button.labelImage);
            WriteSetOrBind(fieldName, DOMButton.kClass, "labelTooltip", button.labelTooltip);
            WriteBind(fieldName, DOMButton.kClass, "onClick", button.onClick);
            WriteSetOrBind(fieldName, DOMButton.kClass, "labelText", button.labelText, "{0}.{1} = \"{2}\";");
        }
    }
}
