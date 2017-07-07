using System;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    [DOMTag(kTag)]
    [Serializable]
    public class DOMVerticalResize : DOMResizeBase
    {
        internal const string kTag = "vertical-resize";
        internal const string kClass = "VerticalResize";
    }

    public partial class VXMLDOMElementVisitor
    {
        void VisitIn(DOMVerticalResize verticalResize)
        {
            Assert.IsNotNull(verticalResize);

            var fieldName = WriteChild(DOMVerticalResize.kTag, GetCSharpType(verticalResize, DOMVerticalResize.kClass), verticalResize);

            WriteClasses(fieldName, verticalResize.@class);
            DoVisit(verticalResize, fieldName, DOMVerticalResize.kClass);
        }
    }
}
