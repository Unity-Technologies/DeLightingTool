namespace UnityEditor.Experimental.CodeGenerator
{
    public interface IAssetCodeGenerator<TDOM>
    {
        string GenerateFrom(string workingDir, string assetName, TDOM document);
    }
}
