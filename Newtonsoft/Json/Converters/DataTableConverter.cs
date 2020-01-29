namespace Newtonsoft.Json.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;

    public class DataTableConverter : JsonConverter
    {
        public override bool CanConvert(Type valueType) => 
            typeof(DataTable).IsAssignableFrom(valueType);

        private static void CreateRow(JsonReader reader, DataTable dt, JsonSerializer serializer)
        {
            DataRow row = dt.NewRow();
            reader.ReadAndAssert();
            while (reader.TokenType == JsonToken.PropertyName)
            {
                string columnName = (string) reader.Value;
                reader.ReadAndAssert();
                DataColumn column = dt.Columns[columnName];
                if (column == null)
                {
                    Type columnDataType = GetColumnDataType(reader);
                    column = new DataColumn(columnName, columnDataType);
                    dt.Columns.Add(column);
                }
                if (column.DataType == typeof(DataTable))
                {
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        reader.ReadAndAssert();
                    }
                    DataTable table = new DataTable();
                    while (reader.TokenType != JsonToken.EndArray)
                    {
                        CreateRow(reader, table, serializer);
                        reader.ReadAndAssert();
                    }
                    row[columnName] = table;
                }
                else if (column.DataType.IsArray && (column.DataType != typeof(byte[])))
                {
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        reader.ReadAndAssert();
                    }
                    List<object> list = new List<object>();
                    while (reader.TokenType != JsonToken.EndArray)
                    {
                        list.Add(reader.Value);
                        reader.ReadAndAssert();
                    }
                    Array destinationArray = Array.CreateInstance(column.DataType.GetElementType(), list.Count);
                    Array.Copy(list.ToArray(), destinationArray, list.Count);
                    row[columnName] = destinationArray;
                }
                else
                {
                    row[columnName] = (reader.Value != null) ? serializer.Deserialize(reader, column.DataType) : DBNull.Value;
                }
                reader.ReadAndAssert();
            }
            row.EndEdit();
            dt.Rows.Add(row);
        }

        private static Type GetColumnDataType(JsonReader reader)
        {
            JsonToken tokenType = reader.TokenType;
            switch (tokenType)
            {
                case JsonToken.StartArray:
                    reader.ReadAndAssert();
                    if (reader.TokenType != JsonToken.StartObject)
                    {
                        return GetColumnDataType(reader).MakeArrayType();
                    }
                    return typeof(DataTable);

                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    return reader.ValueType;

                case JsonToken.Null:
                case JsonToken.Undefined:
                    return typeof(string);
            }
            throw JsonSerializationException.Create(reader, "Unexpected JSON token when reading DataTable: {0}".FormatWith(CultureInfo.InvariantCulture, tokenType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            DataTable dt = existingValue as DataTable;
            if (dt == null)
            {
                dt = (objectType == typeof(DataTable)) ? new DataTable() : ((DataTable) Activator.CreateInstance(objectType));
            }
            if (reader.TokenType == JsonToken.PropertyName)
            {
                dt.TableName = (string) reader.Value;
                reader.ReadAndAssert();
                if (reader.TokenType == JsonToken.Null)
                {
                    return dt;
                }
            }
            if (reader.TokenType != JsonToken.StartArray)
            {
                throw JsonSerializationException.Create(reader, "Unexpected JSON token when reading DataTable. Expected StartArray, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
            reader.ReadAndAssert();
            while (reader.TokenType != JsonToken.EndArray)
            {
                CreateRow(reader, dt, serializer);
                reader.ReadAndAssert();
            }
            return dt;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DefaultContractResolver contractResolver = serializer.ContractResolver as DefaultContractResolver;
            writer.WriteStartArray();
            foreach (DataRow row in ((DataTable) value).Rows)
            {
                writer.WriteStartObject();
                foreach (DataColumn column in row.Table.Columns)
                {
                    object obj2 = row[column];
                    if ((serializer.NullValueHandling != NullValueHandling.Ignore) || ((obj2 != null) && (obj2 != DBNull.Value)))
                    {
                        writer.WritePropertyName((contractResolver != null) ? contractResolver.GetResolvedPropertyName(column.ColumnName) : column.ColumnName);
                        serializer.Serialize(writer, obj2);
                    }
                }
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
    }
}

