using System;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    [DOMTag(kTag)]
    [Serializable]
    public class DOMEditorTextField : DOMElement
    {
        internal const string kTag = "editor-text-field";
        internal const string kClass = "EditorTextField";

        [XmlAttribute]
        public string label;
        [XmlAttribute]
        public string value;
    }

    public partial class VXMLDOMElementVisitor
    {
        void VisitIn(DOMEditorTextField textField)
        {
            Assert.IsNotNull(textField);

            var fieldName = WriteChild(DOMEditorTextField.kTag, GetCSharpType(textField, DOMEditorTextField.kClass), textField);

            WriteClasses(fieldName, textField.@class);
            WriteSetOrBind(fieldName, DOMEditorTextField.kClass, "label", textField.label, "{0}.{1} = new GUIContent(\"{2}\");");
            WriteBind(fieldName, DOMEditorTextField.kClass, "value", textField.value);
        }
    }
}
