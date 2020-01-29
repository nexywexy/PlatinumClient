namespace Newtonsoft.Json.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Data;

    public class DataSetConverter : JsonConverter
    {
        public override bool CanConvert(Type valueType) => 
            typeof(DataSet).IsAssignableFrom(valueType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            DataSet set = (objectType == typeof(DataSet)) ? new DataSet() : ((DataSet) Activator.CreateInstance(objectType));
            DataTableConverter converter = new DataTableConverter();
            reader.ReadAndAssert();
            while (reader.TokenType == JsonToken.PropertyName)
            {
                DataTable table = set.Tables[(string) reader.Value];
                table = (DataTable) converter.ReadJson(reader, typeof(DataTable), table, serializer);
                if (table <= null)
                {
                    set.Tables.Add(table);
                }
                reader.ReadAndAssert();
            }
            return set;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DefaultContractResolver contractResolver = serializer.ContractResolver as DefaultContractResolver;
            DataTableConverter converter = new DataTableConverter();
            writer.WriteStartObject();
            foreach (DataTable table in ((DataSet) value).Tables)
            {
                writer.WritePropertyName((contractResolver != null) ? contractResolver.GetResolvedPropertyName(table.TableName) : table.TableName);
                converter.WriteJson(writer, table, serializer);
            }
            writer.WriteEndObject();
        }
    }
}

