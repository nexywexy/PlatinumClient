﻿namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Runtime.InteropServices;

    public class JsonPropertyCollection : KeyedCollection<string, JsonProperty>
    {
        private readonly Type _type;
        private readonly List<JsonProperty> _list;

        public JsonPropertyCollection(Type type) : base(StringComparer.Ordinal)
        {
            ValidationUtils.ArgumentNotNull(type, "type");
            this._type = type;
            this._list = (List<JsonProperty>) base.Items;
        }

        public void AddProperty(JsonProperty property)
        {
            if (base.Contains(property.PropertyName))
            {
                if (property.Ignored)
                {
                    return;
                }
                JsonProperty item = base[property.PropertyName];
                bool flag = true;
                if (item.Ignored)
                {
                    base.Remove(item);
                    flag = false;
                }
                else if ((property.DeclaringType != null) && (item.DeclaringType != null))
                {
                    if (property.DeclaringType.IsSubclassOf(item.DeclaringType) || (item.DeclaringType.IsInterface() && property.DeclaringType.ImplementInterface(item.DeclaringType)))
                    {
                        base.Remove(item);
                        flag = false;
                    }
                    if (item.DeclaringType.IsSubclassOf(property.DeclaringType) || (property.DeclaringType.IsInterface() && item.DeclaringType.ImplementInterface(property.DeclaringType)))
                    {
                        return;
                    }
                }
                if (flag)
                {
                    throw new JsonSerializationException("A member with the name '{0}' already exists on '{1}'. Use the JsonPropertyAttribute to specify another name.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, this._type));
                }
            }
            base.Add(property);
        }

        public JsonProperty GetClosestMatchProperty(string propertyName)
        {
            JsonProperty property = this.GetProperty(propertyName, StringComparison.Ordinal);
            if (property == null)
            {
                property = this.GetProperty(propertyName, StringComparison.OrdinalIgnoreCase);
            }
            return property;
        }

        protected override string GetKeyForItem(JsonProperty item) => 
            item.PropertyName;

        public JsonProperty GetProperty(string propertyName, StringComparison comparisonType)
        {
            if (comparisonType == StringComparison.Ordinal)
            {
                if (this.TryGetValue(propertyName, out JsonProperty property))
                {
                    return property;
                }
                return null;
            }
            for (int i = 0; i < this._list.Count; i++)
            {
                JsonProperty property2 = this._list[i];
                if (string.Equals(propertyName, property2.PropertyName, comparisonType))
                {
                    return property2;
                }
            }
            return null;
        }

        private bool TryGetValue(string key, out JsonProperty item) => 
            base.Dictionary?.TryGetValue(key, out item);
    }
}

