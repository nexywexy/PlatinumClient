namespace Newtonsoft.Json.Linq
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Globalization;
    using System.Numerics;

    public class JTokenWriter : JsonWriter
    {
        private JContainer _token;
        private JContainer _parent;
        private JValue _value;
        private JToken _current;

        public JTokenWriter()
        {
        }

        public JTokenWriter(JContainer container)
        {
            ValidationUtils.ArgumentNotNull(container, "container");
            this._token = container;
            this._parent = container;
        }

        private void AddParent(JContainer container)
        {
            if (this._parent == null)
            {
                this._token = container;
            }
            else
            {
                this._parent.AddAndSkipParentCheck(container);
            }
            this._parent = container;
            this._current = container;
        }

        internal void AddValue(JValue value, JsonToken token)
        {
            if (this._parent != null)
            {
                this._parent.Add(value);
                this._current = this._parent.Last;
                if (this._parent.Type == JTokenType.Property)
                {
                    this._parent = this._parent.Parent;
                }
            }
            else
            {
                if (value == null)
                {
                }
                this._value = JValue.CreateNull();
                this._current = this._value;
            }
        }

        private void AddValue(object value, JsonToken token)
        {
            this.AddValue(new JValue(value), token);
        }

        public override void Close()
        {
            base.Close();
        }

        public override void Flush()
        {
        }

        private void RemoveParent()
        {
            this._current = this._parent;
            this._parent = this._parent.Parent;
            if ((this._parent != null) && (this._parent.Type == JTokenType.Property))
            {
                this._parent = this._parent.Parent;
            }
        }

        public override void WriteComment(string text)
        {
            base.WriteComment(text);
            this.AddValue(JValue.CreateComment(text), JsonToken.Comment);
        }

        protected override void WriteEnd(JsonToken token)
        {
            this.RemoveParent();
        }

        public override void WriteNull()
        {
            base.WriteNull();
            this.AddValue((JValue) null, JsonToken.Null);
        }

        public override void WritePropertyName(string name)
        {
            JObject obj2 = this._parent as JObject;
            if (obj2 != null)
            {
                obj2.Remove(name);
            }
            this.AddParent(new JProperty(name));
            base.WritePropertyName(name);
        }

        public override void WriteRaw(string json)
        {
            base.WriteRaw(json);
            this.AddValue((JValue) new JRaw(json), JsonToken.Raw);
        }

        public override void WriteStartArray()
        {
            base.WriteStartArray();
            this.AddParent(new JArray());
        }

        public override void WriteStartConstructor(string name)
        {
            base.WriteStartConstructor(name);
            this.AddParent(new JConstructor(name));
        }

        public override void WriteStartObject()
        {
            base.WriteStartObject();
            this.AddParent(new JObject());
        }

        internal override void WriteToken(JsonReader reader, bool writeChildren, bool writeDateConstructorAsDate, bool writeComments)
        {
            JTokenReader reader2 = reader as JTokenReader;
            if ((((reader2 > null) & writeChildren) & writeDateConstructorAsDate) & writeComments)
            {
                if ((reader2.TokenType != JsonToken.None) || reader2.Read())
                {
                    JToken content = reader2.CurrentToken.CloneToken();
                    if (this._parent != null)
                    {
                        this._parent.Add(content);
                        this._current = this._parent.Last;
                        if (this._parent.Type == JTokenType.Property)
                        {
                            this._parent = this._parent.Parent;
                            base.InternalWriteValue(JsonToken.Null);
                        }
                    }
                    else
                    {
                        this._current = content;
                        if ((this._token == null) && (this._value == null))
                        {
                            this._token = content as JContainer;
                            this._value = content as JValue;
                        }
                    }
                    reader2.Skip();
                }
            }
            else
            {
                base.WriteToken(reader, writeChildren, writeDateConstructorAsDate, writeComments);
            }
        }

        public override void WriteUndefined()
        {
            base.WriteUndefined();
            this.AddValue((JValue) null, JsonToken.Undefined);
        }

        public override void WriteValue(bool value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.Boolean);
        }

        public override void WriteValue(byte value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.Integer);
        }

        public override void WriteValue(char value)
        {
            base.WriteValue(value);
            string str = null;
            str = value.ToString(CultureInfo.InvariantCulture);
            this.AddValue(str, JsonToken.String);
        }

        public override void WriteValue(DateTime value)
        {
            base.WriteValue(value);
            value = DateTimeUtils.EnsureDateTime(value, base.DateTimeZoneHandling);
            this.AddValue(value, JsonToken.Date);
        }

        public override void WriteValue(DateTimeOffset value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.Date);
        }

        public override void WriteValue(decimal value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.Float);
        }

        public override void WriteValue(double value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.Float);
        }

        public override void WriteValue(Guid value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.String);
        }

        public override void WriteValue(short value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.Integer);
        }

        public override void WriteValue(int value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.Integer);
        }

        public override void WriteValue(long value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.Integer);
        }

        public override void WriteValue(object value)
        {
            if (value is BigInteger)
            {
                base.InternalWriteValue(JsonToken.Integer);
                this.AddValue(value, JsonToken.Integer);
            }
            else
            {
                base.WriteValue(value);
            }
        }

        [CLSCompliant(false)]
        public override void WriteValue(sbyte value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.Integer);
        }

        public override void WriteValue(float value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.Float);
        }

        public override void WriteValue(string value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.String);
        }

        public override void WriteValue(TimeSpan value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.String);
        }

        [CLSCompliant(false)]
        public override void WriteValue(ushort value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.Integer);
        }

        [CLSCompliant(false)]
        public override void WriteValue(uint value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.Integer);
        }

        [CLSCompliant(false)]
        public override void WriteValue(ulong value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.Integer);
        }

        public override void WriteValue(byte[] value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.Bytes);
        }

        public override void WriteValue(Uri value)
        {
            base.WriteValue(value);
            this.AddValue(value, JsonToken.String);
        }

        public JToken CurrentToken =>
            this._current;

        public JToken Token
        {
            get
            {
                if (this._token != null)
                {
                    return this._token;
                }
                return this._value;
            }
        }
    }
}

