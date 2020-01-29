namespace Newtonsoft.Json.Utilities
{
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal static class FSharpUtils
    {
        private static readonly object Lock = new object();
        private static bool _initialized;
        private static MethodInfo _ofSeq;
        private static Type _mapType;
        public const string FSharpSetTypeName = "FSharpSet`1";
        public const string FSharpListTypeName = "FSharpList`1";
        public const string FSharpMapTypeName = "FSharpMap`2";

        public static ObjectConstructor<object> BuildMapCreator<TKey, TValue>()
        {
            Type[] typeArguments = new Type[] { typeof(TKey), typeof(TValue) };
            Type[] types = new Type[] { typeof(IEnumerable<Tuple<TKey, TValue>>) };
            ConstructorInfo constructor = _mapType.MakeGenericType(typeArguments).GetConstructor(types);
            ObjectConstructor<object> ctorDelegate = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor);
            return delegate (object[] args) {
                if (<>c__52<TKey, TValue>.<>9__52_1 == null)
                {
                }
                IEnumerable<Tuple<TKey, TValue>> enumerable = ((IEnumerable<KeyValuePair<TKey, TValue>>) args[0]).Select<KeyValuePair<TKey, TValue>, Tuple<TKey, TValue>>(<>c__52<TKey, TValue>.<>9__52_1 = new Func<KeyValuePair<TKey, TValue>, Tuple<TKey, TValue>>(<>c__52<TKey, TValue>.<>9.<BuildMapCreator>b__52_1));
                object[] objArray1 = new object[] { enumerable };
                return ctorDelegate(objArray1);
            };
        }

        private static MethodCall<object, object> CreateFSharpFuncCall(Type type, string methodName)
        {
            MethodInfo method = GetMethodWithNonPublicFallback(type, methodName, BindingFlags.Public | BindingFlags.Static);
            MethodInfo info2 = method.ReturnType.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
            MethodCall<object, object> call = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
            MethodCall<object, object> invoke = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(info2);
            return (target, args) => new FSharpFunction(call(target, args), invoke);
        }

        public static ObjectConstructor<object> CreateMap(Type keyType, Type valueType)
        {
            Type[] typeArguments = new Type[] { keyType, valueType };
            return (ObjectConstructor<object>) typeof(FSharpUtils).GetMethod("BuildMapCreator").MakeGenericMethod(typeArguments).Invoke(null, null);
        }

        public static ObjectConstructor<object> CreateSeq(Type t)
        {
            Type[] typeArguments = new Type[] { t };
            MethodInfo method = _ofSeq.MakeGenericMethod(typeArguments);
            return JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(method);
        }

        public static void EnsureInitialized(Assembly fsharpCoreAssembly)
        {
            if (!_initialized)
            {
                object @lock = Lock;
                lock (@lock)
                {
                    if (!_initialized)
                    {
                        FSharpCoreAssembly = fsharpCoreAssembly;
                        Type type1 = fsharpCoreAssembly.GetType("Microsoft.FSharp.Reflection.FSharpType");
                        MethodInfo method = GetMethodWithNonPublicFallback(type1, "IsUnion", BindingFlags.Public | BindingFlags.Static);
                        IsUnion = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
                        MethodInfo info2 = GetMethodWithNonPublicFallback(type1, "GetUnionCases", BindingFlags.Public | BindingFlags.Static);
                        GetUnionCases = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(info2);
                        Type type2 = fsharpCoreAssembly.GetType("Microsoft.FSharp.Reflection.FSharpValue");
                        PreComputeUnionTagReader = CreateFSharpFuncCall(type2, "PreComputeUnionTagReader");
                        PreComputeUnionReader = CreateFSharpFuncCall(type2, "PreComputeUnionReader");
                        PreComputeUnionConstructor = CreateFSharpFuncCall(type2, "PreComputeUnionConstructor");
                        Type type = fsharpCoreAssembly.GetType("Microsoft.FSharp.Reflection.UnionCaseInfo");
                        GetUnionCaseInfoName = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(type.GetProperty("Name"));
                        GetUnionCaseInfoTag = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(type.GetProperty("Tag"));
                        GetUnionCaseInfoDeclaringType = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(type.GetProperty("DeclaringType"));
                        GetUnionCaseInfoFields = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(type.GetMethod("GetFields"));
                        _ofSeq = fsharpCoreAssembly.GetType("Microsoft.FSharp.Collections.ListModule").GetMethod("OfSeq");
                        _mapType = fsharpCoreAssembly.GetType("Microsoft.FSharp.Collections.FSharpMap`2");
                        Thread.MemoryBarrier();
                        _initialized = true;
                    }
                }
            }
        }

        private static MethodInfo GetMethodWithNonPublicFallback(Type type, string methodName, BindingFlags bindingFlags)
        {
            MethodInfo method = type.GetMethod(methodName, bindingFlags);
            if ((method == null) && ((bindingFlags & BindingFlags.NonPublic) != BindingFlags.NonPublic))
            {
                method = type.GetMethod(methodName, bindingFlags | BindingFlags.NonPublic);
            }
            return method;
        }

        public static Assembly FSharpCoreAssembly
        {
            [CompilerGenerated]
            get => 
                <FSharpCoreAssembly>k__BackingField;
            [CompilerGenerated]
            private set => 
                (<FSharpCoreAssembly>k__BackingField = value);
        }

        public static MethodCall<object, object> IsUnion
        {
            [CompilerGenerated]
            get => 
                <IsUnion>k__BackingField;
            [CompilerGenerated]
            private set => 
                (<IsUnion>k__BackingField = value);
        }

        public static MethodCall<object, object> GetUnionCases
        {
            [CompilerGenerated]
            get => 
                <GetUnionCases>k__BackingField;
            [CompilerGenerated]
            private set => 
                (<GetUnionCases>k__BackingField = value);
        }

        public static MethodCall<object, object> PreComputeUnionTagReader
        {
            [CompilerGenerated]
            get => 
                <PreComputeUnionTagReader>k__BackingField;
            [CompilerGenerated]
            private set => 
                (<PreComputeUnionTagReader>k__BackingField = value);
        }

        public static MethodCall<object, object> PreComputeUnionReader
        {
            [CompilerGenerated]
            get => 
                <PreComputeUnionReader>k__BackingField;
            [CompilerGenerated]
            private set => 
                (<PreComputeUnionReader>k__BackingField = value);
        }

        public static MethodCall<object, object> PreComputeUnionConstructor
        {
            [CompilerGenerated]
            get => 
                <PreComputeUnionConstructor>k__BackingField;
            [CompilerGenerated]
            private set => 
                (<PreComputeUnionConstructor>k__BackingField = value);
        }

        public static Func<object, object> GetUnionCaseInfoDeclaringType
        {
            [CompilerGenerated]
            get => 
                <GetUnionCaseInfoDeclaringType>k__BackingField;
            [CompilerGenerated]
            private set => 
                (<GetUnionCaseInfoDeclaringType>k__BackingField = value);
        }

        public static Func<object, object> GetUnionCaseInfoName
        {
            [CompilerGenerated]
            get => 
                <GetUnionCaseInfoName>k__BackingField;
            [CompilerGenerated]
            private set => 
                (<GetUnionCaseInfoName>k__BackingField = value);
        }

        public static Func<object, object> GetUnionCaseInfoTag
        {
            [CompilerGenerated]
            get => 
                <GetUnionCaseInfoTag>k__BackingField;
            [CompilerGenerated]
            private set => 
                (<GetUnionCaseInfoTag>k__BackingField = value);
        }

        public static MethodCall<object, object> GetUnionCaseInfoFields
        {
            [CompilerGenerated]
            get => 
                <GetUnionCaseInfoFields>k__BackingField;
            [CompilerGenerated]
            private set => 
                (<GetUnionCaseInfoFields>k__BackingField = value);
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__52<TKey, TValue>
        {
            public static readonly FSharpUtils.<>c__52<TKey, TValue> <>9;
            public static Func<KeyValuePair<TKey, TValue>, Tuple<TKey, TValue>> <>9__52_1;

            static <>c__52()
            {
                FSharpUtils.<>c__52<TKey, TValue>.<>9 = new FSharpUtils.<>c__52<TKey, TValue>();
            }

            internal Tuple<TKey, TValue> <BuildMapCreator>b__52_1(KeyValuePair<TKey, TValue> kv) => 
                new Tuple<TKey, TValue>(kv.Key, kv.Value);
        }
    }
}

