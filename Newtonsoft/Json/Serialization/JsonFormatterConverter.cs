namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    internal class JsonFormatterConverter : IFormatterConverter
    {
        private readonly JsonSerializerInternalReader _reader;
        private readonly JsonISerializableContract _contract;
        private readonly JsonProperty _member;

        public JsonFormatterConverter(JsonSerializerInternalReader reader, JsonISerializableContract contract, JsonProperty member)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            ValidationUtils.ArgumentNotNull(contract, "contract");
            this._reader = reader;
            this._contract = contract;
            this._member = member;
        }

        public object Convert(object value, Type type)
        {
            ValidationUtils.ArgumentNotNull(value, "value");
            JToken token = value as JToken;
            if (token == null)
            {
                throw new ArgumentException("Value is not a JToken.", "value");
            }
            return this._reader.CreateISerializableItem(token, type, this._contract, this._member);
        }

        public object Convert(object value, TypeCode typeCode)
        {
            ValidationUtils.ArgumentNotNull(value, "value");
            if (value is JValue)
            {
                value = ((JValue) value).Value;
            }
            return System.Convert.ChangeType(value, typeCode, CultureInfo.InvariantCulture);
        }

        private T GetTokenValue<T>(object value)
        {
            ValidationUtils.ArgumentNotNull(value, "value");
            return (T) System.Convert.ChangeType(((JValue) value).Value, typeof(T), CultureInfo.InvariantCulture);
        }

        public bool ToBoolean(object value) => 
            this.GetTokenValue<bool>(value);

        public byte ToByte(object value) => 
            this.GetTokenValue<byte>(value);

        public char ToChar(object value) => 
            this.GetTokenValue<char>(value);

        public DateTime ToDateTime(object value) => 
            this.GetTokenValue<DateTime>(value);

        public decimal ToDecimal(object value) => 
            this.GetTokenValue<decimal>(value);

        public double ToDouble(object value) => 
            this.GetTokenValue<double>(value);

        public short ToInt16(object value) => 
            this.GetTokenValue<short>(value);

        public int ToInt32(object value) => 
            this.GetTokenValue<int>(value);

        public long ToInt64(object value) => 
            this.GetTokenValue<long>(value);

        public sbyte ToSByte(object value) => 
            this.GetTokenValue<sbyte>(value);

        public float ToSingle(object value) => 
            this.GetTokenValue<float>(value);

        public string ToString(object value) => 
            this.GetTokenValue<string>(value);

        public ushort ToUInt16(object value) => 
            this.GetTokenValue<ushort>(value);

        public uint ToUInt32(object value) => 
            this.GetTokenValue<uint>(value);

        public ulong ToUInt64(object value) => 
            this.GetTokenValue<ulong>(value);
    }
}

