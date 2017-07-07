using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental
{
    public class DynamicClassMethodBase
    {
        protected Dictionary<Type, MethodInfo> m_ResolvedMethods = new Dictionary<Type, MethodInfo>();

        public string commandName { get; private set; }

        public DynamicClassMethodBase(string commandName)
        {
            this.commandName = commandName;
        }

        protected MethodInfo ResolveMethodFor(Type type)
        {
            var method = type.GetMethod(commandName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            m_ResolvedMethods[type] = method;

            return method;
        }
    }

    public class DynamicClassMethod : DynamicClassMethodBase, IClassMethod
    {
        public DynamicClassMethod(string commandName)
            : base(commandName) { }

        public void Execute(object context)
        {
            Assert.IsNotNull(context);

            var type = context.GetType();
            MethodInfo method;
            if (!m_ResolvedMethods.TryGetValue(type, out method))
                method = ResolveMethodFor(type);

            if (method != null)
                method.Invoke(context, new object[0]);
        }
    }

    public class DynamicClassMethod<TArg> : DynamicClassMethodBase, IClassMethod<TArg>
    {
        public DynamicClassMethod(string commandName)
            : base(commandName) { }

        public void Execute(object context, TArg arg)
        {
            Assert.IsNotNull(context);

            var type = context.GetType();
            MethodInfo method;
            if (!m_ResolvedMethods.TryGetValue(type, out method))
                method = ResolveMethodFor(type);

            if (method != null)
                method.Invoke(context, new object[] { arg });
        }
    }

    public class DynamicClassMethod<TArg0, TArg1> : DynamicClassMethodBase, IClassMethod<TArg0, TArg1>
    {
        public DynamicClassMethod(string commandName)
            : base(commandName) { }

        public void Execute(object context, TArg0 arg0, TArg1 arg1)
        {
            Assert.IsNotNull(context);

            var type = context.GetType();
            MethodInfo method;
            if (!m_ResolvedMethods.TryGetValue(type, out method))
                method = ResolveMethodFor(type);

            if (method != null)
                method.Invoke(context, new object[] { arg0, arg1 });
        }
    }

    public class DynamicClassMethod<TArg0, TArg1, TArg2> : DynamicClassMethodBase, IClassMethod<TArg0, TArg1, TArg2>
    {
        public DynamicClassMethod(string commandName)
            : base(commandName) { }

        public void Execute(object context, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            Assert.IsNotNull(context);

            var type = context.GetType();
            MethodInfo method;
            if (!m_ResolvedMethods.TryGetValue(type, out method))
                method = ResolveMethodFor(type);

            if (method != null)
                method.Invoke(context, new object[] { arg0, arg1, arg2 });
        }
    }
}
