namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Runtime.CompilerServices;

    public class JsonProperty
    {
        internal Newtonsoft.Json.Required? _required;
        internal bool _hasExplicitDefaultValue;
        private object _defaultValue;
        private bool _hasGeneratedDefaultValue;
        private string _propertyName;
        internal bool _skipPropertyNameEscape;
        private Type _propertyType;

        internal object GetResolvedDefaultValue()
        {
            if (this._propertyType == null)
            {
                return null;
            }
            if (!this._hasExplicitDefaultValue && !this._hasGeneratedDefaultValue)
            {
                this._defaultValue = ReflectionUtils.GetDefaultValue(this.PropertyType);
                this._hasGeneratedDefaultValue = true;
            }
            return this._defaultValue;
        }

        public override string ToString() => 
            this.PropertyName;

        internal void WritePropertyName(JsonWriter writer)
        {
            if (this._skipPropertyNameEscape)
            {
                writer.WritePropertyName(this.PropertyName, false);
            }
            else
            {
                writer.WritePropertyName(this.PropertyName);
            }
        }

        internal JsonContract PropertyContract { get; set; }

        public string PropertyName
        {
            get => 
                this._propertyName;
            set
            {
                this._propertyName = value;
                this._skipPropertyNameEscape = !JavaScriptUtils.ShouldEscapeJavaScriptString(this._propertyName, JavaScriptUtils.HtmlCharEscapeFlags);
            }
        }

        public Type DeclaringType { get; set; }

        public int? Order { get; set; }

        public string UnderlyingName { get; set; }

        public IValueProvider ValueProvider { get; set; }

        public IAttributeProvider AttributeProvider { get; set; }

        public Type PropertyType
        {
            get => 
                this._propertyType;
            set
            {
                if (this._propertyType != value)
                {
                    this._propertyType = value;
                    this._hasGeneratedDefaultValue = false;
                }
            }
        }

        public JsonConverter Converter { get; set; }

        public JsonConverter MemberConverter { get; set; }

        public bool Ignored { get; set; }

        public bool Readable { get; set; }

        public bool Writable { get; set; }

        public bool HasMemberAttribute { get; set; }

        public object DefaultValue
        {
            get
            {
                if (!this._hasExplicitDefaultValue)
                {
                    return null;
                }
                return this._defaultValue;
            }
            set
            {
                this._hasExplicitDefaultValue = true;
                this._defaultValue = value;
            }
        }

        public Newtonsoft.Json.Required Required
        {
            get
            {
                Newtonsoft.Json.Required? nullable = this._required;
                if (!nullable.HasValue)
                {
                    return Newtonsoft.Json.Required.Default;
                }
                return nullable.GetValueOrDefault();
            }
            set => 
                (this._required = new Newtonsoft.Json.Required?(value));
        }

        public bool? IsReference { get; set; }

        public Newtonsoft.Json.NullValueHandling? NullValueHandling { get; set; }

        public Newtonsoft.Json.DefaultValueHandling? DefaultValueHandling { get; set; }

        public Newtonsoft.Json.ReferenceLoopHandling? ReferenceLoopHandling { get; set; }

        public Newtonsoft.Json.ObjectCreationHandling? ObjectCreationHandling { get; set; }

        public Newtonsoft.Json.TypeNameHandling? TypeNameHandling { get; set; }

        public Predicate<object> ShouldSerialize { get; set; }

        public Predicate<object> ShouldDeserialize { get; set; }

        public Predicate<object> GetIsSpecified { get; set; }

        public Action<object, object> SetIsSpecified { get; set; }

        public JsonConverter ItemConverter { get; set; }

        public bool? ItemIsReference { get; set; }

        public Newtonsoft.Json.TypeNameHandling? ItemTypeNameHandling { get; set; }

        public Newtonsoft.Json.ReferenceLoopHandling? ItemReferenceLoopHandling { get; set; }
    }
}

