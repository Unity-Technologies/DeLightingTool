using System;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    [DOMTag(kTag)]
    [Serializable]
    public class DOMDynamicElement : DOMElement
    {
        [Serializable]
        public class DOMElement : DOMContainer
        {
        }

        internal const string kTag = "dynamic-element";
        internal const string kClass = "DynamicElement";

        internal const string kContainerTag = "element";
        internal const string kContainerClass = "IMGUIVisualContainer";

        [XmlAttribute]
        public string activeIndex;

        [XmlElement("element")]
        public DOMElement[] elements;
    }

    public partial class VXMLDOMElementVisitor
    {
        void VisitIn(DOMDynamicElement dynamicElement)
        {
            Assert.IsNotNull(dynamicElement);

            var fieldName = WriteChild(DOMDynamicElement.kTag, GetCSharpType(dynamicElement, DOMDynamicElement.kClass), dynamicElement);

            WriteClasses(fieldName, dynamicElement.@class);
            WriteSetOrBind(fieldName, DOMDynamicElement.kClass, "activeIndex", dynamicElement.activeIndex);

            if (dynamicElement.elements != null)
            {
                for (int i = 0; i < dynamicElement.elements.Length; i++)
                {
                    PushAddChildMethod(fieldName, "AddDynamicChild");
                    var elt = dynamicElement.elements[i];
                    var containerfieldName = WriteChild(DOMDynamicElement.kContainerTag, GetCSharpType(elt, DOMDynamicElement.kContainerClass), elt);
                    WriteClasses(containerfieldName, "expand-width expand-height");
                    PushAddChildMethod(containerfieldName);

                    Visit(dynamicElement.elements[i]);

                    PopAddChildMethod();
                    PopAddChildMethod();
                }
            }
        }
    }
}
