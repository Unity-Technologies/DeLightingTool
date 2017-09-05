using System;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    [DOMTag(kTag)]
    [Serializable]
    public class DOMVHorizontalResize : DOMResizeBase
    {
        internal const string kTag = "horizontal-resize";
        internal const string kClass = "HorizontalResize";
    }

    public partial class VXMLDOMElementVisitor
    {
        void VisitIn(DOMVHorizontalResize horizontalResize)
        {
            Assert.IsNotNull(horizontalResize);

            var fieldName = WriteChild(DOMVHorizontalResize.kTag, GetCSharpType(horizontalResize, DOMVHorizontalResize.kClass), horizontalResize);

            WriteClasses(fieldName, horizontalResize.@class);
            DoVisit(horizontalResize, fieldName, DOMVHorizontalResize.kClass);
        }
    }
}
