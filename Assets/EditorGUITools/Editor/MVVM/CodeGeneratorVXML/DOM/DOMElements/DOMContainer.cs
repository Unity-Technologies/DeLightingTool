using System;
using System.Xml.Serialization;

namespace UnityEditor.Experimental.VXMLInternal
{
    [Serializable]
    public class DOMContainer : DOMElement
    {
        public DOMElement[] children;
    }
}
