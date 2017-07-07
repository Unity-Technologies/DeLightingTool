using System;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    [DOMTag(kTag)]
    [Serializable]
    public class DOMEditorToggle : DOMToggle
    {
        internal new const string kTag = "editor-toggle";
        internal new const string kClass = "EditorToggle";
    }

    public partial class VXMLDOMElementVisitor
    {
        void VisitIn(DOMEditorToggle toggleGroup)
        {
            Assert.IsNotNull(toggleGroup);

            var fieldName = WriteChild(DOMEditorToggle.kTag, GetCSharpType(toggleGroup, DOMEditorToggle.kClass), toggleGroup);

            WriteClasses(fieldName, toggleGroup.@class);
            WriteSetOrBind(fieldName, DOMEditorToggle.kClass, "value", toggleGroup.value);
            WriteSetOrBind(fieldName, DOMEditorToggle.kClass, "labelTooltip", toggleGroup.labelTooltip, "{0}.{1} = \"{2}\";");
            WriteSetOrBind(fieldName, DOMEditorToggle.kClass, "labelText", toggleGroup.labelText, "{0}.{1} = \"{2}\";");
            WriteSetOrBind(fieldName, DOMEditorToggle.kClass, "label", toggleGroup.label);
            WriteSetOrBindTexture(fieldName, DOMEditorToggle.kClass, "labelImage", toggleGroup.labelImage);
        }
    }
}
