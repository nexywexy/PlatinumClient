namespace Newtonsoft.Json.Bson
{
    using System;
    using System.Runtime.CompilerServices;

    internal abstract class BsonToken
    {
        protected BsonToken()
        {
        }

        public abstract BsonType Type { get; }

        public BsonToken Parent { get; set; }

        public int CalculatedSize { get; set; }
    }
}

