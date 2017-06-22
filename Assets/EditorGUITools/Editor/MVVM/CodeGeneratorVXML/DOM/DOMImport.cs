using System;
using System.Xml.Serialization;

namespace UnityEditor.VXMLInternal
{
    [Serializable]
    public class DOMImport : DOMNode
    {
        [XmlText]
        public string textContent;
    }
}
