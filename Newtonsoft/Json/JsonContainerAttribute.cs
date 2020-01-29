namespace Newtonsoft.Json
{
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple=false)]
    public abstract class JsonContainerAttribute : Attribute
    {
        internal bool? _isReference;
        internal bool? _itemIsReference;
        internal ReferenceLoopHandling? _itemReferenceLoopHandling;
        internal TypeNameHandling? _itemTypeNameHandling;
        private Type _namingStrategyType;
        private object[] _namingStrategyParameters;

        protected JsonContainerAttribute()
        {
        }

        protected JsonContainerAttribute(string id)
        {
            this.Id = id;
        }

        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Type ItemConverterType { get; set; }

        public object[] ItemConverterParameters { get; set; }

        public Type NamingStrategyType
        {
            get => 
                this._namingStrategyType;
            set
            {
                this._namingStrategyType = value;
                this.NamingStrategyInstance = null;
            }
        }

        public object[] NamingStrategyParameters
        {
            get => 
                this._namingStrategyParameters;
            set
            {
                this._namingStrategyParameters = value;
                this.NamingStrategyInstance = null;
            }
        }

        internal NamingStrategy NamingStrategyInstance { get; set; }

        public bool IsReference
        {
            get
            {
                bool? nullable = this._isReference;
                if (!nullable.HasValue)
                {
                    return false;
                }
                return nullable.GetValueOrDefault();
            }
            set => 
                (this._isReference = new bool?(value));
        }

        public bool ItemIsReference
        {
            get
            {
                bool? nullable = this._itemIsReference;
                if (!nullable.HasValue)
                {
                    return false;
                }
                return nullable.GetValueOrDefault();
            }
            set => 
                (this._itemIsReference = new bool?(value));
        }

        public ReferenceLoopHandling ItemReferenceLoopHandling
        {
            get
            {
                ReferenceLoopHandling? nullable = this._itemReferenceLoopHandling;
                if (!nullable.HasValue)
                {
                    return ReferenceLoopHandling.Error;
                }
                return nullable.GetValueOrDefault();
            }
            set => 
                (this._itemReferenceLoopHandling = new ReferenceLoopHandling?(value));
        }

        public TypeNameHandling ItemTypeNameHandling
        {
            get
            {
                TypeNameHandling? nullable = this._itemTypeNameHandling;
                if (!nullable.HasValue)
                {
                    return TypeNameHandling.None;
                }
                return nullable.GetValueOrDefault();
            }
            set => 
                (this._itemTypeNameHandling = new TypeNameHandling?(value));
        }
    }
}

