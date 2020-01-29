namespace Newtonsoft.Json.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;

    public class ExpandoObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => 
            (objectType == typeof(ExpandoObject));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => 
            this.ReadValue(reader);

        private object ReadList(JsonReader reader)
        {
            IList<object> list = new List<object>();
            while (reader.Read())
            {
                JsonToken tokenType = reader.TokenType;
                if (tokenType != JsonToken.Comment)
                {
                    if (tokenType != JsonToken.EndArray)
                    {
                        object item = this.ReadValue(reader);
                        list.Add(item);
                    }
                    else
                    {
                        return list;
                    }
                }
            }
            throw JsonSerializationException.Create(reader, "Unexpected end when reading ExpandoObject.");
        }

        private object ReadObject(JsonReader reader)
        {
            IDictionary<string, object> dictionary = new ExpandoObject();
            while (reader.Read())
            {
                JsonToken tokenType = reader.TokenType;
                if (tokenType != JsonToken.PropertyName)
                {
                    if ((tokenType != JsonToken.Comment) && (tokenType == JsonToken.EndObject))
                    {
                        return dictionary;
                    }
                }
                else
                {
                    string str = reader.Value.ToString();
                    if (!reader.Read())
                    {
                        throw JsonSerializationException.Create(reader, "Unexpected end when reading ExpandoObject.");
                    }
                    object obj2 = this.ReadValue(reader);
                    dictionary[str] = obj2;
                    continue;
                }
            }
            throw JsonSerializationException.Create(reader, "Unexpected end when reading ExpandoObject.");
        }

        private object ReadValue(JsonReader reader)
        {
            if (!reader.MoveToContent())
            {
                throw JsonSerializationException.Create(reader, "Unexpected end when reading ExpandoObject.");
            }
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return this.ReadObject(reader);

                case JsonToken.StartArray:
                    return this.ReadList(reader);
            }
            if (!JsonTokenUtils.IsPrimitiveToken(reader.TokenType))
            {
                throw JsonSerializationException.Create(reader, "Unexpected token when converting ExpandoObject: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
            return reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override bool CanWrite =>
            false;
    }
}

