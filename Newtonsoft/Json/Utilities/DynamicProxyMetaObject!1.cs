namespace Newtonsoft.Json.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal sealed class DynamicProxyMetaObject<T> : DynamicMetaObject
    {
        private readonly DynamicProxy<T> _proxy;
        private readonly bool _dontFallbackFirst;
        private static readonly Expression[] NoArgs;

        static DynamicProxyMetaObject()
        {
            DynamicProxyMetaObject<T>.NoArgs = new Expression[0];
        }

        internal DynamicProxyMetaObject(Expression expression, T value, DynamicProxy<T> proxy, bool dontFallbackFirst) : base(expression, BindingRestrictions.Empty, value)
        {
            this._proxy = proxy;
            this._dontFallbackFirst = dontFallbackFirst;
        }

        public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg)
        {
            if (!this.IsOverridden("TryBinaryOperation"))
            {
                return base.BindBinaryOperation(binder, arg);
            }
            DynamicMetaObject[] args = new DynamicMetaObject[] { arg };
            return this.CallMethodWithResult("TryBinaryOperation", binder, DynamicProxyMetaObject<T>.GetArgs(args), e => binder.FallbackBinaryOperation((DynamicProxyMetaObject<T>) this, arg, e), null);
        }

        public override DynamicMetaObject BindConvert(ConvertBinder binder)
        {
            if (!this.IsOverridden("TryConvert"))
            {
                return base.BindConvert(binder);
            }
            return this.CallMethodWithResult("TryConvert", binder, DynamicProxyMetaObject<T>.NoArgs, e => binder.FallbackConvert((DynamicProxyMetaObject<T>) this, e), null);
        }

        public override DynamicMetaObject BindCreateInstance(CreateInstanceBinder binder, DynamicMetaObject[] args)
        {
            if (!this.IsOverridden("TryCreateInstance"))
            {
                return base.BindCreateInstance(binder, args);
            }
            return this.CallMethodWithResult("TryCreateInstance", binder, DynamicProxyMetaObject<T>.GetArgArray(args), e => binder.FallbackCreateInstance((DynamicProxyMetaObject<T>) this, args, e), null);
        }

        public override DynamicMetaObject BindDeleteIndex(DeleteIndexBinder binder, DynamicMetaObject[] indexes)
        {
            if (!this.IsOverridden("TryDeleteIndex"))
            {
                return base.BindDeleteIndex(binder, indexes);
            }
            return this.CallMethodNoResult("TryDeleteIndex", binder, DynamicProxyMetaObject<T>.GetArgArray(indexes), e => binder.FallbackDeleteIndex((DynamicProxyMetaObject<T>) this, indexes, e));
        }

        public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
        {
            if (!this.IsOverridden("TryDeleteMember"))
            {
                return base.BindDeleteMember(binder);
            }
            return this.CallMethodNoResult("TryDeleteMember", binder, DynamicProxyMetaObject<T>.NoArgs, e => binder.FallbackDeleteMember((DynamicProxyMetaObject<T>) this, e));
        }

        public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
        {
            if (!this.IsOverridden("TryGetIndex"))
            {
                return base.BindGetIndex(binder, indexes);
            }
            return this.CallMethodWithResult("TryGetIndex", binder, DynamicProxyMetaObject<T>.GetArgArray(indexes), e => binder.FallbackGetIndex((DynamicProxyMetaObject<T>) this, indexes, e), null);
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            if (!this.IsOverridden("TryGetMember"))
            {
                return base.BindGetMember(binder);
            }
            return this.CallMethodWithResult("TryGetMember", binder, DynamicProxyMetaObject<T>.NoArgs, e => binder.FallbackGetMember((DynamicProxyMetaObject<T>) this, e), null);
        }

        public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
        {
            if (!this.IsOverridden("TryInvoke"))
            {
                return base.BindInvoke(binder, args);
            }
            return this.CallMethodWithResult("TryInvoke", binder, DynamicProxyMetaObject<T>.GetArgArray(args), e => binder.FallbackInvoke((DynamicProxyMetaObject<T>) this, args, e), null);
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            if (!this.IsOverridden("TryInvokeMember"))
            {
                return base.BindInvokeMember(binder, args);
            }
            Fallback<T> fallback = e => binder.FallbackInvokeMember((DynamicProxyMetaObject<T>) this, args, e);
            DynamicMetaObject errorSuggestion = this.BuildCallMethodWithResult("TryInvokeMember", binder, DynamicProxyMetaObject<T>.GetArgArray(args), this.BuildCallMethodWithResult("TryGetMember", new GetBinderAdapter<T>(binder), DynamicProxyMetaObject<T>.NoArgs, fallback(null), e => binder.FallbackInvoke(e, args, null)), null);
            if (!this._dontFallbackFirst)
            {
                return fallback(errorSuggestion);
            }
            return errorSuggestion;
        }

        public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
        {
            if (!this.IsOverridden("TrySetIndex"))
            {
                return base.BindSetIndex(binder, indexes, value);
            }
            return this.CallMethodReturnLast("TrySetIndex", binder, DynamicProxyMetaObject<T>.GetArgArray(indexes, value), e => binder.FallbackSetIndex((DynamicProxyMetaObject<T>) this, indexes, value, e));
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            if (!this.IsOverridden("TrySetMember"))
            {
                return base.BindSetMember(binder, value);
            }
            DynamicMetaObject[] args = new DynamicMetaObject[] { value };
            return this.CallMethodReturnLast("TrySetMember", binder, DynamicProxyMetaObject<T>.GetArgs(args), e => binder.FallbackSetMember((DynamicProxyMetaObject<T>) this, value, e));
        }

        public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
        {
            if (!this.IsOverridden("TryUnaryOperation"))
            {
                return base.BindUnaryOperation(binder);
            }
            return this.CallMethodWithResult("TryUnaryOperation", binder, DynamicProxyMetaObject<T>.NoArgs, e => binder.FallbackUnaryOperation((DynamicProxyMetaObject<T>) this, e), null);
        }

        private DynamicMetaObject BuildCallMethodWithResult(string methodName, DynamicMetaObjectBinder binder, Expression[] args, DynamicMetaObject fallbackResult, Fallback<T> fallbackInvoke)
        {
            ParameterExpression item = Expression.Parameter(typeof(object), null);
            IList<Expression> initial = new List<Expression> {
                Expression.Convert(base.Expression, typeof(T)),
                DynamicProxyMetaObject<T>.Constant(binder)
            };
            initial.AddRange<Expression>(args);
            initial.Add(item);
            DynamicMetaObject errorSuggestion = new DynamicMetaObject(item, BindingRestrictions.Empty);
            if (binder.ReturnType != typeof(object))
            {
                errorSuggestion = new DynamicMetaObject(Expression.Convert(errorSuggestion.Expression, binder.ReturnType), errorSuggestion.Restrictions);
            }
            if (fallbackInvoke != null)
            {
                errorSuggestion = fallbackInvoke(errorSuggestion);
            }
            ParameterExpression[] variables = new ParameterExpression[] { item };
            Expression[] expressions = new Expression[] { Expression.Condition(Expression.Call(Expression.Constant(this._proxy), typeof(DynamicProxy<T>).GetMethod(methodName), initial), errorSuggestion.Expression, fallbackResult.Expression, binder.ReturnType) };
            return new DynamicMetaObject(Expression.Block(variables, expressions), this.GetRestrictions().Merge(errorSuggestion.Restrictions).Merge(fallbackResult.Restrictions));
        }

        private DynamicMetaObject CallMethodNoResult(string methodName, DynamicMetaObjectBinder binder, Expression[] args, Fallback<T> fallback)
        {
            DynamicMetaObject obj2 = fallback(null);
            IList<Expression> initial = new List<Expression> {
                Expression.Convert(base.Expression, typeof(T)),
                DynamicProxyMetaObject<T>.Constant(binder)
            };
            initial.AddRange<Expression>(args);
            DynamicMetaObject errorSuggestion = new DynamicMetaObject(Expression.Condition(Expression.Call(Expression.Constant(this._proxy), typeof(DynamicProxy<T>).GetMethod(methodName), initial), Expression.Empty(), obj2.Expression, typeof(void)), this.GetRestrictions().Merge(obj2.Restrictions));
            if (!this._dontFallbackFirst)
            {
                return fallback(errorSuggestion);
            }
            return errorSuggestion;
        }

        private DynamicMetaObject CallMethodReturnLast(string methodName, DynamicMetaObjectBinder binder, Expression[] args, Fallback<T> fallback)
        {
            DynamicMetaObject obj2 = fallback(null);
            ParameterExpression left = Expression.Parameter(typeof(object), null);
            IList<Expression> initial = new List<Expression> {
                Expression.Convert(base.Expression, typeof(T)),
                DynamicProxyMetaObject<T>.Constant(binder)
            };
            initial.AddRange<Expression>(args);
            initial[args.Length + 1] = Expression.Assign(left, initial[args.Length + 1]);
            ParameterExpression[] variables = new ParameterExpression[] { left };
            Expression[] expressions = new Expression[] { Expression.Condition(Expression.Call(Expression.Constant(this._proxy), typeof(DynamicProxy<T>).GetMethod(methodName), initial), left, obj2.Expression, typeof(object)) };
            DynamicMetaObject errorSuggestion = new DynamicMetaObject(Expression.Block(variables, expressions), this.GetRestrictions().Merge(obj2.Restrictions));
            if (!this._dontFallbackFirst)
            {
                return fallback(errorSuggestion);
            }
            return errorSuggestion;
        }

        private DynamicMetaObject CallMethodWithResult(string methodName, DynamicMetaObjectBinder binder, Expression[] args, Fallback<T> fallback, Fallback<T> fallbackInvoke = null)
        {
            DynamicMetaObject fallbackResult = fallback(null);
            DynamicMetaObject errorSuggestion = this.BuildCallMethodWithResult(methodName, binder, args, fallbackResult, fallbackInvoke);
            if (!this._dontFallbackFirst)
            {
                return fallback(errorSuggestion);
            }
            return errorSuggestion;
        }

        private static ConstantExpression Constant(DynamicMetaObjectBinder binder)
        {
            Type type = binder.GetType();
            while (!type.IsVisible())
            {
                type = type.BaseType();
            }
            return Expression.Constant(binder, type);
        }

        private static Expression[] GetArgArray(DynamicMetaObject[] args) => 
            new NewArrayExpression[] { Expression.NewArrayInit(typeof(object), DynamicProxyMetaObject<T>.GetArgs(args)) };

        private static Expression[] GetArgArray(DynamicMetaObject[] args, DynamicMetaObject value) => 
            new Expression[] { Expression.NewArrayInit(typeof(object), DynamicProxyMetaObject<T>.GetArgs(args)), Expression.Convert(value.Expression, typeof(object)) };

        private static Expression[] GetArgs(params DynamicMetaObject[] args)
        {
            if (<>c<T>.<>9__20_0 == null)
            {
            }
            return args.Select<DynamicMetaObject, UnaryExpression>((<>c<T>.<>9__20_0 = new Func<DynamicMetaObject, UnaryExpression>(<>c<T>.<>9.<GetArgs>b__20_0))).ToArray<UnaryExpression>();
        }

        public override IEnumerable<string> GetDynamicMemberNames() => 
            this._proxy.GetDynamicMemberNames(this.Value);

        private BindingRestrictions GetRestrictions()
        {
            if ((this.Value == null) && base.HasValue)
            {
                return BindingRestrictions.GetInstanceRestriction(base.Expression, null);
            }
            return BindingRestrictions.GetTypeRestriction(base.Expression, base.LimitType);
        }

        private bool IsOverridden(string method) => 
            ReflectionUtils.IsMethodOverridden(this._proxy.GetType(), typeof(DynamicProxy<T>), method);

        private T Value =>
            ((T) base.Value);

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly DynamicProxyMetaObject<T>.<>c <>9;
            public static Func<DynamicMetaObject, UnaryExpression> <>9__20_0;

            static <>c()
            {
                DynamicProxyMetaObject<T>.<>c.<>9 = new DynamicProxyMetaObject<T>.<>c();
            }

            internal UnaryExpression <GetArgs>b__20_0(DynamicMetaObject arg) => 
                Expression.Convert(arg.Expression, typeof(object));
        }

        private delegate DynamicMetaObject Fallback(DynamicMetaObject errorSuggestion);

        private sealed class GetBinderAdapter : GetMemberBinder
        {
            internal GetBinderAdapter(InvokeMemberBinder binder) : base(binder.Name, binder.IgnoreCase)
            {
            }

            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                throw new NotSupportedException();
            }
        }
    }
}

