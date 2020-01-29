namespace Newtonsoft.Json.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Globalization;

    public class IsoDateTimeConverter : DateTimeConverterBase
    {
        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
        private System.Globalization.DateTimeStyles _dateTimeStyles = System.Globalization.DateTimeStyles.RoundtripKind;
        private string _dateTimeFormat;
        private CultureInfo _culture;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool flag = ReflectionUtils.IsNullableType(objectType);
            Type type = flag ? Nullable.GetUnderlyingType(objectType) : objectType;
            if (reader.TokenType == JsonToken.Null)
            {
                if (!ReflectionUtils.IsNullableType(objectType))
                {
                    throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
                }
                return null;
            }
            if (reader.TokenType == JsonToken.Date)
            {
                if (type == typeof(DateTimeOffset))
                {
                    if (reader.Value is DateTimeOffset)
                    {
                        return reader.Value;
                    }
                    return new DateTimeOffset((DateTime) reader.Value);
                }
                if (reader.Value is DateTimeOffset)
                {
                    DateTimeOffset offset = (DateTimeOffset) reader.Value;
                    return offset.DateTime;
                }
                return reader.Value;
            }
            if (reader.TokenType != JsonToken.String)
            {
                throw JsonSerializationException.Create(reader, "Unexpected token parsing date. Expected String, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
            string str = reader.Value.ToString();
            if (string.IsNullOrEmpty(str) & flag)
            {
                return null;
            }
            if (type == typeof(DateTimeOffset))
            {
                if (!string.IsNullOrEmpty(this._dateTimeFormat))
                {
                    return DateTimeOffset.ParseExact(str, this._dateTimeFormat, this.Culture, this._dateTimeStyles);
                }
                return DateTimeOffset.Parse(str, this.Culture, this._dateTimeStyles);
            }
            if (!string.IsNullOrEmpty(this._dateTimeFormat))
            {
                return DateTime.ParseExact(str, this._dateTimeFormat, this.Culture, this._dateTimeStyles);
            }
            return DateTime.Parse(str, this.Culture, this._dateTimeStyles);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string str;
            if (value is DateTime)
            {
                DateTime time = (DateTime) value;
                if (((this._dateTimeStyles & System.Globalization.DateTimeStyles.AdjustToUniversal) == System.Globalization.DateTimeStyles.AdjustToUniversal) || ((this._dateTimeStyles & System.Globalization.DateTimeStyles.AssumeUniversal) == System.Globalization.DateTimeStyles.AssumeUniversal))
                {
                    time = time.ToUniversalTime();
                }
                if (this._dateTimeFormat == null)
                {
                }
                str = time.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", this.Culture);
            }
            else
            {
                if (!(value is DateTimeOffset))
                {
                    throw new JsonSerializationException("Unexpected value when converting date. Expected DateTime or DateTimeOffset, got {0}.".FormatWith(CultureInfo.InvariantCulture, ReflectionUtils.GetObjectType(value)));
                }
                DateTimeOffset offset = (DateTimeOffset) value;
                if (((this._dateTimeStyles & System.Globalization.DateTimeStyles.AdjustToUniversal) == System.Globalization.DateTimeStyles.AdjustToUniversal) || ((this._dateTimeStyles & System.Globalization.DateTimeStyles.AssumeUniversal) == System.Globalization.DateTimeStyles.AssumeUniversal))
                {
                    offset = offset.ToUniversalTime();
                }
                if (this._dateTimeFormat == null)
                {
                }
                str = offset.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", this.Culture);
            }
            writer.WriteValue(str);
        }

        public System.Globalization.DateTimeStyles DateTimeStyles
        {
            get => 
                this._dateTimeStyles;
            set => 
                (this._dateTimeStyles = value);
        }

        public string DateTimeFormat
        {
            get
            {
                if (this._dateTimeFormat == null)
                {
                }
                return string.Empty;
            }
            set => 
                (this._dateTimeFormat = string.IsNullOrEmpty(value) ? null : value);
        }

        public CultureInfo Culture
        {
            get
            {
                if (this._culture == null)
                {
                }
                return CultureInfo.CurrentCulture;
            }
            set => 
                (this._culture = value);
        }
    }
}

