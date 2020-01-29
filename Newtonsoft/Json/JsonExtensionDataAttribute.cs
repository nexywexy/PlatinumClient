namespace Newtonsoft.Json
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false)]
    public class JsonExtensionDataAttribute : Attribute
    {
        public JsonExtensionDataAttribute()
        {
            this.WriteData = true;
            this.ReadData = true;
        }

        public bool WriteData { get; set; }

        public bool ReadData { get; set; }
    }
}

