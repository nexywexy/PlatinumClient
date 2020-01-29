namespace Newtonsoft.Json.Linq
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal class JPropertyKeyedCollection : Collection<JToken>
    {
        private static readonly IEqualityComparer<string> Comparer = StringComparer.Ordinal;
        private Dictionary<string, JToken> _dictionary;

        public JPropertyKeyedCollection() : base(new List<JToken>())
        {
        }

        private void AddKey(string key, JToken item)
        {
            this.EnsureDictionary();
            this._dictionary[key] = item;
        }

        protected void ChangeItemKey(JToken item, string newKey)
        {
            if (!this.ContainsItem(item))
            {
                throw new ArgumentException("The specified item does not exist in this KeyedCollection.");
            }
            string keyForItem = this.GetKeyForItem(item);
            if (!Comparer.Equals(keyForItem, newKey))
            {
                if (newKey != null)
                {
                    this.AddKey(newKey, item);
                }
                if (keyForItem != null)
                {
                    this.RemoveKey(keyForItem);
                }
            }
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            if (this._dictionary != null)
            {
                this._dictionary.Clear();
            }
        }

        public bool Compare(JPropertyKeyedCollection other)
        {
            if (this != other)
            {
                Dictionary<string, JToken> dictionary = this._dictionary;
                Dictionary<string, JToken> dictionary2 = other._dictionary;
                if ((dictionary == null) && (dictionary2 == null))
                {
                    return true;
                }
                if (dictionary == null)
                {
                    return (dictionary2.Count == 0);
                }
                if (dictionary2 == null)
                {
                    return (dictionary.Count == 0);
                }
                if (dictionary.Count != dictionary2.Count)
                {
                    return false;
                }
                foreach (KeyValuePair<string, JToken> pair in dictionary)
                {
                    if (!dictionary2.TryGetValue(pair.Key, out JToken token))
                    {
                        return false;
                    }
                    JProperty property = (JProperty) pair.Value;
                    JProperty property2 = (JProperty) token;
                    if (property.Value == null)
                    {
                        return (property2.Value == null);
                    }
                    if (!property.Value.DeepEquals(property2.Value))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool Contains(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            return ((this._dictionary != null) && this._dictionary.ContainsKey(key));
        }

        private bool ContainsItem(JToken item)
        {
            if (this._dictionary == null)
            {
                return false;
            }
            string keyForItem = this.GetKeyForItem(item);
            return this._dictionary.TryGetValue(keyForItem, out _);
        }

        private void EnsureDictionary()
        {
            if (this._dictionary == null)
            {
                this._dictionary = new Dictionary<string, JToken>(Comparer);
            }
        }

        private string GetKeyForItem(JToken item) => 
            ((JProperty) item).Name;

        public int IndexOfReference(JToken t) => 
            ((List<JToken>) base.Items).IndexOfReference<JToken>(t);

        protected override void InsertItem(int index, JToken item)
        {
            this.AddKey(this.GetKeyForItem(item), item);
            base.InsertItem(index, item);
        }

        public bool Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            return (this._dictionary?.ContainsKey(key) && base.Remove(this._dictionary[key]));
        }

        protected override void RemoveItem(int index)
        {
            string keyForItem = this.GetKeyForItem(base.Items[index]);
            this.RemoveKey(keyForItem);
            base.RemoveItem(index);
        }

        private void RemoveKey(string key)
        {
            if (this._dictionary != null)
            {
                this._dictionary.Remove(key);
            }
        }

        protected override void SetItem(int index, JToken item)
        {
            string keyForItem = this.GetKeyForItem(item);
            string x = this.GetKeyForItem(base.Items[index]);
            if (Comparer.Equals(x, keyForItem))
            {
                if (this._dictionary != null)
                {
                    this._dictionary[keyForItem] = item;
                }
            }
            else
            {
                this.AddKey(keyForItem, item);
                if (x != null)
                {
                    this.RemoveKey(x);
                }
            }
            base.SetItem(index, item);
        }

        public bool TryGetValue(string key, out JToken value) => 
            this._dictionary?.TryGetValue(key, out value);

        public JToken this[string key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                return this._dictionary?[key];
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                this.EnsureDictionary();
                return this._dictionary.Keys;
            }
        }

        public ICollection<JToken> Values
        {
            get
            {
                this.EnsureDictionary();
                return this._dictionary.Values;
            }
        }
    }
}

