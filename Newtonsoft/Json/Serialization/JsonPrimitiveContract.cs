namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class JsonPrimitiveContract : JsonContract
    {
        private static readonly Dictionary<Type, ReadType> ReadTypeMap;

        static JsonPrimitiveContract()
        {
            Type type = typeof(byte[]);
            Dictionary<Type, ReadType> dictionary1 = new Dictionary<Type, ReadType> {
                [type] = ReadType.ReadAsBytes
            };
            Type type2 = typeof(byte);
            dictionary1[type2] = ReadType.ReadAsInt32;
            Type type3 = typeof(short);
            dictionary1[type3] = ReadType.ReadAsInt32;
            Type type4 = typeof(int);
            dictionary1[type4] = ReadType.ReadAsInt32;
            Type type5 = typeof(decimal);
            dictionary1[type5] = ReadType.ReadAsDecimal;
            Type type6 = typeof(bool);
            dictionary1[type6] = ReadType.ReadAsBoolean;
            Type type7 = typeof(string);
            dictionary1[type7] = ReadType.ReadAsString;
            Type type8 = typeof(DateTime);
            dictionary1[type8] = ReadType.ReadAsDateTime;
            Type type9 = typeof(DateTimeOffset);
            dictionary1[type9] = ReadType.ReadAsDateTimeOffset;
            Type type10 = typeof(float);
            dictionary1[type10] = ReadType.ReadAsDouble;
            Type type11 = typeof(double);
            dictionary1[type11] = ReadType.ReadAsDouble;
            ReadTypeMap = dictionary1;
        }

        public JsonPrimitiveContract(Type underlyingType) : base(underlyingType)
        {
            base.ContractType = JsonContractType.Primitive;
            this.TypeCode = ConvertUtils.GetTypeCode(underlyingType);
            base.IsReadOnlyOrFixedSize = true;
            if (ReadTypeMap.TryGetValue(base.NonNullableUnderlyingType, out ReadType type))
            {
                base.InternalReadType = type;
            }
        }

        internal PrimitiveTypeCode TypeCode { get; set; }
    }
}

