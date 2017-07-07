namespace UnityEditor.Experimental
{
    public interface IClassMethod
    {
        void Execute(object context);
    }

    public interface IClassMethod<TArg>
    {
        void Execute(object context, TArg arg);
    }

    public interface IClassMethod<TArg0, TArg1>
    {
        void Execute(object context, TArg0 arg0, TArg1 arg1);
    }

    public interface IClassMethod<TArg0, TArg1, TArg2>
    {
        void Execute(object context, TArg0 arg0, TArg1 arg1, TArg2 arg2);
    }
}
