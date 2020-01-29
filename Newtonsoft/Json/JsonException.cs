namespace Newtonsoft.Json
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class JsonException : Exception
    {
        public JsonException()
        {
        }

        public JsonException(string message) : base(message)
        {
        }

        public JsonException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public JsonException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal static JsonException Create(IJsonLineInfo lineInfo, string path, string message)
        {
            message = JsonPosition.FormatMessage(lineInfo, path, message);
            return new JsonException(message);
        }
    }
}

