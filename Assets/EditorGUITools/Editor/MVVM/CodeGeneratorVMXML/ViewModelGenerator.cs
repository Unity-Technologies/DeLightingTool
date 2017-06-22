using UnityEditor.CodeGenerator;

namespace UnityEditor.VMXMLInternal
{
    class ViewModelGenerator : AssetCodeGeneratorBase<VMXMLDOMVisitor, DOMDocument>, ICodeGenerator
    {
        protected override string sourceFileExtension { get { return "vmxml"; } }
    }
}
