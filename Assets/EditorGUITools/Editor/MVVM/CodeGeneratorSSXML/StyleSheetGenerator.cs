using System.Xml.Serialization;
using UnityEditor.Experimental.CodeGenerator;
using UnityEngine;

namespace UnityEditor.Experimental.SSXMLInternal
{
    class StyleSheetGenerator : AssetCodeGeneratorBase<SSXMLDOMVisitor, DOMDocument>, ICodeGenerator
    {
        protected override string sourceFileExtension { get { return "ssxml"; } }
    }
}
