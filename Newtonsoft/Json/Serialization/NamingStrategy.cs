namespace Newtonsoft.Json.Serialization
{
    using System;
    using System.Runtime.CompilerServices;

    public abstract class NamingStrategy
    {
        protected NamingStrategy()
        {
        }

        public virtual string GetDictionaryKey(string key)
        {
            if (!this.ProcessDictionaryKeys)
            {
                return key;
            }
            return this.ResolvePropertyName(key);
        }

        public virtual string GetPropertyName(string name, bool hasSpecifiedName)
        {
            if (hasSpecifiedName && !this.OverrideSpecifiedNames)
            {
                return name;
            }
            return this.ResolvePropertyName(name);
        }

        protected abstract string ResolvePropertyName(string name);

        public bool ProcessDictionaryKeys { get; set; }

        public bool OverrideSpecifiedNames { get; set; }
    }
}

