namespace Newtonsoft.Json.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Text.RegularExpressions;

    public class RegexConverter : JsonConverter
    {
        private const string PatternName = "Pattern";
        private const string OptionsName = "Options";

        public override bool CanConvert(Type objectType) => 
            (objectType == typeof(Regex));

        private bool HasFlag(RegexOptions options, RegexOptions flag) => 
            ((options & flag) == flag);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                return this.ReadRegexObject(reader, serializer);
            }
            if (reader.TokenType != JsonToken.String)
            {
                throw JsonSerializationException.Create(reader, "Unexpected token when reading Regex.");
            }
            return this.ReadRegexString(reader);
        }

        private Regex ReadRegexObject(JsonReader reader, JsonSerializer serializer)
        {
            string pattern = null;
            RegexOptions? nullable = null;
            while (reader.Read())
            {
                JsonToken tokenType = reader.TokenType;
                if (tokenType != JsonToken.PropertyName)
                {
                    if ((tokenType != JsonToken.Comment) && (tokenType == JsonToken.EndObject))
                    {
                        goto Label_00A5;
                    }
                }
                else
                {
                    string a = reader.Value.ToString();
                    if (!reader.Read())
                    {
                        throw JsonSerializationException.Create(reader, "Unexpected end when reading Regex.");
                    }
                    if (string.Equals(a, "Pattern", StringComparison.OrdinalIgnoreCase))
                    {
                        pattern = (string) reader.Value;
                    }
                    else if (string.Equals(a, "Options", StringComparison.OrdinalIgnoreCase))
                    {
                        nullable = new RegexOptions?(serializer.Deserialize<RegexOptions>(reader));
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
                continue;
            Label_00A5:
                if (pattern == null)
                {
                    throw JsonSerializationException.Create(reader, "Error deserializing Regex. No pattern found.");
                }
                RegexOptions? nullable2 = nullable;
                return new Regex(pattern, nullable2.HasValue ? nullable2.GetValueOrDefault() : RegexOptions.None);
            }
            throw JsonSerializationException.Create(reader, "Unexpected end when reading Regex.");
        }

        private object ReadRegexString(JsonReader reader)
        {
            string text1 = (string) reader.Value;
            int num = text1.LastIndexOf('/');
            string pattern = text1.Substring(1, num - 1);
            RegexOptions none = RegexOptions.None;
            foreach (char ch in text1.Substring(num + 1))
            {
                switch (ch)
                {
                    case 's':
                        none |= RegexOptions.Singleline;
                        break;

                    case 'x':
                        none |= RegexOptions.ExplicitCapture;
                        break;

                    case 'i':
                        none |= RegexOptions.IgnoreCase;
                        break;

                    case 'm':
                        none |= RegexOptions.Multiline;
                        break;
                }
            }
            return new Regex(pattern, none);
        }

        private void WriteBson(BsonWriter writer, Regex regex)
        {
            string options = null;
            if (this.HasFlag(regex.Options, RegexOptions.IgnoreCase))
            {
                options = options + "i";
            }
            if (this.HasFlag(regex.Options, RegexOptions.Multiline))
            {
                options = options + "m";
            }
            if (this.HasFlag(regex.Options, RegexOptions.Singleline))
            {
                options = options + "s";
            }
            options = options + "u";
            if (this.HasFlag(regex.Options, RegexOptions.ExplicitCapture))
            {
                options = options + "x";
            }
            writer.WriteRegex(regex.ToString(), options);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Regex regex = (Regex) value;
            BsonWriter writer2 = writer as BsonWriter;
            if (writer2 != null)
            {
                this.WriteBson(writer2, regex);
            }
            else
            {
                this.WriteJson(writer, regex, serializer);
            }
        }

        private void WriteJson(JsonWriter writer, Regex regex, JsonSerializer serializer)
        {
            DefaultContractResolver contractResolver = serializer.ContractResolver as DefaultContractResolver;
            writer.WriteStartObject();
            writer.WritePropertyName((contractResolver != null) ? contractResolver.GetResolvedPropertyName("Pattern") : "Pattern");
            writer.WriteValue(regex.ToString());
            writer.WritePropertyName((contractResolver != null) ? contractResolver.GetResolvedPropertyName("Options") : "Options");
            serializer.Serialize(writer, regex.Options);
            writer.WriteEndObject();
        }
    }
}

