namespace Newtonsoft.Json.Serialization
{
    using System;

    public class DefaultNamingStrategy : NamingStrategy
    {
        protected override string ResolvePropertyName(string name) => 
            name;
    }
}

