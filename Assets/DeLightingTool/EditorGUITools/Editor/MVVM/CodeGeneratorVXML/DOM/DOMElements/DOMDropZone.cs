using System;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    [DOMTag(kTag)]
    [Serializable]
    public class DOMDropZone : DOMContainer
    {
        internal const string kTag = "drop-zone";
        internal const string kClass = "DropZone";

        [XmlAttribute("on-potential-drop")]
        public string onPotentialDrop;
        [XmlAttribute("can-accept-drop")]
        public string canAcceptDrop;
        [XmlAttribute("on-drop")]
        public string onDrop;
    }

    public partial class VXMLDOMElementVisitor
    {
        void VisitIn(DOMDropZone dropZone)
        {
            Assert.IsNotNull(dropZone);

            var fieldName = WriteChild(DOMDropZone.kTag, GetCSharpType(dropZone, DOMDropZone.kClass), dropZone);
            WriteClasses(fieldName, dropZone.@class);

            WriteSetOrBind(fieldName, DOMDropZone.kClass, "canAcceptDrop", dropZone.canAcceptDrop);
            WriteBind(fieldName, DOMDropZone.kClass, "onDrop", dropZone.onDrop);
            WriteBind(fieldName, DOMDropZone.kClass, "onPotentialDrop", dropZone.onPotentialDrop);

            PushAddChildMethod(fieldName);
        }

        void VisitOut(DOMDropZone dropZone)
        {
            PopAddChildMethod();
        }
    }
}
