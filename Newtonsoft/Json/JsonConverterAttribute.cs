namespace Newtonsoft.Json
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple=false)]
    public sealed class JsonConverterAttribute : Attribute
    {
        private readonly Type _converterType;

        public JsonConverterAttribute(Type converterType)
        {
            if (converterType == null)
            {
                throw new ArgumentNullException("converterType");
            }
            this._converterType = converterType;
        }

        public JsonConverterAttribute(Type converterType, params object[] converterParameters) : this(converterType)
        {
            this.ConverterParameters = converterParameters;
        }

        public Type ConverterType =>
            this._converterType;

        public object[] ConverterParameters { get; private set; }
    }
}

