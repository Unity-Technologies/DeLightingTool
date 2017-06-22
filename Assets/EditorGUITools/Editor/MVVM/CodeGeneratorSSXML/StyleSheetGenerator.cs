using System.Xml.Serialization;
using UnityEditor.CodeGenerator;
using UnityEngine;

namespace UnityEditor.SSXMLInternal
{
    class StyleSheetGenerator : AssetCodeGeneratorBase<SSXMLDOMVisitor, DOMDocument>, ICodeGenerator
    {
        protected override string sourceFileExtension { get { return "ssxml"; } }
    }
}
