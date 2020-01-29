namespace Newtonsoft.Json.Bson
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class BsonObject : BsonToken, IEnumerable<BsonProperty>, IEnumerable
    {
        private readonly List<BsonProperty> _children = new List<BsonProperty>();

        public void Add(string name, BsonToken token)
        {
            BsonProperty item = new BsonProperty {
                Name = new BsonString(name, false),
                Value = token
            };
            this._children.Add(item);
            token.Parent = this;
        }

        public IEnumerator<BsonProperty> GetEnumerator() => 
            this._children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public override BsonType Type =>
            BsonType.Object;
    }
}

