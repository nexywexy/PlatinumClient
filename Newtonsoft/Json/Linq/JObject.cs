namespace Newtonsoft.Json.Linq
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class JObject : JContainer, IDictionary<string, JToken>, ICollection<KeyValuePair<string, JToken>>, IEnumerable<KeyValuePair<string, JToken>>, IEnumerable, INotifyPropertyChanged, ICustomTypeDescriptor, INotifyPropertyChanging
    {
        private readonly JPropertyKeyedCollection _properties;

        [field: CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: CompilerGenerated]
        public event PropertyChangingEventHandler PropertyChanging;

        public JObject()
        {
            this._properties = new JPropertyKeyedCollection();
        }

        public JObject(JObject other) : base(other)
        {
            this._properties = new JPropertyKeyedCollection();
        }

        public JObject(params object[] content) : this(content)
        {
        }

        public JObject(object content)
        {
            this._properties = new JPropertyKeyedCollection();
            this.Add(content);
        }

        public void Add(string propertyName, JToken value)
        {
            this.Add(new JProperty(propertyName, value));
        }

        internal override JToken CloneToken() => 
            new JObject(this);

        internal override bool DeepEquals(JToken node)
        {
            JObject obj2 = node as JObject;
            if (obj2 == null)
            {
                return false;
            }
            return this._properties.Compare(obj2._properties);
        }

        public static JObject FromObject(object o) => 
            FromObject(o, JsonSerializer.CreateDefault());

        public static JObject FromObject(object o, JsonSerializer jsonSerializer)
        {
            JToken token = JToken.FromObjectInternal(o, jsonSerializer);
            if ((token != null) && (token.Type != JTokenType.Object))
            {
                throw new ArgumentException("Object serialized to {0}. JObject instance expected.".FormatWith(CultureInfo.InvariantCulture, token.Type));
            }
            return (JObject) token;
        }

        internal override int GetDeepHashCode() => 
            base.ContentsHashCode();

        [IteratorStateMachine(typeof(<GetEnumerator>d__58))]
        public IEnumerator<KeyValuePair<string, JToken>> GetEnumerator() => 
            new <GetEnumerator>d__58(0) { <>4__this = this };

        protected override DynamicMetaObject GetMetaObject(Expression parameter) => 
            new DynamicProxyMetaObject<JObject>(parameter, this, new JObjectDynamicProxy(), true);

        public JToken GetValue(string propertyName) => 
            this.GetValue(propertyName, StringComparison.Ordinal);

        public JToken GetValue(string propertyName, StringComparison comparison)
        {
            if (propertyName != null)
            {
                JProperty property = this.Property(propertyName);
                if (property != null)
                {
                    return property.Value;
                }
                if (comparison != StringComparison.Ordinal)
                {
                    foreach (JProperty property2 in this._properties)
                    {
                        if (string.Equals(property2.Name, propertyName, comparison))
                        {
                            return property2.Value;
                        }
                    }
                }
            }
            return null;
        }

        internal override int IndexOfItem(JToken item) => 
            this._properties.IndexOfReference(item);

        internal override void InsertItem(int index, JToken item, bool skipParentCheck)
        {
            if ((item == null) || (item.Type != JTokenType.Comment))
            {
                base.InsertItem(index, item, skipParentCheck);
            }
        }

        internal void InternalPropertyChanged(JProperty childProperty)
        {
            this.OnPropertyChanged(childProperty.Name);
            if (base._listChanged != null)
            {
                this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, this.IndexOfItem(childProperty)));
            }
            if (base._collectionChanged != null)
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, childProperty, childProperty, this.IndexOfItem(childProperty)));
            }
        }

        internal void InternalPropertyChanging(JProperty childProperty)
        {
            this.OnPropertyChanging(childProperty.Name);
        }

        public static JObject Load(JsonReader reader) => 
            Load(reader, null);

        public static JObject Load(JsonReader reader, JsonLoadSettings settings)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            if ((reader.TokenType == JsonToken.None) && !reader.Read())
            {
                throw JsonReaderException.Create(reader, "Error reading JObject from JsonReader.");
            }
            reader.MoveToContent();
            if (reader.TokenType != JsonToken.StartObject)
            {
                throw JsonReaderException.Create(reader, "Error reading JObject from JsonReader. Current JsonReader item is not an object: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
            JObject obj1 = new JObject();
            obj1.SetLineInfo(reader as IJsonLineInfo, settings);
            obj1.ReadTokenFrom(reader, settings);
            return obj1;
        }

        internal override void MergeItem(object content, JsonMergeSettings settings)
        {
            JObject obj2 = content as JObject;
            if (obj2 != null)
            {
                foreach (KeyValuePair<string, JToken> pair in obj2)
                {
                    JProperty property = this.Property(pair.Key);
                    if (property == null)
                    {
                        this.Add(pair.Key, pair.Value);
                    }
                    else if (pair.Value != null)
                    {
                        JContainer container = property.Value as JContainer;
                        if (container == null)
                        {
                            if ((pair.Value.Type != JTokenType.Null) || ((settings != null) && (settings.MergeNullValueHandling == MergeNullValueHandling.Merge)))
                            {
                                property.Value = pair.Value;
                            }
                        }
                        else if (container.Type != pair.Value.Type)
                        {
                            property.Value = pair.Value;
                        }
                        else
                        {
                            container.Merge(pair.Value, settings);
                        }
                    }
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        public static JObject Parse(string json) => 
            Parse(json, null);

        public static JObject Parse(string json, JsonLoadSettings settings)
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

        public IEnumerable<JProperty> Properties() => 
            this._properties.Cast<JProperty>();

        public JProperty Property(string name)
        {
            if (name == null)
            {
                return null;
            }
            this._properties.TryGetValue(name, out JToken token);
            return (JProperty) token;
        }

        public JEnumerable<JToken> PropertyValues()
        {
            if (<>c.<>9__25_0 == null)
            {
            }
            return new JEnumerable<JToken>(this.Properties().Select<JProperty, JToken>(<>c.<>9__25_0 = new Func<JProperty, JToken>(<>c.<>9.<PropertyValues>b__25_0)));
        }

        public bool Remove(string propertyName)
        {
            JProperty property = this.Property(propertyName);
            if (property == null)
            {
                return false;
            }
            property.Remove();
            return true;
        }

        void ICollection<KeyValuePair<string, JToken>>.Add(KeyValuePair<string, JToken> item)
        {
            this.Add(new JProperty(item.Key, item.Value));
        }

        void ICollection<KeyValuePair<string, JToken>>.Clear()
        {
            base.RemoveAll();
        }

        bool ICollection<KeyValuePair<string, JToken>>.Contains(KeyValuePair<string, JToken> item)
        {
            JProperty property = this.Property(item.Key);
            return (property?.Value == item.Value);
        }

        void ICollection<KeyValuePair<string, JToken>>.CopyTo(KeyValuePair<string, JToken>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
            }
            if ((arrayIndex >= array.Length) && (arrayIndex != 0))
            {
                throw new ArgumentException("arrayIndex is equal to or greater than the length of array.");
            }
            if (base.Count > (array.Length - arrayIndex))
            {
                throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
            }
            int num = 0;
            foreach (JProperty property in this._properties)
            {
                array[arrayIndex + num] = new KeyValuePair<string, JToken>(property.Name, property.Value);
                num++;
            }
        }

        bool ICollection<KeyValuePair<string, JToken>>.Remove(KeyValuePair<string, JToken> item)
        {
            if (!((ICollection<KeyValuePair<string, JToken>>) this).Contains(item))
            {
                return false;
            }
            this.Remove(item.Key);
            return true;
        }

        bool IDictionary<string, JToken>.ContainsKey(string key) => 
            this._properties.Contains(key);

        AttributeCollection ICustomTypeDescriptor.GetAttributes() => 
            AttributeCollection.Empty;

        string ICustomTypeDescriptor.GetClassName() => 
            null;

        string ICustomTypeDescriptor.GetComponentName() => 
            null;

        TypeConverter ICustomTypeDescriptor.GetConverter() => 
            new TypeConverter();

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => 
            null;

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => 
            null;

        object ICustomTypeDescriptor.GetEditor(System.Type editorBaseType) => 
            null;

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => 
            EventDescriptorCollection.Empty;

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) => 
            EventDescriptorCollection.Empty;

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => 
            ((ICustomTypeDescriptor) this).GetProperties(null);

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection descriptors = new PropertyDescriptorCollection(null);
            foreach (KeyValuePair<string, JToken> pair in this)
            {
                descriptors.Add(new JPropertyDescriptor(pair.Key));
            }
            return descriptors;
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => 
            null;

        public bool TryGetValue(string propertyName, out JToken value)
        {
            JProperty property = this.Property(propertyName);
            if (property == null)
            {
                value = null;
                return false;
            }
            value = property.Value;
            return true;
        }

        public bool TryGetValue(string propertyName, StringComparison comparison, out JToken value)
        {
            value = this.GetValue(propertyName, comparison);
            return (value > null);
        }

        internal override void ValidateToken(JToken o, JToken existing)
        {
            ValidationUtils.ArgumentNotNull(o, "o");
            if (o.Type != JTokenType.Property)
            {
                throw new ArgumentException("Can not add {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, o.GetType(), base.GetType()));
            }
            JProperty property = (JProperty) o;
            if (existing != null)
            {
                JProperty property2 = (JProperty) existing;
                if (property.Name == property2.Name)
                {
                    return;
                }
            }
            if (this._properties.TryGetValue(property.Name, out existing))
            {
                throw new ArgumentException("Can not add property {0} to {1}. Property with the same name already exists on object.".FormatWith(CultureInfo.InvariantCulture, property.Name, base.GetType()));
            }
        }

        public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        {
            writer.WriteStartObject();
            for (int i = 0; i < this._properties.Count; i++)
            {
                this._properties[i].WriteTo(writer, converters);
            }
            writer.WriteEndObject();
        }

        protected override IList<JToken> ChildrenTokens =>
            this._properties;

        public override JTokenType Type =>
            JTokenType.Object;

        public override JToken this[object key]
        {
            get
            {
                ValidationUtils.ArgumentNotNull(key, "key");
                string str = key as string;
                if (str == null)
                {
                    throw new ArgumentException("Accessed JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
                }
                return this[str];
            }
            set
            {
                ValidationUtils.ArgumentNotNull(key, "key");
                string str = key as string;
                if (str == null)
                {
                    throw new ArgumentException("Set JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
                }
                this[str] = value;
            }
        }

        public JToken this[string propertyName]
        {
            get
            {
                ValidationUtils.ArgumentNotNull(propertyName, "propertyName");
                JProperty property = this.Property(propertyName);
                return property?.Value;
            }
            set
            {
                JProperty property = this.Property(propertyName);
                if (property != null)
                {
                    property.Value = value;
                }
                else
                {
                    this.OnPropertyChanging(propertyName);
                    this.Add(new JProperty(propertyName, value));
                    this.OnPropertyChanged(propertyName);
                }
            }
        }

        ICollection<string> IDictionary<string, JToken>.Keys =>
            this._properties.Keys;

        ICollection<JToken> IDictionary<string, JToken>.Values
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ICollection<KeyValuePair<string, JToken>>.IsReadOnly =>
            false;

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly JObject.<>c <>9 = new JObject.<>c();
            public static Func<JProperty, JToken> <>9__25_0;

            internal JToken <PropertyValues>b__25_0(JProperty p) => 
                p.Value;
        }

        [CompilerGenerated]
        private sealed class <GetEnumerator>d__58 : IEnumerator<KeyValuePair<string, JToken>>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private KeyValuePair<string, JToken> <>2__current;
            public JObject <>4__this;
            private IEnumerator<JToken> <>7__wrap1;

            [DebuggerHidden]
            public <GetEnumerator>d__58(int <>1__state)
            {
                this.<>1__state = <>1__state;
            }

            private void <>m__Finally1()
            {
                this.<>1__state = -1;
                if (this.<>7__wrap1 != null)
                {
                    this.<>7__wrap1.Dispose();
                }
            }

            private bool MoveNext()
            {
                try
                {
                    int num = this.<>1__state;
                    if (num == 0)
                    {
                        this.<>1__state = -1;
                        this.<>7__wrap1 = this.<>4__this._properties.GetEnumerator();
                        this.<>1__state = -3;
                        while (this.<>7__wrap1.MoveNext())
                        {
                            JProperty current = (JProperty) this.<>7__wrap1.Current;
                            this.<>2__current = new KeyValuePair<string, JToken>(current.Name, current.Value);
                            this.<>1__state = 1;
                            return true;
                        Label_007B:
                            this.<>1__state = -3;
                        }
                        this.<>m__Finally1();
                        this.<>7__wrap1 = null;
                        return false;
                    }
                    if (num != 1)
                    {
                        return false;
                    }
                    goto Label_007B;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
                switch (this.<>1__state)
                {
                    case -3:
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            this.<>m__Finally1();
                        }
                        break;
                }
            }

            KeyValuePair<string, JToken> IEnumerator<KeyValuePair<string, JToken>>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }

        private class JObjectDynamicProxy : DynamicProxy<JObject>
        {
            public override IEnumerable<string> GetDynamicMemberNames(JObject instance)
            {
                if (<>c.<>9__2_0 == null)
                {
                }
                return instance.Properties().Select<JProperty, string>((<>c.<>9__2_0 = new Func<JProperty, string>(<>c.<>9.<GetDynamicMemberNames>b__2_0)));
            }

            public override bool TryGetMember(JObject instance, GetMemberBinder binder, out object result)
            {
                result = instance[binder.Name];
                return true;
            }

            public override bool TrySetMember(JObject instance, SetMemberBinder binder, object value)
            {
                JToken token = value as JToken;
                if (token == null)
                {
                    token = new JValue(value);
                }
                instance[binder.Name] = token;
                return true;
            }

            [Serializable, CompilerGenerated]
            private sealed class <>c
            {
                public static readonly JObject.JObjectDynamicProxy.<>c <>9 = new JObject.JObjectDynamicProxy.<>c();
                public static Func<JProperty, string> <>9__2_0;

                internal string <GetDynamicMemberNames>b__2_0(JProperty p) => 
                    p.Name;
            }
        }
    }
}

