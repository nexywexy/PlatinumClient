namespace Newtonsoft.Json.Utilities
{
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal static class ImmutableCollectionsUtils
    {
        private const string ImmutableListGenericInterfaceTypeName = "System.Collections.Immutable.IImmutableList`1";
        private const string ImmutableQueueGenericInterfaceTypeName = "System.Collections.Immutable.IImmutableQueue`1";
        private const string ImmutableStackGenericInterfaceTypeName = "System.Collections.Immutable.IImmutableStack`1";
        private const string ImmutableSetGenericInterfaceTypeName = "System.Collections.Immutable.IImmutableSet`1";
        private const string ImmutableArrayTypeName = "System.Collections.Immutable.ImmutableArray";
        private const string ImmutableArrayGenericTypeName = "System.Collections.Immutable.ImmutableArray`1";
        private const string ImmutableListTypeName = "System.Collections.Immutable.ImmutableList";
        private const string ImmutableListGenericTypeName = "System.Collections.Immutable.ImmutableList`1";
        private const string ImmutableQueueTypeName = "System.Collections.Immutable.ImmutableQueue";
        private const string ImmutableQueueGenericTypeName = "System.Collections.Immutable.ImmutableQueue`1";
        private const string ImmutableStackTypeName = "System.Collections.Immutable.ImmutableStack";
        private const string ImmutableStackGenericTypeName = "System.Collections.Immutable.ImmutableStack`1";
        private const string ImmutableSortedSetTypeName = "System.Collections.Immutable.ImmutableSortedSet";
        private const string ImmutableSortedSetGenericTypeName = "System.Collections.Immutable.ImmutableSortedSet`1";
        private const string ImmutableHashSetTypeName = "System.Collections.Immutable.ImmutableHashSet";
        private const string ImmutableHashSetGenericTypeName = "System.Collections.Immutable.ImmutableHashSet`1";
        private static readonly IList<ImmutableCollectionTypeInfo> ArrayContractImmutableCollectionDefinitions;
        private const string ImmutableDictionaryGenericInterfaceTypeName = "System.Collections.Immutable.IImmutableDictionary`2";
        private const string ImmutableDictionaryTypeName = "System.Collections.Immutable.ImmutableDictionary";
        private const string ImmutableDictionaryGenericTypeName = "System.Collections.Immutable.ImmutableDictionary`2";
        private const string ImmutableSortedDictionaryTypeName = "System.Collections.Immutable.ImmutableSortedDictionary";
        private const string ImmutableSortedDictionaryGenericTypeName = "System.Collections.Immutable.ImmutableSortedDictionary`2";
        private static readonly IList<ImmutableCollectionTypeInfo> DictionaryContractImmutableCollectionDefinitions;

        static ImmutableCollectionsUtils()
        {
            List<ImmutableCollectionTypeInfo> list1 = new List<ImmutableCollectionTypeInfo> {
                new ImmutableCollectionTypeInfo("System.Collections.Immutable.IImmutableList`1", "System.Collections.Immutable.ImmutableList`1", "System.Collections.Immutable.ImmutableList"),
                new ImmutableCollectionTypeInfo("System.Collections.Immutable.ImmutableList`1", "System.Collections.Immutable.ImmutableList`1", "System.Collections.Immutable.ImmutableList"),
                new ImmutableCollectionTypeInfo("System.Collections.Immutable.IImmutableQueue`1", "System.Collections.Immutable.ImmutableQueue`1", "System.Collections.Immutable.ImmutableQueue"),
                new ImmutableCollectionTypeInfo("System.Collections.Immutable.ImmutableQueue`1", "System.Collections.Immutable.ImmutableQueue`1", "System.Collections.Immutable.ImmutableQueue"),
                new ImmutableCollectionTypeInfo("System.Collections.Immutable.IImmutableStack`1", "System.Collections.Immutable.ImmutableStack`1", "System.Collections.Immutable.ImmutableStack"),
                new ImmutableCollectionTypeInfo("System.Collections.Immutable.ImmutableStack`1", "System.Collections.Immutable.ImmutableStack`1", "System.Collections.Immutable.ImmutableStack"),
                new ImmutableCollectionTypeInfo("System.Collections.Immutable.IImmutableSet`1", "System.Collections.Immutable.ImmutableSortedSet`1", "System.Collections.Immutable.ImmutableSortedSet"),
                new ImmutableCollectionTypeInfo("System.Collections.Immutable.ImmutableSortedSet`1", "System.Collections.Immutable.ImmutableSortedSet`1", "System.Collections.Immutable.ImmutableSortedSet"),
                new ImmutableCollectionTypeInfo("System.Collections.Immutable.ImmutableHashSet`1", "System.Collections.Immutable.ImmutableHashSet`1", "System.Collections.Immutable.ImmutableHashSet"),
                new ImmutableCollectionTypeInfo("System.Collections.Immutable.ImmutableArray`1", "System.Collections.Immutable.ImmutableArray`1", "System.Collections.Immutable.ImmutableArray")
            };
            ArrayContractImmutableCollectionDefinitions = list1;
            List<ImmutableCollectionTypeInfo> list2 = new List<ImmutableCollectionTypeInfo> {
                new ImmutableCollectionTypeInfo("System.Collections.Immutable.IImmutableDictionary`2", "System.Collections.Immutable.ImmutableSortedDictionary`2", "System.Collections.Immutable.ImmutableSortedDictionary"),
                new ImmutableCollectionTypeInfo("System.Collections.Immutable.ImmutableSortedDictionary`2", "System.Collections.Immutable.ImmutableSortedDictionary`2", "System.Collections.Immutable.ImmutableSortedDictionary"),
                new ImmutableCollectionTypeInfo("System.Collections.Immutable.ImmutableDictionary`2", "System.Collections.Immutable.ImmutableDictionary`2", "System.Collections.Immutable.ImmutableDictionary")
            };
            DictionaryContractImmutableCollectionDefinitions = list2;
        }

        internal static bool TryBuildImmutableForArrayContract(Type underlyingType, Type collectionItemType, out Type createdType, out ObjectConstructor<object> parameterizedCreator)
        {
            if (underlyingType.IsGenericType())
            {
                Type genericTypeDefinition = underlyingType.GetGenericTypeDefinition();
                string name = genericTypeDefinition.FullName;
                ImmutableCollectionTypeInfo info = ArrayContractImmutableCollectionDefinitions.FirstOrDefault<ImmutableCollectionTypeInfo>(d => d.ContractTypeName == name);
                if (info != null)
                {
                    Type type = genericTypeDefinition.Assembly().GetType(info.CreatedTypeName);
                    Type type3 = genericTypeDefinition.Assembly().GetType(info.BuilderTypeName);
                    if ((type != null) && (type3 != null))
                    {
                        if (<>c.<>9__24_1 == null)
                        {
                        }
                        MethodInfo info2 = type3.GetMethods().FirstOrDefault<MethodInfo>(<>c.<>9__24_1 = new Func<MethodInfo, bool>(<>c.<>9.<TryBuildImmutableForArrayContract>b__24_1));
                        if (info2 != null)
                        {
                            Type[] typeArguments = new Type[] { collectionItemType };
                            createdType = type.MakeGenericType(typeArguments);
                            Type[] typeArray2 = new Type[] { collectionItemType };
                            MethodInfo method = info2.MakeGenericMethod(typeArray2);
                            parameterizedCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(method);
                            return true;
                        }
                    }
                }
            }
            createdType = null;
            parameterizedCreator = null;
            return false;
        }

        internal static bool TryBuildImmutableForDictionaryContract(Type underlyingType, Type keyItemType, Type valueItemType, out Type createdType, out ObjectConstructor<object> parameterizedCreator)
        {
            if (underlyingType.IsGenericType())
            {
                Type genericTypeDefinition = underlyingType.GetGenericTypeDefinition();
                string name = genericTypeDefinition.FullName;
                ImmutableCollectionTypeInfo info = DictionaryContractImmutableCollectionDefinitions.FirstOrDefault<ImmutableCollectionTypeInfo>(d => d.ContractTypeName == name);
                if (info != null)
                {
                    Type type = genericTypeDefinition.Assembly().GetType(info.CreatedTypeName);
                    Type type3 = genericTypeDefinition.Assembly().GetType(info.BuilderTypeName);
                    if ((type != null) && (type3 != null))
                    {
                        if (<>c.<>9__25_1 == null)
                        {
                        }
                        MethodInfo info2 = type3.GetMethods().FirstOrDefault<MethodInfo>(<>c.<>9__25_1 = new Func<MethodInfo, bool>(<>c.<>9.<TryBuildImmutableForDictionaryContract>b__25_1));
                        if (info2 != null)
                        {
                            Type[] typeArguments = new Type[] { keyItemType, valueItemType };
                            createdType = type.MakeGenericType(typeArguments);
                            Type[] typeArray2 = new Type[] { keyItemType, valueItemType };
                            MethodInfo method = info2.MakeGenericMethod(typeArray2);
                            parameterizedCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(method);
                            return true;
                        }
                    }
                }
            }
            createdType = null;
            parameterizedCreator = null;
            return false;
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly ImmutableCollectionsUtils.<>c <>9 = new ImmutableCollectionsUtils.<>c();
            public static Func<MethodInfo, bool> <>9__24_1;
            public static Func<MethodInfo, bool> <>9__25_1;

            internal bool <TryBuildImmutableForArrayContract>b__24_1(MethodInfo m) => 
                ((m.Name == "CreateRange") && (m.GetParameters().Length == 1));

            internal bool <TryBuildImmutableForDictionaryContract>b__25_1(MethodInfo m)
            {
                ParameterInfo[] parameters = m.GetParameters();
                return ((((m.Name == "CreateRange") && (parameters.Length == 1)) && parameters[0].ParameterType.IsGenericType()) && (parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)));
            }
        }

        internal class ImmutableCollectionTypeInfo
        {
            public ImmutableCollectionTypeInfo(string contractTypeName, string createdTypeName, string builderTypeName)
            {
                this.ContractTypeName = contractTypeName;
                this.CreatedTypeName = createdTypeName;
                this.BuilderTypeName = builderTypeName;
            }

            public string ContractTypeName { get; set; }

            public string CreatedTypeName { get; set; }

            public string BuilderTypeName { get; set; }
        }
    }
}

