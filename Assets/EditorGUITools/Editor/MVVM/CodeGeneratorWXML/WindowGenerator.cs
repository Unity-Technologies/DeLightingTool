using UnityEditor.Experimental.CodeGenerator;

namespace UnityEditor.Experimental.WXMLInternal
{
    class WindowGenerator : AssetCodeGeneratorBase<WXMLDOMVisitor, DOMDocument>, ICodeGenerator
    {
        protected override string sourceFileExtension { get { return "wxml"; } }
    }
}
