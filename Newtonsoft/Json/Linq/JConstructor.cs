namespace Newtonsoft.Json.Linq
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;

    public class JConstructor : JContainer
    {
        private string _name;
        private readonly List<JToken> _values;

        public JConstructor()
        {
            this._values = new List<JToken>();
        }

        public JConstructor(JConstructor other) : base(other)
        {
            this._values = new List<JToken>();
            this._name = other.Name;
        }

        public JConstructor(string name)
        {
            this._values = new List<JToken>();
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("Constructor name cannot be empty.", "name");
            }
            this._name = name;
        }

        public JConstructor(string name, params object[] content) : this(name, content)
        {
        }

        public JConstructor(string name, object content) : this(name)
        {
            this.Add(content);
        }

        internal override JToken CloneToken() => 
            new JConstructor(this);

        internal override bool DeepEquals(JToken node)
        {
            JConstructor container = node as JConstructor;
            return (((container != null) && (this._name == container.Name)) && base.ContentsEqual(container));
        }

        internal override int GetDeepHashCode() => 
            (this._name.GetHashCode() ^ base.ContentsHashCode());

        internal override int IndexOfItem(JToken item) => 
            this._values.IndexOfReference<JToken>(item);

        public static JConstructor Load(JsonReader reader) => 
            Load(reader, null);

        public static JConstructor Load(JsonReader reader, JsonLoadSettings settings)
        {
            if ((reader.TokenType == JsonToken.None) && !reader.Read())
            {
                throw JsonReaderException.Create(reader, "Error reading JConstructor from JsonReader.");
            }
            reader.MoveToContent();
            if (reader.TokenType != JsonToken.StartConstructor)
            {
                throw JsonReaderException.Create(reader, "Error reading JConstructor from JsonReader. Current JsonReader item is not a constructor: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
            JConstructor constructor1 = new JConstructor((string) reader.Value);
            constructor1.SetLineInfo(reader as IJsonLineInfo, settings);
            constructor1.ReadTokenFrom(reader, settings);
            return constructor1;
        }

        internal override void MergeItem(object content, JsonMergeSettings settings)
        {
            JConstructor constructor = content as JConstructor;
            if (constructor != null)
            {
                if (constructor.Name != null)
                {
                    this.Name = constructor.Name;
                }
                JContainer.MergeEnumerableContent(this, constructor, settings);
            }
        }

        public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        {
            writer.WriteStartConstructor(this._name);
            using (IEnumerator<JToken> enumerator = this.Children().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.WriteTo(writer, converters);
                }
            }
            writer.WriteEndConstructor();
        }

        protected override IList<JToken> ChildrenTokens =>
            this._values;

        public string Name
        {
            get => 
                this._name;
            set => 
                (this._name = value);
        }

        public override JTokenType Type =>
            JTokenType.Constructor;

        public override JToken this[object key]
        {
            get
            {
                ValidationUtils.ArgumentNotNull(key, "key");
                if (!(key is int))
                {
                    throw new ArgumentException("Accessed JConstructor values with invalid key value: {0}. Argument position index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
                }
                return this.GetItem((int) key);
            }
            set
            {
                ValidationUtils.ArgumentNotNull(key, "key");
                if (!(key is int))
                {
                    throw new ArgumentException("Set JConstructor values with invalid key value: {0}. Argument position index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
                }
                this.SetItem((int) key, value);
            }
        }
    }
}

