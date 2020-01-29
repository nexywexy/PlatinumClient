namespace Newtonsoft.Json.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Numerics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Text;

    internal static class ReflectionUtils
    {
        public static readonly Type[] EmptyTypes = Type.EmptyTypes;

        public static bool CanReadMemberValue(MemberInfo member, bool nonPublic)
        {
            MemberTypes types = member.MemberType();
            if (types != MemberTypes.Field)
            {
                if (types != MemberTypes.Property)
                {
                    return false;
                }
            }
            else
            {
                FieldInfo info = (FieldInfo) member;
                return (nonPublic || info.IsPublic);
            }
            PropertyInfo info2 = (PropertyInfo) member;
            if (!info2.CanRead)
            {
                return false;
            }
            return (nonPublic || (info2.GetGetMethod(nonPublic) != null));
        }

        public static bool CanSetMemberValue(MemberInfo member, bool nonPublic, bool canSetReadOnly)
        {
            MemberTypes types = member.MemberType();
            if (types != MemberTypes.Field)
            {
                if (types != MemberTypes.Property)
                {
                    return false;
                }
            }
            else
            {
                FieldInfo info = (FieldInfo) member;
                if (!info.IsLiteral)
                {
                    if (info.IsInitOnly && !canSetReadOnly)
                    {
                        return false;
                    }
                    if (nonPublic)
                    {
                        return true;
                    }
                    if (info.IsPublic)
                    {
                        return true;
                    }
                }
                return false;
            }
            PropertyInfo info2 = (PropertyInfo) member;
            if (!info2.CanWrite)
            {
                return false;
            }
            return (nonPublic || (info2.GetSetMethod(nonPublic) != null));
        }

        public static Type EnsureNotNullableType(Type t)
        {
            if (!IsNullableType(t))
            {
                return t;
            }
            return Nullable.GetUnderlyingType(t);
        }

        private static int? GetAssemblyDelimiterIndex(string fullyQualifiedTypeName)
        {
            int num = 0;
            for (int i = 0; i < fullyQualifiedTypeName.Length; i++)
            {
                switch (fullyQualifiedTypeName[i])
                {
                    case ',':
                        if (num == 0)
                        {
                            return new int?(i);
                        }
                        break;

                    case '[':
                        num++;
                        break;

                    case ']':
                        num--;
                        break;
                }
            }
            return null;
        }

        public static T GetAttribute<T>(object attributeProvider) where T: Attribute => 
            GetAttribute<T>(attributeProvider, true);

        public static T GetAttribute<T>(object attributeProvider, bool inherit) where T: Attribute
        {
            T[] attributes = GetAttributes<T>(attributeProvider, inherit);
            if (attributes == null)
            {
                return default(T);
            }
            return attributes.FirstOrDefault<T>();
        }

        public static T[] GetAttributes<T>(object attributeProvider, bool inherit) where T: Attribute
        {
            Attribute[] source = GetAttributes(attributeProvider, typeof(T), inherit);
            T[] localArray = source as T[];
            if (localArray != null)
            {
                return localArray;
            }
            return source.Cast<T>().ToArray<T>();
        }

        public static Attribute[] GetAttributes(object attributeProvider, Type attributeType, bool inherit)
        {
            ValidationUtils.ArgumentNotNull(attributeProvider, "attributeProvider");
            object obj2 = attributeProvider;
            switch (obj2)
            {
                case (Type _):
                {
                    Type type = (Type) obj2;
                    return ((attributeType != null) ? ((IEnumerable) type.GetCustomAttributes(attributeType, inherit)) : ((IEnumerable) type.GetCustomAttributes(inherit))).Cast<Attribute>().ToArray<Attribute>();
                    break;
                }
            }
            if (obj2 is Assembly)
            {
                Assembly element = (Assembly) obj2;
                if (attributeType == null)
                {
                    return Attribute.GetCustomAttributes(element);
                }
                return Attribute.GetCustomAttributes(element, attributeType);
            }
            if (obj2 is MemberInfo)
            {
                MemberInfo element = (MemberInfo) obj2;
                if (attributeType == null)
                {
                    return Attribute.GetCustomAttributes(element, inherit);
                }
                return Attribute.GetCustomAttributes(element, attributeType, inherit);
            }
            if (obj2 is Module)
            {
                Module element = (Module) obj2;
                if (attributeType == null)
                {
                    return Attribute.GetCustomAttributes(element, inherit);
                }
                return Attribute.GetCustomAttributes(element, attributeType, inherit);
            }
            if (obj2 is ParameterInfo)
            {
                ParameterInfo element = (ParameterInfo) obj2;
                if (attributeType == null)
                {
                    return Attribute.GetCustomAttributes(element, inherit);
                }
                return Attribute.GetCustomAttributes(element, attributeType, inherit);
            }
            ICustomAttributeProvider provider = (ICustomAttributeProvider) attributeProvider;
            return ((attributeType != null) ? ((Attribute[]) provider.GetCustomAttributes(attributeType, inherit)) : ((Attribute[]) provider.GetCustomAttributes(inherit)));
        }

        public static MethodInfo GetBaseDefinition(this PropertyInfo propertyInfo)
        {
            ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");
            MethodInfo getMethod = propertyInfo.GetGetMethod();
            if (getMethod != null)
            {
                return getMethod.GetBaseDefinition();
            }
            getMethod = propertyInfo.GetSetMethod();
            if (getMethod != null)
            {
                return getMethod.GetBaseDefinition();
            }
            return null;
        }

        private static void GetChildPrivateFields(IList<MemberInfo> initialFields, Type targetType, BindingFlags bindingAttr)
        {
            if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
            {
                BindingFlags flags = bindingAttr.RemoveFlag(BindingFlags.Public);
                while ((targetType = targetType.BaseType()) != null)
                {
                    if (<>c.<>9__39_0 == null)
                    {
                    }
                    IEnumerable<MemberInfo> collection = targetType.GetFields(flags).Where<FieldInfo>((<>c.<>9__39_0 = new Func<FieldInfo, bool>(<>c.<>9.<GetChildPrivateFields>b__39_0))).Cast<MemberInfo>();
                    initialFields.AddRange<MemberInfo>(collection);
                }
            }
        }

        private static void GetChildPrivateProperties(IList<PropertyInfo> initialProperties, Type targetType, BindingFlags bindingAttr)
        {
            while ((targetType = targetType.BaseType()) != null)
            {
                foreach (PropertyInfo info in targetType.GetProperties(bindingAttr))
                {
                    if (!IsPublic(info))
                    {
                        int index = initialProperties.IndexOf<PropertyInfo>(p => p.Name == info.Name);
                        if (index == -1)
                        {
                            initialProperties.Add(info);
                        }
                        else if (!IsPublic(initialProperties[index]))
                        {
                            initialProperties[index] = info;
                        }
                    }
                    else if (!info.IsVirtual())
                    {
                        if (initialProperties.IndexOf<PropertyInfo>(p => ((p.Name == info.Name) && (p.DeclaringType == info.DeclaringType))) == -1)
                        {
                            initialProperties.Add(info);
                        }
                    }
                    else if (initialProperties.IndexOf<PropertyInfo>(p => ((((p.Name == info.Name) && p.IsVirtual()) && (p.GetBaseDefinition() != null)) && p.GetBaseDefinition().DeclaringType.IsAssignableFrom(info.GetBaseDefinition().DeclaringType))) == -1)
                    {
                        initialProperties.Add(info);
                    }
                }
            }
        }

        public static Type GetCollectionItemType(Type type)
        {
            ValidationUtils.ArgumentNotNull(type, "type");
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            if (ImplementsGenericDefinition(type, typeof(IEnumerable<>), out Type type2))
            {
                if (type2.IsGenericTypeDefinition())
                {
                    throw new Exception("Type {0} is not a collection.".FormatWith(CultureInfo.InvariantCulture, type));
                }
                return type2.GetGenericArguments()[0];
            }
            if (!typeof(IEnumerable).IsAssignableFrom(type))
            {
                throw new Exception("Type {0} is not a collection.".FormatWith(CultureInfo.InvariantCulture, type));
            }
            return null;
        }

        public static ConstructorInfo GetDefaultConstructor(Type t) => 
            GetDefaultConstructor(t, false);

        public static ConstructorInfo GetDefaultConstructor(Type t, bool nonPublic)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            if (nonPublic)
            {
                bindingAttr |= BindingFlags.NonPublic;
            }
            if (<>c.<>9__10_0 == null)
            {
            }
            return t.GetConstructors(bindingAttr).SingleOrDefault<ConstructorInfo>((<>c.<>9__10_0 = new Func<ConstructorInfo, bool>(<>c.<>9.<GetDefaultConstructor>b__10_0)));
        }

        public static object GetDefaultValue(Type type)
        {
            if (!type.IsValueType())
            {
                return null;
            }
            switch (ConvertUtils.GetTypeCode(type))
            {
                case PrimitiveTypeCode.Char:
                case PrimitiveTypeCode.SByte:
                case PrimitiveTypeCode.Int16:
                case PrimitiveTypeCode.UInt16:
                case PrimitiveTypeCode.Int32:
                case PrimitiveTypeCode.Byte:
                case PrimitiveTypeCode.UInt32:
                    return 0;

                case PrimitiveTypeCode.Boolean:
                    return false;

                case PrimitiveTypeCode.Int64:
                case PrimitiveTypeCode.UInt64:
                    return 0L;

                case PrimitiveTypeCode.Single:
                    return 0f;

                case PrimitiveTypeCode.Double:
                    return 0.0;

                case PrimitiveTypeCode.DateTime:
                    return new DateTime();

                case PrimitiveTypeCode.DateTimeOffset:
                    return new DateTimeOffset();

                case PrimitiveTypeCode.Decimal:
                    return decimal.Zero;

                case PrimitiveTypeCode.Guid:
                    return new Guid();

                case PrimitiveTypeCode.BigInteger:
                    return new BigInteger();
            }
            if (IsNullable(type))
            {
                return null;
            }
            return Activator.CreateInstance(type);
        }

        public static void GetDictionaryKeyValueTypes(Type dictionaryType, out Type keyType, out Type valueType)
        {
            ValidationUtils.ArgumentNotNull(dictionaryType, "dictionaryType");
            if (ImplementsGenericDefinition(dictionaryType, typeof(IDictionary<,>), out Type type))
            {
                if (type.IsGenericTypeDefinition())
                {
                    throw new Exception("Type {0} is not a dictionary.".FormatWith(CultureInfo.InvariantCulture, dictionaryType));
                }
                Type[] genericArguments = type.GetGenericArguments();
                keyType = genericArguments[0];
                valueType = genericArguments[1];
            }
            else
            {
                if (!typeof(IDictionary).IsAssignableFrom(dictionaryType))
                {
                    throw new Exception("Type {0} is not a dictionary.".FormatWith(CultureInfo.InvariantCulture, dictionaryType));
                }
                keyType = null;
                valueType = null;
            }
        }

        public static IEnumerable<FieldInfo> GetFields(Type targetType, BindingFlags bindingAttr)
        {
            ValidationUtils.ArgumentNotNull(targetType, "targetType");
            List<MemberInfo> initialFields = new List<MemberInfo>(targetType.GetFields(bindingAttr));
            GetChildPrivateFields(initialFields, targetType, bindingAttr);
            return initialFields.Cast<FieldInfo>();
        }

        public static List<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags bindingAttr)
        {
            List<MemberInfo> source = new List<MemberInfo>();
            source.AddRange(GetFields(type, bindingAttr));
            source.AddRange(GetProperties(type, bindingAttr));
            List<MemberInfo> list = new List<MemberInfo>(source.Count);
            if (<>c.<>9__29_0 == null)
            {
            }
            foreach (IGrouping<string, MemberInfo> local1 in source.GroupBy<MemberInfo, string>(<>c.<>9__29_0 = new Func<MemberInfo, string>(<>c.<>9.<GetFieldsAndProperties>b__29_0)))
            {
                int num = local1.Count<MemberInfo>();
                IList<MemberInfo> list2 = local1.ToList<MemberInfo>();
                if (num == 1)
                {
                    list.Add(list2.First<MemberInfo>());
                }
                else
                {
                    IList<MemberInfo> collection = new List<MemberInfo>();
                    foreach (MemberInfo info in list2)
                    {
                        if (collection.Count == 0)
                        {
                            collection.Add(info);
                        }
                        else if (!IsOverridenGenericMember(info, bindingAttr) || (info.Name == "Item"))
                        {
                            collection.Add(info);
                        }
                    }
                    list.AddRange(collection);
                }
            }
            return list;
        }

        public static MemberInfo GetMemberInfoFromType(Type targetType, MemberInfo memberInfo)
        {
            if (memberInfo.MemberType() != MemberTypes.Property)
            {
                return targetType.GetMember(memberInfo.Name, memberInfo.MemberType(), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).SingleOrDefault<MemberInfo>();
            }
            PropertyInfo info = (PropertyInfo) memberInfo;
            if (<>c.<>9__37_0 == null)
            {
            }
            Type[] types = info.GetIndexParameters().Select<ParameterInfo, Type>((<>c.<>9__37_0 = new Func<ParameterInfo, Type>(<>c.<>9.<GetMemberInfoFromType>b__37_0))).ToArray<Type>();
            return targetType.GetProperty(info.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, info.PropertyType, types, null);
        }

        public static Type GetMemberUnderlyingType(MemberInfo member)
        {
            ValidationUtils.ArgumentNotNull(member, "member");
            switch (member.MemberType())
            {
                case MemberTypes.Event:
                    return ((EventInfo) member).EventHandlerType;

                case MemberTypes.Field:
                    return ((FieldInfo) member).FieldType;

                case MemberTypes.Method:
                    return ((MethodInfo) member).ReturnType;

                case MemberTypes.Property:
                    return ((PropertyInfo) member).PropertyType;
            }
            throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo, EventInfo or MethodInfo", "member");
        }

        public static object GetMemberValue(MemberInfo member, object target)
        {
            ValidationUtils.ArgumentNotNull(member, "member");
            ValidationUtils.ArgumentNotNull(target, "target");
            switch (member.MemberType())
            {
                case MemberTypes.Field:
                    return ((FieldInfo) member).GetValue(target);

                case MemberTypes.Property:
                    try
                    {
                        return ((PropertyInfo) member).GetValue(target, null);
                    }
                    catch (TargetParameterCountException exception)
                    {
                        throw new ArgumentException("MemberInfo '{0}' has index parameters".FormatWith(CultureInfo.InvariantCulture, member.Name), exception);
                    }
                    break;
            }
            throw new ArgumentException("MemberInfo '{0}' is not of type FieldInfo or PropertyInfo".FormatWith(CultureInfo.InvariantCulture, CultureInfo.InvariantCulture, member.Name), "member");
        }

        public static Type GetObjectType(object v) => 
            v?.GetType();

        public static IEnumerable<PropertyInfo> GetProperties(Type targetType, BindingFlags bindingAttr)
        {
            ValidationUtils.ArgumentNotNull(targetType, "targetType");
            List<PropertyInfo> initialProperties = new List<PropertyInfo>(targetType.GetProperties(bindingAttr));
            if (targetType.IsInterface())
            {
                foreach (Type type in targetType.GetInterfaces())
                {
                    initialProperties.AddRange(type.GetProperties(bindingAttr));
                }
            }
            GetChildPrivateProperties(initialProperties, targetType, bindingAttr);
            for (int i = 0; i < initialProperties.Count; i++)
            {
                PropertyInfo memberInfo = initialProperties[i];
                if (memberInfo.DeclaringType != targetType)
                {
                    PropertyInfo memberInfoFromType = (PropertyInfo) GetMemberInfoFromType(memberInfo.DeclaringType, memberInfo);
                    initialProperties[i] = memberInfoFromType;
                }
            }
            return initialProperties;
        }

        public static string GetTypeName(Type t, FormatterAssemblyStyle assemblyFormat, SerializationBinder binder)
        {
            string assemblyQualifiedName;
            if (binder != null)
            {
                binder.BindToName(t, out string str, out string str2);
                assemblyQualifiedName = str2 + ((str == null) ? "" : (", " + str));
            }
            else
            {
                assemblyQualifiedName = t.AssemblyQualifiedName;
            }
            if (assemblyFormat != FormatterAssemblyStyle.Simple)
            {
                if (assemblyFormat != FormatterAssemblyStyle.Full)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return assemblyQualifiedName;
            }
            return RemoveAssemblyDetails(assemblyQualifiedName);
        }

        public static bool HasDefaultConstructor(Type t, bool nonPublic)
        {
            ValidationUtils.ArgumentNotNull(t, "t");
            return (t.IsValueType() || (GetDefaultConstructor(t, nonPublic) != null));
        }

        public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition) => 
            ImplementsGenericDefinition(type, genericInterfaceDefinition, out _);

        public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition, out Type implementingType)
        {
            ValidationUtils.ArgumentNotNull(type, "type");
            ValidationUtils.ArgumentNotNull(genericInterfaceDefinition, "genericInterfaceDefinition");
            if (!genericInterfaceDefinition.IsInterface() || !genericInterfaceDefinition.IsGenericTypeDefinition())
            {
                throw new ArgumentNullException("'{0}' is not a generic interface definition.".FormatWith(CultureInfo.InvariantCulture, genericInterfaceDefinition));
            }
            if (type.IsInterface() && type.IsGenericType())
            {
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                if (genericInterfaceDefinition == genericTypeDefinition)
                {
                    implementingType = type;
                    return true;
                }
            }
            foreach (Type type3 in type.GetInterfaces())
            {
                if (type3.IsGenericType())
                {
                    Type genericTypeDefinition = type3.GetGenericTypeDefinition();
                    if (genericInterfaceDefinition == genericTypeDefinition)
                    {
                        implementingType = type3;
                        return true;
                    }
                }
            }
            implementingType = null;
            return false;
        }

        public static bool InheritsGenericDefinition(Type type, Type genericClassDefinition) => 
            InheritsGenericDefinition(type, genericClassDefinition, out _);

        public static bool InheritsGenericDefinition(Type type, Type genericClassDefinition, out Type implementingType)
        {
            ValidationUtils.ArgumentNotNull(type, "type");
            ValidationUtils.ArgumentNotNull(genericClassDefinition, "genericClassDefinition");
            if (!genericClassDefinition.IsClass() || !genericClassDefinition.IsGenericTypeDefinition())
            {
                throw new ArgumentNullException("'{0}' is not a generic class definition.".FormatWith(CultureInfo.InvariantCulture, genericClassDefinition));
            }
            return InheritsGenericDefinitionInternal(type, genericClassDefinition, out implementingType);
        }

        private static bool InheritsGenericDefinitionInternal(Type currentType, Type genericClassDefinition, out Type implementingType)
        {
            if (currentType.IsGenericType())
            {
                Type genericTypeDefinition = currentType.GetGenericTypeDefinition();
                if (genericClassDefinition == genericTypeDefinition)
                {
                    implementingType = currentType;
                    return true;
                }
            }
            if (currentType.BaseType() == null)
            {
                implementingType = null;
                return false;
            }
            return InheritsGenericDefinitionInternal(currentType.BaseType(), genericClassDefinition, out implementingType);
        }

        public static bool IsGenericDefinition(Type type, Type genericInterfaceDefinition)
        {
            if (!type.IsGenericType())
            {
                return false;
            }
            return (type.GetGenericTypeDefinition() == genericInterfaceDefinition);
        }

        public static bool IsIndexedProperty(MemberInfo member)
        {
            ValidationUtils.ArgumentNotNull(member, "member");
            PropertyInfo property = member as PropertyInfo;
            return ((property != null) && IsIndexedProperty(property));
        }

        public static bool IsIndexedProperty(PropertyInfo property)
        {
            ValidationUtils.ArgumentNotNull(property, "property");
            return (property.GetIndexParameters().Length > 0);
        }

        public static bool IsMethodOverridden(Type currentType, Type methodDeclaringType, string method) => 
            currentType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Any<MethodInfo>(info => (((info.Name == method) && (info.DeclaringType != methodDeclaringType)) && (info.GetBaseDefinition().DeclaringType == methodDeclaringType)));

        public static bool IsNullable(Type t)
        {
            ValidationUtils.ArgumentNotNull(t, "t");
            if (t.IsValueType())
            {
                return IsNullableType(t);
            }
            return true;
        }

        public static bool IsNullableType(Type t)
        {
            ValidationUtils.ArgumentNotNull(t, "t");
            return (t.IsGenericType() && (t.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        private static bool IsOverridenGenericMember(MemberInfo memberInfo, BindingFlags bindingAttr)
        {
            if (memberInfo.MemberType() != MemberTypes.Property)
            {
                return false;
            }
            PropertyInfo propertyInfo = (PropertyInfo) memberInfo;
            if (!propertyInfo.IsVirtual())
            {
                return false;
            }
            Type declaringType = propertyInfo.DeclaringType;
            if (!declaringType.IsGenericType())
            {
                return false;
            }
            Type genericTypeDefinition = declaringType.GetGenericTypeDefinition();
            if (genericTypeDefinition == null)
            {
                return false;
            }
            MemberInfo[] member = genericTypeDefinition.GetMember(propertyInfo.Name, bindingAttr);
            if (member.Length == 0)
            {
                return false;
            }
            if (!GetMemberUnderlyingType(member[0]).IsGenericParameter)
            {
                return false;
            }
            return true;
        }

        public static bool IsPublic(PropertyInfo property) => 
            (((property.GetGetMethod() != null) && property.GetGetMethod().IsPublic) || ((property.GetSetMethod() != null) && property.GetSetMethod().IsPublic));

        public static bool IsVirtual(this PropertyInfo propertyInfo)
        {
            ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");
            MethodInfo getMethod = propertyInfo.GetGetMethod();
            if ((getMethod != null) && getMethod.IsVirtual)
            {
                return true;
            }
            getMethod = propertyInfo.GetSetMethod();
            return ((getMethod != null) && getMethod.IsVirtual);
        }

        private static string RemoveAssemblyDetails(string fullyQualifiedTypeName)
        {
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            bool flag2 = false;
            for (int i = 0; i < fullyQualifiedTypeName.Length; i++)
            {
                char ch = fullyQualifiedTypeName[i];
                switch (ch)
                {
                    case ',':
                        if (!flag)
                        {
                            flag = true;
                            builder.Append(ch);
                        }
                        else
                        {
                            flag2 = true;
                        }
                        break;

                    case '[':
                        flag = false;
                        flag2 = false;
                        builder.Append(ch);
                        break;

                    case ']':
                        flag = false;
                        flag2 = false;
                        builder.Append(ch);
                        break;

                    default:
                        if (!flag2)
                        {
                            builder.Append(ch);
                        }
                        break;
                }
            }
            return builder.ToString();
        }

        public static BindingFlags RemoveFlag(this BindingFlags bindingAttr, BindingFlags flag)
        {
            if ((bindingAttr & flag) != flag)
            {
                return bindingAttr;
            }
            return (bindingAttr ^ flag);
        }

        public static void SetMemberValue(MemberInfo member, object target, object value)
        {
            ValidationUtils.ArgumentNotNull(member, "member");
            ValidationUtils.ArgumentNotNull(target, "target");
            MemberTypes types = member.MemberType();
            if (types != MemberTypes.Field)
            {
                if (types != MemberTypes.Property)
                {
                    throw new ArgumentException("MemberInfo '{0}' must be of type FieldInfo or PropertyInfo".FormatWith(CultureInfo.InvariantCulture, member.Name), "member");
                }
            }
            else
            {
                ((FieldInfo) member).SetValue(target, value);
                return;
            }
            ((PropertyInfo) member).SetValue(target, value, null);
        }

        public static void SplitFullyQualifiedTypeName(string fullyQualifiedTypeName, out string typeName, out string assemblyName)
        {
            int? assemblyDelimiterIndex = GetAssemblyDelimiterIndex(fullyQualifiedTypeName);
            if (assemblyDelimiterIndex.HasValue)
            {
                typeName = fullyQualifiedTypeName.Substring(0, assemblyDelimiterIndex.GetValueOrDefault()).Trim();
                assemblyName = fullyQualifiedTypeName.Substring(assemblyDelimiterIndex.GetValueOrDefault() + 1, (fullyQualifiedTypeName.Length - assemblyDelimiterIndex.GetValueOrDefault()) - 1).Trim();
            }
            else
            {
                typeName = fullyQualifiedTypeName;
                assemblyName = null;
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly ReflectionUtils.<>c <>9 = new ReflectionUtils.<>c();
            public static Func<ConstructorInfo, bool> <>9__10_0;
            public static Func<MemberInfo, string> <>9__29_0;
            public static Func<ParameterInfo, Type> <>9__37_0;
            public static Func<FieldInfo, bool> <>9__39_0;

            internal bool <GetChildPrivateFields>b__39_0(FieldInfo f) => 
                f.IsPrivate;

            internal bool <GetDefaultConstructor>b__10_0(ConstructorInfo c) => 
                !c.GetParameters().Any<ParameterInfo>();

            internal string <GetFieldsAndProperties>b__29_0(MemberInfo m) => 
                m.Name;

            internal Type <GetMemberInfoFromType>b__37_0(ParameterInfo p) => 
                p.ParameterType;
        }
    }
}

