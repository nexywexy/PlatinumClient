namespace Newtonsoft.Json
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Numerics;
    using System.Runtime.CompilerServices;

    public abstract class JsonReader : IDisposable
    {
        private JsonToken _tokenType;
        private object _value;
        internal char _quoteChar;
        internal State _currentState = State.Start;
        private JsonPosition _currentPosition;
        private CultureInfo _culture;
        private Newtonsoft.Json.DateTimeZoneHandling _dateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
        private int? _maxDepth;
        private bool _hasExceededMaxDepth;
        internal Newtonsoft.Json.DateParseHandling _dateParseHandling = Newtonsoft.Json.DateParseHandling.DateTime;
        internal Newtonsoft.Json.FloatParseHandling _floatParseHandling = Newtonsoft.Json.FloatParseHandling.Double;
        private string _dateFormatString;
        private List<JsonPosition> _stack;

        protected JsonReader()
        {
            this.CloseInput = true;
        }

        public virtual void Close()
        {
            this._currentState = State.Closed;
            this._tokenType = JsonToken.None;
            this._value = null;
        }

        internal JsonReaderException CreateUnexpectedEndException() => 
            JsonReaderException.Create(this, "Unexpected end when reading JSON.");

        protected virtual void Dispose(bool disposing)
        {
            if ((this._currentState != State.Closed) & disposing)
            {
                this.Close();
            }
        }

        private JsonToken GetContentToken()
        {
            JsonToken tokenType;
            do
            {
                if (!this.Read())
                {
                    this.SetToken(JsonToken.None);
                    return JsonToken.None;
                }
                tokenType = this.TokenType;
            }
            while (tokenType == JsonToken.Comment);
            return tokenType;
        }

        internal JsonPosition GetPosition(int depth)
        {
            if ((this._stack != null) && (depth < this._stack.Count))
            {
                return this._stack[depth];
            }
            return this._currentPosition;
        }

        private JsonContainerType GetTypeForCloseToken(JsonToken token)
        {
            switch (token)
            {
                case JsonToken.EndObject:
                    return JsonContainerType.Object;

                case JsonToken.EndArray:
                    return JsonContainerType.Array;

                case JsonToken.EndConstructor:
                    return JsonContainerType.Constructor;
            }
            throw JsonReaderException.Create(this, "Not a valid close JsonToken: {0}".FormatWith(CultureInfo.InvariantCulture, token));
        }

        internal bool MoveToContent()
        {
            for (JsonToken token = this.TokenType; (token == JsonToken.None) || (token == JsonToken.Comment); token = this.TokenType)
            {
                if (!this.Read())
                {
                    return false;
                }
            }
            return true;
        }

        private JsonContainerType Peek() => 
            this._currentPosition.Type;

        private JsonContainerType Pop()
        {
            JsonPosition position;
            if ((this._stack != null) && (this._stack.Count > 0))
            {
                position = this._currentPosition;
                this._currentPosition = this._stack[this._stack.Count - 1];
                this._stack.RemoveAt(this._stack.Count - 1);
            }
            else
            {
                position = this._currentPosition;
                this._currentPosition = new JsonPosition();
            }
            if (this._maxDepth.HasValue)
            {
                int? nullable = this._maxDepth;
                if ((this.Depth <= nullable.GetValueOrDefault()) ? nullable.HasValue : false)
                {
                    this._hasExceededMaxDepth = false;
                }
            }
            return position.Type;
        }

        private void Push(JsonContainerType value)
        {
            this.UpdateScopeWithFinishedValue();
            if (this._currentPosition.Type == JsonContainerType.None)
            {
                this._currentPosition = new JsonPosition(value);
            }
            else
            {
                if (this._stack == null)
                {
                    this._stack = new List<JsonPosition>();
                }
                this._stack.Add(this._currentPosition);
                this._currentPosition = new JsonPosition(value);
                if (this._maxDepth.HasValue)
                {
                    int? nullable = this._maxDepth;
                    if ((((this.Depth + 1) > nullable.GetValueOrDefault()) ? nullable.HasValue : false) && !this._hasExceededMaxDepth)
                    {
                        this._hasExceededMaxDepth = true;
                        throw JsonReaderException.Create(this, "The reader's MaxDepth of {0} has been exceeded.".FormatWith(CultureInfo.InvariantCulture, this._maxDepth));
                    }
                }
            }
        }

        public abstract bool Read();
        internal void ReadAndAssert()
        {
            if (!this.Read())
            {
                throw JsonSerializationException.Create(this, "Unexpected end when reading JSON.");
            }
        }

        internal bool ReadAndMoveToContent() => 
            (this.Read() && this.MoveToContent());

        internal byte[] ReadArrayIntoByteArray()
        {
            List<byte> list = new List<byte>();
            while (true)
            {
                JsonToken contentToken = this.GetContentToken();
                if (contentToken == JsonToken.None)
                {
                    throw JsonReaderException.Create(this, "Unexpected end when reading bytes.");
                }
                if (contentToken != JsonToken.Integer)
                {
                    if (contentToken != JsonToken.EndArray)
                    {
                        throw JsonReaderException.Create(this, "Unexpected token when reading bytes: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
                    }
                    break;
                }
                list.Add(Convert.ToByte(this.Value, CultureInfo.InvariantCulture));
            }
            byte[] buffer = list.ToArray();
            this.SetToken(JsonToken.Bytes, buffer, false);
            return buffer;
        }

        public virtual bool? ReadAsBoolean()
        {
            bool flag;
            JsonToken contentToken = this.GetContentToken();
            switch (contentToken)
            {
                case JsonToken.Integer:
                case JsonToken.Float:
                    if (!(this.Value is BigInteger))
                    {
                        flag = Convert.ToBoolean(this.Value, CultureInfo.InvariantCulture);
                        break;
                    }
                    flag = ((BigInteger) this.Value) != 0L;
                    break;

                case JsonToken.String:
                    return this.ReadBooleanString((string) this.Value);

                case JsonToken.Boolean:
                    return new bool?((bool) this.Value);

                case JsonToken.Null:
                case JsonToken.EndArray:
                case JsonToken.None:
                    return null;

                default:
                    throw JsonReaderException.Create(this, "Error reading boolean. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
            }
            this.SetToken(JsonToken.Boolean, flag, false);
            return new bool?(flag);
        }

        public virtual byte[] ReadAsBytes()
        {
            JsonToken contentToken = this.GetContentToken();
            if (contentToken == JsonToken.None)
            {
                return null;
            }
            if (this.TokenType == JsonToken.StartObject)
            {
                this.ReadIntoWrappedTypeObject();
                byte[] buffer = this.ReadAsBytes();
                this.ReaderReadAndAssert();
                if (this.TokenType != JsonToken.EndObject)
                {
                    throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
                }
                this.SetToken(JsonToken.Bytes, buffer, false);
                return buffer;
            }
            switch (contentToken)
            {
                case JsonToken.StartArray:
                    return this.ReadArrayIntoByteArray();

                case JsonToken.String:
                {
                    byte[] buffer2;
                    string s = (string) this.Value;
                    if (s.Length == 0)
                    {
                        buffer2 = new byte[0];
                    }
                    else if (ConvertUtils.TryConvertGuid(s, out Guid guid))
                    {
                        buffer2 = guid.ToByteArray();
                    }
                    else
                    {
                        buffer2 = Convert.FromBase64String(s);
                    }
                    this.SetToken(JsonToken.Bytes, buffer2, false);
                    return buffer2;
                }
                case JsonToken.Null:
                case JsonToken.EndArray:
                    return null;

                case JsonToken.Bytes:
                    if (this.ValueType == typeof(Guid))
                    {
                        byte[] buffer3 = ((Guid) this.Value).ToByteArray();
                        this.SetToken(JsonToken.Bytes, buffer3, false);
                        return buffer3;
                    }
                    return (byte[]) this.Value;
            }
            throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
        }

        public virtual DateTime? ReadAsDateTime()
        {
            switch (this.GetContentToken())
            {
                case JsonToken.Null:
                case JsonToken.EndArray:
                case JsonToken.None:
                    return null;

                case JsonToken.Date:
                    if (this.Value is DateTimeOffset)
                    {
                        DateTimeOffset offset = (DateTimeOffset) this.Value;
                        this.SetToken(JsonToken.Date, offset.DateTime, false);
                    }
                    return new DateTime?((DateTime) this.Value);

                case JsonToken.String:
                {
                    string s = (string) this.Value;
                    return this.ReadDateTimeString(s);
                }
            }
            throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
        }

        public virtual DateTimeOffset? ReadAsDateTimeOffset()
        {
            JsonToken contentToken = this.GetContentToken();
            switch (contentToken)
            {
                case JsonToken.Null:
                case JsonToken.EndArray:
                case JsonToken.None:
                    return null;

                case JsonToken.Date:
                    if (this.Value is DateTime)
                    {
                        this.SetToken(JsonToken.Date, new DateTimeOffset((DateTime) this.Value), false);
                    }
                    return new DateTimeOffset?((DateTimeOffset) this.Value);

                case JsonToken.String:
                {
                    string s = (string) this.Value;
                    return this.ReadDateTimeOffsetString(s);
                }
            }
            throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
        }

        public virtual decimal? ReadAsDecimal()
        {
            JsonToken contentToken = this.GetContentToken();
            switch (contentToken)
            {
                case JsonToken.Integer:
                case JsonToken.Float:
                    if (!(this.Value is decimal))
                    {
                        this.SetToken(JsonToken.Float, Convert.ToDecimal(this.Value, CultureInfo.InvariantCulture), false);
                    }
                    return new decimal?((decimal) this.Value);

                case JsonToken.String:
                    return this.ReadDecimalString((string) this.Value);

                case JsonToken.Null:
                case JsonToken.EndArray:
                case JsonToken.None:
                    return null;
            }
            throw JsonReaderException.Create(this, "Error reading decimal. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
        }

        public virtual double? ReadAsDouble()
        {
            double num;
            JsonToken contentToken = this.GetContentToken();
            switch (contentToken)
            {
                case JsonToken.Integer:
                case JsonToken.Float:
                    if (this.Value is double)
                    {
                        goto Label_009A;
                    }
                    if (!(this.Value is BigInteger))
                    {
                        num = Convert.ToDouble(this.Value, CultureInfo.InvariantCulture);
                        break;
                    }
                    num = (double) ((BigInteger) this.Value);
                    break;

                case JsonToken.String:
                    return this.ReadDoubleString((string) this.Value);

                case JsonToken.Null:
                case JsonToken.EndArray:
                case JsonToken.None:
                    return null;

                default:
                    throw JsonReaderException.Create(this, "Error reading double. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
            }
            this.SetToken(JsonToken.Float, num, false);
        Label_009A:
            return new double?((double) this.Value);
        }

        public virtual int? ReadAsInt32()
        {
            JsonToken contentToken = this.GetContentToken();
            switch (contentToken)
            {
                case JsonToken.Integer:
                case JsonToken.Float:
                    if (!(this.Value is int))
                    {
                        this.SetToken(JsonToken.Integer, Convert.ToInt32(this.Value, CultureInfo.InvariantCulture), false);
                    }
                    return new int?((int) this.Value);

                case JsonToken.String:
                {
                    string s = (string) this.Value;
                    return this.ReadInt32String(s);
                }
                case JsonToken.Null:
                case JsonToken.EndArray:
                case JsonToken.None:
                    return null;
            }
            throw JsonReaderException.Create(this, "Error reading integer. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
        }

        public virtual string ReadAsString()
        {
            string originalString;
            JsonToken contentToken = this.GetContentToken();
            if (contentToken <= JsonToken.String)
            {
                switch (contentToken)
                {
                    case JsonToken.None:
                        goto Label_0032;

                    case JsonToken.String:
                        return (string) this.Value;
                }
                goto Label_0040;
            }
            if ((contentToken != JsonToken.Null) && (contentToken != JsonToken.EndArray))
            {
                goto Label_0040;
            }
        Label_0032:
            return null;
        Label_0040:
            if (!JsonTokenUtils.IsPrimitiveToken(contentToken) || (this.Value == null))
            {
                throw JsonReaderException.Create(this, "Error reading string. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
            }
            if (this.Value is IFormattable)
            {
                originalString = ((IFormattable) this.Value).ToString(null, this.Culture);
            }
            else if (this.Value is Uri)
            {
                originalString = ((Uri) this.Value).OriginalString;
            }
            else
            {
                originalString = this.Value.ToString();
            }
            this.SetToken(JsonToken.String, originalString, false);
            return originalString;
        }

        internal bool? ReadBooleanString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                this.SetToken(JsonToken.Null, null, false);
                return null;
            }
            if (bool.TryParse(s, out bool flag))
            {
                this.SetToken(JsonToken.Boolean, flag, false);
                return new bool?(flag);
            }
            this.SetToken(JsonToken.String, s, false);
            throw JsonReaderException.Create(this, "Could not convert string to boolean: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
        }

        internal DateTimeOffset? ReadDateTimeOffsetString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                this.SetToken(JsonToken.Null, null, false);
                return null;
            }
            if (DateTimeUtils.TryParseDateTimeOffset(s, this._dateFormatString, this.Culture, out DateTimeOffset offset))
            {
                this.SetToken(JsonToken.Date, offset, false);
                return new DateTimeOffset?(offset);
            }
            if (DateTimeOffset.TryParse(s, this.Culture, DateTimeStyles.RoundtripKind, out offset))
            {
                this.SetToken(JsonToken.Date, offset, false);
                return new DateTimeOffset?(offset);
            }
            this.SetToken(JsonToken.String, s, false);
            throw JsonReaderException.Create(this, "Could not convert string to DateTimeOffset: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
        }

        internal DateTime? ReadDateTimeString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                this.SetToken(JsonToken.Null, null, false);
                return null;
            }
            if (DateTimeUtils.TryParseDateTime(s, this.DateTimeZoneHandling, this._dateFormatString, this.Culture, out DateTime time))
            {
                time = DateTimeUtils.EnsureDateTime(time, this.DateTimeZoneHandling);
                this.SetToken(JsonToken.Date, time, false);
                return new DateTime?(time);
            }
            if (!DateTime.TryParse(s, this.Culture, DateTimeStyles.RoundtripKind, out time))
            {
                throw JsonReaderException.Create(this, "Could not convert string to DateTime: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
            }
            time = DateTimeUtils.EnsureDateTime(time, this.DateTimeZoneHandling);
            this.SetToken(JsonToken.Date, time, false);
            return new DateTime?(time);
        }

        internal decimal? ReadDecimalString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                this.SetToken(JsonToken.Null, null, false);
                return null;
            }
            if (decimal.TryParse(s, NumberStyles.Number, this.Culture, out decimal num))
            {
                this.SetToken(JsonToken.Float, num, false);
                return new decimal?(num);
            }
            this.SetToken(JsonToken.String, s, false);
            throw JsonReaderException.Create(this, "Could not convert string to decimal: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
        }

        internal double? ReadDoubleString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                this.SetToken(JsonToken.Null, null, false);
                return null;
            }
            if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, this.Culture, out double num))
            {
                this.SetToken(JsonToken.Float, num, false);
                return new double?(num);
            }
            this.SetToken(JsonToken.String, s, false);
            throw JsonReaderException.Create(this, "Could not convert string to double: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
        }

        internal void ReaderReadAndAssert()
        {
            if (!this.Read())
            {
                throw this.CreateUnexpectedEndException();
            }
        }

        internal int? ReadInt32String(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                this.SetToken(JsonToken.Null, null, false);
                return null;
            }
            if (int.TryParse(s, NumberStyles.Integer, this.Culture, out int num))
            {
                this.SetToken(JsonToken.Integer, num, false);
                return new int?(num);
            }
            this.SetToken(JsonToken.String, s, false);
            throw JsonReaderException.Create(this, "Could not convert string to integer: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
        }

        internal void ReadIntoWrappedTypeObject()
        {
            this.ReaderReadAndAssert();
            if (this.Value.ToString() == "$type")
            {
                this.ReaderReadAndAssert();
                if ((this.Value != null) && this.Value.ToString().StartsWith("System.Byte[]", StringComparison.Ordinal))
                {
                    this.ReaderReadAndAssert();
                    if (this.Value.ToString() == "$value")
                    {
                        return;
                    }
                }
            }
            throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, JsonToken.StartObject));
        }

        private void SetFinished()
        {
            if (this.SupportMultipleContent)
            {
                this._currentState = State.Start;
            }
            else
            {
                this._currentState = State.Finished;
            }
        }

        internal void SetPostValueState(bool updateIndex)
        {
            if (this.Peek() != JsonContainerType.None)
            {
                this._currentState = State.PostValue;
            }
            else
            {
                this.SetFinished();
            }
            if (updateIndex)
            {
                this.UpdateScopeWithFinishedValue();
            }
        }

        protected void SetStateBasedOnCurrent()
        {
            JsonContainerType type = this.Peek();
            switch (type)
            {
                case JsonContainerType.None:
                    this.SetFinished();
                    return;

                case JsonContainerType.Object:
                    this._currentState = State.Object;
                    return;

                case JsonContainerType.Array:
                    this._currentState = State.Array;
                    return;

                case JsonContainerType.Constructor:
                    this._currentState = State.Constructor;
                    return;
            }
            throw JsonReaderException.Create(this, "While setting the reader state back to current object an unexpected JsonType was encountered: {0}".FormatWith(CultureInfo.InvariantCulture, type));
        }

        protected void SetToken(JsonToken newToken)
        {
            this.SetToken(newToken, null, true);
        }

        protected void SetToken(JsonToken newToken, object value)
        {
            this.SetToken(newToken, value, true);
        }

        internal void SetToken(JsonToken newToken, object value, bool updateIndex)
        {
            this._tokenType = newToken;
            this._value = value;
            switch (newToken)
            {
                case JsonToken.StartObject:
                    this._currentState = State.ObjectStart;
                    this.Push(JsonContainerType.Object);
                    return;

                case JsonToken.StartArray:
                    this._currentState = State.ArrayStart;
                    this.Push(JsonContainerType.Array);
                    return;

                case JsonToken.StartConstructor:
                    this._currentState = State.ConstructorStart;
                    this.Push(JsonContainerType.Constructor);
                    return;

                case JsonToken.PropertyName:
                    this._currentState = State.Property;
                    this._currentPosition.PropertyName = (string) value;
                    return;

                case JsonToken.Comment:
                    break;

                case JsonToken.Raw:
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Null:
                case JsonToken.Undefined:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    this.SetPostValueState(updateIndex);
                    break;

                case JsonToken.EndObject:
                    this.ValidateEnd(JsonToken.EndObject);
                    return;

                case JsonToken.EndArray:
                    this.ValidateEnd(JsonToken.EndArray);
                    return;

                case JsonToken.EndConstructor:
                    this.ValidateEnd(JsonToken.EndConstructor);
                    return;

                default:
                    return;
            }
        }

        public void Skip()
        {
            if (this.TokenType == JsonToken.PropertyName)
            {
                this.Read();
            }
            if (JsonTokenUtils.IsStartToken(this.TokenType))
            {
                int depth = this.Depth;
                while (this.Read() && (depth < this.Depth))
                {
                }
            }
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void UpdateScopeWithFinishedValue()
        {
            if (this._currentPosition.HasIndex)
            {
                this._currentPosition.Position++;
            }
        }

        private void ValidateEnd(JsonToken endToken)
        {
            JsonContainerType type = this.Pop();
            if (this.GetTypeForCloseToken(endToken) != type)
            {
                throw JsonReaderException.Create(this, "JsonToken {0} is not valid for closing JsonType {1}.".FormatWith(CultureInfo.InvariantCulture, endToken, type));
            }
            if (this.Peek() != JsonContainerType.None)
            {
                this._currentState = State.PostValue;
            }
            else
            {
                this.SetFinished();
            }
        }

        protected State CurrentState =>
            this._currentState;

        public bool CloseInput { get; set; }

        public bool SupportMultipleContent { get; set; }

        public virtual char QuoteChar
        {
            get => 
                this._quoteChar;
            protected internal set => 
                (this._quoteChar = value);
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

        public Newtonsoft.Json.DateParseHandling DateParseHandling
        {
            get => 
                this._dateParseHandling;
            set
            {
                if ((value < Newtonsoft.Json.DateParseHandling.None) || (value > Newtonsoft.Json.DateParseHandling.DateTimeOffset))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._dateParseHandling = value;
            }
        }

        public Newtonsoft.Json.FloatParseHandling FloatParseHandling
        {
            get => 
                this._floatParseHandling;
            set
            {
                if ((value < Newtonsoft.Json.FloatParseHandling.Double) || (value > Newtonsoft.Json.FloatParseHandling.Decimal))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._floatParseHandling = value;
            }
        }

        public string DateFormatString
        {
            get => 
                this._dateFormatString;
            set => 
                (this._dateFormatString = value);
        }

        public int? MaxDepth
        {
            get => 
                this._maxDepth;
            set
            {
                int? nullable = value;
                int num = 0;
                if ((nullable.GetValueOrDefault() <= num) ? nullable.HasValue : false)
                {
                    throw new ArgumentException("Value must be positive.", "value");
                }
                this._maxDepth = value;
            }
        }

        public virtual JsonToken TokenType =>
            this._tokenType;

        public virtual object Value =>
            this._value;

        public virtual Type ValueType =>
            this._value?.GetType();

        public virtual int Depth
        {
            get
            {
                int num = (this._stack != null) ? this._stack.Count : 0;
                if (!JsonTokenUtils.IsStartToken(this.TokenType) && (this._currentPosition.Type != JsonContainerType.None))
                {
                    return (num + 1);
                }
                return num;
            }
        }

        public virtual string Path
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

        internal protected enum State
        {
            Start,
            Complete,
            Property,
            ObjectStart,
            Object,
            ArrayStart,
            Array,
            Closed,
            PostValue,
            ConstructorStart,
            Constructor,
            Error,
            Finished
        }
    }
}

