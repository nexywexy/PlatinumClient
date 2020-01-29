namespace Newtonsoft.Json.Bson
{
    using System;
    using System.Runtime.CompilerServices;

    internal class BsonString : BsonValue
    {
        public BsonString(object value, bool includeLength) : base(value, BsonType.String)
        {
            this.IncludeLength = includeLength;
        }

        public int ByteCount { get; set; }

        public bool IncludeLength { get; set; }
    }
}

