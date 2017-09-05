using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental
{
    public class AsyncOperationEnumerator<TType> : AsyncOperation<TType>
    {
        IEnumerator m_Enumerator = null;
        int m_StepCount;
        int m_MaxStep;

        public AsyncOperationEnumerator(IEnumerator enumerator, int maxStep)
        {
            Assert.IsTrue(maxStep >= 0);
            m_Enumerator = enumerator;
            m_MaxStep = maxStep;
            if (m_Enumerator == null)
                isComplete = true;
        }

        public object current { get { return m_Enumerator.Current; } }

        public bool MoveNext()
        {
            if (isComplete)
                return false;

            var hasNext = false;
            try
            {
                hasNext = m_Enumerator.MoveNext();
                ++m_StepCount;
                SetProgress(Mathf.Clamp01(m_StepCount / (float)m_MaxStep));

                if (!hasNext)
                    SetComplete((TType)m_Enumerator.Current);
            }
            catch (Exception e)
            {
                hasNext = false;
                SetError(e);
            }

            return hasNext;
        }

        public void Execute()
        {
            while (MoveNext()) { }
        }
    }
}
