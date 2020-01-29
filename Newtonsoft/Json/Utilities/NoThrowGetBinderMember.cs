namespace Newtonsoft.Json.Utilities
{
    using System;
    using System.Dynamic;

    internal class NoThrowGetBinderMember : GetMemberBinder
    {
        private readonly GetMemberBinder _innerBinder;

        public NoThrowGetBinderMember(GetMemberBinder innerBinder) : base(innerBinder.Name, innerBinder.IgnoreCase)
        {
            this._innerBinder = innerBinder;
        }

        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
            DynamicMetaObject obj2 = this._innerBinder.Bind(target, new DynamicMetaObject[0]);
            return new DynamicMetaObject(new NoThrowExpressionVisitor().Visit(obj2.Expression), obj2.Restrictions);
        }
    }
}

