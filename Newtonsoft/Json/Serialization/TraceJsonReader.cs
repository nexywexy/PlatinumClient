namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.IO;

    internal class TraceJsonReader : JsonReader, IJsonLineInfo
    {
        private readonly JsonReader _innerReader;
        private readonly JsonTextWriter _textWriter;
        private readonly StringWriter _sw;

        public TraceJsonReader(JsonReader innerReader)
        {
            this._innerReader = innerReader;
            this._sw = new StringWriter(CultureInfo.InvariantCulture);
            this._sw.Write("Deserialized JSON: " + Environment.NewLine);
            this._textWriter = new JsonTextWriter(this._sw);
            this._textWriter.Formatting = Formatting.Indented;
        }

        public override void Close()
        {
            this._innerReader.Close();
        }

        public string GetDeserializedJsonMessage() => 
            this._sw.ToString();

        bool IJsonLineInfo.HasLineInfo()
        {
            IJsonLineInfo info = this._innerReader as IJsonLineInfo;
            return ((info != null) && info.HasLineInfo());
        }

        public override bool Read()
        {
            this._textWriter.WriteToken(this._innerReader, false, false, true);
            return this._innerReader.Read();
        }

        public override bool? ReadAsBoolean()
        {
            this._textWriter.WriteToken(this._innerReader, false, false, true);
            return this._innerReader.ReadAsBoolean();
        }

        public override byte[] ReadAsBytes()
        {
            this._textWriter.WriteToken(this._innerReader, false, false, true);
            return this._innerReader.ReadAsBytes();
        }

        public override DateTime? ReadAsDateTime()
        {
            this._textWriter.WriteToken(this._innerReader, false, false, true);
            return this._innerReader.ReadAsDateTime();
        }

        public override DateTimeOffset? ReadAsDateTimeOffset()
        {
            this._textWriter.WriteToken(this._innerReader, false, false, true);
            return this._innerReader.ReadAsDateTimeOffset();
        }

        public override decimal? ReadAsDecimal()
        {
            this._textWriter.WriteToken(this._innerReader, false, false, true);
            return this._innerReader.ReadAsDecimal();
        }

        public override double? ReadAsDouble()
        {
            this._textWriter.WriteToken(this._innerReader, false, false, true);
            return this._innerReader.ReadAsDouble();
        }

        public override int? ReadAsInt32()
        {
            this._textWriter.WriteToken(this._innerReader, false, false, true);
            return this._innerReader.ReadAsInt32();
        }

        public override string ReadAsString()
        {
            this._textWriter.WriteToken(this._innerReader, false, false, true);
            return this._innerReader.ReadAsString();
        }

        public override int Depth =>
            this._innerReader.Depth;

        public override string Path =>
            this._innerReader.Path;

        public override char QuoteChar
        {
            get => 
                this._innerReader.QuoteChar;
            protected internal set => 
                (this._innerReader.QuoteChar = value);
        }

        public override JsonToken TokenType =>
            this._innerReader.TokenType;

        public override object Value =>
            this._innerReader.Value;

        public override Type ValueType =>
            this._innerReader.ValueType;

        int IJsonLineInfo.LineNumber
        {
            get
            {
                IJsonLineInfo info = this._innerReader as IJsonLineInfo;
                return info?.LineNumber;
            }
        }

        int IJsonLineInfo.LinePosition
        {
            get
            {
                IJsonLineInfo info = this._innerReader as IJsonLineInfo;
                return info?.LinePosition;
            }
        }
    }
}

