using System.Xml.Serialization;
using UnityEditor.Experimental.CodeGenerator;

namespace UnityEditor.Experimental.VXMLInternal
{
    class ViewGenerator : AssetCodeGeneratorBase<VXMLDOMVisitor, DOMDocument>, ICodeGenerator
    {
        protected override string sourceFileExtension { get { return "vxml"; } }

        protected override XmlSerializer BuildSerializer()
        {
            var overrides = SerializationUtility.BuildAttributeOverrides<DOMTagAttribute, DOMElement, DOMContainer>();
            return new XmlSerializer(typeof(DOMDocument), overrides);
        }

        public override void Import(string assetPath)
        {
            base.Import(assetPath);

            var targetPath = GetGeneratedFilePathFrom(assetPath);

            AssetDatabase.StartAssetEditing();
            VXMLCodeGeneratorState.state.SetDependencies(assetPath, generator.fileDependencies);
            AssetDatabase.ImportAsset(targetPath);
            foreach (var dependency in VXMLCodeGeneratorState.state.GetFilesThatDependsOn(assetPath))
                AssetDatabase.ImportAsset(dependency);
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
        }

        public override void Move(string fromPath, string toPath)
        {
            VXMLCodeGeneratorState.state.MoveDependency(fromPath, toPath);
            base.Move(fromPath, toPath);
        }

        public override void Delete(string assetPath)
        {
            VXMLCodeGeneratorState.state.DeleteDependency(assetPath);

            base.Delete(assetPath);
        }
    }
}
