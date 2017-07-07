using System;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    [DOMTag(kTag)]
    [Serializable]
    public class DOMEditorModalProgressBar : DOMContainer
    {
        internal const string kTag = "editor-modal-progress-bar";
        internal const string kClass = "EditorModalProgressBar";

        [XmlAttribute]
        public string value;
        [XmlAttribute]
        public string title;
        [XmlAttribute]
        public string cancellable;
        [XmlAttribute]
        public string onCancelled;
        [XmlText]
        public string info;
    }

    public partial class VXMLDOMElementVisitor
    {
        void VisitIn(DOMEditorModalProgressBar editorModalProgressBar)
        {
            Assert.IsNotNull(editorModalProgressBar);

            var fieldName = WriteChild(DOMEditorModalProgressBar.kTag, GetCSharpType(editorModalProgressBar, DOMEditorModalProgressBar.kClass), editorModalProgressBar);
            WriteClasses(fieldName, editorModalProgressBar.@class);

            WriteSetOrBind(fieldName, DOMEditorModalProgressBar.kClass, "value", editorModalProgressBar.value, "{0}.{1} = \"{2}\";");
            WriteSetOrBind(fieldName, DOMEditorModalProgressBar.kClass, "title", editorModalProgressBar.title, "{0}.{1} = \"{2}\";");
            WriteSetOrBind(fieldName, DOMEditorModalProgressBar.kClass, "cancellable", editorModalProgressBar.cancellable);
            WriteSetOrBind(fieldName, DOMEditorModalProgressBar.kClass, "info", editorModalProgressBar.info, "{0}.{1} = \"{2}\";");
            WriteBind(fieldName, DOMEditorModalProgressBar.kClass, "onCancelled", editorModalProgressBar.onCancelled);
        }
    }
}
