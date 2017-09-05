using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace UnityEditor.Experimental.CodeGenerator
{
    public abstract class AssetCodeGeneratorBase<TGenerator, TDOM>
        where TGenerator : IAssetCodeGenerator<TDOM>, new()
        where TDOM : class
    {
        const string kGeneratedFileName = "{0}.gen.cs";
        static TGenerator s_Generator = new TGenerator();
        static XmlSerializer s_DOMSerializer = null;
        protected XmlSerializer domSerializer
        {
            get { return s_DOMSerializer ?? (s_DOMSerializer = BuildSerializer()); }
        }
        internal static TGenerator generator { get { return s_Generator; } }
        internal static XmlSerializer serializer { get { return s_DOMSerializer; } }

        protected abstract string sourceFileExtension { get; }

        protected virtual XmlSerializer BuildSerializer() { return new XmlSerializer(typeof(TDOM)); }

        public virtual bool IsManaged(string assetPath) { return assetPath.EndsWith("." + sourceFileExtension); }

        public virtual void Import(string assetPath)
        {
            TDOM definitionInstance = null;
            using (var reader = new StreamReader(assetPath))
            {
                definitionInstance = domSerializer.Deserialize(reader) as TDOM;
            }
            if (definitionInstance == null)
            {
                Debug.LogWarning("Failed to parse VMXML document at: " + assetPath);
                return;
            }
            var workingDirectory = Path.GetDirectoryName(assetPath);
            var generatedCode = s_Generator.GenerateFrom(workingDirectory, GetAssetName(assetPath), definitionInstance);
            var targetPath = GetGeneratedFilePathFrom(assetPath);
            File.WriteAllText(targetPath, generatedCode);
            AssetDatabase.ImportAsset(targetPath);
        }

        public virtual void Move(string fromPath, string toPath)
        {
            var fromGenPath = GetGeneratedFilePathFrom(fromPath);
            var toGenPath = GetGeneratedFilePathFrom(toPath);

            AssetDatabase.MoveAsset(fromGenPath, toGenPath);
        }

        public virtual void Delete(string assetPath)
        {
            var genPath = GetGeneratedFilePathFrom(assetPath);
            AssetDatabase.DeleteAsset(genPath);
        }

        protected static string GetGeneratedFilePathFrom(string assetPath)
        {
            var directory = Path.GetDirectoryName(assetPath);
            var name = GetAssetName(assetPath);
            return Path.Combine(directory, string.Format(kGeneratedFileName, name));
        }

        protected static string GetAssetName(string assetPath)
        {
            return Path.GetFileNameWithoutExtension(assetPath);
        }
    }
}
