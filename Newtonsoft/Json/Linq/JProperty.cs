namespace Newtonsoft.Json.Linq
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class JProperty : JContainer
    {
        private readonly JPropertyList _content;
        private readonly string _name;

        public JProperty(JProperty other) : base(other)
        {
            this._content = new JPropertyList();
            this._name = other.Name;
        }

        internal JProperty(string name)
        {
            this._content = new JPropertyList();
            ValidationUtils.ArgumentNotNull(name, "name");
            this._name = name;
        }

        public JProperty(string name, params object[] content) : this(name, content)
        {
        }

        public JProperty(string name, object content)
        {
            this._content = new JPropertyList();
            ValidationUtils.ArgumentNotNull(name, "name");
            this._name = name;
            this.Value = base.IsMultiContent(content) ? new JArray(content) : JContainer.CreateFromContent(content);
        }

        internal override void ClearItems()
        {
            throw new JsonException("Cannot add or remove items from {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
        }

        internal override JToken CloneToken() => 
            new JProperty(this);

        internal override bool ContainsItem(JToken item) => 
            (this.Value == item);

        internal override bool DeepEquals(JToken node)
        {
            JProperty container = node as JProperty;
            return (((container != null) && (this._name == container.Name)) && base.ContentsEqual(container));
        }

        internal override int GetDeepHashCode() => 
            (this._name.GetHashCode() ^ ((this.Value != null) ? this.Value.GetDeepHashCode() : 0));

        internal override JToken GetItem(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            return this.Value;
        }

        internal override int IndexOfItem(JToken item) => 
            this._content.IndexOf(item);

        internal override void InsertItem(int index, JToken item, bool skipParentCheck)
        {
            if ((item == null) || (item.Type != JTokenType.Comment))
            {
                if (this.Value != null)
                {
                    throw new JsonException("{0} cannot have multiple values.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
                }
                base.InsertItem(0, item, false);
            }
        }

        public static JProperty Load(JsonReader reader) => 
            Load(reader, null);

        public static JProperty Load(JsonReader reader, JsonLoadSettings settings)
        {
            if ((reader.TokenType == JsonToken.None) && !reader.Read())
            {
                throw JsonReaderException.Create(reader, "Error reading JProperty from JsonReader.");
            }
            reader.MoveToContent();
            if (reader.TokenType != JsonToken.PropertyName)
            {
                throw JsonReaderException.Create(reader, "Error reading JProperty from JsonReader. Current JsonReader item is not a property: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
            JProperty property1 = new JProperty((string) reader.Value);
            property1.SetLineInfo(reader as IJsonLineInfo, settings);
            property1.ReadTokenFrom(reader, settings);
            return property1;
        }

        internal override void MergeItem(object content, JsonMergeSettings settings)
        {
            JProperty property = content as JProperty;
            if ((property != null) && ((property.Value != null) && (property.Value.Type != JTokenType.Null)))
            {
                this.Value = property.Value;
            }
        }

        internal override bool RemoveItem(JToken item)
        {
            throw new JsonException("Cannot add or remove items from {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
        }

        internal override void RemoveItemAt(int index)
        {
            throw new JsonException("Cannot add or remove items from {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
        }

        internal override void SetItem(int index, JToken item)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (!JContainer.IsTokenUnchanged(this.Value, item))
            {
                if (base.Parent != null)
                {
                    ((JObject) base.Parent).InternalPropertyChanging(this);
                }
                base.SetItem(0, item);
                if (base.Parent != null)
                {
                    ((JObject) base.Parent).InternalPropertyChanged(this);
                }
            }
        }

        public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        {
            writer.WritePropertyName(this._name);
            JToken token = this.Value;
            if (token != null)
            {
                token.WriteTo(writer, converters);
            }
            else
            {
                writer.WriteNull();
            }
        }

        protected override IList<JToken> ChildrenTokens =>
            this._content;

        public string Name =>
            this._name;

        public JToken Value
        {
            [DebuggerStepThrough]
            get => 
                this._content._token;
            set
            {
                JToken token;
                base.CheckReentrancy();
                if (value != null)
                {
                    token = value;
                }
                else
                {
                    token = JValue.CreateNull();
                }
                if (this._content._token == null)
                {
                    this.InsertItem(0, token, false);
                }
                else
                {
                    this.SetItem(0, token);
                }
            }
        }

        public override JTokenType Type =>
            JTokenType.Property;

        private class JPropertyList : IList<JToken>, ICollection<JToken>, IEnumerable<JToken>, IEnumerable
        {
            internal JToken _token;

            public void Add(JToken item)
            {
                this._token = item;
            }

            public void Clear()
            {
                this._token = null;
            }

            public bool Contains(JToken item) => 
                (this._token == item);

            public void CopyTo(JToken[] array, int arrayIndex)
            {
                if (this._token != null)
                {
                    array[arrayIndex] = this._token;
                }
            }

            [IteratorStateMachine(typeof(<GetEnumerator>d__1))]
            public IEnumerator<JToken> GetEnumerator() => 
                new <GetEnumerator>d__1(0) { <>4__this = this };

            public int IndexOf(JToken item)
            {
                if (this._token != item)
                {
                    return -1;
                }
                return 0;
            }

            public void Insert(int index, JToken item)
            {
                if (index == 0)
                {
                    this._token = item;
                }
            }

            public bool Remove(JToken item)
            {
                if (this._token == item)
                {
                    this._token = null;
                    return true;
                }
                return false;
            }

            public void RemoveAt(int index)
            {
                if (index == 0)
                {
                    this._token = null;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => 
                this.GetEnumerator();

            public int Count
            {
                get
                {
                    if (this._token == null)
                    {
                        return 0;
                    }
                    return 1;
                }
            }

            public bool IsReadOnly =>
                false;

            public JToken this[int index]
            {
                get
                {
                    if (index != 0)
                    {
                        return null;
                    }
                    return this._token;
                }
                set
                {
                    if (index == 0)
                    {
                        this._token = value;
                    }
                }
            }

            [CompilerGenerated]
            private sealed class <GetEnumerator>d__1 : IEnumerator<JToken>, IDisposable, IEnumerator
            {
                private int <>1__state;
                private JToken <>2__current;
                public JProperty.JPropertyList <>4__this;

                [DebuggerHidden]
                public <GetEnumerator>d__1(int <>1__state)
                {
                    this.<>1__state = <>1__state;
                }

                private bool MoveNext()
                {
                    int num = this.<>1__state;
                    if (num == 0)
                    {
                        this.<>1__state = -1;
                        if (this.<>4__this._token != null)
                        {
                            this.<>2__current = this.<>4__this._token;
                            this.<>1__state = 1;
                            return true;
                        }
                    }
                    else
                    {
                        if (num != 1)
                        {
                            return false;
                        }
                        this.<>1__state = -1;
                    }
                    return false;
                }

                [DebuggerHidden]
                void IEnumerator.Reset()
                {
                    throw new NotSupportedException();
                }

                [DebuggerHidden]
                void IDisposable.Dispose()
                {
                }

                JToken IEnumerator<JToken>.Current =>
                    this.<>2__current;

                object IEnumerator.Current =>
                    this.<>2__current;
            }
        }
    }
}

