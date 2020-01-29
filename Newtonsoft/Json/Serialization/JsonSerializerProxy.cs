namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;

    internal class JsonSerializerProxy : JsonSerializer
    {
        private readonly JsonSerializerInternalReader _serializerReader;
        private readonly JsonSerializerInternalWriter _serializerWriter;
        private readonly JsonSerializer _serializer;

        public event EventHandler<ErrorEventArgs> Error
        {
            add
            {
                this._serializer.Error += value;
            }
            remove
            {
                this._serializer.Error -= value;
            }
        }

        public JsonSerializerProxy(JsonSerializerInternalReader serializerReader)
        {
            ValidationUtils.ArgumentNotNull(serializerReader, "serializerReader");
            this._serializerReader = serializerReader;
            this._serializer = serializerReader.Serializer;
        }

        public JsonSerializerProxy(JsonSerializerInternalWriter serializerWriter)
        {
            ValidationUtils.ArgumentNotNull(serializerWriter, "serializerWriter");
            this._serializerWriter = serializerWriter;
            this._serializer = serializerWriter.Serializer;
        }

        internal override object DeserializeInternal(JsonReader reader, Type objectType)
        {
            if (this._serializerReader != null)
            {
                return this._serializerReader.Deserialize(reader, objectType, false);
            }
            return this._serializer.Deserialize(reader, objectType);
        }

        internal JsonSerializerInternalBase GetInternalSerializer()
        {
            if (this._serializerReader != null)
            {
                return this._serializerReader;
            }
            return this._serializerWriter;
        }

        internal override void PopulateInternal(JsonReader reader, object target)
        {
            if (this._serializerReader != null)
            {
                this._serializerReader.Populate(reader, target);
            }
            else
            {
                this._serializer.Populate(reader, target);
            }
        }

        internal override void SerializeInternal(JsonWriter jsonWriter, object value, Type rootType)
        {
            if (this._serializerWriter != null)
            {
                this._serializerWriter.Serialize(jsonWriter, value, rootType);
            }
            else
            {
                this._serializer.Serialize(jsonWriter, value);
            }
        }

        public override IReferenceResolver ReferenceResolver
        {
            get => 
                this._serializer.ReferenceResolver;
            set => 
                (this._serializer.ReferenceResolver = value);
        }

        public override ITraceWriter TraceWriter
        {
            get => 
                this._serializer.TraceWriter;
            set => 
                (this._serializer.TraceWriter = value);
        }

        public override IEqualityComparer EqualityComparer
        {
            get => 
                this._serializer.EqualityComparer;
            set => 
                (this._serializer.EqualityComparer = value);
        }

        public override JsonConverterCollection Converters =>
            this._serializer.Converters;

        public override Newtonsoft.Json.DefaultValueHandling DefaultValueHandling
        {
            get => 
                this._serializer.DefaultValueHandling;
            set => 
                (this._serializer.DefaultValueHandling = value);
        }

        public override IContractResolver ContractResolver
        {
            get => 
                this._serializer.ContractResolver;
            set => 
                (this._serializer.ContractResolver = value);
        }

        public override Newtonsoft.Json.MissingMemberHandling MissingMemberHandling
        {
            get => 
                this._serializer.MissingMemberHandling;
            set => 
                (this._serializer.MissingMemberHandling = value);
        }

        public override Newtonsoft.Json.NullValueHandling NullValueHandling
        {
            get => 
                this._serializer.NullValueHandling;
            set => 
                (this._serializer.NullValueHandling = value);
        }

        public override Newtonsoft.Json.ObjectCreationHandling ObjectCreationHandling
        {
            get => 
                this._serializer.ObjectCreationHandling;
            set => 
                (this._serializer.ObjectCreationHandling = value);
        }

        public override Newtonsoft.Json.ReferenceLoopHandling ReferenceLoopHandling
        {
            get => 
                this._serializer.ReferenceLoopHandling;
            set => 
                (this._serializer.ReferenceLoopHandling = value);
        }

        public override Newtonsoft.Json.PreserveReferencesHandling PreserveReferencesHandling
        {
            get => 
                this._serializer.PreserveReferencesHandling;
            set => 
                (this._serializer.PreserveReferencesHandling = value);
        }

        public override Newtonsoft.Json.TypeNameHandling TypeNameHandling
        {
            get => 
                this._serializer.TypeNameHandling;
            set => 
                (this._serializer.TypeNameHandling = value);
        }

        public override Newtonsoft.Json.MetadataPropertyHandling MetadataPropertyHandling
        {
            get => 
                this._serializer.MetadataPropertyHandling;
            set => 
                (this._serializer.MetadataPropertyHandling = value);
        }

        public override FormatterAssemblyStyle TypeNameAssemblyFormat
        {
            get => 
                this._serializer.TypeNameAssemblyFormat;
            set => 
                (this._serializer.TypeNameAssemblyFormat = value);
        }

        public override Newtonsoft.Json.ConstructorHandling ConstructorHandling
        {
            get => 
                this._serializer.ConstructorHandling;
            set => 
                (this._serializer.ConstructorHandling = value);
        }

        public override SerializationBinder Binder
        {
            get => 
                this._serializer.Binder;
            set => 
                (this._serializer.Binder = value);
        }

        public override StreamingContext Context
        {
            get => 
                this._serializer.Context;
            set => 
                (this._serializer.Context = value);
        }

        public override Newtonsoft.Json.Formatting Formatting
        {
            get => 
                this._serializer.Formatting;
            set => 
                (this._serializer.Formatting = value);
        }

        public override Newtonsoft.Json.DateFormatHandling DateFormatHandling
        {
            get => 
                this._serializer.DateFormatHandling;
            set => 
                (this._serializer.DateFormatHandling = value);
        }

        public override Newtonsoft.Json.DateTimeZoneHandling DateTimeZoneHandling
        {
            get => 
                this._serializer.DateTimeZoneHandling;
            set => 
                (this._serializer.DateTimeZoneHandling = value);
        }

        public override Newtonsoft.Json.DateParseHandling DateParseHandling
        {
            get => 
                this._serializer.DateParseHandling;
            set => 
                (this._serializer.DateParseHandling = value);
        }

        public override Newtonsoft.Json.FloatFormatHandling FloatFormatHandling
        {
            get => 
                this._serializer.FloatFormatHandling;
            set => 
                (this._serializer.FloatFormatHandling = value);
        }

        public override Newtonsoft.Json.FloatParseHandling FloatParseHandling
        {
            get => 
                this._serializer.FloatParseHandling;
            set => 
                (this._serializer.FloatParseHandling = value);
        }

        public override Newtonsoft.Json.StringEscapeHandling StringEscapeHandling
        {
            get => 
                this._serializer.StringEscapeHandling;
            set => 
                (this._serializer.StringEscapeHandling = value);
        }

        public override string DateFormatString
        {
            get => 
                this._serializer.DateFormatString;
            set => 
                (this._serializer.DateFormatString = value);
        }

        public override CultureInfo Culture
        {
            get => 
                this._serializer.Culture;
            set => 
                (this._serializer.Culture = value);
        }

        public override int? MaxDepth
        {
            get => 
                this._serializer.MaxDepth;
            set => 
                (this._serializer.MaxDepth = value);
        }

        public override bool CheckAdditionalContent
        {
            get => 
                this._serializer.CheckAdditionalContent;
            set => 
                (this._serializer.CheckAdditionalContent = value);
        }
    }
}

