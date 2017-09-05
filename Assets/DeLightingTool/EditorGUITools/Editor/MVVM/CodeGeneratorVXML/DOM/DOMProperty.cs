using System;
using System.Xml.Serialization;

namespace UnityEditor.Experimental.VXMLInternal
{
    [Serializable]
    public class DOMProperty : DOMNode
    {
        [XmlAttribute]
        public string type;
        [XmlAttribute]
        public string name;

        public override int GetHashCode()
        {
            return ("" + type + name).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DOMProperty))
                return false;

            var tobj = (DOMProperty)obj;
            return type == tobj.type && name == tobj.name;
        }
    }
}
