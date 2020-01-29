namespace Newtonsoft.Json.Serialization
{
    using System;
    using System.Runtime.CompilerServices;

    public class ErrorContext
    {
        internal ErrorContext(object originalObject, object member, string path, Exception error)
        {
            this.OriginalObject = originalObject;
            this.Member = member;
            this.Error = error;
            this.Path = path;
        }

        internal bool Traced { get; set; }

        public Exception Error { get; private set; }

        public object OriginalObject { get; private set; }

        public object Member { get; private set; }

        public string Path { get; private set; }

        public bool Handled { get; set; }
    }
}

