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
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public abstract class JContainer : JToken, IList<JToken>, ICollection<JToken>, IEnumerable<JToken>, IEnumerable, ITypedList, IBindingList, IList, ICollection, INotifyCollectionChanged
    {
        private object _syncRoot;
        private bool _busy;

        public event AddingNewEventHandler AddingNew;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event ListChangedEventHandler ListChanged;

        internal JContainer()
        {
        }

        internal JContainer(JContainer other) : this()
        {
            ValidationUtils.ArgumentNotNull(other, "other");
            int index = 0;
            foreach (JToken token in (IEnumerable<JToken>) other)
            {
                this.AddInternal(index, token, false);
                index++;
            }
        }

        public virtual void Add(object content)
        {
            this.AddInternal(this.ChildrenTokens.Count, content, false);
        }

        internal void AddAndSkipParentCheck(JToken token)
        {
            this.AddInternal(this.ChildrenTokens.Count, token, true);
        }

        public void AddFirst(object content)
        {
            this.AddInternal(0, content, false);
        }

        internal void AddInternal(int index, object content, bool skipParentCheck)
        {
            if (this.IsMultiContent(content))
            {
                int num = index;
                foreach (object obj2 in (IEnumerable) content)
                {
                    this.AddInternal(num, obj2, skipParentCheck);
                    num++;
                }
            }
            else
            {
                JToken item = CreateFromContent(content);
                this.InsertItem(index, item, skipParentCheck);
            }
        }

        internal void CheckReentrancy()
        {
            if (this._busy)
            {
                throw new InvalidOperationException("Cannot change {0} during a collection change event.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
            }
        }

        public override JEnumerable<JToken> Children() => 
            new JEnumerable<JToken>(this.ChildrenTokens);

        internal virtual void ClearItems()
        {
            this.CheckReentrancy();
            IList<JToken> childrenTokens = this.ChildrenTokens;
            foreach (JToken local1 in childrenTokens)
            {
                local1.Parent = null;
                local1.Previous = null;
                local1.Next = null;
            }
            childrenTokens.Clear();
            if (this._listChanged != null)
            {
                this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
            if (this._collectionChanged != null)
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        internal virtual bool ContainsItem(JToken item) => 
            (this.IndexOfItem(item) != -1);

        internal bool ContentsEqual(JContainer container)
        {
            if (container != this)
            {
                IList<JToken> childrenTokens = this.ChildrenTokens;
                IList<JToken> list2 = container.ChildrenTokens;
                if (childrenTokens.Count != list2.Count)
                {
                    return false;
                }
                for (int i = 0; i < childrenTokens.Count; i++)
                {
                    if (!childrenTokens[i].DeepEquals(list2[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal int ContentsHashCode()
        {
            int num = 0;
            foreach (JToken token in this.ChildrenTokens)
            {
                num ^= token.GetDeepHashCode();
            }
            return num;
        }

        internal virtual void CopyItemsTo(Array array, int arrayIndex)
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
            if (this.Count > (array.Length - arrayIndex))
            {
                throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
            }
            int num = 0;
            foreach (JToken token in this.ChildrenTokens)
            {
                array.SetValue(token, (int) (arrayIndex + num));
                num++;
            }
        }

        internal virtual IList<JToken> CreateChildrenCollection() => 
            new List<JToken>();

        internal static JToken CreateFromContent(object content)
        {
            JToken token = content as JToken;
            if (token != null)
            {
                return token;
            }
            return new JValue(content);
        }

        public JsonWriter CreateWriter() => 
            new JTokenWriter(this);

        public IEnumerable<JToken> Descendants() => 
            this.GetDescendants(false);

        public IEnumerable<JToken> DescendantsAndSelf() => 
            this.GetDescendants(true);

        internal JToken EnsureParentToken(JToken item, bool skipParentCheck)
        {
            if (item == null)
            {
                return JValue.CreateNull();
            }
            if (!skipParentCheck)
            {
                if (((item.Parent == null) && (item != this)) && (!item.HasValues || (base.Root != item)))
                {
                    return item;
                }
                item = item.CloneToken();
            }
            return item;
        }

        private JToken EnsureValue(object value)
        {
            if (value == null)
            {
                return null;
            }
            JToken token = value as JToken;
            if (token == null)
            {
                throw new ArgumentException("Argument is not a JToken.");
            }
            return token;
        }

        [IteratorStateMachine(typeof(<GetDescendants>d__34))]
        internal IEnumerable<JToken> GetDescendants(bool self)
        {
            if (self)
            {
                yield return this;
            }
            IEnumerator<JToken> enumerator = this.ChildrenTokens.GetEnumerator();
            while (enumerator.MoveNext())
            {
                JToken current = enumerator.Current;
                yield return current;
                JContainer container = current as JContainer;
                if (container != null)
                {
                    foreach (JToken token in container.Descendants())
                    {
                        yield return token;
                    }
                }
                current = null;
            }
            enumerator = null;
        }

        internal virtual JToken GetItem(int index) => 
            this.ChildrenTokens[index];

        internal abstract int IndexOfItem(JToken item);
        internal virtual void InsertItem(int index, JToken item, bool skipParentCheck)
        {
            IList<JToken> childrenTokens = this.ChildrenTokens;
            if (index > childrenTokens.Count)
            {
                throw new ArgumentOutOfRangeException("index", "Index must be within the bounds of the List.");
            }
            this.CheckReentrancy();
            item = this.EnsureParentToken(item, skipParentCheck);
            JToken token = (index == 0) ? null : childrenTokens[index - 1];
            JToken token2 = (index == childrenTokens.Count) ? null : childrenTokens[index];
            this.ValidateToken(item, null);
            item.Parent = this;
            item.Previous = token;
            if (token != null)
            {
                token.Next = item;
            }
            item.Next = token2;
            if (token2 != null)
            {
                token2.Previous = item;
            }
            childrenTokens.Insert(index, item);
            if (this._listChanged != null)
            {
                this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
            }
            if (this._collectionChanged != null)
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
        }

        internal bool IsMultiContent(object content) => 
            ((((content is IEnumerable) && !(content is string)) && !(content is JToken)) && !(content is byte[]));

        internal static bool IsTokenUnchanged(JToken currentValue, JToken newValue)
        {
            JValue value2 = currentValue as JValue;
            return (((value2?.Type == JTokenType.Null) && (newValue == null)) || value2.Equals(newValue));
        }

        public void Merge(object content)
        {
            this.MergeItem(content, new JsonMergeSettings());
        }

        public void Merge(object content, JsonMergeSettings settings)
        {
            this.MergeItem(content, settings);
        }

        internal static void MergeEnumerableContent(JContainer target, IEnumerable content, JsonMergeSettings settings)
        {
            switch (settings.MergeArrayHandling)
            {
                case MergeArrayHandling.Concat:
                    foreach (JToken token in content)
                    {
                        target.Add(token);
                    }
                    break;

                case MergeArrayHandling.Union:
                {
                    HashSet<JToken> set = new HashSet<JToken>(target, JToken.EqualityComparer);
                    foreach (JToken token2 in content)
                    {
                        if (set.Add(token2))
                        {
                            target.Add(token2);
                        }
                    }
                    break;
                }
                case MergeArrayHandling.Replace:
                    target.ClearItems();
                    foreach (JToken token3 in content)
                    {
                        target.Add(token3);
                    }
                    break;

                case MergeArrayHandling.Merge:
                {
                    int num = 0;
                    foreach (object obj2 in content)
                    {
                        if (num < target.Count)
                        {
                            JContainer container = target[num] as JContainer;
                            if (container != null)
                            {
                                container.Merge(obj2, settings);
                            }
                            else if (obj2 != null)
                            {
                                JToken token4 = CreateFromContent(obj2);
                                if (token4.Type != JTokenType.Null)
                                {
                                    target[num] = token4;
                                }
                            }
                        }
                        else
                        {
                            target.Add(obj2);
                        }
                        num++;
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException("settings", "Unexpected merge array handling when merging JSON.");
            }
        }

        internal abstract void MergeItem(object content, JsonMergeSettings settings);
        protected virtual void OnAddingNew(AddingNewEventArgs e)
        {
            AddingNewEventHandler handler = this._addingNew;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handler = this._collectionChanged;
            if (handler != null)
            {
                this._busy = true;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    this._busy = false;
                }
            }
        }

        protected virtual void OnListChanged(ListChangedEventArgs e)
        {
            ListChangedEventHandler handler = this._listChanged;
            if (handler != null)
            {
                this._busy = true;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    this._busy = false;
                }
            }
        }

        internal void ReadContentFrom(JsonReader r, JsonLoadSettings settings)
        {
            JProperty property1;
            JValue value2;
            JProperty property;
            ValidationUtils.ArgumentNotNull(r, "r");
            IJsonLineInfo lineInfo = r as IJsonLineInfo;
            JContainer parent = this;
        Label_0014:
            property1 = parent as JProperty;
            if (property1 == null)
            {
            }
            if (null.Value != null)
            {
                if (parent == this)
                {
                    return;
                }
                parent = parent.Parent;
            }
            switch (r.TokenType)
            {
                case JsonToken.None:
                    goto Label_0247;

                case JsonToken.StartObject:
                {
                    JObject content = new JObject();
                    content.SetLineInfo(lineInfo, settings);
                    parent.Add(content);
                    parent = content;
                    goto Label_0247;
                }
                case JsonToken.StartArray:
                {
                    JArray content = new JArray();
                    content.SetLineInfo(lineInfo, settings);
                    parent.Add(content);
                    parent = content;
                    goto Label_0247;
                }
                case JsonToken.StartConstructor:
                {
                    JConstructor content = new JConstructor(r.Value.ToString());
                    content.SetLineInfo(lineInfo, settings);
                    parent.Add(content);
                    parent = content;
                    goto Label_0247;
                }
                case JsonToken.PropertyName:
                {
                    string name = r.Value.ToString();
                    property = new JProperty(name);
                    property.SetLineInfo(lineInfo, settings);
                    JProperty property2 = ((JObject) parent).Property(name);
                    if (property2 != null)
                    {
                        property2.Replace(property);
                        break;
                    }
                    parent.Add(property);
                    break;
                }
                case JsonToken.Comment:
                    if ((settings != null) && (settings.CommentHandling == CommentHandling.Load))
                    {
                        value2 = JValue.CreateComment(r.Value.ToString());
                        value2.SetLineInfo(lineInfo, settings);
                        parent.Add(value2);
                    }
                    goto Label_0247;

                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    value2 = new JValue(r.Value);
                    value2.SetLineInfo(lineInfo, settings);
                    parent.Add(value2);
                    goto Label_0247;

                case JsonToken.Null:
                    value2 = JValue.CreateNull();
                    value2.SetLineInfo(lineInfo, settings);
                    parent.Add(value2);
                    goto Label_0247;

                case JsonToken.Undefined:
                    value2 = JValue.CreateUndefined();
                    value2.SetLineInfo(lineInfo, settings);
                    parent.Add(value2);
                    goto Label_0247;

                case JsonToken.EndObject:
                    if (parent != this)
                    {
                        parent = parent.Parent;
                        goto Label_0247;
                    }
                    return;

                case JsonToken.EndArray:
                    if (parent != this)
                    {
                        parent = parent.Parent;
                        goto Label_0247;
                    }
                    return;

                case JsonToken.EndConstructor:
                    if (parent != this)
                    {
                        parent = parent.Parent;
                        goto Label_0247;
                    }
                    return;

                default:
                    throw new InvalidOperationException("The JsonReader should not be on a token of type {0}.".FormatWith(CultureInfo.InvariantCulture, r.TokenType));
            }
            parent = property;
        Label_0247:
            if (r.Read())
            {
                goto Label_0014;
            }
        }

        internal void ReadTokenFrom(JsonReader reader, JsonLoadSettings options)
        {
            int depth = reader.Depth;
            if (!reader.Read())
            {
                throw JsonReaderException.Create(reader, "Error reading {0} from JsonReader.".FormatWith(CultureInfo.InvariantCulture, base.GetType().Name));
            }
            this.ReadContentFrom(reader, options);
            if (reader.Depth > depth)
            {
                throw JsonReaderException.Create(reader, "Unexpected end of content while loading {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType().Name));
            }
        }

        public void RemoveAll()
        {
            this.ClearItems();
        }

        internal virtual bool RemoveItem(JToken item)
        {
            int index = this.IndexOfItem(item);
            if (index >= 0)
            {
                this.RemoveItemAt(index);
                return true;
            }
            return false;
        }

        internal virtual void RemoveItemAt(int index)
        {
            IList<JToken> childrenTokens = this.ChildrenTokens;
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
            }
            if (index >= childrenTokens.Count)
            {
                throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");
            }
            this.CheckReentrancy();
            JToken changedItem = childrenTokens[index];
            JToken token2 = (index == 0) ? null : childrenTokens[index - 1];
            JToken token3 = (index == (childrenTokens.Count - 1)) ? null : childrenTokens[index + 1];
            if (token2 != null)
            {
                token2.Next = token3;
            }
            if (token3 != null)
            {
                token3.Previous = token2;
            }
            changedItem.Parent = null;
            changedItem.Previous = null;
            changedItem.Next = null;
            childrenTokens.RemoveAt(index);
            if (this._listChanged != null)
            {
                this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
            }
            if (this._collectionChanged != null)
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItem, index));
            }
        }

        public void ReplaceAll(object content)
        {
            this.ClearItems();
            this.Add(content);
        }

        internal virtual void ReplaceItem(JToken existing, JToken replacement)
        {
            if ((existing != null) && (existing.Parent == this))
            {
                int index = this.IndexOfItem(existing);
                this.SetItem(index, replacement);
            }
        }

        internal virtual void SetItem(int index, JToken item)
        {
            IList<JToken> childrenTokens = this.ChildrenTokens;
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
            }
            if (index >= childrenTokens.Count)
            {
                throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");
            }
            JToken currentValue = childrenTokens[index];
            if (!IsTokenUnchanged(currentValue, item))
            {
                this.CheckReentrancy();
                item = this.EnsureParentToken(item, false);
                this.ValidateToken(item, currentValue);
                JToken token2 = (index == 0) ? null : childrenTokens[index - 1];
                JToken token3 = (index == (childrenTokens.Count - 1)) ? null : childrenTokens[index + 1];
                item.Parent = this;
                item.Previous = token2;
                if (token2 != null)
                {
                    token2.Next = item;
                }
                item.Next = token3;
                if (token3 != null)
                {
                    token3.Previous = item;
                }
                childrenTokens[index] = item;
                currentValue.Parent = null;
                currentValue.Previous = null;
                currentValue.Next = null;
                if (this._listChanged != null)
                {
                    this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
                }
                if (this._collectionChanged != null)
                {
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, currentValue, index));
                }
            }
        }

        void ICollection<JToken>.Add(JToken item)
        {
            this.Add(item);
        }

        void ICollection<JToken>.Clear()
        {
            this.ClearItems();
        }

        bool ICollection<JToken>.Contains(JToken item) => 
            this.ContainsItem(item);

        void ICollection<JToken>.CopyTo(JToken[] array, int arrayIndex)
        {
            this.CopyItemsTo(array, arrayIndex);
        }

        bool ICollection<JToken>.Remove(JToken item) => 
            this.RemoveItem(item);

        int IList<JToken>.IndexOf(JToken item) => 
            this.IndexOfItem(item);

        void IList<JToken>.Insert(int index, JToken item)
        {
            this.InsertItem(index, item, false);
        }

        void IList<JToken>.RemoveAt(int index)
        {
            this.RemoveItemAt(index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.CopyItemsTo(array, index);
        }

        int IList.Add(object value)
        {
            this.Add(this.EnsureValue(value));
            return (this.Count - 1);
        }

        void IList.Clear()
        {
            this.ClearItems();
        }

        bool IList.Contains(object value) => 
            this.ContainsItem(this.EnsureValue(value));

        int IList.IndexOf(object value) => 
            this.IndexOfItem(this.EnsureValue(value));

        void IList.Insert(int index, object value)
        {
            this.InsertItem(index, this.EnsureValue(value), false);
        }

        void IList.Remove(object value)
        {
            this.RemoveItem(this.EnsureValue(value));
        }

        void IList.RemoveAt(int index)
        {
            this.RemoveItemAt(index);
        }

        void IBindingList.AddIndex(PropertyDescriptor property)
        {
        }

        object IBindingList.AddNew()
        {
            AddingNewEventArgs e = new AddingNewEventArgs();
            this.OnAddingNew(e);
            if (e.NewObject == null)
            {
                throw new JsonException("Could not determine new value to add to '{0}'.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
            }
            if (!(e.NewObject is JToken))
            {
                throw new JsonException("New item to be added to collection must be compatible with {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JToken)));
            }
            JToken newObject = (JToken) e.NewObject;
            this.Add(newObject);
            return newObject;
        }

        void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotSupportedException();
        }

        int IBindingList.Find(PropertyDescriptor property, object key)
        {
            throw new NotSupportedException();
        }

        void IBindingList.RemoveIndex(PropertyDescriptor property)
        {
        }

        void IBindingList.RemoveSort()
        {
            throw new NotSupportedException();
        }

        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (this.First is ICustomTypeDescriptor descriptor1)
            {
                return descriptor1.GetProperties();
            }
            return null;
        }

        string ITypedList.GetListName(PropertyDescriptor[] listAccessors) => 
            string.Empty;

        internal virtual void ValidateToken(JToken o, JToken existing)
        {
            ValidationUtils.ArgumentNotNull(o, "o");
            if (o.Type == JTokenType.Property)
            {
                throw new ArgumentException("Can not add {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, o.GetType(), base.GetType()));
            }
        }

        public override IEnumerable<T> Values<T>() => 
            this.ChildrenTokens.Convert<JToken, T>();

        protected abstract IList<JToken> ChildrenTokens { get; }

        public override bool HasValues =>
            (this.ChildrenTokens.Count > 0);

        public override JToken First
        {
            get
            {
                IList<JToken> childrenTokens = this.ChildrenTokens;
                if (childrenTokens.Count <= 0)
                {
                    return null;
                }
                return childrenTokens[0];
            }
        }

        public override JToken Last
        {
            get
            {
                IList<JToken> childrenTokens = this.ChildrenTokens;
                int count = childrenTokens.Count;
                if (count <= 0)
                {
                    return null;
                }
                return childrenTokens[count - 1];
            }
        }

        JToken IList<JToken>.this[int index]
        {
            get => 
                this.GetItem(index);
            set => 
                this.SetItem(index, value);
        }

        bool ICollection<JToken>.IsReadOnly =>
            false;

        bool IList.IsFixedSize =>
            false;

        bool IList.IsReadOnly =>
            false;

        object IList.this[int index]
        {
            get => 
                this.GetItem(index);
            set => 
                this.SetItem(index, this.EnsureValue(value));
        }

        public int Count =>
            this.ChildrenTokens.Count;

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                {
                    Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
                }
                return this._syncRoot;
            }
        }

        bool IBindingList.AllowEdit =>
            true;

        bool IBindingList.AllowNew =>
            true;

        bool IBindingList.AllowRemove =>
            true;

        bool IBindingList.IsSorted =>
            false;

        ListSortDirection IBindingList.SortDirection =>
            ListSortDirection.Ascending;

        PropertyDescriptor IBindingList.SortProperty =>
            null;

        bool IBindingList.SupportsChangeNotification =>
            true;

        bool IBindingList.SupportsSearching =>
            false;

        bool IBindingList.SupportsSorting =>
            false;

    }
}

