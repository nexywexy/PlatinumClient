namespace Newtonsoft.Json
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class JsonSerializationException : JsonException
    {
        public JsonSerializationException()
        {
        }

        public JsonSerializationException(string message) : base(message)
        {
        }

        public JsonSerializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public JsonSerializationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal static JsonSerializationException Create(JsonReader reader, string message) => 
            Create(reader, message, null);

        internal static JsonSerializationException Create(JsonReader reader, string message, Exception ex) => 
            Create(reader as IJsonLineInfo, reader.Path, message, ex);

        internal static JsonSerializationException Create(IJsonLineInfo lineInfo, string path, string message, Exception ex)
        {
            message = JsonPosition.FormatMessage(lineInfo, path, message);
            return new JsonSerializationException(message, ex);
        }
    }
}

