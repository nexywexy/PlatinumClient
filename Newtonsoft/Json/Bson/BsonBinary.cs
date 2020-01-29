namespace Newtonsoft.Json.Bson
{
    using System;
    using System.Runtime.CompilerServices;

    internal class BsonBinary : BsonValue
    {
        public BsonBinary(byte[] value, BsonBinaryType binaryType) : base(value, BsonType.Binary)
        {
            this.BinaryType = binaryType;
        }

        public BsonBinaryType BinaryType { get; set; }
    }
}

