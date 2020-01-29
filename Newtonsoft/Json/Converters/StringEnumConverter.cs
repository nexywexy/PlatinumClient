namespace Newtonsoft.Json.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;

    public class StringEnumConverter : JsonConverter
    {
        public StringEnumConverter()
        {
            this.AllowIntegerValues = true;
        }

        public StringEnumConverter(bool camelCaseText) : this()
        {
            this.CamelCaseText = camelCaseText;
        }

        public override bool CanConvert(Type objectType) => 
            (ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType).IsEnum();

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (!ReflectionUtils.IsNullableType(objectType))
                {
                    throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
                }
                return null;
            }
            bool isNullable = ReflectionUtils.IsNullableType(objectType);
            Type t = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;
            try
            {
                if (reader.TokenType == JsonToken.String)
                {
                    return EnumUtils.ParseEnumName(reader.Value.ToString(), isNullable, t);
                }
                if (reader.TokenType == JsonToken.Integer)
                {
                    if (!this.AllowIntegerValues)
                    {
                        throw JsonSerializationException.Create(reader, "Integer value {0} is not allowed.".FormatWith(CultureInfo.InvariantCulture, reader.Value));
                    }
                    return ConvertUtils.ConvertOrCast(reader.Value, CultureInfo.InvariantCulture, t);
                }
            }
            catch (Exception exception)
            {
                throw JsonSerializationException.Create(reader, "Error converting value {0} to type '{1}'.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.FormatValueForPrint(reader.Value), objectType), exception);
            }
            throw JsonSerializationException.Create(reader, "Unexpected token {0} when parsing enum.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                Enum enum2 = (Enum) value;
                string enumText = enum2.ToString("G");
                if (char.IsNumber(enumText[0]) || (enumText[0] == '-'))
                {
                    writer.WriteValue(value);
                }
                else
                {
                    string str2 = EnumUtils.ToEnumName(enum2.GetType(), enumText, this.CamelCaseText);
                    writer.WriteValue(str2);
                }
            }
        }

        public bool CamelCaseText { get; set; }

        public bool AllowIntegerValues { get; set; }
    }
}

