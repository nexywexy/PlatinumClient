namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Globalization;

    internal class DefaultReferenceResolver : IReferenceResolver
    {
        private int _referenceCount;

        public void AddReference(object context, string reference, object value)
        {
            this.GetMappings(context).Set(reference, value);
        }

        private BidirectionalDictionary<string, object> GetMappings(object context)
        {
            JsonSerializerInternalBase internalSerializer;
            if (context is JsonSerializerInternalBase)
            {
                internalSerializer = (JsonSerializerInternalBase) context;
            }
            else
            {
                if (!(context is JsonSerializerProxy))
                {
                    throw new JsonException("The DefaultReferenceResolver can only be used internally.");
                }
                internalSerializer = ((JsonSerializerProxy) context).GetInternalSerializer();
            }
            return internalSerializer.DefaultReferenceMappings;
        }

        public string GetReference(object context, object value)
        {
            BidirectionalDictionary<string, object> mappings = this.GetMappings(context);
            if (!mappings.TryGetBySecond(value, out string str))
            {
                this._referenceCount++;
                str = this._referenceCount.ToString(CultureInfo.InvariantCulture);
                mappings.Set(str, value);
            }
            return str;
        }

        public bool IsReferenced(object context, object value) => 
            this.GetMappings(context).TryGetBySecond(value, out _);

        public object ResolveReference(object context, string reference)
        {
            this.GetMappings(context).TryGetByFirst(reference, out object obj2);
            return obj2;
        }
    }
}

