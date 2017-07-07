using System;
using System.Xml.Serialization;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    [DOMTag(kTag)]
    [Serializable]
    public class DOMToggleGroup : DOMContainer
    {
        internal const string kTag = "toggle-group";
        internal const string kClass = "ToggleGroup";

        [XmlAttribute("class-first")]
        public string firstClass;
        [XmlAttribute("class-middle")]
        public string middleClass;
        [XmlAttribute("class-last")]
        public string lastClass;
        [XmlAttribute("class-single")]
        public string singleClass; 
        [XmlAttribute("allow-none-selected")]
        public string allowNoneSelected;
        [XmlAttribute("allow-multiple")]
        public string allowMultiple;
        [XmlAttribute("active-indexes")]
        public string activeIndexes;
        [XmlAttribute("active-index")]
        public string activeIndex;
    }

    public partial class VXMLDOMElementVisitor
    {
        void VisitIn(DOMToggleGroup toggleGroup)
        {
            Assert.IsNotNull(toggleGroup);

            var fieldName = WriteChild(DOMToggleGroup.kTag, GetCSharpType(toggleGroup, DOMToggleGroup.kClass), toggleGroup);

            WriteClasses(fieldName, toggleGroup.@class);
            WriteSetOrBind(fieldName, DOMToggleGroup.kClass, "firstClass", toggleGroup.firstClass, "{0}.{1} = \"{2}\";");
            WriteSetOrBind(fieldName, DOMToggleGroup.kClass, "middleClass", toggleGroup.middleClass, "{0}.{1} = \"{2}\";");
            WriteSetOrBind(fieldName, DOMToggleGroup.kClass, "lastClass", toggleGroup.lastClass, "{0}.{1} = \"{2}\";");
            WriteSetOrBind(fieldName, DOMToggleGroup.kClass, "singleClass", toggleGroup.singleClass, "{0}.{1} = \"{2}\";");
            WriteSetOrBind(fieldName, DOMToggleGroup.kClass, "allowNoneSelected", toggleGroup.allowNoneSelected);
            WriteSetOrBind(fieldName, DOMToggleGroup.kClass, "allowMultiple", toggleGroup.allowMultiple);
            WriteSetOrBind(fieldName, DOMToggleGroup.kClass, "activeIndexes", toggleGroup.activeIndexes);
            WriteSetOrBind(fieldName, DOMToggleGroup.kClass, "activeIndex", toggleGroup.activeIndex);

            PushAddChildMethod(fieldName);
        }

        void VisitOut(DOMToggleGroup toggleGroup)
        {
            PopAddChildMethod();
        }
    }
}
