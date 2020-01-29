namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    public class DefaultContractResolver : IContractResolver
    {
        private static readonly IContractResolver _instance = new DefaultContractResolver(true);
        private static readonly JsonConverter[] BuiltInConverters = new JsonConverter[] { new EntityKeyMemberConverter(), new ExpandoObjectConverter(), new XmlNodeConverter(), new BinaryConverter(), new DataSetConverter(), new DataTableConverter(), new DiscriminatedUnionConverter(), new KeyValuePairConverter(), new BsonObjectIdConverter(), new RegexConverter() };
        private static readonly object TypeContractCacheLock = new object();
        private static readonly DefaultContractResolverState _sharedState = new DefaultContractResolverState();
        private readonly DefaultContractResolverState _instanceState;
        private readonly bool _sharedCache;

        public DefaultContractResolver()
        {
            this._instanceState = new DefaultContractResolverState();
            this.IgnoreSerializableAttribute = true;
            this.DefaultMembersSearchFlags = BindingFlags.Public | BindingFlags.Instance;
        }

        [Obsolete("DefaultContractResolver(bool) is obsolete. Use the parameterless constructor and cache instances of the contract resolver within your application for optimal performance.")]
        public DefaultContractResolver(bool shareCache) : this()
        {
            this._sharedCache = shareCache;
        }

        internal static bool CanConvertToString(Type type)
        {
            TypeConverter converter = ConvertUtils.GetConverter(type);
            if (((((converter == null) || (converter is ComponentConverter)) || ((converter is ReferenceConverter) || (converter.GetType() == typeof(TypeConverter)))) || !converter.CanConvertTo(typeof(string))) && (!(type == typeof(Type)) && !type.IsSubclassOf(typeof(Type))))
            {
                return false;
            }
            return true;
        }

        protected virtual JsonArrayContract CreateArrayContract(Type objectType)
        {
            JsonArrayContract contract = new JsonArrayContract(objectType);
            this.InitializeContract(contract);
            ConstructorInfo attributeConstructor = this.GetAttributeConstructor(contract.NonNullableUnderlyingType);
            if (attributeConstructor != null)
            {
                ParameterInfo[] parameters = attributeConstructor.GetParameters();
                Type type = (contract.CollectionItemType != null) ? typeof(IEnumerable<>).MakeGenericType(new Type[] { contract.CollectionItemType }) : typeof(IEnumerable);
                if (parameters.Length == 0)
                {
                    contract.HasParameterizedCreator = false;
                }
                else
                {
                    if ((parameters.Length != 1) || !type.IsAssignableFrom(parameters[0].ParameterType))
                    {
                        throw new JsonException("Constructor for '{0}' must have no parameters or a single parameter that implements '{1}'.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType, type));
                    }
                    contract.HasParameterizedCreator = true;
                }
                contract.OverrideCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(attributeConstructor);
            }
            return contract;
        }

        protected virtual IList<JsonProperty> CreateConstructorParameters(ConstructorInfo constructor, JsonPropertyCollection memberProperties)
        {
            JsonPropertyCollection propertys = new JsonPropertyCollection(constructor.DeclaringType);
            foreach (ParameterInfo info in constructor.GetParameters())
            {
                JsonProperty matchingMemberProperty = (info.Name != null) ? memberProperties.GetClosestMatchProperty(info.Name) : null;
                if ((matchingMemberProperty != null) && (matchingMemberProperty.PropertyType != info.ParameterType))
                {
                    matchingMemberProperty = null;
                }
                if ((matchingMemberProperty != null) || (info.Name != null))
                {
                    JsonProperty property = this.CreatePropertyFromConstructorParameter(matchingMemberProperty, info);
                    if (property != null)
                    {
                        propertys.AddProperty(property);
                    }
                }
            }
            return propertys;
        }

        protected virtual JsonContract CreateContract(Type objectType)
        {
            if (IsJsonPrimitiveType(objectType))
            {
                return this.CreatePrimitiveContract(objectType);
            }
            Type attributeProvider = ReflectionUtils.EnsureNotNullableType(objectType);
            JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(attributeProvider);
            if (!(cachedAttribute is JsonObjectAttribute))
            {
                if (cachedAttribute is JsonArrayAttribute)
                {
                    return this.CreateArrayContract(objectType);
                }
                if (cachedAttribute is JsonDictionaryAttribute)
                {
                    return this.CreateDictionaryContract(objectType);
                }
                if ((attributeProvider == typeof(JToken)) || attributeProvider.IsSubclassOf(typeof(JToken)))
                {
                    return this.CreateLinqContract(objectType);
                }
                if (CollectionUtils.IsDictionaryType(attributeProvider))
                {
                    return this.CreateDictionaryContract(objectType);
                }
                if (typeof(IEnumerable).IsAssignableFrom(attributeProvider))
                {
                    return this.CreateArrayContract(objectType);
                }
                if (CanConvertToString(attributeProvider))
                {
                    return this.CreateStringContract(objectType);
                }
                if (!this.IgnoreSerializableInterface && typeof(ISerializable).IsAssignableFrom(attributeProvider))
                {
                    return this.CreateISerializableContract(objectType);
                }
                if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(attributeProvider))
                {
                    return this.CreateDynamicContract(objectType);
                }
                if (IsIConvertible(attributeProvider))
                {
                    return this.CreatePrimitiveContract(attributeProvider);
                }
            }
            return this.CreateObjectContract(objectType);
        }

        protected virtual JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            JsonDictionaryContract contract = new JsonDictionaryContract(objectType);
            this.InitializeContract(contract);
            JsonContainerAttribute containerAttribute = JsonTypeReflector.GetAttribute<JsonContainerAttribute>(objectType);
            if (containerAttribute?.NamingStrategyType != null)
            {
                Newtonsoft.Json.Serialization.NamingStrategy namingStrategy = JsonTypeReflector.GetContainerNamingStrategy(containerAttribute);
                contract.DictionaryKeyResolver = s => namingStrategy.GetDictionaryKey(s);
            }
            else
            {
                contract.DictionaryKeyResolver = new Func<string, string>(this.ResolveDictionaryKey);
            }
            ConstructorInfo attributeConstructor = this.GetAttributeConstructor(contract.NonNullableUnderlyingType);
            if (attributeConstructor != null)
            {
                ParameterInfo[] parameters = attributeConstructor.GetParameters();
                Type type = ((contract.DictionaryKeyType != null) && (contract.DictionaryValueType != null)) ? typeof(IEnumerable<>).MakeGenericType(new Type[1]) : typeof(IDictionary);
                if (parameters.Length == 0)
                {
                    contract.HasParameterizedCreator = false;
                }
                else
                {
                    if ((parameters.Length != 1) || !type.IsAssignableFrom(parameters[0].ParameterType))
                    {
                        throw new JsonException("Constructor for '{0}' must have no parameters or a single parameter that implements '{1}'.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType, type));
                    }
                    contract.HasParameterizedCreator = true;
                }
                contract.OverrideCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(attributeConstructor);
            }
            return contract;
        }

        protected virtual JsonDynamicContract CreateDynamicContract(Type objectType)
        {
            JsonDynamicContract contract = new JsonDynamicContract(objectType);
            this.InitializeContract(contract);
            JsonContainerAttribute containerAttribute = JsonTypeReflector.GetAttribute<JsonContainerAttribute>(objectType);
            if (containerAttribute?.NamingStrategyType != null)
            {
                Newtonsoft.Json.Serialization.NamingStrategy namingStrategy = JsonTypeReflector.GetContainerNamingStrategy(containerAttribute);
                contract.PropertyNameResolver = s => namingStrategy.GetDictionaryKey(s);
            }
            else
            {
                contract.PropertyNameResolver = new Func<string, string>(this.ResolveDictionaryKey);
            }
            contract.Properties.AddRange<JsonProperty>(this.CreateProperties(objectType, MemberSerialization.OptOut));
            return contract;
        }

        protected virtual JsonISerializableContract CreateISerializableContract(Type objectType)
        {
            JsonISerializableContract contract = new JsonISerializableContract(objectType);
            this.InitializeContract(contract);
            Type[] types = new Type[] { typeof(SerializationInfo), typeof(StreamingContext) };
            ConstructorInfo method = contract.NonNullableUnderlyingType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, types, null);
            if (method != null)
            {
                ObjectConstructor<object> constructor = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(method);
                contract.ISerializableCreator = constructor;
            }
            return contract;
        }

        protected virtual JsonLinqContract CreateLinqContract(Type objectType)
        {
            JsonLinqContract contract = new JsonLinqContract(objectType);
            this.InitializeContract(contract);
            return contract;
        }

        protected virtual IValueProvider CreateMemberValueProvider(MemberInfo member)
        {
            if (this.DynamicCodeGeneration)
            {
                return new DynamicValueProvider(member);
            }
            return new ReflectionValueProvider(member);
        }

        protected virtual JsonObjectContract CreateObjectContract(Type objectType)
        {
            JsonObjectContract contract = new JsonObjectContract(objectType);
            this.InitializeContract(contract);
            bool ignoreSerializableAttribute = this.IgnoreSerializableAttribute;
            contract.MemberSerialization = JsonTypeReflector.GetObjectMemberSerialization(contract.NonNullableUnderlyingType, ignoreSerializableAttribute);
            contract.Properties.AddRange<JsonProperty>(this.CreateProperties(contract.NonNullableUnderlyingType, contract.MemberSerialization));
            JsonObjectAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonObjectAttribute>(contract.NonNullableUnderlyingType);
            if (cachedAttribute != null)
            {
                contract.ItemRequired = cachedAttribute._itemRequired;
            }
            if (contract.IsInstantiable)
            {
                ConstructorInfo attributeConstructor = this.GetAttributeConstructor(contract.NonNullableUnderlyingType);
                if (attributeConstructor != null)
                {
                    contract.OverrideConstructor = attributeConstructor;
                    contract.CreatorParameters.AddRange<JsonProperty>(this.CreateConstructorParameters(attributeConstructor, contract.Properties));
                }
                else if (contract.MemberSerialization == MemberSerialization.Fields)
                {
                    if (JsonTypeReflector.FullyTrusted)
                    {
                        contract.DefaultCreator = new Func<object>(contract.GetUninitializedObject);
                    }
                }
                else if ((contract.DefaultCreator == null) || contract.DefaultCreatorNonPublic)
                {
                    ConstructorInfo parameterizedConstructor = this.GetParameterizedConstructor(contract.NonNullableUnderlyingType);
                    if (parameterizedConstructor != null)
                    {
                        contract.ParametrizedConstructor = parameterizedConstructor;
                        contract.CreatorParameters.AddRange<JsonProperty>(this.CreateConstructorParameters(parameterizedConstructor, contract.Properties));
                    }
                }
            }
            MemberInfo extensionDataMemberForType = this.GetExtensionDataMemberForType(contract.NonNullableUnderlyingType);
            if (extensionDataMemberForType != null)
            {
                SetExtensionDataDelegates(contract, extensionDataMemberForType);
            }
            return contract;
        }

        protected virtual JsonPrimitiveContract CreatePrimitiveContract(Type objectType)
        {
            JsonPrimitiveContract contract = new JsonPrimitiveContract(objectType);
            this.InitializeContract(contract);
            return contract;
        }

        protected virtual IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            List<MemberInfo> serializableMembers = this.GetSerializableMembers(type);
            if (serializableMembers == null)
            {
                throw new JsonSerializationException("Null collection of seralizable members returned.");
            }
            JsonPropertyCollection source = new JsonPropertyCollection(type);
            foreach (MemberInfo info in serializableMembers)
            {
                JsonProperty property = this.CreateProperty(info, memberSerialization);
                if (property != null)
                {
                    DefaultContractResolverState state = this.GetState();
                    PropertyNameTable nameTable = state.NameTable;
                    lock (nameTable)
                    {
                        property.PropertyName = state.NameTable.Add(property.PropertyName);
                    }
                    source.AddProperty(property);
                }
            }
            if (<>c.<>9__65_0 == null)
            {
            }
            return source.OrderBy<JsonProperty, int>((<>c.<>9__65_0 = new Func<JsonProperty, int>(<>c.<>9.<CreateProperties>b__65_0))).ToList<JsonProperty>();
        }

        protected virtual JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = new JsonProperty {
                PropertyType = ReflectionUtils.GetMemberUnderlyingType(member),
                DeclaringType = member.DeclaringType,
                ValueProvider = this.CreateMemberValueProvider(member),
                AttributeProvider = new ReflectionAttributeProvider(member)
            };
            this.SetPropertySettingsFromAttributes(property, member, member.Name, member.DeclaringType, memberSerialization, out bool flag);
            if (memberSerialization != MemberSerialization.Fields)
            {
                property.Readable = ReflectionUtils.CanReadMemberValue(member, flag);
                property.Writable = ReflectionUtils.CanSetMemberValue(member, flag, property.HasMemberAttribute);
            }
            else
            {
                property.Readable = true;
                property.Writable = true;
            }
            property.ShouldSerialize = this.CreateShouldSerializeTest(member);
            this.SetIsSpecifiedActions(property, member, flag);
            return property;
        }

        protected virtual JsonProperty CreatePropertyFromConstructorParameter(JsonProperty matchingMemberProperty, ParameterInfo parameterInfo)
        {
            JsonProperty property = new JsonProperty {
                PropertyType = parameterInfo.ParameterType,
                AttributeProvider = new ReflectionAttributeProvider(parameterInfo)
            };
            this.SetPropertySettingsFromAttributes(property, parameterInfo, parameterInfo.Name, parameterInfo.Member.DeclaringType, MemberSerialization.OptOut, out _);
            property.Readable = false;
            property.Writable = true;
            if (matchingMemberProperty != null)
            {
                property.PropertyName = (property.PropertyName != parameterInfo.Name) ? property.PropertyName : matchingMemberProperty.PropertyName;
                if (property.Converter == null)
                {
                }
                property.Converter = matchingMemberProperty.Converter;
                if (property.MemberConverter == null)
                {
                }
                property.MemberConverter = matchingMemberProperty.MemberConverter;
                if (!property._hasExplicitDefaultValue && matchingMemberProperty._hasExplicitDefaultValue)
                {
                    property.DefaultValue = matchingMemberProperty.DefaultValue;
                }
                Required? nullable = property._required;
                property._required = nullable.HasValue ? nullable : matchingMemberProperty._required;
                bool? isReference = property.IsReference;
                property.IsReference = isReference.HasValue ? isReference : matchingMemberProperty.IsReference;
                NullValueHandling? nullValueHandling = property.NullValueHandling;
                property.NullValueHandling = nullValueHandling.HasValue ? nullValueHandling : matchingMemberProperty.NullValueHandling;
                DefaultValueHandling? defaultValueHandling = property.DefaultValueHandling;
                property.DefaultValueHandling = defaultValueHandling.HasValue ? defaultValueHandling : matchingMemberProperty.DefaultValueHandling;
                ReferenceLoopHandling? referenceLoopHandling = property.ReferenceLoopHandling;
                property.ReferenceLoopHandling = referenceLoopHandling.HasValue ? referenceLoopHandling : matchingMemberProperty.ReferenceLoopHandling;
                ObjectCreationHandling? objectCreationHandling = property.ObjectCreationHandling;
                property.ObjectCreationHandling = objectCreationHandling.HasValue ? objectCreationHandling : matchingMemberProperty.ObjectCreationHandling;
                TypeNameHandling? typeNameHandling = property.TypeNameHandling;
                property.TypeNameHandling = typeNameHandling.HasValue ? typeNameHandling : matchingMemberProperty.TypeNameHandling;
            }
            return property;
        }

        private Predicate<object> CreateShouldSerializeTest(MemberInfo member)
        {
            MethodInfo method = member.DeclaringType.GetMethod("ShouldSerialize" + member.Name, ReflectionUtils.EmptyTypes);
            if ((method == null) || (method.ReturnType != typeof(bool)))
            {
                return null;
            }
            MethodCall<object, object> shouldSerializeCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
            return o => ((bool) shouldSerializeCall(o, new object[0]));
        }

        protected virtual JsonStringContract CreateStringContract(Type objectType)
        {
            JsonStringContract contract = new JsonStringContract(objectType);
            this.InitializeContract(contract);
            return contract;
        }

        private ConstructorInfo GetAttributeConstructor(Type objectType)
        {
            if (<>c.<>9__40_0 == null)
            {
            }
            IList<ConstructorInfo> list = objectType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where<ConstructorInfo>((<>c.<>9__40_0 = new Func<ConstructorInfo, bool>(<>c.<>9.<GetAttributeConstructor>b__40_0))).ToList<ConstructorInfo>();
            if (list.Count > 1)
            {
                throw new JsonException("Multiple constructors with the JsonConstructorAttribute.");
            }
            if (list.Count == 1)
            {
                return list[0];
            }
            if (objectType == typeof(Version))
            {
                Type[] types = new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) };
                return objectType.GetConstructor(types);
            }
            return null;
        }

        private void GetCallbackMethodsForType(Type type, out List<SerializationCallback> onSerializing, out List<SerializationCallback> onSerialized, out List<SerializationCallback> onDeserializing, out List<SerializationCallback> onDeserialized, out List<SerializationErrorCallback> onError)
        {
            onSerializing = null;
            onSerialized = null;
            onDeserializing = null;
            onDeserialized = null;
            onError = null;
            foreach (Type local1 in this.GetClassHierarchyForType(type))
            {
                MethodInfo currentCallback = null;
                MethodInfo info2 = null;
                MethodInfo info3 = null;
                MethodInfo info4 = null;
                MethodInfo info5 = null;
                bool flag = ShouldSkipSerializing(local1);
                bool flag2 = ShouldSkipDeserialized(local1);
                foreach (MethodInfo info6 in local1.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (!info6.ContainsGenericParameters)
                    {
                        Type prevAttributeType = null;
                        ParameterInfo[] parameters = info6.GetParameters();
                        if (!flag && IsValidCallback(info6, parameters, typeof(OnSerializingAttribute), currentCallback, ref prevAttributeType))
                        {
                            if (onSerializing == null)
                            {
                            }
                            onSerializing = new List<SerializationCallback>();
                            onSerializing.Add(JsonContract.CreateSerializationCallback(info6));
                            currentCallback = info6;
                        }
                        if (IsValidCallback(info6, parameters, typeof(OnSerializedAttribute), info2, ref prevAttributeType))
                        {
                            if (onSerialized == null)
                            {
                            }
                            onSerialized = new List<SerializationCallback>();
                            onSerialized.Add(JsonContract.CreateSerializationCallback(info6));
                            info2 = info6;
                        }
                        if (IsValidCallback(info6, parameters, typeof(OnDeserializingAttribute), info3, ref prevAttributeType))
                        {
                            if (onDeserializing == null)
                            {
                            }
                            onDeserializing = new List<SerializationCallback>();
                            onDeserializing.Add(JsonContract.CreateSerializationCallback(info6));
                            info3 = info6;
                        }
                        if (!flag2 && IsValidCallback(info6, parameters, typeof(OnDeserializedAttribute), info4, ref prevAttributeType))
                        {
                            if (onDeserialized == null)
                            {
                            }
                            onDeserialized = new List<SerializationCallback>();
                            onDeserialized.Add(JsonContract.CreateSerializationCallback(info6));
                            info4 = info6;
                        }
                        if (IsValidCallback(info6, parameters, typeof(OnErrorAttribute), info5, ref prevAttributeType))
                        {
                            if (onError == null)
                            {
                            }
                            onError = new List<SerializationErrorCallback>();
                            onError.Add(JsonContract.CreateSerializationErrorCallback(info6));
                            info5 = info6;
                        }
                    }
                }
            }
        }

        private List<Type> GetClassHierarchyForType(Type type)
        {
            List<Type> list = new List<Type>();
            for (Type type2 = type; (type2 != null) && (type2 != typeof(object)); type2 = type2.BaseType())
            {
                list.Add(type2);
            }
            list.Reverse();
            return list;
        }

        internal static string GetClrTypeFullName(Type type)
        {
            if (type.IsGenericTypeDefinition() || !type.ContainsGenericParameters())
            {
                return type.FullName;
            }
            object[] args = new object[] { type.Namespace, type.Name };
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", args);
        }

        private Func<object> GetDefaultCreator(Type createdType) => 
            JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(createdType);

        private MemberInfo GetExtensionDataMemberForType(Type type)
        {
            if (<>c.<>9__37_0 == null)
            {
            }
            if (<>c.<>9__37_1 == null)
            {
            }
            return this.GetClassHierarchyForType(type).SelectMany<Type, MemberInfo>((<>c.<>9__37_0 = new Func<Type, IEnumerable<MemberInfo>>(<>c.<>9.<GetExtensionDataMemberForType>b__37_0))).LastOrDefault<MemberInfo>((<>c.<>9__37_1 = new Func<MemberInfo, bool>(<>c.<>9.<GetExtensionDataMemberForType>b__37_1)));
        }

        private ConstructorInfo GetParameterizedConstructor(Type objectType)
        {
            IList<ConstructorInfo> list = objectType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).ToList<ConstructorInfo>();
            if (list.Count == 1)
            {
                return list[0];
            }
            return null;
        }

        public string GetResolvedPropertyName(string propertyName) => 
            this.ResolvePropertyName(propertyName);

        protected virtual List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            bool ignoreSerializableAttribute = this.IgnoreSerializableAttribute;
            MemberSerialization objectMemberSerialization = JsonTypeReflector.GetObjectMemberSerialization(objectType, ignoreSerializableAttribute);
            if (<>c.<>9__34_0 == null)
            {
            }
            List<MemberInfo> list = ReflectionUtils.GetFieldsAndProperties(objectType, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).Where<MemberInfo>((<>c.<>9__34_0 = new Func<MemberInfo, bool>(<>c.<>9.<GetSerializableMembers>b__34_0))).ToList<MemberInfo>();
            List<MemberInfo> source = new List<MemberInfo>();
            if (objectMemberSerialization != MemberSerialization.Fields)
            {
                DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(objectType);
                if (<>c.<>9__34_1 == null)
                {
                }
                List<MemberInfo> list3 = ReflectionUtils.GetFieldsAndProperties(objectType, this.DefaultMembersSearchFlags).Where<MemberInfo>((<>c.<>9__34_1 = new Func<MemberInfo, bool>(<>c.<>9.<GetSerializableMembers>b__34_1))).ToList<MemberInfo>();
                foreach (MemberInfo info in list)
                {
                    if (this.SerializeCompilerGeneratedMembers || !info.IsDefined(typeof(CompilerGeneratedAttribute), true))
                    {
                        if (list3.Contains(info))
                        {
                            source.Add(info);
                        }
                        else if (JsonTypeReflector.GetAttribute<JsonPropertyAttribute>(info) != null)
                        {
                            source.Add(info);
                        }
                        else if (JsonTypeReflector.GetAttribute<JsonRequiredAttribute>(info) != null)
                        {
                            source.Add(info);
                        }
                        else if ((dataContractAttribute != null) && (JsonTypeReflector.GetAttribute<DataMemberAttribute>(info) != null))
                        {
                            source.Add(info);
                        }
                        else if ((objectMemberSerialization == MemberSerialization.Fields) && (info.MemberType() == MemberTypes.Field))
                        {
                            source.Add(info);
                        }
                    }
                }
                if (objectType.AssignableToTypeName("System.Data.Objects.DataClasses.EntityObject", out _))
                {
                    source = source.Where<MemberInfo>(new Func<MemberInfo, bool>(this.ShouldSerializeEntityMember)).ToList<MemberInfo>();
                }
                return source;
            }
            foreach (MemberInfo info2 in list)
            {
                FieldInfo info3 = info2 as FieldInfo;
                if ((info3 != null) && !info3.IsStatic)
                {
                    source.Add(info2);
                }
            }
            return source;
        }

        internal DefaultContractResolverState GetState()
        {
            if (this._sharedCache)
            {
                return _sharedState;
            }
            return this._instanceState;
        }

        private void InitializeContract(JsonContract contract)
        {
            JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(contract.NonNullableUnderlyingType);
            if (cachedAttribute != null)
            {
                contract.IsReference = cachedAttribute._isReference;
            }
            else
            {
                DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(contract.NonNullableUnderlyingType);
                if ((dataContractAttribute != null) && dataContractAttribute.IsReference)
                {
                    contract.IsReference = true;
                }
            }
            contract.Converter = this.ResolveContractConverter(contract.NonNullableUnderlyingType);
            contract.InternalConverter = JsonSerializer.GetMatchingConverter(BuiltInConverters, contract.NonNullableUnderlyingType);
            if (contract.IsInstantiable && (ReflectionUtils.HasDefaultConstructor(contract.CreatedType, true) || contract.CreatedType.IsValueType()))
            {
                contract.DefaultCreator = this.GetDefaultCreator(contract.CreatedType);
                contract.DefaultCreatorNonPublic = !contract.CreatedType.IsValueType() && (ReflectionUtils.GetDefaultConstructor(contract.CreatedType) == null);
            }
            this.ResolveCallbackMethods(contract, contract.NonNullableUnderlyingType);
        }

        internal static bool IsIConvertible(Type t)
        {
            if (!typeof(IConvertible).IsAssignableFrom(t) && (!ReflectionUtils.IsNullableType(t) || !typeof(IConvertible).IsAssignableFrom(Nullable.GetUnderlyingType(t))))
            {
                return false;
            }
            return !typeof(JToken).IsAssignableFrom(t);
        }

        internal static bool IsJsonPrimitiveType(Type t)
        {
            PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(t);
            return ((typeCode != PrimitiveTypeCode.Empty) && (typeCode != PrimitiveTypeCode.Object));
        }

        private static bool IsValidCallback(MethodInfo method, ParameterInfo[] parameters, Type attributeType, MethodInfo currentCallback, ref Type prevAttributeType)
        {
            if (!method.IsDefined(attributeType, false))
            {
                return false;
            }
            if (currentCallback != null)
            {
                throw new JsonException("Invalid attribute. Both '{0}' and '{1}' in type '{2}' have '{3}'.".FormatWith(CultureInfo.InvariantCulture, method, currentCallback, GetClrTypeFullName(method.DeclaringType), attributeType));
            }
            if (prevAttributeType != null)
            {
                throw new JsonException("Invalid Callback. Method '{3}' in type '{2}' has both '{0}' and '{1}'.".FormatWith(CultureInfo.InvariantCulture, prevAttributeType, attributeType, GetClrTypeFullName(method.DeclaringType), method));
            }
            if (method.IsVirtual)
            {
                throw new JsonException("Virtual Method '{0}' of type '{1}' cannot be marked with '{2}' attribute.".FormatWith(CultureInfo.InvariantCulture, method, GetClrTypeFullName(method.DeclaringType), attributeType));
            }
            if (method.ReturnType != typeof(void))
            {
                throw new JsonException("Serialization Callback '{1}' in type '{0}' must return void.".FormatWith(CultureInfo.InvariantCulture, GetClrTypeFullName(method.DeclaringType), method));
            }
            if (attributeType == typeof(OnErrorAttribute))
            {
                if (((parameters == null) || (parameters.Length != 2)) || ((parameters[0].ParameterType != typeof(StreamingContext)) || (parameters[1].ParameterType != typeof(ErrorContext))))
                {
                    throw new JsonException("Serialization Error Callback '{1}' in type '{0}' must have two parameters of type '{2}' and '{3}'.".FormatWith(CultureInfo.InvariantCulture, GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext), typeof(ErrorContext)));
                }
            }
            else if (((parameters == null) || (parameters.Length != 1)) || (parameters[0].ParameterType != typeof(StreamingContext)))
            {
                throw new JsonException("Serialization Callback '{1}' in type '{0}' must have a single parameter of type '{2}'.".FormatWith(CultureInfo.InvariantCulture, GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext)));
            }
            prevAttributeType = attributeType;
            return true;
        }

        private void ResolveCallbackMethods(JsonContract contract, Type t)
        {
            this.GetCallbackMethodsForType(t, out List<SerializationCallback> list, out List<SerializationCallback> list2, out List<SerializationCallback> list3, out List<SerializationCallback> list4, out List<SerializationErrorCallback> list5);
            if (list != null)
            {
                contract.OnSerializingCallbacks.AddRange<SerializationCallback>(list);
            }
            if (list2 != null)
            {
                contract.OnSerializedCallbacks.AddRange<SerializationCallback>(list2);
            }
            if (list3 != null)
            {
                contract.OnDeserializingCallbacks.AddRange<SerializationCallback>(list3);
            }
            if (list4 != null)
            {
                contract.OnDeserializedCallbacks.AddRange<SerializationCallback>(list4);
            }
            if (list5 != null)
            {
                contract.OnErrorCallbacks.AddRange<SerializationErrorCallback>(list5);
            }
        }

        public virtual JsonContract ResolveContract(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            DefaultContractResolverState state = this.GetState();
            ResolverContractKey key = new ResolverContractKey(base.GetType(), type);
            Dictionary<ResolverContractKey, JsonContract> contractCache = state.ContractCache;
            if ((contractCache == null) || !contractCache.TryGetValue(key, out JsonContract contract))
            {
                contract = this.CreateContract(type);
                object typeContractCacheLock = TypeContractCacheLock;
                lock (typeContractCacheLock)
                {
                    contractCache = state.ContractCache;
                    Dictionary<ResolverContractKey, JsonContract> dictionary2 = (contractCache != null) ? new Dictionary<ResolverContractKey, JsonContract>(contractCache) : new Dictionary<ResolverContractKey, JsonContract>();
                    dictionary2[key] = contract;
                    state.ContractCache = dictionary2;
                }
            }
            return contract;
        }

        protected virtual JsonConverter ResolveContractConverter(Type objectType) => 
            JsonTypeReflector.GetJsonConverter(objectType);

        protected virtual string ResolveDictionaryKey(string dictionaryKey)
        {
            if (this.NamingStrategy != null)
            {
                return this.NamingStrategy.GetDictionaryKey(dictionaryKey);
            }
            return this.ResolvePropertyName(dictionaryKey);
        }

        protected virtual string ResolvePropertyName(string propertyName)
        {
            if (this.NamingStrategy != null)
            {
                return this.NamingStrategy.GetPropertyName(propertyName, false);
            }
            return propertyName;
        }

        private static void SetExtensionDataDelegates(JsonObjectContract contract, MemberInfo member)
        {
            JsonExtensionDataAttribute attribute = ReflectionUtils.GetAttribute<JsonExtensionDataAttribute>(member);
            if (attribute != null)
            {
                Type type5;
                Type memberUnderlyingType = ReflectionUtils.GetMemberUnderlyingType(member);
                ReflectionUtils.ImplementsGenericDefinition(memberUnderlyingType, typeof(IDictionary<,>), out Type type2);
                Type type3 = type2.GetGenericArguments()[0];
                Type type4 = type2.GetGenericArguments()[1];
                if (ReflectionUtils.IsGenericDefinition(memberUnderlyingType, typeof(IDictionary<,>)))
                {
                    Type[] typeArguments = new Type[] { type3, type4 };
                    type5 = typeof(Dictionary<,>).MakeGenericType(typeArguments);
                }
                else
                {
                    type5 = memberUnderlyingType;
                }
                Func<object, object> getExtensionDataDictionary = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(member);
                if (attribute.ReadData)
                {
                    Action<object, object> setExtensionDataDictionary = ReflectionUtils.CanSetMemberValue(member, true, false) ? JsonTypeReflector.ReflectionDelegateFactory.CreateSet<object>(member) : null;
                    Func<object> createExtensionDataDictionary = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type5);
                    Type[] types = new Type[] { type3, type4 };
                    MethodInfo method = memberUnderlyingType.GetMethod("Add", types);
                    MethodCall<object, object> setExtensionDataDictionaryValue = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
                    ExtensionDataSetter setter = delegate (object o, string key, object value) {
                        object obj2 = getExtensionDataDictionary(o);
                        if (obj2 == null)
                        {
                            if (setExtensionDataDictionary == null)
                            {
                                throw new JsonSerializationException("Cannot set value onto extension data member '{0}'. The extension data collection is null and it cannot be set.".FormatWith(CultureInfo.InvariantCulture, member.Name));
                            }
                            obj2 = createExtensionDataDictionary();
                            setExtensionDataDictionary(o, obj2);
                        }
                        object[] args = new object[] { key, value };
                        setExtensionDataDictionaryValue(obj2, args);
                    };
                    contract.ExtensionDataSetter = setter;
                }
                if (attribute.WriteData)
                {
                    Type[] typeArguments = new Type[] { type3, type4 };
                    ConstructorInfo method = typeof(EnumerableDictionaryWrapper).MakeGenericType(typeArguments).GetConstructors().First<ConstructorInfo>();
                    ObjectConstructor<object> createEnumerableWrapper = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(method);
                    ExtensionDataGetter getter = delegate (object o) {
                        object obj2 = getExtensionDataDictionary(o);
                        if (obj2 == null)
                        {
                            return null;
                        }
                        object[] args = new object[] { obj2 };
                        return (IEnumerable<KeyValuePair<object, object>>) createEnumerableWrapper(args);
                    };
                    contract.ExtensionDataGetter = getter;
                }
                contract.ExtensionDataValueType = type4;
            }
        }

        private void SetIsSpecifiedActions(JsonProperty property, MemberInfo member, bool allowNonPublicAccess)
        {
            MemberInfo field = member.DeclaringType.GetProperty(member.Name + "Specified");
            if (field == null)
            {
                field = member.DeclaringType.GetField(member.Name + "Specified");
            }
            if ((field != null) && (ReflectionUtils.GetMemberUnderlyingType(field) == typeof(bool)))
            {
                Func<object, object> specifiedPropertyGet = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(field);
                property.GetIsSpecified = o => (bool) specifiedPropertyGet(o);
                if (ReflectionUtils.CanSetMemberValue(field, allowNonPublicAccess, false))
                {
                    property.SetIsSpecified = JsonTypeReflector.ReflectionDelegateFactory.CreateSet<object>(field);
                }
            }
        }

        private void SetPropertySettingsFromAttributes(JsonProperty property, object attributeProvider, string name, Type declaringType, MemberSerialization memberSerialization, out bool allowNonPublicAccess)
        {
            DataMemberAttribute dataMemberAttribute;
            string propertyName;
            bool flag;
            Newtonsoft.Json.Serialization.NamingStrategy containerNamingStrategy;
            MemberInfo memberInfo = attributeProvider as MemberInfo;
            if ((JsonTypeReflector.GetDataContractAttribute(declaringType) != null) && (memberInfo != null))
            {
                dataMemberAttribute = JsonTypeReflector.GetDataMemberAttribute(memberInfo);
            }
            else
            {
                dataMemberAttribute = null;
            }
            JsonPropertyAttribute attribute2 = JsonTypeReflector.GetAttribute<JsonPropertyAttribute>(attributeProvider);
            if ((attribute2 != null) && (attribute2.PropertyName != null))
            {
                propertyName = attribute2.PropertyName;
                flag = true;
            }
            else if ((dataMemberAttribute != null) && (dataMemberAttribute.Name != null))
            {
                propertyName = dataMemberAttribute.Name;
                flag = true;
            }
            else
            {
                propertyName = name;
                flag = false;
            }
            JsonContainerAttribute containerAttribute = JsonTypeReflector.GetAttribute<JsonContainerAttribute>(declaringType);
            if (attribute2?.NamingStrategyType != null)
            {
                containerNamingStrategy = JsonTypeReflector.CreateNamingStrategyInstance(attribute2.NamingStrategyType, attribute2.NamingStrategyParameters);
            }
            else if (containerAttribute?.NamingStrategyType != null)
            {
                containerNamingStrategy = JsonTypeReflector.GetContainerNamingStrategy(containerAttribute);
            }
            else
            {
                containerNamingStrategy = this.NamingStrategy;
            }
            if (containerNamingStrategy != null)
            {
                property.PropertyName = containerNamingStrategy.GetPropertyName(propertyName, flag);
            }
            else
            {
                property.PropertyName = this.ResolvePropertyName(propertyName);
            }
            property.UnderlyingName = name;
            bool flag2 = false;
            if (attribute2 != null)
            {
                property._required = attribute2._required;
                property.Order = attribute2._order;
                property.DefaultValueHandling = attribute2._defaultValueHandling;
                flag2 = true;
            }
            else if (dataMemberAttribute != null)
            {
                property._required = new Required?(dataMemberAttribute.IsRequired ? Required.AllowNull : Required.Default);
                property.Order = (dataMemberAttribute.Order != -1) ? new int?(dataMemberAttribute.Order) : null;
                property.DefaultValueHandling = !dataMemberAttribute.EmitDefaultValue ? ((DefaultValueHandling?) 1) : null;
                flag2 = true;
            }
            if (JsonTypeReflector.GetAttribute<JsonRequiredAttribute>(attributeProvider) != null)
            {
                property._required = 2;
                flag2 = true;
            }
            property.HasMemberAttribute = flag2;
            bool flag3 = ((JsonTypeReflector.GetAttribute<JsonIgnoreAttribute>(attributeProvider) != null) || (JsonTypeReflector.GetAttribute<JsonExtensionDataAttribute>(attributeProvider) != null)) || (JsonTypeReflector.GetAttribute<NonSerializedAttribute>(attributeProvider) > null);
            if (memberSerialization != MemberSerialization.OptIn)
            {
                bool flag4 = false;
                flag4 = JsonTypeReflector.GetAttribute<IgnoreDataMemberAttribute>(attributeProvider) > null;
                property.Ignored = flag3 | flag4;
            }
            else
            {
                property.Ignored = flag3 || !flag2;
            }
            property.Converter = JsonTypeReflector.GetJsonConverter(attributeProvider);
            property.MemberConverter = JsonTypeReflector.GetJsonConverter(attributeProvider);
            DefaultValueAttribute attribute4 = JsonTypeReflector.GetAttribute<DefaultValueAttribute>(attributeProvider);
            if (attribute4 != null)
            {
                property.DefaultValue = attribute4.Value;
            }
            property.NullValueHandling = (attribute2 != null) ? attribute2._nullValueHandling : null;
            property.ReferenceLoopHandling = (attribute2 != null) ? attribute2._referenceLoopHandling : null;
            property.ObjectCreationHandling = (attribute2 != null) ? attribute2._objectCreationHandling : null;
            property.TypeNameHandling = (attribute2 != null) ? attribute2._typeNameHandling : null;
            property.IsReference = (attribute2 != null) ? attribute2._isReference : null;
            property.ItemIsReference = (attribute2 != null) ? attribute2._itemIsReference : null;
            property.ItemConverter = ((attribute2 != null) && (attribute2.ItemConverterType != null)) ? JsonTypeReflector.CreateJsonConverterInstance(attribute2.ItemConverterType, attribute2.ItemConverterParameters) : null;
            property.ItemReferenceLoopHandling = (attribute2 != null) ? attribute2._itemReferenceLoopHandling : null;
            property.ItemTypeNameHandling = (attribute2 != null) ? attribute2._itemTypeNameHandling : null;
            allowNonPublicAccess = false;
            if ((this.DefaultMembersSearchFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic)
            {
                allowNonPublicAccess = true;
            }
            if (flag2)
            {
                allowNonPublicAccess = true;
            }
            if (memberSerialization == MemberSerialization.Fields)
            {
                allowNonPublicAccess = true;
            }
        }

        private bool ShouldSerializeEntityMember(MemberInfo memberInfo)
        {
            PropertyInfo info = memberInfo as PropertyInfo;
            if (((info != null) && info.PropertyType.IsGenericType()) && (info.PropertyType.GetGenericTypeDefinition().FullName == "System.Data.Objects.DataClasses.EntityReference`1"))
            {
                return false;
            }
            return true;
        }

        private static bool ShouldSkipDeserialized(Type t)
        {
            if ((!t.IsGenericType() || (t.GetGenericTypeDefinition() != typeof(ConcurrentDictionary<,>))) && ((t.Name != "FSharpSet`1") && (t.Name != "FSharpMap`2")))
            {
                return false;
            }
            return true;
        }

        private static bool ShouldSkipSerializing(Type t)
        {
            if ((t.Name != "FSharpSet`1") && (t.Name != "FSharpMap`2"))
            {
                return false;
            }
            return true;
        }

        internal static IContractResolver Instance =>
            _instance;

        public bool DynamicCodeGeneration =>
            JsonTypeReflector.DynamicCodeGeneration;

        [Obsolete("DefaultMembersSearchFlags is obsolete. To modify the members serialized inherit from DefaultContractResolver and override the GetSerializableMembers method instead.")]
        public BindingFlags DefaultMembersSearchFlags { get; set; }

        public bool SerializeCompilerGeneratedMembers { get; set; }

        public bool IgnoreSerializableInterface { get; set; }

        public bool IgnoreSerializableAttribute { get; set; }

        public Newtonsoft.Json.Serialization.NamingStrategy NamingStrategy { get; set; }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly DefaultContractResolver.<>c <>9 = new DefaultContractResolver.<>c();
            public static Func<MemberInfo, bool> <>9__34_0;
            public static Func<MemberInfo, bool> <>9__34_1;
            public static Func<Type, IEnumerable<MemberInfo>> <>9__37_0;
            public static Func<MemberInfo, bool> <>9__37_1;
            public static Func<ConstructorInfo, bool> <>9__40_0;
            public static Func<JsonProperty, int> <>9__65_0;

            internal int <CreateProperties>b__65_0(JsonProperty p)
            {
                int? order = p.Order;
                if (!order.HasValue)
                {
                    return -1;
                }
                return order.GetValueOrDefault();
            }

            internal bool <GetAttributeConstructor>b__40_0(ConstructorInfo c) => 
                c.IsDefined(typeof(JsonConstructorAttribute), true);

            internal IEnumerable<MemberInfo> <GetExtensionDataMemberForType>b__37_0(Type baseType)
            {
                List<MemberInfo> initial = new List<MemberInfo>();
                initial.AddRange<MemberInfo>(baseType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
                initial.AddRange<MemberInfo>(baseType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
                return initial;
            }

            internal bool <GetExtensionDataMemberForType>b__37_1(MemberInfo m)
            {
                MemberTypes types = m.MemberType();
                if ((types != MemberTypes.Property) && (types != MemberTypes.Field))
                {
                    return false;
                }
                if (!m.IsDefined(typeof(JsonExtensionDataAttribute), false))
                {
                    return false;
                }
                if (!ReflectionUtils.CanReadMemberValue(m, true))
                {
                    throw new JsonException("Invalid extension data attribute on '{0}'. Member '{1}' must have a getter.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(m.DeclaringType), m.Name));
                }
                if (ReflectionUtils.ImplementsGenericDefinition(ReflectionUtils.GetMemberUnderlyingType(m), typeof(IDictionary<,>), out Type type))
                {
                    Type type2 = type.GetGenericArguments()[1];
                    if (type.GetGenericArguments()[0].IsAssignableFrom(typeof(string)) && type2.IsAssignableFrom(typeof(JToken)))
                    {
                        return true;
                    }
                }
                throw new JsonException("Invalid extension data attribute on '{0}'. Member '{1}' type must implement IDictionary<string, JToken>.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(m.DeclaringType), m.Name));
            }

            internal bool <GetSerializableMembers>b__34_0(MemberInfo m) => 
                !ReflectionUtils.IsIndexedProperty(m);

            internal bool <GetSerializableMembers>b__34_1(MemberInfo m) => 
                !ReflectionUtils.IsIndexedProperty(m);
        }

        internal class EnumerableDictionaryWrapper<TEnumeratorKey, TEnumeratorValue> : IEnumerable<KeyValuePair<object, object>>, IEnumerable
        {
            private readonly IEnumerable<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> _e;

            public EnumerableDictionaryWrapper(IEnumerable<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> e)
            {
                ValidationUtils.ArgumentNotNull(e, "e");
                this._e = e;
            }

            [IteratorStateMachine(typeof(<GetEnumerator>d__2<,>))]
            public IEnumerator<KeyValuePair<object, object>> GetEnumerator() => 
                new <GetEnumerator>d__2<TEnumeratorKey, TEnumeratorValue>(0) { <>4__this = (DefaultContractResolver.EnumerableDictionaryWrapper<TEnumeratorKey, TEnumeratorValue>) this };

            IEnumerator IEnumerable.GetEnumerator() => 
                this.GetEnumerator();

            [CompilerGenerated]
            private sealed class <GetEnumerator>d__2 : IEnumerator<KeyValuePair<object, object>>, IDisposable, IEnumerator
            {
                private int <>1__state;
                private KeyValuePair<object, object> <>2__current;
                public DefaultContractResolver.EnumerableDictionaryWrapper<TEnumeratorKey, TEnumeratorValue> <>4__this;
                private IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> <>7__wrap1;

                [DebuggerHidden]
                public <GetEnumerator>d__2(int <>1__state)
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
                            this.<>7__wrap1 = this.<>4__this._e.GetEnumerator();
                            this.<>1__state = -3;
                            while (this.<>7__wrap1.MoveNext())
                            {
                                KeyValuePair<TEnumeratorKey, TEnumeratorValue> current = this.<>7__wrap1.Current;
                                this.<>2__current = new KeyValuePair<object, object>(current.Key, current.Value);
                                this.<>1__state = 1;
                                return true;
                            Label_0082:
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
                        goto Label_0082;
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

                KeyValuePair<object, object> IEnumerator<KeyValuePair<object, object>>.Current =>
                    this.<>2__current;

                object IEnumerator.Current =>
                    this.<>2__current;
            }
        }
    }
}

