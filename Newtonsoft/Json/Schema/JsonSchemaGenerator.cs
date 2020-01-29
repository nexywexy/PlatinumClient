namespace Newtonsoft.Json.Schema
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;

    [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
    public class JsonSchemaGenerator
    {
        private IContractResolver _contractResolver;
        private JsonSchemaResolver _resolver;
        private readonly IList<TypeSchema> _stack = new List<TypeSchema>();
        private JsonSchema _currentSchema;

        private JsonSchemaType AddNullType(JsonSchemaType type, Required valueRequired)
        {
            if (valueRequired != Required.Always)
            {
                return (type | JsonSchemaType.Null);
            }
            return type;
        }

        public JsonSchema Generate(Type type) => 
            this.Generate(type, new JsonSchemaResolver(), false);

        public JsonSchema Generate(Type type, JsonSchemaResolver resolver) => 
            this.Generate(type, resolver, false);

        public JsonSchema Generate(Type type, bool rootSchemaNullable) => 
            this.Generate(type, new JsonSchemaResolver(), rootSchemaNullable);

        public JsonSchema Generate(Type type, JsonSchemaResolver resolver, bool rootSchemaNullable)
        {
            ValidationUtils.ArgumentNotNull(type, "type");
            ValidationUtils.ArgumentNotNull(resolver, "resolver");
            this._resolver = resolver;
            return this.GenerateInternal(type, !rootSchemaNullable ? Required.Always : Required.Default, false);
        }

        private JsonSchema GenerateInternal(Type type, Required valueRequired, bool required)
        {
            JsonConverter converter;
            ValidationUtils.ArgumentNotNull(type, "type");
            string typeId = this.GetTypeId(type, false);
            string str2 = this.GetTypeId(type, true);
            if (!string.IsNullOrEmpty(typeId))
            {
                JsonSchema schema = this._resolver.GetSchema(typeId);
                if (schema != null)
                {
                    if ((valueRequired != Required.Always) && !HasFlag(schema.Type, JsonSchemaType.Null))
                    {
                        schema.Type = ((JsonSchemaType) schema.Type) | JsonSchemaType.Null;
                    }
                    if (required)
                    {
                        bool? nullable3 = schema.Required;
                        bool flag = true;
                        if ((nullable3.GetValueOrDefault() == flag) ? !nullable3.HasValue : true)
                        {
                            schema.Required = true;
                        }
                    }
                    return schema;
                }
            }
            if (this._stack.Any<TypeSchema>(tc => tc.Type == type))
            {
                throw new JsonException("Unresolved circular reference for type '{0}'. Explicitly define an Id for the type using a JsonObject/JsonArray attribute or automatically generate a type Id using the UndefinedSchemaIdHandling property.".FormatWith(CultureInfo.InvariantCulture, type));
            }
            JsonContract contract = this.ContractResolver.ResolveContract(type);
            if (((converter = contract.Converter) != null) || ((converter = contract.InternalConverter) != null))
            {
                JsonSchema schema = converter.GetSchema();
                if (schema != null)
                {
                    return schema;
                }
            }
            this.Push(new TypeSchema(type, new JsonSchema()));
            if (str2 != null)
            {
                this.CurrentSchema.Id = str2;
            }
            if (required)
            {
                this.CurrentSchema.Required = true;
            }
            this.CurrentSchema.Title = this.GetTitle(type);
            this.CurrentSchema.Description = this.GetDescription(type);
            if (converter != null)
            {
                this.CurrentSchema.Type = 0x7f;
            }
            else
            {
                switch (contract.ContractType)
                {
                    case JsonContractType.Object:
                        this.CurrentSchema.Type = new JsonSchemaType?(this.AddNullType(JsonSchemaType.Object, valueRequired));
                        this.CurrentSchema.Id = this.GetTypeId(type, false);
                        this.GenerateObjectSchema(type, (JsonObjectContract) contract);
                        goto Label_0516;

                    case JsonContractType.Array:
                    {
                        this.CurrentSchema.Type = new JsonSchemaType?(this.AddNullType(JsonSchemaType.Array, valueRequired));
                        this.CurrentSchema.Id = this.GetTypeId(type, false);
                        JsonArrayAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonArrayAttribute>(type);
                        bool flag2 = (cachedAttribute == null) || cachedAttribute.AllowNullItems;
                        Type collectionItemType = ReflectionUtils.GetCollectionItemType(type);
                        if (collectionItemType != null)
                        {
                            this.CurrentSchema.Items = new List<JsonSchema>();
                            this.CurrentSchema.Items.Add(this.GenerateInternal(collectionItemType, !flag2 ? Required.Always : Required.Default, false));
                        }
                        goto Label_0516;
                    }
                    case JsonContractType.Primitive:
                    {
                        this.CurrentSchema.Type = new JsonSchemaType?(this.GetJsonSchemaType(type, valueRequired));
                        JsonSchemaType? nullable = this.CurrentSchema.Type;
                        JsonSchemaType integer = JsonSchemaType.Integer;
                        if ((!((((JsonSchemaType) nullable.GetValueOrDefault()) == integer) ? nullable.HasValue : false) || !type.IsEnum()) || type.IsDefined(typeof(FlagsAttribute), true))
                        {
                            goto Label_0516;
                        }
                        this.CurrentSchema.Enum = new List<JToken>();
                        using (IEnumerator<EnumValue<long>> enumerator = EnumUtils.GetNamesAndValues<long>(type).GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                JToken item = JToken.FromObject(enumerator.Current.Value);
                                this.CurrentSchema.Enum.Add(item);
                            }
                            goto Label_0516;
                        }
                    }
                    case JsonContractType.String:
                    {
                        JsonSchemaType type5 = !ReflectionUtils.IsNullable(contract.UnderlyingType) ? JsonSchemaType.String : this.AddNullType(JsonSchemaType.String, valueRequired);
                        this.CurrentSchema.Type = new JsonSchemaType?(type5);
                        goto Label_0516;
                    }
                    case JsonContractType.Dictionary:
                        this.CurrentSchema.Type = new JsonSchemaType?(this.AddNullType(JsonSchemaType.Object, valueRequired));
                        ReflectionUtils.GetDictionaryKeyValueTypes(type, out Type type6, out Type type7);
                        if ((type6 != null) && (this.ContractResolver.ResolveContract(type6).ContractType == JsonContractType.Primitive))
                        {
                            this.CurrentSchema.AdditionalProperties = this.GenerateInternal(type7, Required.Default, false);
                        }
                        goto Label_0516;

                    case JsonContractType.Dynamic:
                    case JsonContractType.Linq:
                        this.CurrentSchema.Type = 0x7f;
                        goto Label_0516;

                    case JsonContractType.Serializable:
                        this.CurrentSchema.Type = new JsonSchemaType?(this.AddNullType(JsonSchemaType.Object, valueRequired));
                        this.CurrentSchema.Id = this.GetTypeId(type, false);
                        this.GenerateISerializableContract(type, (JsonISerializableContract) contract);
                        goto Label_0516;
                }
                throw new JsonException("Unexpected contract type: {0}".FormatWith(CultureInfo.InvariantCulture, contract));
            }
        Label_0516:
            return this.Pop().Schema;
        }

        private void GenerateISerializableContract(Type type, JsonISerializableContract contract)
        {
            this.CurrentSchema.AllowAdditionalProperties = true;
        }

        private void GenerateObjectSchema(Type type, JsonObjectContract contract)
        {
            this.CurrentSchema.Properties = new Dictionary<string, JsonSchema>();
            foreach (JsonProperty property in contract.Properties)
            {
                if (!property.Ignored)
                {
                    NullValueHandling? nullValueHandling = property.NullValueHandling;
                    NullValueHandling ignore = NullValueHandling.Ignore;
                    bool flag = ((((((NullValueHandling) nullValueHandling.GetValueOrDefault()) == ignore) ? nullValueHandling.HasValue : false) || this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(), DefaultValueHandling.Ignore)) || (property.ShouldSerialize != null)) || (property.GetIsSpecified > null);
                    JsonSchema schema = this.GenerateInternal(property.PropertyType, property.Required, !flag);
                    if (property.DefaultValue != null)
                    {
                        schema.Default = JToken.FromObject(property.DefaultValue);
                    }
                    this.CurrentSchema.Properties.Add(property.PropertyName, schema);
                }
            }
            if (type.IsSealed())
            {
                this.CurrentSchema.AllowAdditionalProperties = false;
            }
        }

        private string GetDescription(Type type)
        {
            JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);
            if ((cachedAttribute != null) && !string.IsNullOrEmpty(cachedAttribute.Description))
            {
                return cachedAttribute.Description;
            }
            DescriptionAttribute attribute = ReflectionUtils.GetAttribute<DescriptionAttribute>(type);
            if (attribute != null)
            {
                return attribute.Description;
            }
            return null;
        }

        private JsonSchemaType GetJsonSchemaType(Type type, Required valueRequired)
        {
            JsonSchemaType none = JsonSchemaType.None;
            if ((valueRequired != Required.Always) && ReflectionUtils.IsNullable(type))
            {
                none = JsonSchemaType.Null;
                if (ReflectionUtils.IsNullableType(type))
                {
                    type = Nullable.GetUnderlyingType(type);
                }
            }
            PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(type);
            switch (typeCode)
            {
                case PrimitiveTypeCode.Empty:
                case PrimitiveTypeCode.Object:
                    return (none | JsonSchemaType.String);

                case PrimitiveTypeCode.Char:
                    return (none | JsonSchemaType.String);

                case PrimitiveTypeCode.Boolean:
                    return (none | JsonSchemaType.Boolean);

                case PrimitiveTypeCode.SByte:
                case PrimitiveTypeCode.Int16:
                case PrimitiveTypeCode.UInt16:
                case PrimitiveTypeCode.Int32:
                case PrimitiveTypeCode.Byte:
                case PrimitiveTypeCode.UInt32:
                case PrimitiveTypeCode.Int64:
                case PrimitiveTypeCode.UInt64:
                case PrimitiveTypeCode.BigInteger:
                    return (none | JsonSchemaType.Integer);

                case PrimitiveTypeCode.Single:
                case PrimitiveTypeCode.Double:
                case PrimitiveTypeCode.Decimal:
                    return (none | JsonSchemaType.Float);

                case PrimitiveTypeCode.DateTime:
                case PrimitiveTypeCode.DateTimeOffset:
                    return (none | JsonSchemaType.String);

                case PrimitiveTypeCode.Guid:
                case PrimitiveTypeCode.TimeSpan:
                case PrimitiveTypeCode.Uri:
                case PrimitiveTypeCode.String:
                case PrimitiveTypeCode.Bytes:
                    return (none | JsonSchemaType.String);

                case PrimitiveTypeCode.DBNull:
                    return (none | JsonSchemaType.Null);
            }
            throw new JsonException("Unexpected type code '{0}' for type '{1}'.".FormatWith(CultureInfo.InvariantCulture, typeCode, type));
        }

        private string GetTitle(Type type)
        {
            JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);
            if ((cachedAttribute != null) && !string.IsNullOrEmpty(cachedAttribute.Title))
            {
                return cachedAttribute.Title;
            }
            return null;
        }

        private string GetTypeId(Type type, bool explicitOnly)
        {
            JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);
            if ((cachedAttribute != null) && !string.IsNullOrEmpty(cachedAttribute.Id))
            {
                return cachedAttribute.Id;
            }
            if (explicitOnly)
            {
                return null;
            }
            Newtonsoft.Json.Schema.UndefinedSchemaIdHandling undefinedSchemaIdHandling = this.UndefinedSchemaIdHandling;
            if (undefinedSchemaIdHandling != Newtonsoft.Json.Schema.UndefinedSchemaIdHandling.UseTypeName)
            {
                if (undefinedSchemaIdHandling == Newtonsoft.Json.Schema.UndefinedSchemaIdHandling.UseAssemblyQualifiedName)
                {
                    return type.AssemblyQualifiedName;
                }
                return null;
            }
            return type.FullName;
        }

        private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag) => 
            ((value & flag) == flag);

        internal static bool HasFlag(JsonSchemaType? value, JsonSchemaType flag)
        {
            if (!value.HasValue)
            {
                return true;
            }
            JsonSchemaType? nullable = value;
            JsonSchemaType type = flag;
            JsonSchemaType? nullable3 = nullable.HasValue ? new JsonSchemaType?(((JsonSchemaType) nullable.GetValueOrDefault()) & type) : null;
            JsonSchemaType @float = flag;
            if ((((JsonSchemaType) nullable3.GetValueOrDefault()) == @float) ? nullable3.HasValue : false)
            {
                return true;
            }
            if (flag == JsonSchemaType.Integer)
            {
                nullable3 = ((JsonSchemaType) value) & JsonSchemaType.Float;
                @float = JsonSchemaType.Float;
                if ((((JsonSchemaType) nullable3.GetValueOrDefault()) == @float) ? nullable3.HasValue : false)
                {
                    return true;
                }
            }
            return false;
        }

        private TypeSchema Pop()
        {
            // This item is obfuscated and can not be translated.
        }

        private void Push(TypeSchema typeSchema)
        {
            this._currentSchema = typeSchema.Schema;
            this._stack.Add(typeSchema);
            this._resolver.LoadedSchemas.Add(typeSchema.Schema);
        }

        public Newtonsoft.Json.Schema.UndefinedSchemaIdHandling UndefinedSchemaIdHandling { get; set; }

        public IContractResolver ContractResolver
        {
            get
            {
                if (this._contractResolver == null)
                {
                    return DefaultContractResolver.Instance;
                }
                return this._contractResolver;
            }
            set => 
                (this._contractResolver = value);
        }

        private JsonSchema CurrentSchema =>
            this._currentSchema;

        private class TypeSchema
        {
            public TypeSchema(System.Type type, JsonSchema schema)
            {
                ValidationUtils.ArgumentNotNull(type, "type");
                ValidationUtils.ArgumentNotNull(schema, "schema");
                this.Type = type;
                this.Schema = schema;
            }

            public System.Type Type { get; private set; }

            public JsonSchema Schema { get; private set; }
        }
    }
}

