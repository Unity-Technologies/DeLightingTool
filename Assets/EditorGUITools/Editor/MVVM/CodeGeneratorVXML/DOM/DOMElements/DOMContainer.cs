using System;
using System.Xml.Serialization;

namespace UnityEditor.VXMLInternal
{
    [Serializable]
    public class DOMContainer : DOMElement
    {
        public DOMElement[] children;
    }
}
