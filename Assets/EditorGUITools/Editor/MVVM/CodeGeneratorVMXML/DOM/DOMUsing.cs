using System;
using System.Xml;
using System.Xml.Serialization;

namespace UnityEditor.Experimental.VMXMLInternal
{
    [Serializable]
    public class DOMUsing : DOMMember
    {
        [XmlText]
        public string textContent;
    }
}
