using System;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    [DOMTag(kTag)]
    [Serializable]
    public class DOMDiv : DOMContainer
    {
        internal const string kTag = "div";
        internal const string kClass = "IMGUIVisualContainer";
    }

    public partial class VXMLDOMElementVisitor
    {
        void VisitIn(DOMDiv div)
        {
            Assert.IsNotNull(div);

            var fieldName = WriteChild(DOMDiv.kTag, GetCSharpType(div, DOMDiv.kClass), div);
            WriteClasses(fieldName, div.@class);

            PushAddChildMethod(fieldName);
        }

        void VisitOut(DOMDiv dropZone)
        {
            PopAddChildMethod();
        }
    }
}
