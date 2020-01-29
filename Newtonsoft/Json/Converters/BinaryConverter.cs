namespace Newtonsoft.Json.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.Globalization;

    public class BinaryConverter : JsonConverter
    {
        private const string BinaryTypeName = "System.Data.Linq.Binary";
        private const string BinaryToArrayName = "ToArray";
        private ReflectionObject _reflectionObject;

        public override bool CanConvert(Type objectType)
        {
            if (!objectType.AssignableToTypeName("System.Data.Linq.Binary") && (!(objectType == typeof(SqlBinary)) && !(objectType == typeof(SqlBinary?))))
            {
                return false;
            }
            return true;
        }

        private void EnsureReflectionObject(Type t)
        {
            if (this._reflectionObject == null)
            {
                Type[] types = new Type[] { typeof(byte[]) };
                string[] memberNames = new string[] { "ToArray" };
                this._reflectionObject = ReflectionObject.Create(t, t.GetConstructor(types), memberNames);
            }
        }

        private byte[] GetByteArray(object value)
        {
            if (value.GetType().AssignableToTypeName("System.Data.Linq.Binary"))
            {
                this.EnsureReflectionObject(value.GetType());
                return (byte[]) this._reflectionObject.GetValue(value, "ToArray");
            }
            if (!(value is SqlBinary))
            {
                throw new JsonSerializationException("Unexpected value type when writing binary: {0}".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
            }
            SqlBinary binary = (SqlBinary) value;
            return binary.Value;
        }

        private byte[] ReadByteArray(JsonReader reader)
        {
            List<byte> list = new List<byte>();
            while (reader.Read())
            {
                JsonToken tokenType = reader.TokenType;
                if (tokenType != JsonToken.Comment)
                {
                    if (tokenType != JsonToken.Integer)
                    {
                        if (tokenType != JsonToken.EndArray)
                        {
                            throw JsonSerializationException.Create(reader, "Unexpected token when reading bytes: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                        }
                        return list.ToArray();
                    }
                    list.Add(Convert.ToByte(reader.Value, CultureInfo.InvariantCulture));
                }
            }
            throw JsonSerializationException.Create(reader, "Unexpected end when reading bytes.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            byte[] buffer;
            if (reader.TokenType == JsonToken.Null)
            {
                if (!ReflectionUtils.IsNullable(objectType))
                {
                    throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
                }
                return null;
            }
            if (reader.TokenType == JsonToken.StartArray)
            {
                buffer = this.ReadByteArray(reader);
            }
            else
            {
                if (reader.TokenType != JsonToken.String)
                {
                    throw JsonSerializationException.Create(reader, "Unexpected token parsing binary. Expected String or StartArray, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
                buffer = Convert.FromBase64String(reader.Value.ToString());
            }
            Type type = ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType;
            if (type.AssignableToTypeName("System.Data.Linq.Binary"))
            {
                this.EnsureReflectionObject(type);
                object[] args = new object[] { buffer };
                return this._reflectionObject.Creator(args);
            }
            if (type != typeof(SqlBinary))
            {
                throw JsonSerializationException.Create(reader, "Unexpected object type when writing binary: {0}".FormatWith(CultureInfo.InvariantCulture, objectType));
            }
            return new SqlBinary(buffer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                byte[] byteArray = this.GetByteArray(value);
                writer.WriteValue(byteArray);
            }
        }
    }
}

