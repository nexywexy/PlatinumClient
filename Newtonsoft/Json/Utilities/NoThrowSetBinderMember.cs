namespace Newtonsoft.Json.Utilities
{
    using System;
    using System.Dynamic;

    internal class NoThrowSetBinderMember : SetMemberBinder
    {
        private readonly SetMemberBinder _innerBinder;

        public NoThrowSetBinderMember(SetMemberBinder innerBinder) : base(innerBinder.Name, innerBinder.IgnoreCase)
        {
            this._innerBinder = innerBinder;
        }

        public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
        {
            DynamicMetaObject[] args = new DynamicMetaObject[] { value };
            DynamicMetaObject obj2 = this._innerBinder.Bind(target, args);
            return new DynamicMetaObject(new NoThrowExpressionVisitor().Visit(obj2.Expression), obj2.Restrictions);
        }
    }
}

