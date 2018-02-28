using UnityEngine;

namespace UnityEditor
{
    public static partial class Delighting
    {
        public interface ILoadAssetFolderOperation : IInput
        {
            string folderPath { get; }
        }
    }
}
