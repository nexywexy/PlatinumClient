namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Dynamic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class JsonDynamicContract : JsonContainerContract
    {
        private readonly ThreadSafeStore<string, CallSite<Func<CallSite, object, object>>> _callSiteGetters;
        private readonly ThreadSafeStore<string, CallSite<Func<CallSite, object, object, object>>> _callSiteSetters;

        public JsonDynamicContract(Type underlyingType) : base(underlyingType)
        {
            this._callSiteGetters = new ThreadSafeStore<string, CallSite<Func<CallSite, object, object>>>(new Func<string, CallSite<Func<CallSite, object, object>>>(JsonDynamicContract.CreateCallSiteGetter));
            this._callSiteSetters = new ThreadSafeStore<string, CallSite<Func<CallSite, object, object, object>>>(new Func<string, CallSite<Func<CallSite, object, object, object>>>(JsonDynamicContract.CreateCallSiteSetter));
            base.ContractType = JsonContractType.Dynamic;
            this.Properties = new JsonPropertyCollection(base.UnderlyingType);
        }

        private static CallSite<Func<CallSite, object, object>> CreateCallSiteGetter(string name) => 
            CallSite<Func<CallSite, object, object>>.Create(new NoThrowGetBinderMember((GetMemberBinder) DynamicUtils.BinderWrapper.GetMember(name, typeof(DynamicUtils))));

        private static CallSite<Func<CallSite, object, object, object>> CreateCallSiteSetter(string name) => 
            CallSite<Func<CallSite, object, object, object>>.Create(new NoThrowSetBinderMember((SetMemberBinder) DynamicUtils.BinderWrapper.SetMember(name, typeof(DynamicUtils))));

        internal bool TryGetMember(IDynamicMetaObjectProvider dynamicProvider, string name, out object value)
        {
            ValidationUtils.ArgumentNotNull(dynamicProvider, "dynamicProvider");
            CallSite<Func<CallSite, object, object>> site = this._callSiteGetters.Get(name);
            object obj2 = site.Target(site, dynamicProvider);
            if (obj2 != NoThrowExpressionVisitor.ErrorResult)
            {
                value = obj2;
                return true;
            }
            value = null;
            return false;
        }

        internal bool TrySetMember(IDynamicMetaObjectProvider dynamicProvider, string name, object value)
        {
            ValidationUtils.ArgumentNotNull(dynamicProvider, "dynamicProvider");
            CallSite<Func<CallSite, object, object, object>> site = this._callSiteSetters.Get(name);
            return (site.Target(site, dynamicProvider, value) != NoThrowExpressionVisitor.ErrorResult);
        }

        public JsonPropertyCollection Properties { get; private set; }

        public Func<string, string> PropertyNameResolver { get; set; }
    }
}

