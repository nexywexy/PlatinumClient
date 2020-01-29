namespace Newtonsoft.Json.Utilities
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal static class TypeExtensions
    {
        public static System.Reflection.Assembly Assembly(this Type type) => 
            type.Assembly;

        public static bool AssignableToTypeName(this Type type, string fullTypeName) => 
            type.AssignableToTypeName(fullTypeName, out _);

        public static bool AssignableToTypeName(this Type type, string fullTypeName, out Type match)
        {
            for (Type type2 = type; type2 != null; type2 = type2.BaseType())
            {
                if (string.Equals(type2.FullName, fullTypeName, StringComparison.Ordinal))
                {
                    match = type2;
                    return true;
                }
            }
            Type[] interfaces = type.GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                if (string.Equals(interfaces[i].Name, fullTypeName, StringComparison.Ordinal))
                {
                    match = type;
                    return true;
                }
            }
            match = null;
            return false;
        }

        public static Type BaseType(this Type type) => 
            type.BaseType;

        public static bool ContainsGenericParameters(this Type type) => 
            type.ContainsGenericParameters;

        public static bool ImplementInterface(this Type type, Type interfaceType)
        {
            for (Type type2 = type; type2 != null; type2 = type2.BaseType())
            {
                foreach (Type type3 in type2.GetInterfaces())
                {
                    if ((type3 == interfaceType) || ((type3 != null) && type3.ImplementInterface(interfaceType)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsAbstract(this Type type) => 
            type.IsAbstract;

        public static bool IsClass(this Type type) => 
            type.IsClass;

        public static bool IsEnum(this Type type) => 
            type.IsEnum;

        public static bool IsGenericType(this Type type) => 
            type.IsGenericType;

        public static bool IsGenericTypeDefinition(this Type type) => 
            type.IsGenericTypeDefinition;

        public static bool IsInterface(this Type type) => 
            type.IsInterface;

        public static bool IsSealed(this Type type) => 
            type.IsSealed;

        public static bool IsValueType(this Type type) => 
            type.IsValueType;

        public static bool IsVisible(this Type type) => 
            type.IsVisible;

        public static MemberTypes MemberType(this MemberInfo memberInfo) => 
            memberInfo.MemberType;

        public static MethodInfo Method(this Delegate d) => 
            d.Method;
    }
}

