namespace Newtonsoft.Json.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Globalization;

    public class BsonObjectIdConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => 
            (objectType == typeof(BsonObjectId));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Bytes)
            {
                throw new JsonSerializationException("Expected Bytes but got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
            return new BsonObjectId((byte[]) reader.Value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            BsonObjectId id = (BsonObjectId) value;
            BsonWriter writer2 = writer as BsonWriter;
            if (writer2 != null)
            {
                writer2.WriteObjectId(id.Value);
            }
            else
            {
                writer.WriteValue(id.Value);
            }
        }
    }
}

