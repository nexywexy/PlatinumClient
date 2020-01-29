namespace Newtonsoft.Json.Utilities
{
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Reflection;

    internal class LateBoundReflectionDelegateFactory : ReflectionDelegateFactory
    {
        private static readonly LateBoundReflectionDelegateFactory _instance = new LateBoundReflectionDelegateFactory();

        public override Func<T> CreateDefaultConstructor<T>(Type type)
        {
            ConstructorInfo constructorInfo;
            ValidationUtils.ArgumentNotNull(type, "type");
            if (!type.IsValueType())
            {
                constructorInfo = ReflectionUtils.GetDefaultConstructor(type, true);
            }
            return () => ((T) constructorInfo.Invoke(null));
        }

        public override Func<T, object> CreateGet<T>(FieldInfo fieldInfo)
        {
            ValidationUtils.ArgumentNotNull(fieldInfo, "fieldInfo");
            return o => fieldInfo.GetValue(o);
        }

        public override Func<T, object> CreateGet<T>(PropertyInfo propertyInfo)
        {
            ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");
            return o => propertyInfo.GetValue(o, null);
        }

        public override MethodCall<T, object> CreateMethodCall<T>(MethodBase method)
        {
            ValidationUtils.ArgumentNotNull(method, "method");
            ConstructorInfo c = method as ConstructorInfo;
            if (c != null)
            {
                return (o, a) => c.Invoke(a);
            }
            return (o, a) => method.Invoke(o, a);
        }

        public override ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method)
        {
            ValidationUtils.ArgumentNotNull(method, "method");
            ConstructorInfo c = method as ConstructorInfo;
            if (c != null)
            {
                return a => c.Invoke(a);
            }
            return a => method.Invoke(null, a);
        }

        public override Action<T, object> CreateSet<T>(FieldInfo fieldInfo)
        {
            ValidationUtils.ArgumentNotNull(fieldInfo, "fieldInfo");
            return delegate (T o, object v) {
                fieldInfo.SetValue(o, v);
            };
        }

        public override Action<T, object> CreateSet<T>(PropertyInfo propertyInfo)
        {
            ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");
            return delegate (T o, object v) {
                propertyInfo.SetValue(o, v, null);
            };
        }

        internal static ReflectionDelegateFactory Instance =>
            _instance;
    }
}

