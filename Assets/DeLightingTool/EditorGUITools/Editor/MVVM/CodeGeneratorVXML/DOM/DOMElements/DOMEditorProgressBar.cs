using System;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    [DOMTag(kTag)]
    [Serializable]
    public class DOMEditorProgressBar : DOMContainer
    {
        internal const string kTag = "editor-progress-bar";
        internal const string kClass = "EditorProgressBar";

        [XmlAttribute]
        public string value;
        [XmlText]
        public string content;
    }

    public partial class VXMLDOMElementVisitor
    {
        void VisitIn(DOMEditorProgressBar editorProgressBar)
        {
            Assert.IsNotNull(editorProgressBar);

            var fieldName = WriteChild(DOMEditorProgressBar.kTag, GetCSharpType(editorProgressBar, DOMEditorProgressBar.kClass), editorProgressBar);
            WriteClasses(fieldName, editorProgressBar.@class);

            WriteSetOrBind(fieldName, DOMEditorProgressBar.kClass, "value", editorProgressBar.value);
            WriteSetOrBind(fieldName, DOMEditorProgressBar.kClass, "content", editorProgressBar.content, "{0}.{1} = \"{2}\";");
        }
    }
}
