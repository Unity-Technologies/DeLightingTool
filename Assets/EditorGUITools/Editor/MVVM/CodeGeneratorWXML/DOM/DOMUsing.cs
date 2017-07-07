using System;
using System.Xml.Serialization;

namespace UnityEditor.Experimental.WXMLInternal
{
    [Serializable]
    public class DOMUsing
    {
        [XmlText]
        public string textContent;
    }
}
