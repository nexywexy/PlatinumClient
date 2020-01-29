namespace Newtonsoft.Json
{
    using System;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple=false)]
    public sealed class JsonObjectAttribute : JsonContainerAttribute
    {
        private Newtonsoft.Json.MemberSerialization _memberSerialization;
        internal Required? _itemRequired;

        public JsonObjectAttribute()
        {
        }

        public JsonObjectAttribute(Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            this.MemberSerialization = memberSerialization;
        }

        public JsonObjectAttribute(string id) : base(id)
        {
        }

        public Newtonsoft.Json.MemberSerialization MemberSerialization
        {
            get => 
                this._memberSerialization;
            set => 
                (this._memberSerialization = value);
        }

        public Required ItemRequired
        {
            get
            {
                Required? nullable = this._itemRequired;
                if (!nullable.HasValue)
                {
                    return Required.Default;
                }
                return nullable.GetValueOrDefault();
            }
            set => 
                (this._itemRequired = new Required?(value));
        }
    }
}

