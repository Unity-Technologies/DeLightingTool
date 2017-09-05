using System;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    [DOMTag(kTag)]
    [Serializable]
    public class DOMEditorIntSlider : DOMElement
    {
        internal const string kTag = "editor-int-slider";
        internal const string kClass = "EditorIntSlider";

        [XmlAttribute]
        public string label;
        [XmlAttribute]
        public string value;
        [XmlAttribute]
        public string maxValue;
        [XmlAttribute]
        public string minValue;
    }

    public partial class VXMLDOMElementVisitor
    {
        void VisitIn(DOMEditorIntSlider slider)
        {
            Assert.IsNotNull(slider);

            var fieldName = WriteChild(DOMEditorIntSlider.kTag, GetCSharpType(slider, DOMEditorIntSlider.kClass), slider);

            WriteClasses(fieldName, slider.@class);
            WriteSetOrBind(fieldName, DOMEditorIntSlider.kClass, "label", slider.label, "{0}.{1} = new GUIContent(\"{2}\");");
            WriteSetOrBind(fieldName, DOMEditorIntSlider.kClass, "minValue", slider.minValue);
            WriteSetOrBind(fieldName, DOMEditorIntSlider.kClass, "maxValue", slider.maxValue);
            WriteBind(fieldName, DOMEditorIntSlider.kClass, "value", slider.value);
        }
    }
}
