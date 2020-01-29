namespace Newtonsoft.Json.Linq
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;

    public class JTokenReader : JsonReader, IJsonLineInfo
    {
        private readonly JToken _root;
        private string _initialPath;
        private JToken _parent;
        private JToken _current;

        public JTokenReader(JToken token)
        {
            ValidationUtils.ArgumentNotNull(token, "token");
            this._root = token;
        }

        internal JTokenReader(JToken token, string initialPath) : this(token)
        {
            this._initialPath = initialPath;
        }

        private JsonToken? GetEndToken(JContainer c)
        {
            switch (c.Type)
            {
                case JTokenType.Object:
                    return 13;

                case JTokenType.Array:
                    return 14;

                case JTokenType.Constructor:
                    return 15;

                case JTokenType.Property:
                    return null;
            }
            throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", c.Type, "Unexpected JContainer type.");
        }

        bool IJsonLineInfo.HasLineInfo()
        {
            if (base.CurrentState == JsonReader.State.Start)
            {
                return false;
            }
            IJsonLineInfo info = this._current;
            return ((info != null) && info.HasLineInfo());
        }

        public override bool Read()
        {
            if (base.CurrentState != JsonReader.State.Start)
            {
                if (this._current == null)
                {
                    return false;
                }
                JContainer c = this._current as JContainer;
                if ((c != null) && (this._parent != c))
                {
                    return this.ReadInto(c);
                }
                return this.ReadOver(this._current);
            }
            this._current = this._root;
            this.SetToken(this._current);
            return true;
        }

        private bool ReadInto(JContainer c)
        {
            JToken first = c.First;
            if (first == null)
            {
                return this.SetEnd(c);
            }
            this.SetToken(first);
            this._current = first;
            this._parent = c;
            return true;
        }

        private bool ReadOver(JToken t)
        {
            if (t == this._root)
            {
                return this.ReadToEnd();
            }
            JToken next = t.Next;
            if (((next == null) || (next == t)) || (t == t.Parent.Last))
            {
                if (t.Parent == null)
                {
                    return this.ReadToEnd();
                }
                return this.SetEnd(t.Parent);
            }
            this._current = next;
            this.SetToken(this._current);
            return true;
        }

        private bool ReadToEnd()
        {
            this._current = null;
            base.SetToken(JsonToken.None);
            return false;
        }

        private string SafeToString(object value) => 
            value?.ToString();

        private bool SetEnd(JContainer c)
        {
            JsonToken? endToken = this.GetEndToken(c);
            if (endToken.HasValue)
            {
                base.SetToken(endToken.GetValueOrDefault());
                this._current = c;
                this._parent = c;
                return true;
            }
            return this.ReadOver(c);
        }

        private void SetToken(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    base.SetToken(JsonToken.StartObject);
                    return;

                case JTokenType.Array:
                    base.SetToken(JsonToken.StartArray);
                    return;

                case JTokenType.Constructor:
                    base.SetToken(JsonToken.StartConstructor, ((JConstructor) token).Name);
                    return;

                case JTokenType.Property:
                    base.SetToken(JsonToken.PropertyName, ((JProperty) token).Name);
                    return;

                case JTokenType.Comment:
                    base.SetToken(JsonToken.Comment, ((JValue) token).Value);
                    return;

                case JTokenType.Integer:
                    base.SetToken(JsonToken.Integer, ((JValue) token).Value);
                    return;

                case JTokenType.Float:
                    base.SetToken(JsonToken.Float, ((JValue) token).Value);
                    return;

                case JTokenType.String:
                    base.SetToken(JsonToken.String, ((JValue) token).Value);
                    return;

                case JTokenType.Boolean:
                    base.SetToken(JsonToken.Boolean, ((JValue) token).Value);
                    return;

                case JTokenType.Null:
                    base.SetToken(JsonToken.Null, ((JValue) token).Value);
                    return;

                case JTokenType.Undefined:
                    base.SetToken(JsonToken.Undefined, ((JValue) token).Value);
                    return;

                case JTokenType.Date:
                    base.SetToken(JsonToken.Date, ((JValue) token).Value);
                    return;

                case JTokenType.Raw:
                    base.SetToken(JsonToken.Raw, ((JValue) token).Value);
                    return;

                case JTokenType.Bytes:
                    base.SetToken(JsonToken.Bytes, ((JValue) token).Value);
                    return;

                case JTokenType.Guid:
                    base.SetToken(JsonToken.String, this.SafeToString(((JValue) token).Value));
                    return;

                case JTokenType.Uri:
                {
                    object obj2 = ((JValue) token).Value;
                    if (!(obj2 is Uri))
                    {
                        base.SetToken(JsonToken.String, this.SafeToString(obj2));
                        return;
                    }
                    base.SetToken(JsonToken.String, ((Uri) obj2).OriginalString);
                    return;
                }
                case JTokenType.TimeSpan:
                    base.SetToken(JsonToken.String, this.SafeToString(((JValue) token).Value));
                    return;
            }
            throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", token.Type, "Unexpected JTokenType.");
        }

        public JToken CurrentToken =>
            this._current;

        int IJsonLineInfo.LineNumber
        {
            get
            {
                if (base.CurrentState != JsonReader.State.Start)
                {
                    IJsonLineInfo info = this._current;
                    if (info != null)
                    {
                        return info.LineNumber;
                    }
                }
                return 0;
            }
        }

        int IJsonLineInfo.LinePosition
        {
            get
            {
                if (base.CurrentState != JsonReader.State.Start)
                {
                    IJsonLineInfo info = this._current;
                    if (info != null)
                    {
                        return info.LinePosition;
                    }
                }
                return 0;
            }
        }

        public override string Path
        {
            get
            {
                string path = base.Path;
                if (this._initialPath == null)
                {
                    this._initialPath = this._root.Path;
                }
                if (string.IsNullOrEmpty(this._initialPath))
                {
                    return path;
                }
                if (string.IsNullOrEmpty(path))
                {
                    return this._initialPath;
                }
                if (path.StartsWith('['))
                {
                    return (this._initialPath + path);
                }
                return (this._initialPath + "." + path);
            }
        }
    }
}

