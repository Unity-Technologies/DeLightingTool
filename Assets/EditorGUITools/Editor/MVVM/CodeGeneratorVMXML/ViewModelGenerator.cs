using UnityEditor.Experimental.CodeGenerator;

namespace UnityEditor.Experimental.VMXMLInternal
{
    class ViewModelGenerator : AssetCodeGeneratorBase<VMXMLDOMVisitor, DOMDocument>, ICodeGenerator
    {
        protected override string sourceFileExtension { get { return "vmxml"; } }
    }
}
