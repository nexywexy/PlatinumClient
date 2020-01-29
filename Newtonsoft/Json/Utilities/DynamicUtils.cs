namespace Newtonsoft.Json.Utilities
{
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class DynamicUtils
    {
        public static IEnumerable<string> GetDynamicMemberNames(this IDynamicMetaObjectProvider dynamicProvider) => 
            dynamicProvider.GetMetaObject(Expression.Constant(dynamicProvider)).GetDynamicMemberNames();

        internal static class BinderWrapper
        {
            public const string CSharpAssemblyName = "Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
            private const string BinderTypeName = "Microsoft.CSharp.RuntimeBinder.Binder, Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
            private const string CSharpArgumentInfoTypeName = "Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo, Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
            private const string CSharpArgumentInfoFlagsTypeName = "Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags, Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
            private const string CSharpBinderFlagsTypeName = "Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags, Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
            private static object _getCSharpArgumentInfoArray;
            private static object _setCSharpArgumentInfoArray;
            private static MethodCall<object, object> _getMemberCall;
            private static MethodCall<object, object> _setMemberCall;
            private static bool _init;

            private static void CreateMemberCalls()
            {
                Type type = Type.GetType("Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo, Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true);
                Type type2 = Type.GetType("Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags, Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true);
                Type type3 = Type.GetType("Microsoft.CSharp.RuntimeBinder.Binder, Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true);
                Type[] typeArguments = new Type[] { type };
                Type type4 = typeof(IEnumerable<>).MakeGenericType(typeArguments);
                Type[] types = new Type[] { type2, typeof(string), typeof(Type), type4 };
                MethodInfo method = type3.GetMethod("GetMember", types);
                _getMemberCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
                Type[] typeArray3 = new Type[] { type2, typeof(string), typeof(Type), type4 };
                MethodInfo info2 = type3.GetMethod("SetMember", typeArray3);
                _setMemberCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(info2);
            }

            private static object CreateSharpArgumentInfoArray(params int[] values)
            {
                Type elementType = Type.GetType("Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo, Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                Type type = Type.GetType("Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags, Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                Array array = Array.CreateInstance(elementType, values.Length);
                for (int i = 0; i < values.Length; i++)
                {
                    Type[] types = new Type[] { type, typeof(string) };
                    object[] parameters = new object[2];
                    parameters[0] = 0;
                    object obj2 = elementType.GetMethod("Create", types).Invoke(null, parameters);
                    array.SetValue(obj2, i);
                }
                return array;
            }

            public static CallSiteBinder GetMember(string name, Type context)
            {
                Init();
                object[] args = new object[] { 0, name, context, _getCSharpArgumentInfoArray };
                return (CallSiteBinder) _getMemberCall(null, args);
            }

            private static void Init()
            {
                if (!_init)
                {
                    if (Type.GetType("Microsoft.CSharp.RuntimeBinder.Binder, Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false) == null)
                    {
                        throw new InvalidOperationException("Could not resolve type '{0}'. You may need to add a reference to Microsoft.CSharp.dll to work with dynamic types.".FormatWith(CultureInfo.InvariantCulture, "Microsoft.CSharp.RuntimeBinder.Binder, Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
                    }
                    _getCSharpArgumentInfoArray = CreateSharpArgumentInfoArray(new int[1]);
                    int[] values = new int[2];
                    values[1] = 3;
                    _setCSharpArgumentInfoArray = CreateSharpArgumentInfoArray(values);
                    CreateMemberCalls();
                    _init = true;
                }
            }

            public static CallSiteBinder SetMember(string name, Type context)
            {
                Init();
                object[] args = new object[] { 0, name, context, _setCSharpArgumentInfoArray };
                return (CallSiteBinder) _setMemberCall(null, args);
            }
        }
    }
}

