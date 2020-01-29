namespace Newtonsoft.Json.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Globalization;

    public class EntityKeyMemberConverter : JsonConverter
    {
        private const string EntityKeyMemberFullTypeName = "System.Data.EntityKeyMember";
        private const string KeyPropertyName = "Key";
        private const string TypePropertyName = "Type";
        private const string ValuePropertyName = "Value";
        private static ReflectionObject _reflectionObject;

        public override bool CanConvert(Type objectType) => 
            objectType.AssignableToTypeName("System.Data.EntityKeyMember");

        private static void EnsureReflectionObject(Type objectType)
        {
            if (_reflectionObject == null)
            {
                string[] memberNames = new string[] { "Key", "Value" };
                _reflectionObject = ReflectionObject.Create(objectType, memberNames);
            }
        }

        private static void ReadAndAssertProperty(JsonReader reader, string propertyName)
        {
            reader.ReadAndAssert();
            if ((reader.TokenType != JsonToken.PropertyName) || !string.Equals(reader.Value.ToString(), propertyName, StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonSerializationException("Expected JSON property '{0}'.".FormatWith(CultureInfo.InvariantCulture, propertyName));
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            EnsureReflectionObject(objectType);
            object target = _reflectionObject.Creator(new object[0]);
            ReadAndAssertProperty(reader, "Key");
            reader.ReadAndAssert();
            _reflectionObject.SetValue(target, "Key", reader.Value.ToString());
            ReadAndAssertProperty(reader, "Type");
            reader.ReadAndAssert();
            Type type = Type.GetType(reader.Value.ToString());
            ReadAndAssertProperty(reader, "Value");
            reader.ReadAndAssert();
            _reflectionObject.SetValue(target, "Value", serializer.Deserialize(reader, type));
            reader.ReadAndAssert();
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            EnsureReflectionObject(value.GetType());
            DefaultContractResolver contractResolver = serializer.ContractResolver as DefaultContractResolver;
            string str = (string) _reflectionObject.GetValue(value, "Key");
            object obj2 = _reflectionObject.GetValue(value, "Value");
            Type type = obj2?.GetType();
            writer.WriteStartObject();
            writer.WritePropertyName((contractResolver != null) ? contractResolver.GetResolvedPropertyName("Key") : "Key");
            writer.WriteValue(str);
            writer.WritePropertyName((contractResolver != null) ? contractResolver.GetResolvedPropertyName("Type") : "Type");
            writer.WriteValue(type?.FullName);
            writer.WritePropertyName((contractResolver != null) ? contractResolver.GetResolvedPropertyName("Value") : "Value");
            if (type != null)
            {
                if (JsonSerializerInternalWriter.TryConvertToString(obj2, type, out string str2))
                {
                    writer.WriteValue(str2);
                }
                else
                {
                    writer.WriteValue(obj2);
                }
            }
            else
            {
                writer.WriteNull();
            }
            writer.WriteEndObject();
        }
    }
}

