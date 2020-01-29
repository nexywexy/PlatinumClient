namespace Newtonsoft.Json.Bson
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class BsonArray : BsonToken, IEnumerable<BsonToken>, IEnumerable
    {
        private readonly List<BsonToken> _children = new List<BsonToken>();

        public void Add(BsonToken token)
        {
            this._children.Add(token);
            token.Parent = this;
        }

        public IEnumerator<BsonToken> GetEnumerator() => 
            this._children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public override BsonType Type =>
            BsonType.Array;
    }
}

