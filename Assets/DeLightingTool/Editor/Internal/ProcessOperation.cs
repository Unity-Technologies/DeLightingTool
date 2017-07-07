using UnityEngine;

namespace UnityEditor.Experimental.DelightingInternal
{
    class ProcessOperation : Delighting.IProcessOperation
    {
        public Delighting.ErrorCode error { get; internal set; }
        public Texture2D result { get; internal set; }
    }
}
