namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;

    internal class DefaultContractResolverState
    {
        public Dictionary<ResolverContractKey, JsonContract> ContractCache;
        public PropertyNameTable NameTable = new PropertyNameTable();
    }
}

