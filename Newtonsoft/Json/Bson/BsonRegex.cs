namespace Newtonsoft.Json.Bson
{
    using System;
    using System.Runtime.CompilerServices;

    internal class BsonRegex : BsonToken
    {
        public BsonRegex(string pattern, string options)
        {
            this.Pattern = new BsonString(pattern, false);
            this.Options = new BsonString(options, false);
        }

        public BsonString Pattern { get; set; }

        public BsonString Options { get; set; }

        public override BsonType Type =>
            BsonType.Regex;
    }
}

