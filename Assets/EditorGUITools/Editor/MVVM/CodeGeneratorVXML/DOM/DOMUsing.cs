using System;
using System.Xml.Serialization;

namespace UnityEditor.Experimental.VXMLInternal
{
    [Serializable]
    public class DOMUsing : DOMNode
    {
        [XmlText]
        public string textContent;

        public override int GetHashCode()
        {
            return ("" + textContent).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DOMUsing))
                return false;

            var tobj = (DOMUsing)obj;
            return textContent == tobj.textContent;
        }
    }
}
