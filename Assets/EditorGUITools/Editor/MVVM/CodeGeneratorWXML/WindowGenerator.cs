using UnityEditor.CodeGenerator;

namespace UnityEditor.WXMLInternal
{
    class WindowGenerator : AssetCodeGeneratorBase<WXMLDOMVisitor, DOMDocument>, ICodeGenerator
    {
        protected override string sourceFileExtension { get { return "wxml"; } }
    }
}
