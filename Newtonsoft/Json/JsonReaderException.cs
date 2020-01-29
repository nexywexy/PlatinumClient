namespace Newtonsoft.Json
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [Serializable]
    public class JsonReaderException : JsonException
    {
        public JsonReaderException()
        {
        }

        public JsonReaderException(string message) : base(message)
        {
        }

        public JsonReaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public JsonReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal JsonReaderException(string message, Exception innerException, string path, int lineNumber, int linePosition) : base(message, innerException)
        {
            this.Path = path;
            this.LineNumber = lineNumber;
            this.LinePosition = linePosition;
        }

        internal static JsonReaderException Create(JsonReader reader, string message) => 
            Create(reader, message, null);

        internal static JsonReaderException Create(JsonReader reader, string message, Exception ex) => 
            Create(reader as IJsonLineInfo, reader.Path, message, ex);

        internal static JsonReaderException Create(IJsonLineInfo lineInfo, string path, string message, Exception ex)
        {
            int lineNumber;
            int linePosition;
            message = JsonPosition.FormatMessage(lineInfo, path, message);
            if ((lineInfo != null) && lineInfo.HasLineInfo())
            {
                lineNumber = lineInfo.LineNumber;
                linePosition = lineInfo.LinePosition;
            }
            else
            {
                lineNumber = 0;
                linePosition = 0;
            }
            return new JsonReaderException(message, ex, path, lineNumber, linePosition);
        }

        public int LineNumber { get; private set; }

        public int LinePosition { get; private set; }

        public string Path { get; private set; }
    }
}

