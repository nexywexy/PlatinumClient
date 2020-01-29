namespace Newtonsoft.Json.Serialization
{
    using System;

    internal enum JsonContractType
    {
        None,
        Object,
        Array,
        Primitive,
        String,
        Dictionary,
        Dynamic,
        Serializable,
        Linq
    }
}

