using System;

namespace UnityEditor.Experimental
{
    public class AsyncOperation<TType>
    {
        Action<TType> m_Callbacks;
        Action<Exception> m_ErrorCallbacks;
        Action<float> m_ProgressCallback;

        public bool isComplete { get; protected set; }
        public TType data { get; protected set; }
        public Exception error { get; protected set; }
        public float progress { get; protected set; }

        public AsyncOperation<TType> OnComplete(Action<TType> callback)
        {
            if (isComplete && error == null)
                callback(data);
            else if (!isComplete)
                m_Callbacks += callback;

            return this;
        }

        public AsyncOperation<TType> OnError(Action<Exception> callback)
        {
            if (isComplete && error != null)
                callback(error);
            else if (!isComplete)
                m_ErrorCallbacks += callback;

            return this;
        }

        public AsyncOperation<TType> OnProgress(Action<float> callback)
        {
            if (!isComplete)
                m_ProgressCallback += callback;
            callback(progress);
            return this;
        }

        public void SetError(Exception e)
        {
            isComplete = true;
            error = e;
            if (m_ErrorCallbacks != null)
                m_ErrorCallbacks(error);
            m_ErrorCallbacks = null;
            m_ProgressCallback = null;
        }

        public void SetComplete(TType data)
        {
            SetProgress(1f);

            isComplete = true;
            this.data = data;
            if (m_Callbacks != null)
                m_Callbacks(data);
            m_Callbacks = null;
            m_ProgressCallback = null;
        }

        public void SetProgress(float progress)
        {
            this.progress = progress;
            if (m_ProgressCallback != null)
                m_ProgressCallback(progress);
        }
    }
}
