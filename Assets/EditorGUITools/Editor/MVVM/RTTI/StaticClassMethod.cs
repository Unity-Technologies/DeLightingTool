using System;
using System.Reflection;
using UnityEngine;

namespace UnityEditor.Experimental
{
    public class StaticClassMethodBase<TOwner>
    {
        protected MethodInfo m_MethodInfo = null;

        public string commandName { get; private set; }
        public StaticClassMethodBase(string commandName)
        {
            this.commandName = commandName;
            m_MethodInfo = typeof(TOwner).GetMethod(commandName, BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, new Type[0], new ParameterModifier[0]);
            if (m_MethodInfo == null)
                Debug.LogWarningFormat("Method {0} was not found for type {1}", commandName, typeof(TOwner));
        }
    }

    public class StaticClassMethod<TOwner> : StaticClassMethodBase<TOwner>, IClassMethod
    {
        public StaticClassMethod(string commandName)
            : base(commandName) { }

        public void Execute(object context)
        {
            m_MethodInfo.Invoke(context, new object[0]);
        }
    }

    public class StaticClassMethod<TOwner, TArg0> : StaticClassMethodBase<TOwner>, IClassMethod<TArg0>
    {
        public StaticClassMethod(string commandName)
            : base(commandName) { }

        public void Execute(object context, TArg0 arg0)
        {
            m_MethodInfo.Invoke(context, new object[] { arg0 });
        }
    }

    public class StaticClassMethod<TOwner, TArg0, TArg1> : StaticClassMethodBase<TOwner>, IClassMethod<TArg0, TArg1>
    {
        public StaticClassMethod(string commandName)
            : base(commandName) { }

        public void Execute(object context, TArg0 arg0, TArg1 arg1)
        {
            m_MethodInfo.Invoke(context, new object[] { arg0, arg1 });
        }
    }

    public class StaticClassMethod<TOwner, TArg0, TArg1, TArg2> : StaticClassMethodBase<TOwner>, IClassMethod<TArg0, TArg1, TArg2>
    {
        public StaticClassMethod(string commandName)
            : base(commandName) { }

        public void Execute(object context, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            m_MethodInfo.Invoke(context, new object[] { arg0, arg1, arg2 });
        }
    }
}
