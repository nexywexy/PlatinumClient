namespace Newtonsoft.Json.Serialization
{
    using System;
    using System.Runtime.CompilerServices;

    public class JsonISerializableContract : JsonContainerContract
    {
        public JsonISerializableContract(Type underlyingType) : base(underlyingType)
        {
            base.ContractType = JsonContractType.Serializable;
        }

        public ObjectConstructor<object> ISerializableCreator { get; set; }
    }
}

