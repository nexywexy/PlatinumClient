namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;

    internal static class JsonTypeReflector
    {
        private static bool? _dynamicCodeGeneration;
        private static bool? _fullyTrusted;
        public const string IdPropertyName = "$id";
        public const string RefPropertyName = "$ref";
        public const string TypePropertyName = "$type";
        public const string ValuePropertyName = "$value";
        public const string ArrayValuesPropertyName = "$values";
        public const string ShouldSerializePrefix = "ShouldSerialize";
        public const string SpecifiedPostfix = "Specified";
        private static readonly ThreadSafeStore<Type, Func<object[], object>> CreatorCache = new ThreadSafeStore<Type, Func<object[], object>>(new Func<Type, Func<object[], object>>(JsonTypeReflector.GetCreator));
        private static readonly ThreadSafeStore<Type, Type> AssociatedMetadataTypesCache = new ThreadSafeStore<Type, Type>(new Func<Type, Type>(JsonTypeReflector.GetAssociateMetadataTypeFromAttribute));
        private static ReflectionObject _metadataTypeAttributeReflectionObject;

        public static JsonConverter CreateJsonConverterInstance(Type converterType, object[] converterArgs) => 
            ((JsonConverter) CreatorCache.Get(converterType)(converterArgs));

        public static NamingStrategy CreateNamingStrategyInstance(Type namingStrategyType, object[] converterArgs) => 
            ((NamingStrategy) CreatorCache.Get(namingStrategyType)(converterArgs));

        private static Type GetAssociatedMetadataType(Type type) => 
            AssociatedMetadataTypesCache.Get(type);

        private static Type GetAssociateMetadataTypeFromAttribute(Type type)
        {
            foreach (Attribute attribute in ReflectionUtils.GetAttributes(type, null, true))
            {
                Type t = attribute.GetType();
                if (string.Equals(t.FullName, "System.ComponentModel.DataAnnotations.MetadataTypeAttribute", StringComparison.Ordinal))
                {
                    if (_metadataTypeAttributeReflectionObject == null)
                    {
                        string[] memberNames = new string[] { "MetadataClassType" };
                        _metadataTypeAttributeReflectionObject = ReflectionObject.Create(t, memberNames);
                    }
                    return (Type) _metadataTypeAttributeReflectionObject.GetValue(attribute, "MetadataClassType");
                }
            }
            return null;
        }

        public static T GetAttribute<T>(object provider) where T: Attribute
        {
            Type type = provider as Type;
            if (type != null)
            {
                return GetAttribute<T>(type);
            }
            MemberInfo memberInfo = provider as MemberInfo;
            if (memberInfo != null)
            {
                return GetAttribute<T>(memberInfo);
            }
            return ReflectionUtils.GetAttribute<T>(provider, true);
        }

        private static T GetAttribute<T>(MemberInfo memberInfo) where T: Attribute
        {
            T attribute;
            Type associatedMetadataType = GetAssociatedMetadataType(memberInfo.DeclaringType);
            if (associatedMetadataType != null)
            {
                MemberInfo memberInfoFromType = ReflectionUtils.GetMemberInfoFromType(associatedMetadataType, memberInfo);
                if (memberInfoFromType != null)
                {
                    attribute = ReflectionUtils.GetAttribute<T>(memberInfoFromType, true);
                    if (attribute != null)
                    {
                        return attribute;
                    }
                }
            }
            attribute = ReflectionUtils.GetAttribute<T>(memberInfo, true);
            if (attribute != null)
            {
                return attribute;
            }
            if (memberInfo.DeclaringType != null)
            {
                Type[] interfaces = memberInfo.DeclaringType.GetInterfaces();
                for (int i = 0; i < interfaces.Length; i++)
                {
                    MemberInfo memberInfoFromType = ReflectionUtils.GetMemberInfoFromType(interfaces[i], memberInfo);
                    if (memberInfoFromType != null)
                    {
                        attribute = ReflectionUtils.GetAttribute<T>(memberInfoFromType, true);
                        if (attribute != null)
                        {
                            return attribute;
                        }
                    }
                }
            }
            return default(T);
        }

        private static T GetAttribute<T>(Type type) where T: Attribute
        {
            T attribute;
            Type associatedMetadataType = GetAssociatedMetadataType(type);
            if (associatedMetadataType != null)
            {
                attribute = ReflectionUtils.GetAttribute<T>(associatedMetadataType, true);
                if (attribute != null)
                {
                    return attribute;
                }
            }
            attribute = ReflectionUtils.GetAttribute<T>(type, true);
            if (attribute != null)
            {
                return attribute;
            }
            Type[] interfaces = type.GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                attribute = ReflectionUtils.GetAttribute<T>(interfaces[i], true);
                if (attribute != null)
                {
                    return attribute;
                }
            }
            return default(T);
        }

        public static T GetCachedAttribute<T>(object attributeProvider) where T: Attribute => 
            CachedAttributeGetter<T>.GetAttribute(attributeProvider);

        public static NamingStrategy GetContainerNamingStrategy(JsonContainerAttribute containerAttribute)
        {
            if (containerAttribute.NamingStrategyInstance == null)
            {
                if (containerAttribute.NamingStrategyType == null)
                {
                    return null;
                }
                containerAttribute.NamingStrategyInstance = CreateNamingStrategyInstance(containerAttribute.NamingStrategyType, containerAttribute.NamingStrategyParameters);
            }
            return containerAttribute.NamingStrategyInstance;
        }

        private static Func<object[], object> GetCreator(Type type)
        {
            Func<object> defaultConstructor = ReflectionUtils.HasDefaultConstructor(type, false) ? ReflectionDelegateFactory.CreateDefaultConstructor<object>(type) : null;
            return delegate (object[] parameters) {
                object obj2;
                try
                {
                    if (parameters != null)
                    {
                        if (<>c.<>9__20_1 == null)
                        {
                        }
                        Type[] types = parameters.Select<object, Type>((<>c.<>9__20_1 = new Func<object, Type>(<>c.<>9.<GetCreator>b__20_1))).ToArray<Type>();
                        ConstructorInfo constructor = type.GetConstructor(types);
                        if (null == constructor)
                        {
                            throw new JsonException("No matching parameterized constructor found for '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
                        }
                        return ReflectionDelegateFactory.CreateParameterizedConstructor(constructor)(parameters);
                    }
                    if (defaultConstructor == null)
                    {
                        throw new JsonException("No parameterless constructor defined for '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
                    }
                    obj2 = defaultConstructor();
                }
                catch (Exception exception)
                {
                    throw new JsonException("Error creating '{0}'.".FormatWith(CultureInfo.InvariantCulture, type), exception);
                }
                return obj2;
            };
        }

        public static DataContractAttribute GetDataContractAttribute(Type type)
        {
            for (Type type2 = type; type2 != null; type2 = type2.BaseType())
            {
                DataContractAttribute attribute = CachedAttributeGetter<DataContractAttribute>.GetAttribute(type2);
                if (attribute != null)
                {
                    return attribute;
                }
            }
            return null;
        }

        public static DataMemberAttribute GetDataMemberAttribute(MemberInfo memberInfo)
        {
            if (memberInfo.MemberType() == MemberTypes.Field)
            {
                return CachedAttributeGetter<DataMemberAttribute>.GetAttribute(memberInfo);
            }
            PropertyInfo info = (PropertyInfo) memberInfo;
            DataMemberAttribute attribute = CachedAttributeGetter<DataMemberAttribute>.GetAttribute(info);
            if ((attribute == null) && info.IsVirtual())
            {
                for (Type type = info.DeclaringType; (attribute == null) && (type != null); type = type.BaseType())
                {
                    PropertyInfo memberInfoFromType = (PropertyInfo) ReflectionUtils.GetMemberInfoFromType(type, info);
                    if ((memberInfoFromType != null) && memberInfoFromType.IsVirtual())
                    {
                        attribute = CachedAttributeGetter<DataMemberAttribute>.GetAttribute(memberInfoFromType);
                    }
                }
            }
            return attribute;
        }

        public static JsonConverter GetJsonConverter(object attributeProvider)
        {
            JsonConverterAttribute cachedAttribute = GetCachedAttribute<JsonConverterAttribute>(attributeProvider);
            if (cachedAttribute != null)
            {
                Func<object[], object> func = CreatorCache.Get(cachedAttribute.ConverterType);
                if (func != null)
                {
                    return (JsonConverter) func(cachedAttribute.ConverterParameters);
                }
            }
            return null;
        }

        public static MemberSerialization GetObjectMemberSerialization(Type objectType, bool ignoreSerializableAttribute)
        {
            JsonObjectAttribute cachedAttribute = GetCachedAttribute<JsonObjectAttribute>(objectType);
            if (cachedAttribute != null)
            {
                return cachedAttribute.MemberSerialization;
            }
            if (GetDataContractAttribute(objectType) != null)
            {
                return MemberSerialization.OptIn;
            }
            if (!ignoreSerializableAttribute && (GetCachedAttribute<SerializableAttribute>(objectType) != null))
            {
                return MemberSerialization.Fields;
            }
            return MemberSerialization.OptOut;
        }

        public static TypeConverter GetTypeConverter(Type type) => 
            TypeDescriptor.GetConverter(type);

        public static bool DynamicCodeGeneration
        {
            [SecuritySafeCritical]
            get
            {
                if (!_dynamicCodeGeneration.HasValue)
                {
                    try
                    {
                        new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Demand();
                        new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess).Demand();
                        new SecurityPermission(SecurityPermissionFlag.SkipVerification).Demand();
                        new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                        new SecurityPermission(PermissionState.Unrestricted).Demand();
                        _dynamicCodeGeneration = true;
                    }
                    catch (Exception)
                    {
                        _dynamicCodeGeneration = false;
                    }
                }
                return _dynamicCodeGeneration.GetValueOrDefault();
            }
        }

        public static bool FullyTrusted
        {
            get
            {
                if (!_fullyTrusted.HasValue)
                {
                    AppDomain currentDomain = AppDomain.CurrentDomain;
                    _fullyTrusted = new bool?(currentDomain.IsHomogenous && currentDomain.IsFullyTrusted);
                }
                return _fullyTrusted.GetValueOrDefault();
            }
        }

        public static Newtonsoft.Json.Utilities.ReflectionDelegateFactory ReflectionDelegateFactory
        {
            get
            {
                if (DynamicCodeGeneration)
                {
                    return DynamicReflectionDelegateFactory.Instance;
                }
                return LateBoundReflectionDelegateFactory.Instance;
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly JsonTypeReflector.<>c <>9 = new JsonTypeReflector.<>c();
            public static Func<object, Type> <>9__20_1;

            internal Type <GetCreator>b__20_1(object param) => 
                param.GetType();
        }
    }
}

