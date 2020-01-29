namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json.Utilities;
    using System;

    public class SnakeCaseNamingStrategy : NamingStrategy
    {
        public SnakeCaseNamingStrategy()
        {
        }

        public SnakeCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames)
        {
            base.ProcessDictionaryKeys = processDictionaryKeys;
            base.OverrideSpecifiedNames = overrideSpecifiedNames;
        }

        protected override string ResolvePropertyName(string name) => 
            StringUtils.ToSnakeCase(name);
    }
}

