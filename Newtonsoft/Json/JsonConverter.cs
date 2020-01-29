namespace Newtonsoft.Json
{
    using Newtonsoft.Json.Schema;
    using System;

    public abstract class JsonConverter
    {
        protected JsonConverter()
        {
        }

        public abstract bool CanConvert(Type objectType);
        [Obsolete("JSON Schema validation has been moved to its own package. It is strongly recommended that you do not override GetSchema() in your own converter. It is not used by Json.NET and will be removed at some point in the future. Converter's that override GetSchema() will stop working when it is removed.")]
        public virtual JsonSchema GetSchema() => 
            null;

        public abstract object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer);
        public abstract void WriteJson(JsonWriter writer, object value, JsonSerializer serializer);

        public virtual bool CanRead =>
            true;

        public virtual bool CanWrite =>
            true;
    }
}

