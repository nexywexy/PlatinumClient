namespace Newtonsoft.Json
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.CompilerServices;

    public abstract class JsonWriter : IDisposable
    {
        private static readonly State[][] StateArray = BuildStateArray();
        internal static readonly State[][] StateArrayTempate = new State[][] { new State[] { State.Error }, new State[] { State.ObjectStart }, new State[] { State.ArrayStart }, new State[] { State.ConstructorStart }, new State[] { State.Property }, new State[] { State.Start }, new State[] { State.Start }, new State[] { State.Start } };
        private List<JsonPosition> _stack;
        private JsonPosition _currentPosition;
        private State _currentState = State.Start;
        private Newtonsoft.Json.Formatting _formatting = Newtonsoft.Json.Formatting.None;
        private Newtonsoft.Json.DateFormatHandling _dateFormatHandling;
        private Newtonsoft.Json.DateTimeZoneHandling _dateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
        private Newtonsoft.Json.StringEscapeHandling _stringEscapeHandling;
        private Newtonsoft.Json.FloatFormatHandling _floatFormatHandling;
        private string _dateFormatString;
        private CultureInfo _culture;

        protected JsonWriter()
        {
            this.CloseOutput = true;
        }

        internal void AutoComplete(JsonToken tokenBeingWritten)
        {
            State state = StateArray[(int) tokenBeingWritten][(int) this._currentState];
            if (state == State.Error)
            {
                throw JsonWriterException.Create(this, "Token {0} in state {1} would result in an invalid JSON object.".FormatWith(CultureInfo.InvariantCulture, tokenBeingWritten.ToString(), this._currentState.ToString()), null);
            }
            if ((((this._currentState == State.Object) || (this._currentState == State.Array)) || (this._currentState == State.Constructor)) && (tokenBeingWritten != JsonToken.Comment))
            {
                this.WriteValueDelimiter();
            }
            if (this._formatting == Newtonsoft.Json.Formatting.Indented)
            {
                if (this._currentState == State.Property)
                {
                    this.WriteIndentSpace();
                }
                if (((this._currentState == State.Array) || (this._currentState == State.ArrayStart)) || (((this._currentState == State.Constructor) || (this._currentState == State.ConstructorStart)) || ((tokenBeingWritten == JsonToken.PropertyName) && (this._currentState != State.Start))))
                {
                    this.WriteIndent();
                }
            }
            this._currentState = state;
        }

        private void AutoCompleteAll()
        {
            while (this.Top > 0)
            {
                this.WriteEnd();
            }
        }

        private void AutoCompleteClose(JsonContainerType type)
        {
            int num = 0;
            if (this._currentPosition.Type == type)
            {
                num = 1;
            }
            else
            {
                int num2 = this.Top - 2;
                for (int j = num2; j >= 0; j--)
                {
                    int num4 = num2 - j;
                    if (this._stack[num4].Type == type)
                    {
                        num = j + 2;
                        break;
                    }
                }
            }
            if (num == 0)
            {
                throw JsonWriterException.Create(this, "No token to close.", null);
            }
            for (int i = 0; i < num; i++)
            {
                JsonToken closeTokenForType = this.GetCloseTokenForType(this.Pop());
                if (this._currentState == State.Property)
                {
                    this.WriteNull();
                }
                if (((this._formatting == Newtonsoft.Json.Formatting.Indented) && (this._currentState != State.ObjectStart)) && (this._currentState != State.ArrayStart))
                {
                    this.WriteIndent();
                }
                this.WriteEnd(closeTokenForType);
                JsonContainerType type2 = this.Peek();
                switch (type2)
                {
                    case JsonContainerType.None:
                        this._currentState = State.Start;
                        break;

                    case JsonContainerType.Object:
                        this._currentState = State.Object;
                        break;

                    case JsonContainerType.Array:
                        this._currentState = State.Array;
                        break;

                    case JsonContainerType.Constructor:
                        this._currentState = State.Array;
                        break;

                    default:
                        throw JsonWriterException.Create(this, "Unknown JsonType: " + type2, null);
                }
            }
        }

        internal static State[][] BuildStateArray()
        {
            List<State[]> list = StateArrayTempate.ToList<State[]>();
            State[] item = StateArrayTempate[0];
            State[] stateArray2 = StateArrayTempate[7];
            foreach (JsonToken token in EnumUtils.GetValues(typeof(JsonToken)))
            {
                if (list.Count > token)
                {
                    continue;
                }
                switch (token)
                {
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.String:
                    case JsonToken.Boolean:
                    case JsonToken.Null:
                    case JsonToken.Undefined:
                    case JsonToken.Date:
                    case JsonToken.Bytes:
                    {
                        list.Add(stateArray2);
                        continue;
                    }
                }
                list.Add(item);
            }
            return list.ToArray();
        }

        public virtual void Close()
        {
            this.AutoCompleteAll();
        }

        private static JsonWriterException CreateUnsupportedTypeException(JsonWriter writer, object value) => 
            JsonWriterException.Create(writer, "Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()), null);

        protected virtual void Dispose(bool disposing)
        {
            if ((this._currentState != State.Closed) & disposing)
            {
                this.Close();
            }
        }

        public abstract void Flush();
        private JsonToken GetCloseTokenForType(JsonContainerType type)
        {
            switch (type)
            {
                case JsonContainerType.Object:
                    return JsonToken.EndObject;

                case JsonContainerType.Array:
                    return JsonToken.EndArray;

                case JsonContainerType.Constructor:
                    return JsonToken.EndConstructor;
            }
            throw JsonWriterException.Create(this, "No close token for type: " + type, null);
        }

        internal void InternalWriteComment()
        {
            this.AutoComplete(JsonToken.Comment);
        }

        internal void InternalWriteEnd(JsonContainerType container)
        {
            this.AutoCompleteClose(container);
        }

        internal void InternalWritePropertyName(string name)
        {
            this._currentPosition.PropertyName = name;
            this.AutoComplete(JsonToken.PropertyName);
        }

        internal void InternalWriteRaw()
        {
        }

        internal void InternalWriteStart(JsonToken token, JsonContainerType container)
        {
            this.UpdateScopeWithFinishedValue();
            this.AutoComplete(token);
            this.Push(container);
        }

        internal void InternalWriteValue(JsonToken token)
        {
            this.UpdateScopeWithFinishedValue();
            this.AutoComplete(token);
        }

        internal void InternalWriteWhitespace(string ws)
        {
            if ((ws != null) && !StringUtils.IsWhiteSpace(ws))
            {
                throw JsonWriterException.Create(this, "Only white space characters should be used.", null);
            }
        }

        internal virtual void OnStringEscapeHandlingChanged()
        {
        }

        private JsonContainerType Peek() => 
            this._currentPosition.Type;

        private JsonContainerType Pop()
        {
            if ((this._stack != null) && (this._stack.Count > 0))
            {
                this._currentPosition = this._stack[this._stack.Count - 1];
                this._stack.RemoveAt(this._stack.Count - 1);
            }
            else
            {
                this._currentPosition = new JsonPosition();
            }
            return this._currentPosition.Type;
        }

        private void Push(JsonContainerType value)
        {
            if (this._currentPosition.Type != JsonContainerType.None)
            {
                if (this._stack == null)
                {
                    this._stack = new List<JsonPosition>();
                }
                this._stack.Add(this._currentPosition);
            }
            this._currentPosition = new JsonPosition(value);
        }

        protected void SetWriteState(JsonToken token, object value)
        {
            switch (token)
            {
                case JsonToken.StartObject:
                    this.InternalWriteStart(token, JsonContainerType.Object);
                    return;

                case JsonToken.StartArray:
                    this.InternalWriteStart(token, JsonContainerType.Array);
                    return;

                case JsonToken.StartConstructor:
                    this.InternalWriteStart(token, JsonContainerType.Constructor);
                    return;

                case JsonToken.PropertyName:
                    if (!(value is string))
                    {
                        throw new ArgumentException("A name is required when setting property name state.", "value");
                    }
                    this.InternalWritePropertyName((string) value);
                    return;

                case JsonToken.Comment:
                    this.InternalWriteComment();
                    return;

                case JsonToken.Raw:
                    this.InternalWriteRaw();
                    return;

                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Null:
                case JsonToken.Undefined:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    this.InternalWriteValue(token);
                    return;

                case JsonToken.EndObject:
                    this.InternalWriteEnd(JsonContainerType.Object);
                    return;

                case JsonToken.EndArray:
                    this.InternalWriteEnd(JsonContainerType.Array);
                    return;

                case JsonToken.EndConstructor:
                    this.InternalWriteEnd(JsonContainerType.Constructor);
                    return;
            }
            throw new ArgumentOutOfRangeException("token");
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void UpdateScopeWithFinishedValue()
        {
            if (this._currentPosition.HasIndex)
            {
                this._currentPosition.Position++;
            }
        }

        public virtual void WriteComment(string text)
        {
            this.InternalWriteComment();
        }

        private void WriteConstructorDate(JsonReader reader)
        {
            if (!reader.Read())
            {
                throw JsonWriterException.Create(this, "Unexpected end when reading date constructor.", null);
            }
            if (reader.TokenType != JsonToken.Integer)
            {
                throw JsonWriterException.Create(this, "Unexpected token when reading date constructor. Expected Integer, got " + reader.TokenType, null);
            }
            DateTime time = DateTimeUtils.ConvertJavaScriptTicksToDateTime((long) reader.Value);
            if (!reader.Read())
            {
                throw JsonWriterException.Create(this, "Unexpected end when reading date constructor.", null);
            }
            if (reader.TokenType != JsonToken.EndConstructor)
            {
                throw JsonWriterException.Create(this, "Unexpected token when reading date constructor. Expected EndConstructor, got " + reader.TokenType, null);
            }
            this.WriteValue(time);
        }

        public virtual void WriteEnd()
        {
            this.WriteEnd(this.Peek());
        }

        private void WriteEnd(JsonContainerType type)
        {
            switch (type)
            {
                case JsonContainerType.Object:
                    this.WriteEndObject();
                    return;

                case JsonContainerType.Array:
                    this.WriteEndArray();
                    return;

                case JsonContainerType.Constructor:
                    this.WriteEndConstructor();
                    return;
            }
            throw JsonWriterException.Create(this, "Unexpected type when writing end: " + type, null);
        }

        protected virtual void WriteEnd(JsonToken token)
        {
        }

        public virtual void WriteEndArray()
        {
            this.InternalWriteEnd(JsonContainerType.Array);
        }

        public virtual void WriteEndConstructor()
        {
            this.InternalWriteEnd(JsonContainerType.Constructor);
        }

        public virtual void WriteEndObject()
        {
            this.InternalWriteEnd(JsonContainerType.Object);
        }

        protected virtual void WriteIndent()
        {
        }

        protected virtual void WriteIndentSpace()
        {
        }

        public virtual void WriteNull()
        {
            this.InternalWriteValue(JsonToken.Null);
        }

        public virtual void WritePropertyName(string name)
        {
            this.InternalWritePropertyName(name);
        }

        public virtual void WritePropertyName(string name, bool escape)
        {
            this.WritePropertyName(name);
        }

        public virtual void WriteRaw(string json)
        {
            this.InternalWriteRaw();
        }

        public virtual void WriteRawValue(string json)
        {
            this.UpdateScopeWithFinishedValue();
            this.AutoComplete(JsonToken.Undefined);
            this.WriteRaw(json);
        }

        public virtual void WriteStartArray()
        {
            this.InternalWriteStart(JsonToken.StartArray, JsonContainerType.Array);
        }

        public virtual void WriteStartConstructor(string name)
        {
            this.InternalWriteStart(JsonToken.StartConstructor, JsonContainerType.Constructor);
        }

        public virtual void WriteStartObject()
        {
            this.InternalWriteStart(JsonToken.StartObject, JsonContainerType.Object);
        }

        public void WriteToken(JsonReader reader)
        {
            this.WriteToken(reader, true);
        }

        public void WriteToken(JsonToken token)
        {
            this.WriteToken(token, null);
        }

        public void WriteToken(JsonReader reader, bool writeChildren)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            this.WriteToken(reader, writeChildren, true, true);
        }

        public void WriteToken(JsonToken token, object value)
        {
            switch (token)
            {
                case JsonToken.None:
                    return;

                case JsonToken.StartObject:
                    this.WriteStartObject();
                    return;

                case JsonToken.StartArray:
                    this.WriteStartArray();
                    return;

                case JsonToken.StartConstructor:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    this.WriteStartConstructor(value.ToString());
                    return;

                case JsonToken.PropertyName:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    this.WritePropertyName(value.ToString());
                    return;

                case JsonToken.Comment:
                    this.WriteComment(value?.ToString());
                    return;

                case JsonToken.Raw:
                    this.WriteRawValue(value?.ToString());
                    return;

                case JsonToken.Integer:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    if (!(value is BigInteger))
                    {
                        this.WriteValue(Convert.ToInt64(value, CultureInfo.InvariantCulture));
                        return;
                    }
                    this.WriteValue((BigInteger) value);
                    return;

                case JsonToken.Float:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    if (!(value is decimal))
                    {
                        if (value is double)
                        {
                            this.WriteValue((double) value);
                            return;
                        }
                        if (value is float)
                        {
                            this.WriteValue((float) value);
                            return;
                        }
                        this.WriteValue(Convert.ToDouble(value, CultureInfo.InvariantCulture));
                        return;
                    }
                    this.WriteValue((decimal) value);
                    return;

                case JsonToken.String:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    this.WriteValue(value.ToString());
                    return;

                case JsonToken.Boolean:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    this.WriteValue(Convert.ToBoolean(value, CultureInfo.InvariantCulture));
                    return;

                case JsonToken.Null:
                    this.WriteNull();
                    return;

                case JsonToken.Undefined:
                    this.WriteUndefined();
                    return;

                case JsonToken.EndObject:
                    this.WriteEndObject();
                    return;

                case JsonToken.EndArray:
                    this.WriteEndArray();
                    return;

                case JsonToken.EndConstructor:
                    this.WriteEndConstructor();
                    return;

                case JsonToken.Date:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    if (!(value is DateTimeOffset))
                    {
                        this.WriteValue(Convert.ToDateTime(value, CultureInfo.InvariantCulture));
                        return;
                    }
                    this.WriteValue((DateTimeOffset) value);
                    return;

                case JsonToken.Bytes:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    if (!(value is Guid))
                    {
                        this.WriteValue((byte[]) value);
                        return;
                    }
                    this.WriteValue((Guid) value);
                    return;
            }
            throw MiscellaneousUtils.CreateArgumentOutOfRangeException("token", token, "Unexpected token type.");
        }

        internal virtual void WriteToken(JsonReader reader, bool writeChildren, bool writeDateConstructorAsDate, bool writeComments)
        {
            int depth;
            if (reader.TokenType == JsonToken.None)
            {
                depth = -1;
            }
            else if (!JsonTokenUtils.IsStartToken(reader.TokenType))
            {
                depth = reader.Depth + 1;
            }
            else
            {
                depth = reader.Depth;
            }
            do
            {
                if ((writeDateConstructorAsDate && (reader.TokenType == JsonToken.StartConstructor)) && string.Equals(reader.Value.ToString(), "Date", StringComparison.Ordinal))
                {
                    this.WriteConstructorDate(reader);
                }
                else if (writeComments || (reader.TokenType != JsonToken.Comment))
                {
                    this.WriteToken(reader.TokenType, reader.Value);
                }
            }
            while ((((depth - 1) < (reader.Depth - (JsonTokenUtils.IsEndToken(reader.TokenType) ? 1 : 0))) & writeChildren) && reader.Read());
        }

        public virtual void WriteUndefined()
        {
            this.InternalWriteValue(JsonToken.Undefined);
        }

        public virtual void WriteValue(bool value)
        {
            this.InternalWriteValue(JsonToken.Boolean);
        }

        public virtual void WriteValue(byte value)
        {
            this.InternalWriteValue(JsonToken.Integer);
        }

        public virtual void WriteValue(char value)
        {
            this.InternalWriteValue(JsonToken.String);
        }

        public virtual void WriteValue(DateTime value)
        {
            this.InternalWriteValue(JsonToken.Date);
        }

        public virtual void WriteValue(DateTimeOffset value)
        {
            this.InternalWriteValue(JsonToken.Date);
        }

        public virtual void WriteValue(decimal value)
        {
            this.InternalWriteValue(JsonToken.Float);
        }

        public virtual void WriteValue(double value)
        {
            this.InternalWriteValue(JsonToken.Float);
        }

        public virtual void WriteValue(Guid value)
        {
            this.InternalWriteValue(JsonToken.String);
        }

        public virtual void WriteValue(short value)
        {
            this.InternalWriteValue(JsonToken.Integer);
        }

        public virtual void WriteValue(int value)
        {
            this.InternalWriteValue(JsonToken.Integer);
        }

        public virtual void WriteValue(long value)
        {
            this.InternalWriteValue(JsonToken.Integer);
        }

        public virtual void WriteValue(bool? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        public virtual void WriteValue(byte? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        public virtual void WriteValue(char? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        public virtual void WriteValue(DateTime? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        public virtual void WriteValue(DateTimeOffset? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        public virtual void WriteValue(decimal? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        public virtual void WriteValue(double? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        public virtual void WriteValue(Guid? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        public virtual void WriteValue(short? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        public virtual void WriteValue(int? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        public virtual void WriteValue(long? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(sbyte? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        public virtual void WriteValue(float? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        public virtual void WriteValue(TimeSpan? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(ushort? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(uint? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(ulong? value)
        {
            if (!value.HasValue)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.GetValueOrDefault());
            }
        }

        public virtual void WriteValue(object value)
        {
            if (value == null)
            {
                this.WriteNull();
            }
            else
            {
                if (value is BigInteger)
                {
                    throw CreateUnsupportedTypeException(this, value);
                }
                WriteValue(this, ConvertUtils.GetTypeCode(value.GetType()), value);
            }
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(sbyte value)
        {
            this.InternalWriteValue(JsonToken.Integer);
        }

        public virtual void WriteValue(float value)
        {
            this.InternalWriteValue(JsonToken.Float);
        }

        public virtual void WriteValue(string value)
        {
            this.InternalWriteValue(JsonToken.String);
        }

        public virtual void WriteValue(TimeSpan value)
        {
            this.InternalWriteValue(JsonToken.String);
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(ushort value)
        {
            this.InternalWriteValue(JsonToken.Integer);
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(uint value)
        {
            this.InternalWriteValue(JsonToken.Integer);
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(ulong value)
        {
            this.InternalWriteValue(JsonToken.Integer);
        }

        public virtual void WriteValue(byte[] value)
        {
            if (value == null)
            {
                this.WriteNull();
            }
            else
            {
                this.InternalWriteValue(JsonToken.Bytes);
            }
        }

        public virtual void WriteValue(Uri value)
        {
            if (value == null)
            {
                this.WriteNull();
            }
            else
            {
                this.InternalWriteValue(JsonToken.String);
            }
        }

        internal static void WriteValue(JsonWriter writer, PrimitiveTypeCode typeCode, object value)
        {
            switch (typeCode)
            {
                case PrimitiveTypeCode.Char:
                    writer.WriteValue((char) value);
                    return;

                case PrimitiveTypeCode.CharNullable:
                    writer.WriteValue((value == null) ? null : new char?((char) value));
                    return;

                case PrimitiveTypeCode.Boolean:
                    writer.WriteValue((bool) value);
                    return;

                case PrimitiveTypeCode.BooleanNullable:
                    writer.WriteValue((value == null) ? null : new bool?((bool) value));
                    return;

                case PrimitiveTypeCode.SByte:
                    writer.WriteValue((sbyte) value);
                    return;

                case PrimitiveTypeCode.SByteNullable:
                    writer.WriteValue((value == null) ? null : new sbyte?((sbyte) value));
                    return;

                case PrimitiveTypeCode.Int16:
                    writer.WriteValue((short) value);
                    return;

                case PrimitiveTypeCode.Int16Nullable:
                    writer.WriteValue((value == null) ? null : new short?((short) value));
                    return;

                case PrimitiveTypeCode.UInt16:
                    writer.WriteValue((ushort) value);
                    return;

                case PrimitiveTypeCode.UInt16Nullable:
                    writer.WriteValue((value == null) ? null : new ushort?((ushort) value));
                    return;

                case PrimitiveTypeCode.Int32:
                    writer.WriteValue((int) value);
                    return;

                case PrimitiveTypeCode.Int32Nullable:
                    writer.WriteValue((value == null) ? null : new int?((int) value));
                    return;

                case PrimitiveTypeCode.Byte:
                    writer.WriteValue((byte) value);
                    return;

                case PrimitiveTypeCode.ByteNullable:
                    writer.WriteValue((value == null) ? null : new byte?((byte) value));
                    return;

                case PrimitiveTypeCode.UInt32:
                    writer.WriteValue((uint) value);
                    return;

                case PrimitiveTypeCode.UInt32Nullable:
                    writer.WriteValue((value == null) ? null : new uint?((uint) value));
                    return;

                case PrimitiveTypeCode.Int64:
                    writer.WriteValue((long) value);
                    return;

                case PrimitiveTypeCode.Int64Nullable:
                    writer.WriteValue((value == null) ? null : new long?((long) value));
                    return;

                case PrimitiveTypeCode.UInt64:
                    writer.WriteValue((ulong) value);
                    return;

                case PrimitiveTypeCode.UInt64Nullable:
                    writer.WriteValue((value == null) ? null : new ulong?((ulong) value));
                    return;

                case PrimitiveTypeCode.Single:
                    writer.WriteValue((float) value);
                    return;

                case PrimitiveTypeCode.SingleNullable:
                    writer.WriteValue((value == null) ? null : new float?((float) value));
                    return;

                case PrimitiveTypeCode.Double:
                    writer.WriteValue((double) value);
                    return;

                case PrimitiveTypeCode.DoubleNullable:
                    writer.WriteValue((value == null) ? null : new double?((double) value));
                    return;

                case PrimitiveTypeCode.DateTime:
                    writer.WriteValue((DateTime) value);
                    return;

                case PrimitiveTypeCode.DateTimeNullable:
                    writer.WriteValue((value == null) ? null : new DateTime?((DateTime) value));
                    return;

                case PrimitiveTypeCode.DateTimeOffset:
                    writer.WriteValue((DateTimeOffset) value);
                    return;

                case PrimitiveTypeCode.DateTimeOffsetNullable:
                    writer.WriteValue((value == null) ? null : new DateTimeOffset?((DateTimeOffset) value));
                    return;

                case PrimitiveTypeCode.Decimal:
                    writer.WriteValue((decimal) value);
                    return;

                case PrimitiveTypeCode.DecimalNullable:
                    writer.WriteValue((value == null) ? null : new decimal?((decimal) value));
                    return;

                case PrimitiveTypeCode.Guid:
                    writer.WriteValue((Guid) value);
                    return;

                case PrimitiveTypeCode.GuidNullable:
                    writer.WriteValue((value == null) ? null : new Guid?((Guid) value));
                    return;

                case PrimitiveTypeCode.TimeSpan:
                    writer.WriteValue((TimeSpan) value);
                    return;

                case PrimitiveTypeCode.TimeSpanNullable:
                    writer.WriteValue((value == null) ? null : new TimeSpan?((TimeSpan) value));
                    return;

                case PrimitiveTypeCode.BigInteger:
                    writer.WriteValue((BigInteger) value);
                    return;

                case PrimitiveTypeCode.BigIntegerNullable:
                    writer.WriteValue((value == null) ? null : new BigInteger?((BigInteger) value));
                    return;

                case PrimitiveTypeCode.Uri:
                    writer.WriteValue((Uri) value);
                    return;

                case PrimitiveTypeCode.String:
                    writer.WriteValue((string) value);
                    return;

                case PrimitiveTypeCode.Bytes:
                    writer.WriteValue((byte[]) value);
                    return;

                case PrimitiveTypeCode.DBNull:
                    writer.WriteNull();
                    return;
            }
            if (!(value is IConvertible))
            {
                throw CreateUnsupportedTypeException(writer, value);
            }
            IConvertible convertable = (IConvertible) value;
            TypeInformation typeInformation = ConvertUtils.GetTypeInformation(convertable);
            PrimitiveTypeCode code = (typeInformation.TypeCode == PrimitiveTypeCode.Object) ? PrimitiveTypeCode.String : typeInformation.TypeCode;
            Type conversionType = (typeInformation.TypeCode == PrimitiveTypeCode.Object) ? typeof(string) : typeInformation.Type;
            object obj2 = convertable.ToType(conversionType, CultureInfo.InvariantCulture);
            WriteValue(writer, code, obj2);
        }

        protected virtual void WriteValueDelimiter()
        {
        }

        public virtual void WriteWhitespace(string ws)
        {
            this.InternalWriteWhitespace(ws);
        }

        public bool CloseOutput { get; set; }

        protected internal int Top
        {
            get
            {
                int num = (this._stack != null) ? this._stack.Count : 0;
                if (this.Peek() != JsonContainerType.None)
                {
                    num++;
                }
                return num;
            }
        }

        public Newtonsoft.Json.WriteState WriteState
        {
            get
            {
                switch (this._currentState)
                {
                    case State.Start:
                        return Newtonsoft.Json.WriteState.Start;

                    case State.Property:
                        return Newtonsoft.Json.WriteState.Property;

                    case State.ObjectStart:
                    case State.Object:
                        return Newtonsoft.Json.WriteState.Object;

                    case State.ArrayStart:
                    case State.Array:
                        return Newtonsoft.Json.WriteState.Array;

                    case State.ConstructorStart:
                    case State.Constructor:
                        return Newtonsoft.Json.WriteState.Constructor;

                    case State.Closed:
                        return Newtonsoft.Json.WriteState.Closed;

                    case State.Error:
                        return Newtonsoft.Json.WriteState.Error;
                }
                throw JsonWriterException.Create(this, "Invalid state: " + this._currentState, null);
            }
        }

        internal string ContainerPath
        {
            get
            {
                if ((this._currentPosition.Type != JsonContainerType.None) && (this._stack != null))
                {
                    return JsonPosition.BuildPath(this._stack, null);
                }
                return string.Empty;
            }
        }

        public string Path
        {
            get
            {
                if (this._currentPosition.Type == JsonContainerType.None)
                {
                    return string.Empty;
                }
                JsonPosition? currentPosition = (((this._currentState != State.ArrayStart) && (this._currentState != State.ConstructorStart)) && (this._currentState != State.ObjectStart)) ? new JsonPosition?(this._currentPosition) : null;
                return JsonPosition.BuildPath(this._stack, currentPosition);
            }
        }

        public Newtonsoft.Json.Formatting Formatting
        {
            get => 
                this._formatting;
            set
            {
                if ((value < Newtonsoft.Json.Formatting.None) || (value > Newtonsoft.Json.Formatting.Indented))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._formatting = value;
            }
        }

        public Newtonsoft.Json.DateFormatHandling DateFormatHandling
        {
            get => 
                this._dateFormatHandling;
            set
            {
                if ((value < Newtonsoft.Json.DateFormatHandling.IsoDateFormat) || (value > Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._dateFormatHandling = value;
            }
        }

        public Newtonsoft.Json.DateTimeZoneHandling DateTimeZoneHandling
        {
            get => 
                this._dateTimeZoneHandling;
            set
            {
                if ((value < Newtonsoft.Json.DateTimeZoneHandling.Local) || (value > Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._dateTimeZoneHandling = value;
            }
        }

        public Newtonsoft.Json.StringEscapeHandling StringEscapeHandling
        {
            get => 
                this._stringEscapeHandling;
            set
            {
                if ((value < Newtonsoft.Json.StringEscapeHandling.Default) || (value > Newtonsoft.Json.StringEscapeHandling.EscapeHtml))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._stringEscapeHandling = value;
                this.OnStringEscapeHandlingChanged();
            }
        }

        public Newtonsoft.Json.FloatFormatHandling FloatFormatHandling
        {
            get => 
                this._floatFormatHandling;
            set
            {
                if ((value < Newtonsoft.Json.FloatFormatHandling.String) || (value > Newtonsoft.Json.FloatFormatHandling.DefaultValue))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._floatFormatHandling = value;
            }
        }

        public string DateFormatString
        {
            get => 
                this._dateFormatString;
            set => 
                (this._dateFormatString = value);
        }

        public CultureInfo Culture
        {
            get
            {
                if (this._culture == null)
                {
                }
                return CultureInfo.InvariantCulture;
            }
            set => 
                (this._culture = value);
        }

        internal enum State
        {
            Start,
            Property,
            ObjectStart,
            Object,
            ArrayStart,
            Array,
            ConstructorStart,
            Constructor,
            Closed,
            Error
        }
    }
}

