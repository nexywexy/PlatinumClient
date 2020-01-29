namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Security;

    public class JsonObjectContract : JsonContainerContract
    {
        internal bool ExtensionDataIsJToken;
        private bool? _hasRequiredOrDefaultValueProperties;
        private ConstructorInfo _parametrizedConstructor;
        private ConstructorInfo _overrideConstructor;
        private ObjectConstructor<object> _overrideCreator;
        private ObjectConstructor<object> _parameterizedCreator;
        private JsonPropertyCollection _creatorParameters;
        private Type _extensionDataValueType;

        public JsonObjectContract(Type underlyingType) : base(underlyingType)
        {
            base.ContractType = JsonContractType.Object;
            this.Properties = new JsonPropertyCollection(base.UnderlyingType);
        }

        [SecuritySafeCritical]
        internal object GetUninitializedObject()
        {
            if (!JsonTypeReflector.FullyTrusted)
            {
                throw new JsonException("Insufficient permissions. Creating an uninitialized '{0}' type requires full trust.".FormatWith(CultureInfo.InvariantCulture, base.NonNullableUnderlyingType));
            }
            return FormatterServices.GetUninitializedObject(base.NonNullableUnderlyingType);
        }

        public Newtonsoft.Json.MemberSerialization MemberSerialization { get; set; }

        public Required? ItemRequired { get; set; }

        public JsonPropertyCollection Properties { get; private set; }

        [Obsolete("ConstructorParameters is obsolete. Use CreatorParameters instead.")]
        public JsonPropertyCollection ConstructorParameters =>
            this.CreatorParameters;

        public JsonPropertyCollection CreatorParameters
        {
            get
            {
                if (this._creatorParameters == null)
                {
                    this._creatorParameters = new JsonPropertyCollection(base.UnderlyingType);
                }
                return this._creatorParameters;
            }
        }

        [Obsolete("OverrideConstructor is obsolete. Use OverrideCreator instead.")]
        public ConstructorInfo OverrideConstructor
        {
            get => 
                this._overrideConstructor;
            set
            {
                this._overrideConstructor = value;
                this._overrideCreator = (value != null) ? JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(value) : null;
            }
        }

        [Obsolete("ParametrizedConstructor is obsolete. Use OverrideCreator instead.")]
        public ConstructorInfo ParametrizedConstructor
        {
            get => 
                this._parametrizedConstructor;
            set
            {
                this._parametrizedConstructor = value;
                this._parameterizedCreator = (value != null) ? JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(value) : null;
            }
        }

        public ObjectConstructor<object> OverrideCreator
        {
            get => 
                this._overrideCreator;
            set
            {
                this._overrideCreator = value;
                this._overrideConstructor = null;
            }
        }

        internal ObjectConstructor<object> ParameterizedCreator =>
            this._parameterizedCreator;

        public Newtonsoft.Json.Serialization.ExtensionDataSetter ExtensionDataSetter { get; set; }

        public Newtonsoft.Json.Serialization.ExtensionDataGetter ExtensionDataGetter { get; set; }

        public Type ExtensionDataValueType
        {
            get => 
                this._extensionDataValueType;
            set
            {
                this._extensionDataValueType = value;
                this.ExtensionDataIsJToken = (value != null) && typeof(JToken).IsAssignableFrom(value);
            }
        }

        internal bool HasRequiredOrDefaultValueProperties
        {
            get
            {
                if (!this._hasRequiredOrDefaultValueProperties.HasValue)
                {
                    this._hasRequiredOrDefaultValueProperties = false;
                    if (((Required) this.ItemRequired.GetValueOrDefault(Required.Default)) != Required.Default)
                    {
                        this._hasRequiredOrDefaultValueProperties = true;
                    }
                    else
                    {
                        foreach (JsonProperty property in this.Properties)
                        {
                            if (property.Required == Required.Default)
                            {
                                DefaultValueHandling? nullable4 = ((DefaultValueHandling) property.DefaultValueHandling) & DefaultValueHandling.Populate;
                                DefaultValueHandling populate = DefaultValueHandling.Populate;
                                if (!((((DefaultValueHandling) nullable4.GetValueOrDefault()) == populate) ? nullable4.HasValue : false))
                                {
                                    continue;
                                }
                            }
                            this._hasRequiredOrDefaultValueProperties = true;
                            break;
                        }
                    }
                }
                return this._hasRequiredOrDefaultValueProperties.GetValueOrDefault();
            }
        }
    }
}

