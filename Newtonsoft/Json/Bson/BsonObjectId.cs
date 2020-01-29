namespace Newtonsoft.Json.Bson
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Runtime.CompilerServices;

    public class BsonObjectId
    {
        public BsonObjectId(byte[] value)
        {
            ValidationUtils.ArgumentNotNull(value, "value");
            if (value.Length != 12)
            {
                throw new ArgumentException("An ObjectId must be 12 bytes", "value");
            }
            this.Value = value;
        }

        public byte[] Value { get; private set; }
    }
}

