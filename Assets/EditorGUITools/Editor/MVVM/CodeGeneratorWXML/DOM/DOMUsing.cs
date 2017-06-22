using System;
using System.Xml.Serialization;

namespace UnityEditor.WXMLInternal
{
    [Serializable]
    public class DOMUsing
    {
        [XmlText]
        public string textContent;
    }
}
