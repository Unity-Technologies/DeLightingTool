using System;
using UnityEditor.Experimental;
using UnityEngine;

namespace UnityEditor
{
    public interface IDelighting : IDisposable
    {
        AsyncOperationEnumerator<Delighting.ILoadAssetFolderOperation> LoadInputFolderAsync(string inputfolder);
        Delighting.ErrorCode ValidateInputs();
        AsyncOperationEnumerator<Delighting.IProcessOperation> ProcessAsync(Delighting.ProcessArgs args);
        void SetInput(Delighting.IInput data);
    }
}
