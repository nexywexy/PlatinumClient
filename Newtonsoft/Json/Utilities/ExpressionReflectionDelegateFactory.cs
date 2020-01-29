namespace Newtonsoft.Json.Utilities
{
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class ExpressionReflectionDelegateFactory : ReflectionDelegateFactory
    {
        private static readonly ExpressionReflectionDelegateFactory _instance = new ExpressionReflectionDelegateFactory();

        private Expression BuildMethodCall(MethodBase method, Type type, ParameterExpression targetParameterExpression, ParameterExpression argsParameterExpression)
        {
            Expression expression6;
            ParameterInfo[] parameters = method.GetParameters();
            Expression[] arguments = new Expression[parameters.Length];
            IList<ByRefParameter> list = new List<ByRefParameter>();
            for (int i = 0; i < parameters.Length; i++)
            {
                Expression expression4;
                ParameterInfo info = parameters[i];
                Type parameterType = info.ParameterType;
                bool flag = false;
                if (parameterType.IsByRef)
                {
                    parameterType = parameterType.GetElementType();
                    flag = true;
                }
                Expression index = Expression.Constant(i);
                Expression left = Expression.ArrayIndex(argsParameterExpression, index);
                if (parameterType.IsValueType())
                {
                    BinaryExpression expression = Expression.Coalesce(left, Expression.New(parameterType));
                    expression4 = this.EnsureCastExpression(expression, parameterType);
                }
                else
                {
                    expression4 = this.EnsureCastExpression(left, parameterType);
                }
                if (flag)
                {
                    ParameterExpression expression5 = Expression.Variable(parameterType);
                    ByRefParameter item = new ByRefParameter {
                        Value = expression4,
                        Variable = expression5,
                        IsOut = info.IsOut
                    };
                    list.Add(item);
                    expression4 = expression5;
                }
                arguments[i] = expression4;
            }
            if (method.IsConstructor)
            {
                expression6 = Expression.New((ConstructorInfo) method, arguments);
            }
            else if (method.IsStatic)
            {
                expression6 = Expression.Call((MethodInfo) method, arguments);
            }
            else
            {
                expression6 = Expression.Call(this.EnsureCastExpression(targetParameterExpression, method.DeclaringType), (MethodInfo) method, arguments);
            }
            if (method is MethodInfo)
            {
                if (((MethodInfo) method).ReturnType != typeof(void))
                {
                    expression6 = this.EnsureCastExpression(expression6, type);
                }
                else
                {
                    expression6 = Expression.Block(expression6, Expression.Constant(null));
                }
            }
            else
            {
                expression6 = this.EnsureCastExpression(expression6, type);
            }
            if (list.Count <= 0)
            {
                return expression6;
            }
            IList<ParameterExpression> list2 = new List<ParameterExpression>();
            IList<Expression> list3 = new List<Expression>();
            foreach (ByRefParameter parameter in list)
            {
                if (!parameter.IsOut)
                {
                    list3.Add(Expression.Assign(parameter.Variable, parameter.Value));
                }
                list2.Add(parameter.Variable);
            }
            list3.Add(expression6);
            return Expression.Block((IEnumerable<ParameterExpression>) list2, (IEnumerable<Expression>) list3);
        }

        public override Func<T> CreateDefaultConstructor<T>(Type type)
        {
            ValidationUtils.ArgumentNotNull(type, "type");
            if (type.IsAbstract())
            {
                return () => ((T) Activator.CreateInstance(type));
            }
            try
            {
                Type targetType = typeof(T);
                Expression expression = Expression.New(type);
                expression = this.EnsureCastExpression(expression, targetType);
                return (Func<T>) Expression.Lambda(typeof(Func<T>), expression, new ParameterExpression[0]).Compile();
            }
            catch
            {
                return () => ((T) Activator.CreateInstance(type));
            }
        }

        public override Func<T, object> CreateGet<T>(FieldInfo fieldInfo)
        {
            ParameterExpression expression;
            Expression expression2;
            ValidationUtils.ArgumentNotNull(fieldInfo, "fieldInfo");
            if (fieldInfo.IsStatic)
            {
                expression2 = Expression.Field(null, fieldInfo);
            }
            else
            {
                expression2 = Expression.Field(this.EnsureCastExpression(expression = Expression.Parameter(typeof(T), "source"), fieldInfo.DeclaringType), fieldInfo);
            }
            ParameterExpression[] parameters = new ParameterExpression[] { expression };
            return Expression.Lambda<Func<T, object>>(this.EnsureCastExpression(expression2, typeof(object)), parameters).Compile();
        }

        public override Func<T, object> CreateGet<T>(PropertyInfo propertyInfo)
        {
            ParameterExpression expression;
            Expression expression2;
            ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");
            Type targetType = typeof(object);
            if (propertyInfo.GetGetMethod(true).IsStatic)
            {
                expression2 = Expression.MakeMemberAccess(null, propertyInfo);
            }
            else
            {
                expression2 = Expression.MakeMemberAccess(this.EnsureCastExpression(expression = Expression.Parameter(typeof(T), "instance"), propertyInfo.DeclaringType), propertyInfo);
            }
            expression2 = this.EnsureCastExpression(expression2, targetType);
            ParameterExpression[] parameters = new ParameterExpression[] { expression };
            return (Func<T, object>) Expression.Lambda(typeof(Func<T, object>), expression2, parameters).Compile();
        }

        public override MethodCall<T, object> CreateMethodCall<T>(MethodBase method)
        {
            ParameterExpression expression2;
            ValidationUtils.ArgumentNotNull(method, "method");
            Type type = typeof(object);
            ParameterExpression targetParameterExpression = Expression.Parameter(type, "target");
            Expression body = this.BuildMethodCall(method, type, targetParameterExpression, expression2 = Expression.Parameter(typeof(object[]), "args"));
            ParameterExpression[] parameters = new ParameterExpression[] { targetParameterExpression, expression2 };
            return (MethodCall<T, object>) Expression.Lambda(typeof(MethodCall<T, object>), body, parameters).Compile();
        }

        public override ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method)
        {
            ParameterExpression expression;
            ValidationUtils.ArgumentNotNull(method, "method");
            Type type = typeof(object);
            Expression body = this.BuildMethodCall(method, type, null, expression = Expression.Parameter(typeof(object[]), "args"));
            ParameterExpression[] parameters = new ParameterExpression[] { expression };
            return (ObjectConstructor<object>) Expression.Lambda(typeof(ObjectConstructor<object>), body, parameters).Compile();
        }

        public override Action<T, object> CreateSet<T>(FieldInfo fieldInfo)
        {
            ParameterExpression expression;
            ParameterExpression expression2;
            Expression expression3;
            ValidationUtils.ArgumentNotNull(fieldInfo, "fieldInfo");
            if (fieldInfo.DeclaringType.IsValueType() || fieldInfo.IsInitOnly)
            {
                return LateBoundReflectionDelegateFactory.Instance.CreateSet<T>(fieldInfo);
            }
            if (fieldInfo.IsStatic)
            {
                expression3 = Expression.Field(null, fieldInfo);
            }
            else
            {
                expression3 = Expression.Field(this.EnsureCastExpression(expression = Expression.Parameter(typeof(T), "source"), fieldInfo.DeclaringType), fieldInfo);
            }
            Expression right = this.EnsureCastExpression(expression2 = Expression.Parameter(typeof(object), "value"), expression3.Type);
            BinaryExpression body = Expression.Assign(expression3, right);
            ParameterExpression[] parameters = new ParameterExpression[] { expression, expression2 };
            return (Action<T, object>) Expression.Lambda(typeof(Action<T, object>), body, parameters).Compile();
        }

        public override Action<T, object> CreateSet<T>(PropertyInfo propertyInfo)
        {
            ParameterExpression expression;
            Expression expression4;
            ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");
            if (propertyInfo.DeclaringType.IsValueType())
            {
                return LateBoundReflectionDelegateFactory.Instance.CreateSet<T>(propertyInfo);
            }
            Type type = typeof(object);
            ParameterExpression expression2 = Expression.Parameter(type, "value");
            Expression expression3 = this.EnsureCastExpression(expression2, propertyInfo.PropertyType);
            MethodInfo setMethod = propertyInfo.GetSetMethod(true);
            if (setMethod.IsStatic)
            {
                expression4 = Expression.Call(setMethod, expression3);
            }
            else
            {
                Expression[] arguments = new Expression[] { expression3 };
                expression4 = Expression.Call(this.EnsureCastExpression(expression = Expression.Parameter(typeof(T), "instance"), propertyInfo.DeclaringType), setMethod, arguments);
            }
            ParameterExpression[] parameters = new ParameterExpression[] { expression, expression2 };
            return (Action<T, object>) Expression.Lambda(typeof(Action<T, object>), expression4, parameters).Compile();
        }

        private Expression EnsureCastExpression(Expression expression, Type targetType)
        {
            Type type = expression.Type;
            if (!(type == targetType) && (type.IsValueType() || !targetType.IsAssignableFrom(type)))
            {
                return Expression.Convert(expression, targetType);
            }
            return expression;
        }

        internal static ReflectionDelegateFactory Instance =>
            _instance;

        private class ByRefParameter
        {
            public Expression Value;
            public ParameterExpression Variable;
            public bool IsOut;
        }
    }
}

