using UnityEngine;

namespace UnityEditor.Experimental
{
    public static partial class Delighting
    {
        public enum ProcessStep
        {
            None,
            Gather,
            Delight,
            ColorCorrection
        }

        public struct ProcessArgs
        {
            public ProcessStep fromStep;
            public bool calculateResult;
        }

        public interface IProcessOperation
        {
            Delighting.ErrorCode error { get; }
            Texture2D result { get; }
        }
    }
    
}
