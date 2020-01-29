namespace Newtonsoft.Json.Bson
{
    using System;
    using System.Runtime.CompilerServices;

    internal class BsonProperty
    {
        public BsonString Name { get; set; }

        public BsonToken Value { get; set; }
    }
}

