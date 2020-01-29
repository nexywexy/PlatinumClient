namespace Newtonsoft.Json.Serialization
{
    using System;

    public class JsonLinqContract : JsonContract
    {
        public JsonLinqContract(Type underlyingType) : base(underlyingType)
        {
            base.ContractType = JsonContractType.Linq;
        }
    }
}

