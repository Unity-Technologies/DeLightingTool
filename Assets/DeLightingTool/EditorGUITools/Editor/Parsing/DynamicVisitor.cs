using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental
{
    abstract public class DynamicVisitor<TBaseType>
    {
        private object m_Handler = null;
        private Dictionary<Type, MethodInfo> m_VisitInHandlers = new Dictionary<Type, MethodInfo>();
        private Dictionary<Type, MethodInfo> m_VisitOutHandlers = new Dictionary<Type, MethodInfo>();

        public void SetHandler(object handler)
        {
            Assert.IsNotNull(handler);
            m_Handler = handler;

            PopulateHandler(m_Handler.GetType(), "VisitIn", m_VisitInHandlers);
            PopulateHandler(m_Handler.GetType(), "VisitOut", m_VisitOutHandlers);
        }

        private void PopulateHandler(Type handlerType, string name, Dictionary<Type, MethodInfo> handlers)
        {
            foreach (
                var methodInfo in
                handlerType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public
                                               | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
            {
                if (methodInfo.Name == name
                    && methodInfo.GetParameters().Length == 1
                    && typeof(TBaseType).IsAssignableFrom(methodInfo.GetParameters()[0].ParameterType)
                    // Avoid recursion
                    && typeof(TBaseType) != methodInfo.GetParameters()[0].ParameterType)
                {
                    var type = methodInfo.GetParameters()[0].ParameterType;
                    handlers.Add(type, methodInfo);
                }
            }
        }

        public void Visit(TBaseType node)
        {
            var type = node.GetType();
            var methodInfo = GetMethodFor(type, m_VisitInHandlers);
            if (methodInfo != null)
                methodInfo.Invoke(m_Handler, new object[] { node });

            var children = GetChildrenOf(node);
            if (children != null)
            {
                foreach(var child in children)
                    Visit(child);
            }

            methodInfo = GetMethodFor(type, m_VisitOutHandlers);
            if (methodInfo != null)
                methodInfo.Invoke(m_Handler, new object[] { node });
        }

        abstract protected IEnumerable<TBaseType> GetChildrenOf(TBaseType node);

        private MethodInfo GetMethodFor(Type type, Dictionary<Type, MethodInfo> handlers)
        {
            if (handlers.ContainsKey(type))
                return handlers[type];

            Type candidate = null;
            foreach (var handlerPair in handlers)
            {
                if (handlerPair.Key.IsAssignableFrom(type)
                    && (candidate == null || candidate != null && candidate.IsAssignableFrom(handlerPair.Key)))
                    candidate = handlerPair.Key;
            }

            return candidate != null
                ? handlers[candidate]
                : null;
        }
    }
}
