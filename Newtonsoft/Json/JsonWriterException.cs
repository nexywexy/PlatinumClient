namespace Newtonsoft.Json
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [Serializable]
    public class JsonWriterException : JsonException
    {
        public JsonWriterException()
        {
        }

        public JsonWriterException(string message) : base(message)
        {
        }

        public JsonWriterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public JsonWriterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal JsonWriterException(string message, Exception innerException, string path) : base(message, innerException)
        {
            this.Path = path;
        }

        internal static JsonWriterException Create(JsonWriter writer, string message, Exception ex) => 
            Create(writer.ContainerPath, message, ex);

        internal static JsonWriterException Create(string path, string message, Exception ex)
        {
            message = JsonPosition.FormatMessage(null, path, message);
            return new JsonWriterException(message, ex, path);
        }

        public string Path { get; private set; }
    }
}

