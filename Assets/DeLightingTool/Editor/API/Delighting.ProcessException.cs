using System;
using System.Collections.Generic;
using UnityEditor.Experimental.DelightingInternal;

namespace UnityEditor.Experimental
{
    public static partial class Delighting
    {
        public class ProcessException : Exception
        {
            string m_Errors;
            public readonly ErrorCode errorCode;

            public ProcessException(ErrorCode errorCode, string message, int width, int height)
                : base(message)
            {
                this.errorCode = errorCode;
                var errors = new List<string>();
                DelightingHelpers.GetErrorMessagesFrom(errorCode, errors, width, height);
                m_Errors = string.Join(", ", errors.ToArray());
            }

            public override string Message
            {
                get { return base.Message + "\n" + m_Errors; }
            }
        }
    }
}
