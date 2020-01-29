namespace Newtonsoft.Json
{
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Threading;

    public class JsonSerializer
    {
        internal Newtonsoft.Json.TypeNameHandling _typeNameHandling = Newtonsoft.Json.TypeNameHandling.None;
        internal FormatterAssemblyStyle _typeNameAssemblyFormat;
        internal Newtonsoft.Json.PreserveReferencesHandling _preserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
        internal Newtonsoft.Json.ReferenceLoopHandling _referenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Error;
        internal Newtonsoft.Json.MissingMemberHandling _missingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
        internal Newtonsoft.Json.ObjectCreationHandling _objectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Auto;
        internal Newtonsoft.Json.NullValueHandling _nullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
        internal Newtonsoft.Json.DefaultValueHandling _defaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include;
        internal Newtonsoft.Json.ConstructorHandling _constructorHandling = Newtonsoft.Json.ConstructorHandling.Default;
        internal Newtonsoft.Json.MetadataPropertyHandling _metadataPropertyHandling = Newtonsoft.Json.MetadataPropertyHandling.Default;
        internal JsonConverterCollection _converters;
        internal IContractResolver _contractResolver = DefaultContractResolver.Instance;
        internal ITraceWriter _traceWriter;
        internal IEqualityComparer _equalityComparer;
        internal SerializationBinder _binder = DefaultSerializationBinder.Instance;
        internal StreamingContext _context = JsonSerializerSettings.DefaultContext;
        private IReferenceResolver _referenceResolver;
        private Newtonsoft.Json.Formatting? _formatting;
        private Newtonsoft.Json.DateFormatHandling? _dateFormatHandling;
        private Newtonsoft.Json.DateTimeZoneHandling? _dateTimeZoneHandling;
        private Newtonsoft.Json.DateParseHandling? _dateParseHandling;
        private Newtonsoft.Json.FloatFormatHandling? _floatFormatHandling;
        private Newtonsoft.Json.FloatParseHandling? _floatParseHandling;
        private Newtonsoft.Json.StringEscapeHandling? _stringEscapeHandling;
        private CultureInfo _culture = JsonSerializerSettings.DefaultCulture;
        private int? _maxDepth;
        private bool _maxDepthSet;
        private bool? _checkAdditionalContent;
        private string _dateFormatString;
        private bool _dateFormatStringSet;

        [field: CompilerGenerated]
        public event EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> Error;

        private static void ApplySerializerSettings(JsonSerializer serializer, JsonSerializerSettings settings)
        {
            if (!CollectionUtils.IsNullOrEmpty<JsonConverter>(settings.Converters))
            {
                for (int i = 0; i < settings.Converters.Count; i++)
                {
                    serializer.Converters.Insert(i, settings.Converters[i]);
                }
            }
            if (settings._typeNameHandling.HasValue)
            {
                serializer.TypeNameHandling = settings.TypeNameHandling;
            }
            if (settings._metadataPropertyHandling.HasValue)
            {
                serializer.MetadataPropertyHandling = settings.MetadataPropertyHandling;
            }
            if (settings._typeNameAssemblyFormat.HasValue)
            {
                serializer.TypeNameAssemblyFormat = settings.TypeNameAssemblyFormat;
            }
            if (settings._preserveReferencesHandling.HasValue)
            {
                serializer.PreserveReferencesHandling = settings.PreserveReferencesHandling;
            }
            if (settings._referenceLoopHandling.HasValue)
            {
                serializer.ReferenceLoopHandling = settings.ReferenceLoopHandling;
            }
            if (settings._missingMemberHandling.HasValue)
            {
                serializer.MissingMemberHandling = settings.MissingMemberHandling;
            }
            if (settings._objectCreationHandling.HasValue)
            {
                serializer.ObjectCreationHandling = settings.ObjectCreationHandling;
            }
            if (settings._nullValueHandling.HasValue)
            {
                serializer.NullValueHandling = settings.NullValueHandling;
            }
            if (settings._defaultValueHandling.HasValue)
            {
                serializer.DefaultValueHandling = settings.DefaultValueHandling;
            }
            if (settings._constructorHandling.HasValue)
            {
                serializer.ConstructorHandling = settings.ConstructorHandling;
            }
            if (settings._context.HasValue)
            {
                serializer.Context = settings.Context;
            }
            if (settings._checkAdditionalContent.HasValue)
            {
                serializer._checkAdditionalContent = settings._checkAdditionalContent;
            }
            if (settings.Error != null)
            {
                serializer.Error += settings.Error;
            }
            if (settings.ContractResolver != null)
            {
                serializer.ContractResolver = settings.ContractResolver;
            }
            if (settings.ReferenceResolverProvider != null)
            {
                serializer.ReferenceResolver = settings.ReferenceResolverProvider();
            }
            if (settings.TraceWriter != null)
            {
                serializer.TraceWriter = settings.TraceWriter;
            }
            if (settings.EqualityComparer != null)
            {
                serializer.EqualityComparer = settings.EqualityComparer;
            }
            if (settings.Binder != null)
            {
                serializer.Binder = settings.Binder;
            }
            if (settings._formatting.HasValue)
            {
                serializer._formatting = settings._formatting;
            }
            if (settings._dateFormatHandling.HasValue)
            {
                serializer._dateFormatHandling = settings._dateFormatHandling;
            }
            if (settings._dateTimeZoneHandling.HasValue)
            {
                serializer._dateTimeZoneHandling = settings._dateTimeZoneHandling;
            }
            if (settings._dateParseHandling.HasValue)
            {
                serializer._dateParseHandling = settings._dateParseHandling;
            }
            if (settings._dateFormatStringSet)
            {
                serializer._dateFormatString = settings._dateFormatString;
                serializer._dateFormatStringSet = settings._dateFormatStringSet;
            }
            if (settings._floatFormatHandling.HasValue)
            {
                serializer._floatFormatHandling = settings._floatFormatHandling;
            }
            if (settings._floatParseHandling.HasValue)
            {
                serializer._floatParseHandling = settings._floatParseHandling;
            }
            if (settings._stringEscapeHandling.HasValue)
            {
                serializer._stringEscapeHandling = settings._stringEscapeHandling;
            }
            if (settings._culture != null)
            {
                serializer._culture = settings._culture;
            }
            if (settings._maxDepthSet)
            {
                serializer._maxDepth = settings._maxDepth;
                serializer._maxDepthSet = settings._maxDepthSet;
            }
        }

        public static JsonSerializer Create() => 
            new JsonSerializer();

        public static JsonSerializer Create(JsonSerializerSettings settings)
        {
            JsonSerializer serializer = Create();
            if (settings != null)
            {
                ApplySerializerSettings(serializer, settings);
            }
            return serializer;
        }

        public static JsonSerializer CreateDefault()
        {
            Func<JsonSerializerSettings> defaultSettings = JsonConvert.DefaultSettings;
            return Create(defaultSettings?.Invoke());
        }

        public static JsonSerializer CreateDefault(JsonSerializerSettings settings)
        {
            JsonSerializer serializer = CreateDefault();
            if (settings != null)
            {
                ApplySerializerSettings(serializer, settings);
            }
            return serializer;
        }

        public object Deserialize(JsonReader reader) => 
            this.Deserialize(reader, null);

        public T Deserialize<T>(JsonReader reader) => 
            ((T) this.Deserialize(reader, typeof(T)));

        public object Deserialize(JsonReader reader, Type objectType) => 
            this.DeserializeInternal(reader, objectType);

        public object Deserialize(TextReader reader, Type objectType) => 
            this.Deserialize(new JsonTextReader(reader), objectType);

        internal virtual object DeserializeInternal(JsonReader reader, Type objectType)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            this.SetupReader(reader, out CultureInfo info, out Newtonsoft.Json.DateTimeZoneHandling? nullable, out Newtonsoft.Json.DateParseHandling? nullable2, out Newtonsoft.Json.FloatParseHandling? nullable3, out int? nullable4, out string str);
            TraceJsonReader reader2 = ((this.TraceWriter != null) && (this.TraceWriter.LevelFilter >= TraceLevel.Verbose)) ? new TraceJsonReader(reader) : null;
            if (reader2 == null)
            {
            }
            if (reader2 != null)
            {
                this.TraceWriter.Trace(TraceLevel.Verbose, reader2.GetDeserializedJsonMessage(), null);
            }
            this.ResetReader(reader, info, nullable, nullable2, nullable3, nullable4, str);
            return new JsonSerializerInternalReader(this).Deserialize(reader, objectType, this.CheckAdditionalContent);
        }

        internal JsonConverter GetMatchingConverter(Type type) => 
            GetMatchingConverter(this._converters, type);

        internal static JsonConverter GetMatchingConverter(IList<JsonConverter> converters, Type objectType)
        {
            if (converters != null)
            {
                for (int i = 0; i < converters.Count; i++)
                {
                    JsonConverter converter = converters[i];
                    if (converter.CanConvert(objectType))
                    {
                        return converter;
                    }
                }
            }
            return null;
        }

        internal IReferenceResolver GetReferenceResolver()
        {
            if (this._referenceResolver == null)
            {
                this._referenceResolver = new DefaultReferenceResolver();
            }
            return this._referenceResolver;
        }

        internal bool IsCheckAdditionalContentSet() => 
            this._checkAdditionalContent.HasValue;

        internal void OnError(Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> error = this.Error;
            if (error != null)
            {
                error(this, e);
            }
        }

        public void Populate(JsonReader reader, object target)
        {
            this.PopulateInternal(reader, target);
        }

        public void Populate(TextReader reader, object target)
        {
            this.Populate(new JsonTextReader(reader), target);
        }

        internal virtual void PopulateInternal(JsonReader reader, object target)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            ValidationUtils.ArgumentNotNull(target, "target");
            this.SetupReader(reader, out CultureInfo info, out Newtonsoft.Json.DateTimeZoneHandling? nullable, out Newtonsoft.Json.DateParseHandling? nullable2, out Newtonsoft.Json.FloatParseHandling? nullable3, out int? nullable4, out string str);
            TraceJsonReader reader2 = ((this.TraceWriter != null) && (this.TraceWriter.LevelFilter >= TraceLevel.Verbose)) ? new TraceJsonReader(reader) : null;
            if (reader2 == null)
            {
            }
            new JsonSerializerInternalReader(this).Populate(reader, target);
            if (reader2 != null)
            {
                this.TraceWriter.Trace(TraceLevel.Verbose, reader2.GetDeserializedJsonMessage(), null);
            }
            this.ResetReader(reader, info, nullable, nullable2, nullable3, nullable4, str);
        }

        private void ResetReader(JsonReader reader, CultureInfo previousCulture, Newtonsoft.Json.DateTimeZoneHandling? previousDateTimeZoneHandling, Newtonsoft.Json.DateParseHandling? previousDateParseHandling, Newtonsoft.Json.FloatParseHandling? previousFloatParseHandling, int? previousMaxDepth, string previousDateFormatString)
        {
            if (previousCulture != null)
            {
                reader.Culture = previousCulture;
            }
            if (previousDateTimeZoneHandling.HasValue)
            {
                reader.DateTimeZoneHandling = previousDateTimeZoneHandling.GetValueOrDefault();
            }
            if (previousDateParseHandling.HasValue)
            {
                reader.DateParseHandling = previousDateParseHandling.GetValueOrDefault();
            }
            if (previousFloatParseHandling.HasValue)
            {
                reader.FloatParseHandling = previousFloatParseHandling.GetValueOrDefault();
            }
            if (this._maxDepthSet)
            {
                reader.MaxDepth = previousMaxDepth;
            }
            if (this._dateFormatStringSet)
            {
                reader.DateFormatString = previousDateFormatString;
            }
            JsonTextReader reader2 = reader as JsonTextReader;
            if (reader2 != null)
            {
                reader2.NameTable = null;
            }
        }

        public void Serialize(JsonWriter jsonWriter, object value)
        {
            this.SerializeInternal(jsonWriter, value, null);
        }

        public void Serialize(TextWriter textWriter, object value)
        {
            this.Serialize(new JsonTextWriter(textWriter), value);
        }

        public void Serialize(JsonWriter jsonWriter, object value, Type objectType)
        {
            this.SerializeInternal(jsonWriter, value, objectType);
        }

        public void Serialize(TextWriter textWriter, object value, Type objectType)
        {
            this.Serialize(new JsonTextWriter(textWriter), value, objectType);
        }

        internal virtual void SerializeInternal(JsonWriter jsonWriter, object value, Type objectType)
        {
            ValidationUtils.ArgumentNotNull(jsonWriter, "jsonWriter");
            Newtonsoft.Json.Formatting? nullable = null;
            if (this._formatting.HasValue)
            {
                Newtonsoft.Json.Formatting? nullable2 = this._formatting;
                if ((jsonWriter.Formatting == ((Newtonsoft.Json.Formatting) nullable2.GetValueOrDefault())) ? !nullable2.HasValue : true)
                {
                    nullable = new Newtonsoft.Json.Formatting?(jsonWriter.Formatting);
                    jsonWriter.Formatting = this._formatting.GetValueOrDefault();
                }
            }
            Newtonsoft.Json.DateFormatHandling? nullable3 = null;
            if (this._dateFormatHandling.HasValue)
            {
                Newtonsoft.Json.DateFormatHandling? nullable4 = this._dateFormatHandling;
                if ((jsonWriter.DateFormatHandling == ((Newtonsoft.Json.DateFormatHandling) nullable4.GetValueOrDefault())) ? !nullable4.HasValue : true)
                {
                    nullable3 = new Newtonsoft.Json.DateFormatHandling?(jsonWriter.DateFormatHandling);
                    jsonWriter.DateFormatHandling = this._dateFormatHandling.GetValueOrDefault();
                }
            }
            Newtonsoft.Json.DateTimeZoneHandling? nullable5 = null;
            if (this._dateTimeZoneHandling.HasValue)
            {
                Newtonsoft.Json.DateTimeZoneHandling? nullable6 = this._dateTimeZoneHandling;
                if ((jsonWriter.DateTimeZoneHandling == ((Newtonsoft.Json.DateTimeZoneHandling) nullable6.GetValueOrDefault())) ? !nullable6.HasValue : true)
                {
                    nullable5 = new Newtonsoft.Json.DateTimeZoneHandling?(jsonWriter.DateTimeZoneHandling);
                    jsonWriter.DateTimeZoneHandling = this._dateTimeZoneHandling.GetValueOrDefault();
                }
            }
            Newtonsoft.Json.FloatFormatHandling? nullable7 = null;
            if (this._floatFormatHandling.HasValue)
            {
                Newtonsoft.Json.FloatFormatHandling? nullable8 = this._floatFormatHandling;
                if ((jsonWriter.FloatFormatHandling == ((Newtonsoft.Json.FloatFormatHandling) nullable8.GetValueOrDefault())) ? !nullable8.HasValue : true)
                {
                    nullable7 = new Newtonsoft.Json.FloatFormatHandling?(jsonWriter.FloatFormatHandling);
                    jsonWriter.FloatFormatHandling = this._floatFormatHandling.GetValueOrDefault();
                }
            }
            Newtonsoft.Json.StringEscapeHandling? nullable9 = null;
            if (this._stringEscapeHandling.HasValue)
            {
                Newtonsoft.Json.StringEscapeHandling? nullable10 = this._stringEscapeHandling;
                if ((jsonWriter.StringEscapeHandling == ((Newtonsoft.Json.StringEscapeHandling) nullable10.GetValueOrDefault())) ? !nullable10.HasValue : true)
                {
                    nullable9 = new Newtonsoft.Json.StringEscapeHandling?(jsonWriter.StringEscapeHandling);
                    jsonWriter.StringEscapeHandling = this._stringEscapeHandling.GetValueOrDefault();
                }
            }
            CultureInfo culture = null;
            if ((this._culture != null) && !this._culture.Equals(jsonWriter.Culture))
            {
                culture = jsonWriter.Culture;
                jsonWriter.Culture = this._culture;
            }
            string dateFormatString = null;
            if (this._dateFormatStringSet && (jsonWriter.DateFormatString != this._dateFormatString))
            {
                dateFormatString = jsonWriter.DateFormatString;
                jsonWriter.DateFormatString = this._dateFormatString;
            }
            TraceJsonWriter writer = ((this.TraceWriter != null) && (this.TraceWriter.LevelFilter >= TraceLevel.Verbose)) ? new TraceJsonWriter(jsonWriter) : null;
            if (writer == null)
            {
            }
            new JsonSerializerInternalWriter(this).Serialize(jsonWriter, value, objectType);
            if (writer != null)
            {
                this.TraceWriter.Trace(TraceLevel.Verbose, writer.GetSerializedJsonMessage(), null);
            }
            if (nullable.HasValue)
            {
                jsonWriter.Formatting = nullable.GetValueOrDefault();
            }
            if (nullable3.HasValue)
            {
                jsonWriter.DateFormatHandling = nullable3.GetValueOrDefault();
            }
            if (nullable5.HasValue)
            {
                jsonWriter.DateTimeZoneHandling = nullable5.GetValueOrDefault();
            }
            if (nullable7.HasValue)
            {
                jsonWriter.FloatFormatHandling = nullable7.GetValueOrDefault();
            }
            if (nullable9.HasValue)
            {
                jsonWriter.StringEscapeHandling = nullable9.GetValueOrDefault();
            }
            if (this._dateFormatStringSet)
            {
                jsonWriter.DateFormatString = dateFormatString;
            }
            if (culture != null)
            {
                jsonWriter.Culture = culture;
            }
        }

        private void SetupReader(JsonReader reader, out CultureInfo previousCulture, out Newtonsoft.Json.DateTimeZoneHandling? previousDateTimeZoneHandling, out Newtonsoft.Json.DateParseHandling? previousDateParseHandling, out Newtonsoft.Json.FloatParseHandling? previousFloatParseHandling, out int? previousMaxDepth, out string previousDateFormatString)
        {
            if ((this._culture != null) && !this._culture.Equals(reader.Culture))
            {
                previousCulture = reader.Culture;
                reader.Culture = this._culture;
            }
            else
            {
                previousCulture = null;
            }
            if (this._dateTimeZoneHandling.HasValue)
            {
                Newtonsoft.Json.DateTimeZoneHandling? nullable = this._dateTimeZoneHandling;
                if ((reader.DateTimeZoneHandling == ((Newtonsoft.Json.DateTimeZoneHandling) nullable.GetValueOrDefault())) ? !nullable.HasValue : true)
                {
                    previousDateTimeZoneHandling = new Newtonsoft.Json.DateTimeZoneHandling?(reader.DateTimeZoneHandling);
                    reader.DateTimeZoneHandling = this._dateTimeZoneHandling.GetValueOrDefault();
                    goto Label_00A9;
                }
            }
            previousDateTimeZoneHandling = 0;
        Label_00A9:
            if (this._dateParseHandling.HasValue)
            {
                Newtonsoft.Json.DateParseHandling? nullable2 = this._dateParseHandling;
                if ((reader.DateParseHandling == ((Newtonsoft.Json.DateParseHandling) nullable2.GetValueOrDefault())) ? !nullable2.HasValue : true)
                {
                    previousDateParseHandling = new Newtonsoft.Json.DateParseHandling?(reader.DateParseHandling);
                    reader.DateParseHandling = this._dateParseHandling.GetValueOrDefault();
                    goto Label_0117;
                }
            }
            previousDateParseHandling = 0;
        Label_0117:
            if (this._floatParseHandling.HasValue)
            {
                Newtonsoft.Json.FloatParseHandling? nullable3 = this._floatParseHandling;
                if ((reader.FloatParseHandling == ((Newtonsoft.Json.FloatParseHandling) nullable3.GetValueOrDefault())) ? !nullable3.HasValue : true)
                {
                    previousFloatParseHandling = new Newtonsoft.Json.FloatParseHandling?(reader.FloatParseHandling);
                    reader.FloatParseHandling = this._floatParseHandling.GetValueOrDefault();
                    goto Label_0185;
                }
            }
            previousFloatParseHandling = 0;
        Label_0185:
            if (this._maxDepthSet)
            {
                int? maxDepth = reader.MaxDepth;
                int? nullable5 = this._maxDepth;
                if ((maxDepth.GetValueOrDefault() == nullable5.GetValueOrDefault()) ? (maxDepth.HasValue != nullable5.HasValue) : true)
                {
                    previousMaxDepth = reader.MaxDepth;
                    reader.MaxDepth = this._maxDepth;
                    goto Label_01F6;
                }
            }
            previousMaxDepth = 0;
        Label_01F6:
            if (this._dateFormatStringSet && (reader.DateFormatString != this._dateFormatString))
            {
                previousDateFormatString = reader.DateFormatString;
                reader.DateFormatString = this._dateFormatString;
            }
            else
            {
                previousDateFormatString = null;
            }
            JsonTextReader reader2 = reader as JsonTextReader;
            if (reader2 != null)
            {
                DefaultContractResolver resolver = this._contractResolver as DefaultContractResolver;
                if (resolver != null)
                {
                    reader2.NameTable = resolver.GetState().NameTable;
                }
            }
        }

        public virtual IReferenceResolver ReferenceResolver
        {
            get => 
                this.GetReferenceResolver();
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Reference resolver cannot be null.");
                }
                this._referenceResolver = value;
            }
        }

        public virtual SerializationBinder Binder
        {
            get => 
                this._binder;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Serialization binder cannot be null.");
                }
                this._binder = value;
            }
        }

        public virtual ITraceWriter TraceWriter
        {
            get => 
                this._traceWriter;
            set => 
                (this._traceWriter = value);
        }

        public virtual IEqualityComparer EqualityComparer
        {
            get => 
                this._equalityComparer;
            set => 
                (this._equalityComparer = value);
        }

        public virtual Newtonsoft.Json.TypeNameHandling TypeNameHandling
        {
            get => 
                this._typeNameHandling;
            set
            {
                if ((value < Newtonsoft.Json.TypeNameHandling.None) || (value > Newtonsoft.Json.TypeNameHandling.Auto))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._typeNameHandling = value;
            }
        }

        public virtual FormatterAssemblyStyle TypeNameAssemblyFormat
        {
            get => 
                this._typeNameAssemblyFormat;
            set
            {
                if ((value < FormatterAssemblyStyle.Simple) || (value > FormatterAssemblyStyle.Full))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._typeNameAssemblyFormat = value;
            }
        }

        public virtual Newtonsoft.Json.PreserveReferencesHandling PreserveReferencesHandling
        {
            get => 
                this._preserveReferencesHandling;
            set
            {
                if ((value < Newtonsoft.Json.PreserveReferencesHandling.None) || (value > Newtonsoft.Json.PreserveReferencesHandling.All))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._preserveReferencesHandling = value;
            }
        }

        public virtual Newtonsoft.Json.ReferenceLoopHandling ReferenceLoopHandling
        {
            get => 
                this._referenceLoopHandling;
            set
            {
                if ((value < Newtonsoft.Json.ReferenceLoopHandling.Error) || (value > Newtonsoft.Json.ReferenceLoopHandling.Serialize))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._referenceLoopHandling = value;
            }
        }

        public virtual Newtonsoft.Json.MissingMemberHandling MissingMemberHandling
        {
            get => 
                this._missingMemberHandling;
            set
            {
                if ((value < Newtonsoft.Json.MissingMemberHandling.Ignore) || (value > Newtonsoft.Json.MissingMemberHandling.Error))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._missingMemberHandling = value;
            }
        }

        public virtual Newtonsoft.Json.NullValueHandling NullValueHandling
        {
            get => 
                this._nullValueHandling;
            set
            {
                if ((value < Newtonsoft.Json.NullValueHandling.Include) || (value > Newtonsoft.Json.NullValueHandling.Ignore))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._nullValueHandling = value;
            }
        }

        public virtual Newtonsoft.Json.DefaultValueHandling DefaultValueHandling
        {
            get => 
                this._defaultValueHandling;
            set
            {
                if ((value < Newtonsoft.Json.DefaultValueHandling.Include) || (value > Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._defaultValueHandling = value;
            }
        }

        public virtual Newtonsoft.Json.ObjectCreationHandling ObjectCreationHandling
        {
            get => 
                this._objectCreationHandling;
            set
            {
                if ((value < Newtonsoft.Json.ObjectCreationHandling.Auto) || (value > Newtonsoft.Json.ObjectCreationHandling.Replace))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._objectCreationHandling = value;
            }
        }

        public virtual Newtonsoft.Json.ConstructorHandling ConstructorHandling
        {
            get => 
                this._constructorHandling;
            set
            {
                if ((value < Newtonsoft.Json.ConstructorHandling.Default) || (value > Newtonsoft.Json.ConstructorHandling.AllowNonPublicDefaultConstructor))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._constructorHandling = value;
            }
        }

        public virtual Newtonsoft.Json.MetadataPropertyHandling MetadataPropertyHandling
        {
            get => 
                this._metadataPropertyHandling;
            set
            {
                if ((value < Newtonsoft.Json.MetadataPropertyHandling.Default) || (value > Newtonsoft.Json.MetadataPropertyHandling.Ignore))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._metadataPropertyHandling = value;
            }
        }

        public virtual JsonConverterCollection Converters
        {
            get
            {
                if (this._converters == null)
                {
                    this._converters = new JsonConverterCollection();
                }
                return this._converters;
            }
        }

        public virtual IContractResolver ContractResolver
        {
            get => 
                this._contractResolver;
            set
            {
                if (value == null)
                {
                }
                this._contractResolver = DefaultContractResolver.Instance;
            }
        }

        public virtual StreamingContext Context
        {
            get => 
                this._context;
            set => 
                (this._context = value);
        }

        public virtual Newtonsoft.Json.Formatting Formatting
        {
            get
            {
                Newtonsoft.Json.Formatting? nullable = this._formatting;
                if (!nullable.HasValue)
                {
                    return Newtonsoft.Json.Formatting.None;
                }
                return nullable.GetValueOrDefault();
            }
            set => 
                (this._formatting = new Newtonsoft.Json.Formatting?(value));
        }

        public virtual Newtonsoft.Json.DateFormatHandling DateFormatHandling
        {
            get
            {
                Newtonsoft.Json.DateFormatHandling? nullable = this._dateFormatHandling;
                if (!nullable.HasValue)
                {
                    return Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                }
                return nullable.GetValueOrDefault();
            }
            set => 
                (this._dateFormatHandling = new Newtonsoft.Json.DateFormatHandling?(value));
        }

        public virtual Newtonsoft.Json.DateTimeZoneHandling DateTimeZoneHandling
        {
            get
            {
                Newtonsoft.Json.DateTimeZoneHandling? nullable = this._dateTimeZoneHandling;
                if (!nullable.HasValue)
                {
                    return Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
                }
                return nullable.GetValueOrDefault();
            }
            set => 
                (this._dateTimeZoneHandling = new Newtonsoft.Json.DateTimeZoneHandling?(value));
        }

        public virtual Newtonsoft.Json.DateParseHandling DateParseHandling
        {
            get
            {
                Newtonsoft.Json.DateParseHandling? nullable = this._dateParseHandling;
                if (!nullable.HasValue)
                {
                    return Newtonsoft.Json.DateParseHandling.DateTime;
                }
                return nullable.GetValueOrDefault();
            }
            set => 
                (this._dateParseHandling = new Newtonsoft.Json.DateParseHandling?(value));
        }

        public virtual Newtonsoft.Json.FloatParseHandling FloatParseHandling
        {
            get
            {
                Newtonsoft.Json.FloatParseHandling? nullable = this._floatParseHandling;
                if (!nullable.HasValue)
                {
                    return Newtonsoft.Json.FloatParseHandling.Double;
                }
                return nullable.GetValueOrDefault();
            }
            set => 
                (this._floatParseHandling = new Newtonsoft.Json.FloatParseHandling?(value));
        }

        public virtual Newtonsoft.Json.FloatFormatHandling FloatFormatHandling
        {
            get
            {
                Newtonsoft.Json.FloatFormatHandling? nullable = this._floatFormatHandling;
                if (!nullable.HasValue)
                {
                    return Newtonsoft.Json.FloatFormatHandling.String;
                }
                return nullable.GetValueOrDefault();
            }
            set => 
                (this._floatFormatHandling = new Newtonsoft.Json.FloatFormatHandling?(value));
        }

        public virtual Newtonsoft.Json.StringEscapeHandling StringEscapeHandling
        {
            get
            {
                Newtonsoft.Json.StringEscapeHandling? nullable = this._stringEscapeHandling;
                if (!nullable.HasValue)
                {
                    return Newtonsoft.Json.StringEscapeHandling.Default;
                }
                return nullable.GetValueOrDefault();
            }
            set => 
                (this._stringEscapeHandling = new Newtonsoft.Json.StringEscapeHandling?(value));
        }

        public virtual string DateFormatString
        {
            get
            {
                if (this._dateFormatString == null)
                {
                }
                return "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
            }
            set
            {
                this._dateFormatString = value;
                this._dateFormatStringSet = true;
            }
        }

        public virtual CultureInfo Culture
        {
            get
            {
                if (this._culture == null)
                {
                }
                return JsonSerializerSettings.DefaultCulture;
            }
            set => 
                (this._culture = value);
        }

        public virtual int? MaxDepth
        {
            get => 
                this._maxDepth;
            set
            {
                int? nullable = value;
                int num = 0;
                if ((nullable.GetValueOrDefault() <= num) ? nullable.HasValue : false)
                {
                    throw new ArgumentException("Value must be positive.", "value");
                }
                this._maxDepth = value;
                this._maxDepthSet = true;
            }
        }

        public virtual bool CheckAdditionalContent
        {
            get
            {
                bool? nullable = this._checkAdditionalContent;
                if (!nullable.HasValue)
                {
                    return false;
                }
                return nullable.GetValueOrDefault();
            }
            set => 
                (this._checkAdditionalContent = new bool?(value));
        }
    }
}

