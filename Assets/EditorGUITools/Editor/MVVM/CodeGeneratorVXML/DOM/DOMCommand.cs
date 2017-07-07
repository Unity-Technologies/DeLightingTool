using System;
using System.Xml.Serialization;

namespace UnityEditor.Experimental.VXMLInternal
{
    [Serializable]
    public class DOMCommand : DOMNode
    {
        [Serializable]
        public class DOMArgument
        {
            [XmlAttribute]
            public string type;

            public override int GetHashCode()
            {
                return ("" + type).GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var tobj = obj as DOMArgument;
                if (tobj == null)
                    return false;

                return tobj.type == type;
            }
        }

        [XmlAttribute]
        public string name;

        [XmlElement("arg")]
        public DOMArgument[] args;

        public override int GetHashCode()
        {
            var @base = ("" + name).GetHashCode();
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                    @base ^= ("" + args[i].type).GetHashCode();
            }
            return @base;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DOMCommand))
                return false;

            var tobj = (DOMCommand)obj;
            if (name != tobj.name)
                return false;

            if (args == null && tobj.args == null)
                return true;

            if (args != null && tobj.args == null
                || args == null && tobj.args != null)
                return false;

            if (args.Length != tobj.args.Length)
                return false;

            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].GetHashCode() != tobj.args[i].GetHashCode())
                    return false;
            }

            return true;
        }
    }
}
