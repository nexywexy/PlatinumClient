namespace Newtonsoft.Json.Serialization
{
    using System;

    public class JsonStringContract : JsonPrimitiveContract
    {
        public JsonStringContract(Type underlyingType) : base(underlyingType)
        {
            base.ContractType = JsonContractType.String;
        }
    }
}

