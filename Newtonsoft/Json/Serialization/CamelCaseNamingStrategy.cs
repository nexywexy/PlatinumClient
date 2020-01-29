namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json.Utilities;
    using System;

    public class CamelCaseNamingStrategy : NamingStrategy
    {
        public CamelCaseNamingStrategy()
        {
        }

        public CamelCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames)
        {
            base.ProcessDictionaryKeys = processDictionaryKeys;
            base.OverrideSpecifiedNames = overrideSpecifiedNames;
        }

        protected override string ResolvePropertyName(string name) => 
            StringUtils.ToCamelCase(name);
    }
}

