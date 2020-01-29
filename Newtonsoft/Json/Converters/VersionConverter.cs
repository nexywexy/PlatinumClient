namespace Newtonsoft.Json.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Globalization;

    public class VersionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => 
            (objectType == typeof(Version));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            if (reader.TokenType == JsonToken.String)
            {
                try
                {
                    return new Version((string) reader.Value);
                }
                catch (Exception exception)
                {
                    throw JsonSerializationException.Create(reader, "Error parsing version string: {0}".FormatWith(CultureInfo.InvariantCulture, reader.Value), exception);
                }
            }
            throw JsonSerializationException.Create(reader, "Unexpected token or value when parsing version. Token: {0}, Value: {1}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType, reader.Value));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                if (!(value is Version))
                {
                    throw new JsonSerializationException("Expected Version object value");
                }
                writer.WriteValue(value.ToString());
            }
        }
    }
}

