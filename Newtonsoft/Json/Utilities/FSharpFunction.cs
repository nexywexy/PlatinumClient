namespace Newtonsoft.Json.Utilities
{
    using System;

    internal class FSharpFunction
    {
        private readonly object _instance;
        private readonly MethodCall<object, object> _invoker;

        public FSharpFunction(object instance, MethodCall<object, object> invoker)
        {
            this._instance = instance;
            this._invoker = invoker;
        }

        public object Invoke(params object[] args) => 
            this._invoker(this._instance, args);
    }
}

