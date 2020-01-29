namespace Newtonsoft.Json.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;

    public class KeyValuePairConverter : JsonConverter
    {
        private const string KeyName = "Key";
        private const string ValueName = "Value";
        private static readonly ThreadSafeStore<Type, ReflectionObject> ReflectionObjectPerType = new ThreadSafeStore<Type, ReflectionObject>(new Func<Type, ReflectionObject>(KeyValuePairConverter.InitializeReflectionObject));

        public override bool CanConvert(Type objectType)
        {
            Type type = ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType;
            return ((type.IsValueType() && type.IsGenericType()) && (type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)));
        }

        private static ReflectionObject InitializeReflectionObject(Type t)
        {
            Type[] genericArguments = t.GetGenericArguments();
            Type type = genericArguments[0];
            Type type2 = genericArguments[1];
            Type[] types = new Type[] { type, type2 };
            string[] memberNames = new string[] { "Key", "Value" };
            return ReflectionObject.Create(t, t.GetConstructor(types), memberNames);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (!ReflectionUtils.IsNullableType(objectType))
                {
                    throw JsonSerializationException.Create(reader, "Cannot convert null value to KeyValuePair.");
                }
                return null;
            }
            object obj2 = null;
            object obj3 = null;
            reader.ReadAndAssert();
            Type key = ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType;
            ReflectionObject obj4 = ReflectionObjectPerType.Get(key);
            while (reader.TokenType == JsonToken.PropertyName)
            {
                string a = reader.Value.ToString();
                if (string.Equals(a, "Key", StringComparison.OrdinalIgnoreCase))
                {
                    reader.ReadAndAssert();
                    obj2 = serializer.Deserialize(reader, obj4.GetType("Key"));
                }
                else if (string.Equals(a, "Value", StringComparison.OrdinalIgnoreCase))
                {
                    reader.ReadAndAssert();
                    obj3 = serializer.Deserialize(reader, obj4.GetType("Value"));
                }
                else
                {
                    reader.Skip();
                }
                reader.ReadAndAssert();
            }
            object[] args = new object[] { obj2, obj3 };
            return obj4.Creator(args);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            ReflectionObject obj2 = ReflectionObjectPerType.Get(value.GetType());
            DefaultContractResolver contractResolver = serializer.ContractResolver as DefaultContractResolver;
            writer.WriteStartObject();
            writer.WritePropertyName((contractResolver != null) ? contractResolver.GetResolvedPropertyName("Key") : "Key");
            serializer.Serialize(writer, obj2.GetValue(value, "Key"), obj2.GetType("Key"));
            writer.WritePropertyName((contractResolver != null) ? contractResolver.GetResolvedPropertyName("Value") : "Value");
            serializer.Serialize(writer, obj2.GetValue(value, "Value"), obj2.GetType("Value"));
            writer.WriteEndObject();
        }
    }
}

