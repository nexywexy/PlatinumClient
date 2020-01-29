namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Runtime.CompilerServices;

    public class JsonContainerContract : JsonContract
    {
        private JsonContract _itemContract;
        private JsonContract _finalItemContract;

        internal JsonContainerContract(Type underlyingType) : base(underlyingType)
        {
            JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(underlyingType);
            if (cachedAttribute != null)
            {
                if (cachedAttribute.ItemConverterType != null)
                {
                    this.ItemConverter = JsonTypeReflector.CreateJsonConverterInstance(cachedAttribute.ItemConverterType, cachedAttribute.ItemConverterParameters);
                }
                this.ItemIsReference = cachedAttribute._itemIsReference;
                this.ItemReferenceLoopHandling = cachedAttribute._itemReferenceLoopHandling;
                this.ItemTypeNameHandling = cachedAttribute._itemTypeNameHandling;
            }
        }

        internal JsonContract ItemContract
        {
            get => 
                this._itemContract;
            set
            {
                this._itemContract = value;
                if (this._itemContract != null)
                {
                    this._finalItemContract = this._itemContract.UnderlyingType.IsSealed() ? this._itemContract : null;
                }
                else
                {
                    this._finalItemContract = null;
                }
            }
        }

        internal JsonContract FinalItemContract =>
            this._finalItemContract;

        public JsonConverter ItemConverter { get; set; }

        public bool? ItemIsReference { get; set; }

        public ReferenceLoopHandling? ItemReferenceLoopHandling { get; set; }

        public TypeNameHandling? ItemTypeNameHandling { get; set; }
    }
}

