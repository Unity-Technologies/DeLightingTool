using System;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    [Serializable]
    public class DOMResizeBase : DOMElement
    {
        [XmlAttribute]
        public string value;
        [XmlAttribute("min-value")]
        public string minValue;
        [XmlAttribute("max-value")]
        public string maxValue;
    }

    public partial class VXMLDOMElementVisitor
    {
        void DoVisit(DOMResizeBase tag, string fieldName, string @class)
        {
            WriteSetOrBind(fieldName, @class, "value", tag.value);
            WriteSetOrBind(fieldName, @class, "minValue", tag.minValue);
            WriteSetOrBind(fieldName, @class, "maxValue", tag.maxValue);
        }
    }
}
