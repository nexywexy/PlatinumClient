namespace Newtonsoft.Json
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Numerics;

    public class JsonTextWriter : JsonWriter
    {
        private readonly TextWriter _writer;
        private Newtonsoft.Json.Utilities.Base64Encoder _base64Encoder;
        private char _indentChar;
        private int _indentation;
        private char _quoteChar;
        private bool _quoteName;
        private bool[] _charEscapeFlags;
        private char[] _writeBuffer;
        private IArrayPool<char> _arrayPool;
        private char[] _indentChars;

        public JsonTextWriter(TextWriter textWriter)
        {
            if (textWriter == null)
            {
                throw new ArgumentNullException("textWriter");
            }
            this._writer = textWriter;
            this._quoteChar = '"';
            this._quoteName = true;
            this._indentChar = ' ';
            this._indentation = 2;
            this.UpdateCharEscapeFlags();
        }

        public override void Close()
        {
            base.Close();
            if (this._writeBuffer != null)
            {
                BufferUtils.ReturnBuffer(this._arrayPool, this._writeBuffer);
                this._writeBuffer = null;
            }
            if (base.CloseOutput && (this._writer != null))
            {
                this._writer.Close();
            }
        }

        private void EnsureWriteBuffer()
        {
            if (this._writeBuffer == null)
            {
                this._writeBuffer = BufferUtils.RentBuffer(this._arrayPool, 0x23);
            }
        }

        public override void Flush()
        {
            this._writer.Flush();
        }

        internal override void OnStringEscapeHandlingChanged()
        {
            this.UpdateCharEscapeFlags();
        }

        private void UpdateCharEscapeFlags()
        {
            this._charEscapeFlags = JavaScriptUtils.GetCharEscapeFlags(base.StringEscapeHandling, this._quoteChar);
        }

        public override void WriteComment(string text)
        {
            base.InternalWriteComment();
            this._writer.Write("/*");
            this._writer.Write(text);
            this._writer.Write("*/");
        }

        protected override void WriteEnd(JsonToken token)
        {
            switch (token)
            {
                case JsonToken.EndObject:
                    this._writer.Write('}');
                    return;

                case JsonToken.EndArray:
                    this._writer.Write(']');
                    return;

                case JsonToken.EndConstructor:
                    this._writer.Write(')');
                    return;
            }
            throw JsonWriterException.Create(this, "Invalid JsonToken: " + token, null);
        }

        private void WriteEscapedString(string value, bool quote)
        {
            this.EnsureWriteBuffer();
            JavaScriptUtils.WriteEscapedJavaScriptString(this._writer, value, this._quoteChar, quote, this._charEscapeFlags, base.StringEscapeHandling, this._arrayPool, ref this._writeBuffer);
        }

        protected override void WriteIndent()
        {
            this._writer.WriteLine();
            int num = base.Top * this._indentation;
            if (num > 0)
            {
                if (this._indentChars == null)
                {
                    this._indentChars = new string(this._indentChar, 10).ToCharArray();
                }
                while (num > 0)
                {
                    int count = Math.Min(num, 10);
                    this._writer.Write(this._indentChars, 0, count);
                    num -= count;
                }
            }
        }

        protected override void WriteIndentSpace()
        {
            this._writer.Write(' ');
        }

        private void WriteIntegerValue(long value)
        {
            if ((value >= 0L) && (value <= 9L))
            {
                this._writer.Write((char) ((ushort) (0x30L + value)));
            }
            else
            {
                ulong uvalue = (value < 0L) ? ((ulong) -value) : ((ulong) value);
                if (value < 0L)
                {
                    this._writer.Write('-');
                }
                this.WriteIntegerValue(uvalue);
            }
        }

        private void WriteIntegerValue(ulong uvalue)
        {
            if (uvalue <= 9L)
            {
                this._writer.Write((char) (((ulong) 0x30L) + uvalue));
            }
            else
            {
                this.EnsureWriteBuffer();
                int num = MathUtils.IntLength(uvalue);
                int count = 0;
                do
                {
                    this._writeBuffer[num - ++count] = (char) (((ulong) 0x30L) + (uvalue % ((ulong) 10L)));
                    uvalue /= (ulong) 10L;
                }
                while (uvalue != 0);
                this._writer.Write(this._writeBuffer, 0, count);
            }
        }

        public override void WriteNull()
        {
            base.InternalWriteValue(JsonToken.Null);
            this.WriteValueInternal(JsonConvert.Null, JsonToken.Null);
        }

        public override void WritePropertyName(string name)
        {
            base.InternalWritePropertyName(name);
            this.WriteEscapedString(name, this._quoteName);
            this._writer.Write(':');
        }

        public override void WritePropertyName(string name, bool escape)
        {
            base.InternalWritePropertyName(name);
            if (escape)
            {
                this.WriteEscapedString(name, this._quoteName);
            }
            else
            {
                if (this._quoteName)
                {
                    this._writer.Write(this._quoteChar);
                }
                this._writer.Write(name);
                if (this._quoteName)
                {
                    this._writer.Write(this._quoteChar);
                }
            }
            this._writer.Write(':');
        }

        public override void WriteRaw(string json)
        {
            base.InternalWriteRaw();
            this._writer.Write(json);
        }

        public override void WriteStartArray()
        {
            base.InternalWriteStart(JsonToken.StartArray, JsonContainerType.Array);
            this._writer.Write('[');
        }

        public override void WriteStartConstructor(string name)
        {
            base.InternalWriteStart(JsonToken.StartConstructor, JsonContainerType.Constructor);
            this._writer.Write("new ");
            this._writer.Write(name);
            this._writer.Write('(');
        }

        public override void WriteStartObject()
        {
            base.InternalWriteStart(JsonToken.StartObject, JsonContainerType.Object);
            this._writer.Write('{');
        }

        public override void WriteUndefined()
        {
            base.InternalWriteValue(JsonToken.Undefined);
            this.WriteValueInternal(JsonConvert.Undefined, JsonToken.Undefined);
        }

        public override void WriteValue(bool value)
        {
            base.InternalWriteValue(JsonToken.Boolean);
            this.WriteValueInternal(JsonConvert.ToString(value), JsonToken.Boolean);
        }

        public override void WriteValue(byte value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            this.WriteIntegerValue((long) value);
        }

        public override void WriteValue(char value)
        {
            base.InternalWriteValue(JsonToken.String);
            this.WriteValueInternal(JsonConvert.ToString(value), JsonToken.String);
        }

        public override void WriteValue(DateTime value)
        {
            base.InternalWriteValue(JsonToken.Date);
            value = DateTimeUtils.EnsureDateTime(value, base.DateTimeZoneHandling);
            if (string.IsNullOrEmpty(base.DateFormatString))
            {
                this.EnsureWriteBuffer();
                int start = 0;
                this._writeBuffer[start++] = this._quoteChar;
                start = DateTimeUtils.WriteDateTimeString(this._writeBuffer, start, value, null, value.Kind, base.DateFormatHandling);
                this._writeBuffer[start++] = this._quoteChar;
                this._writer.Write(this._writeBuffer, 0, start);
            }
            else
            {
                this._writer.Write(this._quoteChar);
                this._writer.Write(value.ToString(base.DateFormatString, base.Culture));
                this._writer.Write(this._quoteChar);
            }
        }

        public override void WriteValue(DateTimeOffset value)
        {
            base.InternalWriteValue(JsonToken.Date);
            if (string.IsNullOrEmpty(base.DateFormatString))
            {
                this.EnsureWriteBuffer();
                int start = 0;
                this._writeBuffer[start++] = this._quoteChar;
                start = DateTimeUtils.WriteDateTimeString(this._writeBuffer, start, (base.DateFormatHandling == DateFormatHandling.IsoDateFormat) ? value.DateTime : value.UtcDateTime, new TimeSpan?(value.Offset), DateTimeKind.Local, base.DateFormatHandling);
                this._writeBuffer[start++] = this._quoteChar;
                this._writer.Write(this._writeBuffer, 0, start);
            }
            else
            {
                this._writer.Write(this._quoteChar);
                this._writer.Write(value.ToString(base.DateFormatString, base.Culture));
                this._writer.Write(this._quoteChar);
            }
        }

        public override void WriteValue(decimal value)
        {
            base.InternalWriteValue(JsonToken.Float);
            this.WriteValueInternal(JsonConvert.ToString(value), JsonToken.Float);
        }

        public override void WriteValue(double value)
        {
            base.InternalWriteValue(JsonToken.Float);
            this.WriteValueInternal(JsonConvert.ToString(value, base.FloatFormatHandling, this.QuoteChar, false), JsonToken.Float);
        }

        public override void WriteValue(Guid value)
        {
            base.InternalWriteValue(JsonToken.String);
            string str = null;
            str = value.ToString("D", CultureInfo.InvariantCulture);
            this._writer.Write(this._quoteChar);
            this._writer.Write(str);
            this._writer.Write(this._quoteChar);
        }

        public override void WriteValue(short value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            this.WriteIntegerValue((long) value);
        }

        public override void WriteValue(int value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            this.WriteIntegerValue((long) value);
        }

        public override void WriteValue(long value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            this.WriteIntegerValue(value);
        }

        public override void WriteValue(double? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                base.InternalWriteValue(JsonToken.Float);
                this.WriteValueInternal(JsonConvert.ToString(value.GetValueOrDefault(), base.FloatFormatHandling, this.QuoteChar, true), JsonToken.Float);
            }
        }

        public override void WriteValue(float? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                base.InternalWriteValue(JsonToken.Float);
                this.WriteValueInternal(JsonConvert.ToString(value.GetValueOrDefault(), base.FloatFormatHandling, this.QuoteChar, true), JsonToken.Float);
            }
        }

        public override void WriteValue(object value)
        {
            if (value is BigInteger)
            {
                base.InternalWriteValue(JsonToken.Integer);
                this.WriteValueInternal(((BigInteger) value).ToString(CultureInfo.InvariantCulture), JsonToken.String);
            }
            else
            {
                base.WriteValue(value);
            }
        }

        [CLSCompliant(false)]
        public override void WriteValue(sbyte value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            this.WriteIntegerValue((long) value);
        }

        public override void WriteValue(float value)
        {
            base.InternalWriteValue(JsonToken.Float);
            this.WriteValueInternal(JsonConvert.ToString(value, base.FloatFormatHandling, this.QuoteChar, false), JsonToken.Float);
        }

        public override void WriteValue(string value)
        {
            base.InternalWriteValue(JsonToken.String);
            if (value == null)
            {
                this.WriteValueInternal(JsonConvert.Null, JsonToken.Null);
            }
            else
            {
                this.WriteEscapedString(value, true);
            }
        }

        public override void WriteValue(TimeSpan value)
        {
            base.InternalWriteValue(JsonToken.String);
            string str = value.ToString(null, CultureInfo.InvariantCulture);
            this._writer.Write(this._quoteChar);
            this._writer.Write(str);
            this._writer.Write(this._quoteChar);
        }

        [CLSCompliant(false)]
        public override void WriteValue(ushort value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            this.WriteIntegerValue((long) value);
        }

        [CLSCompliant(false)]
        public override void WriteValue(uint value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            this.WriteIntegerValue((long) value);
        }

        [CLSCompliant(false)]
        public override void WriteValue(ulong value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            this.WriteIntegerValue(value);
        }

        public override void WriteValue(byte[] value)
        {
            if (value == null)
            {
                this.WriteNull();
            }
            else
            {
                base.InternalWriteValue(JsonToken.Bytes);
                this._writer.Write(this._quoteChar);
                this.Base64Encoder.Encode(value, 0, value.Length);
                this.Base64Encoder.Flush();
                this._writer.Write(this._quoteChar);
            }
        }

        public override void WriteValue(Uri value)
        {
            if (value == null)
            {
                this.WriteNull();
            }
            else
            {
                base.InternalWriteValue(JsonToken.String);
                this.WriteEscapedString(value.OriginalString, true);
            }
        }

        protected override void WriteValueDelimiter()
        {
            this._writer.Write(',');
        }

        private void WriteValueInternal(string value, JsonToken token)
        {
            this._writer.Write(value);
        }

        public override void WriteWhitespace(string ws)
        {
            base.InternalWriteWhitespace(ws);
            this._writer.Write(ws);
        }

        private Newtonsoft.Json.Utilities.Base64Encoder Base64Encoder
        {
            get
            {
                if (this._base64Encoder == null)
                {
                    this._base64Encoder = new Newtonsoft.Json.Utilities.Base64Encoder(this._writer);
                }
                return this._base64Encoder;
            }
        }

        public IArrayPool<char> ArrayPool
        {
            get => 
                this._arrayPool;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this._arrayPool = value;
            }
        }

        public int Indentation
        {
            get => 
                this._indentation;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Indentation value must be greater than 0.");
                }
                this._indentation = value;
            }
        }

        public char QuoteChar
        {
            get => 
                this._quoteChar;
            set
            {
                if ((value != '"') && (value != '\''))
                {
                    throw new ArgumentException("Invalid JavaScript string quote character. Valid quote characters are ' and \".");
                }
                this._quoteChar = value;
                this.UpdateCharEscapeFlags();
            }
        }

        public char IndentChar
        {
            get => 
                this._indentChar;
            set
            {
                if (value != this._indentChar)
                {
                    this._indentChar = value;
                    this._indentChars = null;
                }
            }
        }

        public bool QuoteName
        {
            get => 
                this._quoteName;
            set => 
                (this._quoteName = value);
        }
    }
}

