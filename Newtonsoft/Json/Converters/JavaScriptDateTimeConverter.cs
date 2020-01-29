namespace Newtonsoft.Json.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Globalization;

    public class JavaScriptDateTimeConverter : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (!ReflectionUtils.IsNullable(objectType))
                {
                    throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
                }
                return null;
            }
            if ((reader.TokenType != JsonToken.StartConstructor) || !string.Equals(reader.Value.ToString(), "Date", StringComparison.Ordinal))
            {
                throw JsonSerializationException.Create(reader, "Unexpected token or value when parsing date. Token: {0}, Value: {1}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType, reader.Value));
            }
            reader.Read();
            if (reader.TokenType != JsonToken.Integer)
            {
                throw JsonSerializationException.Create(reader, "Unexpected token parsing date. Expected Integer, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
            DateTime dateTime = DateTimeUtils.ConvertJavaScriptTicksToDateTime((long) reader.Value);
            reader.Read();
            if (reader.TokenType != JsonToken.EndConstructor)
            {
                throw JsonSerializationException.Create(reader, "Unexpected token parsing date. Expected EndConstructor, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
            if ((ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType) == typeof(DateTimeOffset))
            {
                return new DateTimeOffset(dateTime);
            }
            return dateTime;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            long num;
            if (value is DateTime)
            {
                num = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(((DateTime) value).ToUniversalTime());
            }
            else
            {
                if (!(value is DateTimeOffset))
                {
                    throw new JsonSerializationException("Expected date object value.");
                }
                DateTimeOffset offset = (DateTimeOffset) value;
                num = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(offset.ToUniversalTime().UtcDateTime);
            }
            writer.WriteStartConstructor("Date");
            writer.WriteValue(num);
            writer.WriteEndConstructor();
        }
    }
}

