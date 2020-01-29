namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;

    public class ReflectionAttributeProvider : IAttributeProvider
    {
        private readonly object _attributeProvider;

        public ReflectionAttributeProvider(object attributeProvider)
        {
            ValidationUtils.ArgumentNotNull(attributeProvider, "attributeProvider");
            this._attributeProvider = attributeProvider;
        }

        public IList<Attribute> GetAttributes(bool inherit) => 
            ReflectionUtils.GetAttributes(this._attributeProvider, null, inherit);

        public IList<Attribute> GetAttributes(Type attributeType, bool inherit) => 
            ReflectionUtils.GetAttributes(this._attributeProvider, attributeType, inherit);
    }
}

