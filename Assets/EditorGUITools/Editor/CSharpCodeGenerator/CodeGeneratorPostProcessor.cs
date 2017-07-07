using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditor.Experimental.CodeGenerator
{
    class CodeGeneratorPostProcessor : AssetPostprocessor
    {
        static List<ICodeGenerator> s_CodeGenerators = null;
        static List<ICodeGenerator> CodeGenerators { get { FindCodeGeneratorIfNecessary(); return s_CodeGenerators; } }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var codeGenerator in CodeGenerators)
            {
                foreach (string str in importedAssets.Where(codeGenerator.IsManaged))
                {
                    codeGenerator.Import(str);
                }
                foreach (string str in deletedAssets.Where(codeGenerator.IsManaged))
                {
                    codeGenerator.Delete(str);
                }
                for (int i = 0; i < movedAssets.Length; i++)
                {
                    if (!codeGenerator.IsManaged(movedAssets[i]))
                        continue;

                    codeGenerator.Move(movedFromAssetPaths[i], movedAssets[i]);
                }
            }
        }

        static void FindCodeGeneratorIfNecessary()
        {
            // TODO: We should cache the search here
            s_CodeGenerators = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(ICodeGenerator).IsAssignableFrom(t) && !(t.IsInterface && t.IsAbstract))
                .Select(t => t.GetConstructor(new Type[0]))
                .Where(constructor => constructor != null)
                .Select(constructor => (ICodeGenerator)constructor.Invoke(null))
                .Where(instance => instance != null)
                .ToList();
        }
    }
}
