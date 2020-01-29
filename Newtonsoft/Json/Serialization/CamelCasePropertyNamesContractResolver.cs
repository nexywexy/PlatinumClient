namespace Newtonsoft.Json.Serialization
{
    using System;

    public class CamelCasePropertyNamesContractResolver : DefaultContractResolver
    {
        public CamelCasePropertyNamesContractResolver() : base(true)
        {
            CamelCaseNamingStrategy strategy1 = new CamelCaseNamingStrategy {
                ProcessDictionaryKeys = true,
                OverrideSpecifiedNames = true
            };
            base.NamingStrategy = strategy1;
        }
    }
}

