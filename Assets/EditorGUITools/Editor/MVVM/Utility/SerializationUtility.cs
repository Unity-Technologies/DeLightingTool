using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace UnityEditor.Experimental
{
    [AttributeUsage(AttributeTargets.Class)]
    public class XmlTagBaseAttribute : Attribute
    {
        public readonly string tagName;
        public XmlTagBaseAttribute(string tagName)
        {
            this.tagName = tagName;
        }
    }

    class SerializationUtility
    {
        public static XmlAttributeOverrides BuildAttributeOverrides<TDOMTagAttribute, TDOMNode, TDOMContainer>()
            where TDOMContainer : TDOMNode
            where TDOMTagAttribute : XmlTagBaseAttribute
        {
            var attrs = new XmlAttributes();
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(TDOMNode).IsAssignableFrom(t) && t.GetCustomAttributes(typeof(TDOMTagAttribute), false).Length > 0))
            {
                var xmlRootAttr = (TDOMTagAttribute)type.GetCustomAttributes(typeof(TDOMTagAttribute), false)[0];
                var newAttr = new XmlElementAttribute(xmlRootAttr.tagName, type);
                attrs.XmlElements.Add(newAttr);
            }
            var attrOverride = new XmlAttributeOverrides();

            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(TDOMContainer).IsAssignableFrom(t)))
                attrOverride.Add(type, "children", attrs);

            return attrOverride;
        }
    }
}
