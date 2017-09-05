namespace UnityEditor.Experimental.CodeGenerator
{
    public interface ICodeGenerator
    {
        bool IsManaged(string assetPath);
        void Import(string assetPath);
        void Move(string fromPath, string toPath);
        void Delete(string assetPath);
    }
}
