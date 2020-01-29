namespace Newtonsoft.Json.Linq
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;

    public class JArray : JContainer, IList<JToken>, ICollection<JToken>, IEnumerable<JToken>, IEnumerable
    {
        private readonly List<JToken> _values;

        public JArray()
        {
            this._values = new List<JToken>();
        }

        public JArray(JArray other) : base(other)
        {
            this._values = new List<JToken>();
        }

        public JArray(params object[] content) : this(content)
        {
        }

        public JArray(object content)
        {
            this._values = new List<JToken>();
            this.Add(content);
        }

        public void Add(JToken item)
        {
            this.Add(item);
        }

        public void Clear()
        {
            this.ClearItems();
        }

        internal override JToken CloneToken() => 
            new JArray(this);

        public bool Contains(JToken item) => 
            this.ContainsItem(item);

        public void CopyTo(JToken[] array, int arrayIndex)
        {
            this.CopyItemsTo(array, arrayIndex);
        }

        internal override bool DeepEquals(JToken node)
        {
            JArray container = node as JArray;
            return ((container != null) && base.ContentsEqual(container));
        }

        public static JArray FromObject(object o) => 
            FromObject(o, JsonSerializer.CreateDefault());

        public static JArray FromObject(object o, JsonSerializer jsonSerializer)
        {
            JToken token = JToken.FromObjectInternal(o, jsonSerializer);
            if (token.Type != JTokenType.Array)
            {
                throw new ArgumentException("Object serialized to {0}. JArray instance expected.".FormatWith(CultureInfo.InvariantCulture, token.Type));
            }
            return (JArray) token;
        }

        internal override int GetDeepHashCode() => 
            base.ContentsHashCode();

        public IEnumerator<JToken> GetEnumerator() => 
            this.Children().GetEnumerator();

        public int IndexOf(JToken item) => 
            this.IndexOfItem(item);

        internal override int IndexOfItem(JToken item) => 
            this._values.IndexOfReference<JToken>(item);

        public void Insert(int index, JToken item)
        {
            this.InsertItem(index, item, false);
        }

        public static JArray Load(JsonReader reader) => 
            Load(reader, null);

        public static JArray Load(JsonReader reader, JsonLoadSettings settings)
        {
            if ((reader.TokenType == JsonToken.None) && !reader.Read())
            {
                throw JsonReaderException.Create(reader, "Error reading JArray from JsonReader.");
            }
            reader.MoveToContent();
            if (reader.TokenType != JsonToken.StartArray)
            {
                throw JsonReaderException.Create(reader, "Error reading JArray from JsonReader. Current JsonReader item is not an array: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
            JArray array1 = new JArray();
            array1.SetLineInfo(reader as IJsonLineInfo, settings);
            array1.ReadTokenFrom(reader, settings);
            return array1;
        }

        internal override void MergeItem(object content, JsonMergeSettings settings)
        {
            IEnumerable enumerable = (base.IsMultiContent(content) || (content is JArray)) ? ((IEnumerable) content) : null;
            if (enumerable != null)
            {
                JContainer.MergeEnumerableContent(this, enumerable, settings);
            }
        }

        public static JArray Parse(string json) => 
            Parse(json, null);

        public static JArray Parse(string json, JsonLoadSettings settings)
        {
            using (JsonReader reader = new JsonTextReader(new StringReader(json)))
            {
                if (reader.Read() && (reader.TokenType != JsonToken.Comment))
                {
                    throw JsonReaderException.Create(reader, "Additional text found in JSON string after parsing content.");
                }
                return Load(reader, settings);
            }
        }

        public bool Remove(JToken item) => 
            this.RemoveItem(item);

        public void RemoveAt(int index)
        {
            this.RemoveItemAt(index);
        }

        public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        {
            writer.WriteStartArray();
            for (int i = 0; i < this._values.Count; i++)
            {
                this._values[i].WriteTo(writer, converters);
            }
            writer.WriteEndArray();
        }

        protected override IList<JToken> ChildrenTokens =>
            this._values;

        public override JTokenType Type =>
            JTokenType.Array;

        public override JToken this[object key]
        {
            get
            {
                ValidationUtils.ArgumentNotNull(key, "key");
                if (!(key is int))
                {
                    throw new ArgumentException("Accessed JArray values with invalid key value: {0}. Int32 array index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
                }
                return this.GetItem((int) key);
            }
            set
            {
                ValidationUtils.ArgumentNotNull(key, "key");
                if (!(key is int))
                {
                    throw new ArgumentException("Set JArray values with invalid key value: {0}. Int32 array index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
                }
                this.SetItem((int) key, value);
            }
        }

        public JToken this[int index]
        {
            get => 
                this.GetItem(index);
            set => 
                this.SetItem(index, value);
        }

        public bool IsReadOnly =>
            false;
    }
}

