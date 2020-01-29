namespace Newtonsoft.Json
{
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    public static class JsonConvert
    {
        public static readonly string True = "true";
        public static readonly string False = "false";
        public static readonly string Null = "null";
        public static readonly string Undefined = "undefined";
        public static readonly string PositiveInfinity = "Infinity";
        public static readonly string NegativeInfinity = "-Infinity";
        public static readonly string NaN = "NaN";

        public static T DeserializeAnonymousType<T>(string value, T anonymousTypeObject) => 
            DeserializeObject<T>(value);

        public static T DeserializeAnonymousType<T>(string value, T anonymousTypeObject, JsonSerializerSettings settings) => 
            DeserializeObject<T>(value, settings);

        public static object DeserializeObject(string value) => 
            DeserializeObject(value, null, (JsonSerializerSettings) null);

        public static T DeserializeObject<T>(string value) => 
            DeserializeObject<T>(value, (JsonSerializerSettings) null);

        public static object DeserializeObject(string value, JsonSerializerSettings settings) => 
            DeserializeObject(value, null, settings);

        public static T DeserializeObject<T>(string value, params JsonConverter[] converters) => 
            ((T) DeserializeObject(value, typeof(T), converters));

        public static T DeserializeObject<T>(string value, JsonSerializerSettings settings) => 
            ((T) DeserializeObject(value, typeof(T), settings));

        public static object DeserializeObject(string value, Type type) => 
            DeserializeObject(value, type, (JsonSerializerSettings) null);

        public static object DeserializeObject(string value, Type type, params JsonConverter[] converters)
        {
            JsonSerializerSettings settings = ((converters != null) && (converters.Length != 0)) ? new JsonSerializerSettings() : null;
            return DeserializeObject(value, type, settings);
        }

        public static object DeserializeObject(string value, Type type, JsonSerializerSettings settings)
        {
            ValidationUtils.ArgumentNotNull(value, "value");
            JsonSerializer serializer = JsonSerializer.CreateDefault(settings);
            if (!serializer.IsCheckAdditionalContentSet())
            {
                serializer.CheckAdditionalContent = true;
            }
            using (JsonTextReader reader = new JsonTextReader(new StringReader(value)))
            {
                return serializer.Deserialize(reader, type);
            }
        }

        [Obsolete("DeserializeObjectAsync is obsolete. Use the Task.Factory.StartNew method to deserialize JSON asynchronously: Task.Factory.StartNew(() => JsonConvert.DeserializeObject(value))")]
        public static Task<object> DeserializeObjectAsync(string value) => 
            DeserializeObjectAsync(value, null, null);

        [Obsolete("DeserializeObjectAsync is obsolete. Use the Task.Factory.StartNew method to deserialize JSON asynchronously: Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(value))")]
        public static Task<T> DeserializeObjectAsync<T>(string value) => 
            DeserializeObjectAsync<T>(value, null);

        [Obsolete("DeserializeObjectAsync is obsolete. Use the Task.Factory.StartNew method to deserialize JSON asynchronously: Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(value, settings))")]
        public static Task<T> DeserializeObjectAsync<T>(string value, JsonSerializerSettings settings) => 
            Task.Factory.StartNew<T>(() => DeserializeObject<T>(value, settings));

        [Obsolete("DeserializeObjectAsync is obsolete. Use the Task.Factory.StartNew method to deserialize JSON asynchronously: Task.Factory.StartNew(() => JsonConvert.DeserializeObject(value, type, settings))")]
        public static Task<object> DeserializeObjectAsync(string value, Type type, JsonSerializerSettings settings) => 
            Task.Factory.StartNew<object>(() => DeserializeObject(value, type, settings));

        public static XmlDocument DeserializeXmlNode(string value) => 
            DeserializeXmlNode(value, null);

        public static XmlDocument DeserializeXmlNode(string value, string deserializeRootElementName) => 
            DeserializeXmlNode(value, deserializeRootElementName, false);

        public static XmlDocument DeserializeXmlNode(string value, string deserializeRootElementName, bool writeArrayAttribute)
        {
            XmlNodeConverter converter = new XmlNodeConverter {
                DeserializeRootElementName = deserializeRootElementName,
                WriteArrayAttribute = writeArrayAttribute
            };
            JsonConverter[] converters = new JsonConverter[] { converter };
            return (XmlDocument) DeserializeObject(value, typeof(XmlDocument), converters);
        }

        public static XDocument DeserializeXNode(string value) => 
            DeserializeXNode(value, null);

        public static XDocument DeserializeXNode(string value, string deserializeRootElementName) => 
            DeserializeXNode(value, deserializeRootElementName, false);

        public static XDocument DeserializeXNode(string value, string deserializeRootElementName, bool writeArrayAttribute)
        {
            XmlNodeConverter converter = new XmlNodeConverter {
                DeserializeRootElementName = deserializeRootElementName,
                WriteArrayAttribute = writeArrayAttribute
            };
            JsonConverter[] converters = new JsonConverter[] { converter };
            return (XDocument) DeserializeObject(value, typeof(XDocument), converters);
        }

        private static string EnsureDecimalPlace(string text)
        {
            if (text.IndexOf('.') != -1)
            {
                return text;
            }
            return (text + ".0");
        }

        private static string EnsureDecimalPlace(double value, string text)
        {
            if ((!double.IsNaN(value) && !double.IsInfinity(value)) && (((text.IndexOf('.') == -1) && (text.IndexOf('E') == -1)) && (text.IndexOf('e') == -1)))
            {
                return (text + ".0");
            }
            return text;
        }

        private static string EnsureFloatFormat(double value, string text, FloatFormatHandling floatFormatHandling, char quoteChar, bool nullable)
        {
            if ((floatFormatHandling == FloatFormatHandling.Symbol) || (!double.IsInfinity(value) && !double.IsNaN(value)))
            {
                return text;
            }
            if (floatFormatHandling != FloatFormatHandling.DefaultValue)
            {
                return (quoteChar.ToString() + text + quoteChar.ToString());
            }
            if (nullable)
            {
                return Null;
            }
            return "0.0";
        }

        public static void PopulateObject(string value, object target)
        {
            PopulateObject(value, target, null);
        }

        public static void PopulateObject(string value, object target, JsonSerializerSettings settings)
        {
            JsonSerializer serializer = JsonSerializer.CreateDefault(settings);
            using (JsonReader reader = new JsonTextReader(new StringReader(value)))
            {
                serializer.Populate(reader, target);
                if (reader.Read() && (reader.TokenType != JsonToken.Comment))
                {
                    throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
                }
            }
        }

        [Obsolete("PopulateObjectAsync is obsolete. Use the Task.Factory.StartNew method to populate an object with JSON values asynchronously: Task.Factory.StartNew(() => JsonConvert.PopulateObject(value, target, settings))")]
        public static Task PopulateObjectAsync(string value, object target, JsonSerializerSettings settings) => 
            Task.Factory.StartNew(delegate {
                PopulateObject(value, target, settings);
            });

        public static string SerializeObject(object value) => 
            SerializeObject(value, (Type) null, (JsonSerializerSettings) null);

        public static string SerializeObject(object value, Newtonsoft.Json.Formatting formatting) => 
            SerializeObject(value, formatting, (JsonSerializerSettings) null);

        public static string SerializeObject(object value, params JsonConverter[] converters)
        {
            JsonSerializerSettings settings = ((converters != null) && (converters.Length != 0)) ? new JsonSerializerSettings() : null;
            return SerializeObject(value, (Type) null, settings);
        }

        public static string SerializeObject(object value, JsonSerializerSettings settings) => 
            SerializeObject(value, (Type) null, settings);

        public static string SerializeObject(object value, Newtonsoft.Json.Formatting formatting, params JsonConverter[] converters)
        {
            JsonSerializerSettings settings = ((converters != null) && (converters.Length != 0)) ? new JsonSerializerSettings() : null;
            return SerializeObject(value, null, formatting, settings);
        }

        public static string SerializeObject(object value, Newtonsoft.Json.Formatting formatting, JsonSerializerSettings settings) => 
            SerializeObject(value, null, formatting, settings);

        public static string SerializeObject(object value, Type type, JsonSerializerSettings settings)
        {
            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);
            return SerializeObjectInternal(value, type, jsonSerializer);
        }

        public static string SerializeObject(object value, Type type, Newtonsoft.Json.Formatting formatting, JsonSerializerSettings settings)
        {
            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);
            jsonSerializer.Formatting = formatting;
            return SerializeObjectInternal(value, type, jsonSerializer);
        }

        [Obsolete("SerializeObjectAsync is obsolete. Use the Task.Factory.StartNew method to serialize JSON asynchronously: Task.Factory.StartNew(() => JsonConvert.SerializeObject(value))")]
        public static Task<string> SerializeObjectAsync(object value) => 
            SerializeObjectAsync(value, Newtonsoft.Json.Formatting.None, null);

        [Obsolete("SerializeObjectAsync is obsolete. Use the Task.Factory.StartNew method to serialize JSON asynchronously: Task.Factory.StartNew(() => JsonConvert.SerializeObject(value, formatting))")]
        public static Task<string> SerializeObjectAsync(object value, Newtonsoft.Json.Formatting formatting) => 
            SerializeObjectAsync(value, formatting, null);

        [Obsolete("SerializeObjectAsync is obsolete. Use the Task.Factory.StartNew method to serialize JSON asynchronously: Task.Factory.StartNew(() => JsonConvert.SerializeObject(value, formatting, settings))")]
        public static Task<string> SerializeObjectAsync(object value, Newtonsoft.Json.Formatting formatting, JsonSerializerSettings settings) => 
            Task.Factory.StartNew<string>(() => SerializeObject(value, formatting, settings));

        private static string SerializeObjectInternal(object value, Type type, JsonSerializer jsonSerializer)
        {
            StringWriter textWriter = new StringWriter(new StringBuilder(0x100), CultureInfo.InvariantCulture);
            using (JsonTextWriter writer2 = new JsonTextWriter(textWriter))
            {
                writer2.Formatting = jsonSerializer.Formatting;
                jsonSerializer.Serialize(writer2, value, type);
            }
            return textWriter.ToString();
        }

        public static string SerializeXmlNode(System.Xml.XmlNode node) => 
            SerializeXmlNode(node, Newtonsoft.Json.Formatting.None);

        public static string SerializeXmlNode(System.Xml.XmlNode node, Newtonsoft.Json.Formatting formatting)
        {
            XmlNodeConverter converter = new XmlNodeConverter();
            JsonConverter[] converters = new JsonConverter[] { converter };
            return SerializeObject(node, formatting, converters);
        }

        public static string SerializeXmlNode(System.Xml.XmlNode node, Newtonsoft.Json.Formatting formatting, bool omitRootObject)
        {
            XmlNodeConverter converter = new XmlNodeConverter {
                OmitRootObject = omitRootObject
            };
            JsonConverter[] converters = new JsonConverter[] { converter };
            return SerializeObject(node, formatting, converters);
        }

        public static string SerializeXNode(XObject node) => 
            SerializeXNode(node, Newtonsoft.Json.Formatting.None);

        public static string SerializeXNode(XObject node, Newtonsoft.Json.Formatting formatting) => 
            SerializeXNode(node, formatting, false);

        public static string SerializeXNode(XObject node, Newtonsoft.Json.Formatting formatting, bool omitRootObject)
        {
            XmlNodeConverter converter = new XmlNodeConverter {
                OmitRootObject = omitRootObject
            };
            JsonConverter[] converters = new JsonConverter[] { converter };
            return SerializeObject(node, formatting, converters);
        }

        public static string ToString(bool value)
        {
            if (!value)
            {
                return False;
            }
            return True;
        }

        public static string ToString(byte value) => 
            value.ToString(null, CultureInfo.InvariantCulture);

        public static string ToString(char value) => 
            ToString(char.ToString(value));

        public static string ToString(DateTime value) => 
            ToString(value, DateFormatHandling.IsoDateFormat, DateTimeZoneHandling.RoundtripKind);

        public static string ToString(DateTimeOffset value) => 
            ToString(value, DateFormatHandling.IsoDateFormat);

        public static string ToString(decimal value) => 
            EnsureDecimalPlace(value.ToString(null, CultureInfo.InvariantCulture));

        public static string ToString(double value) => 
            EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture));

        public static string ToString(Enum value) => 
            value.ToString("D");

        public static string ToString(Guid value) => 
            ToString(value, '"');

        public static string ToString(short value) => 
            value.ToString(null, CultureInfo.InvariantCulture);

        public static string ToString(int value) => 
            value.ToString(null, CultureInfo.InvariantCulture);

        public static string ToString(long value) => 
            value.ToString(null, CultureInfo.InvariantCulture);

        public static string ToString(object value)
        {
            if (value == null)
            {
                return Null;
            }
            switch (ConvertUtils.GetTypeCode(value.GetType()))
            {
                case PrimitiveTypeCode.Char:
                    return ToString((char) value);

                case PrimitiveTypeCode.Boolean:
                    return ToString((bool) value);

                case PrimitiveTypeCode.SByte:
                    return ToString((sbyte) value);

                case PrimitiveTypeCode.Int16:
                    return ToString((short) value);

                case PrimitiveTypeCode.UInt16:
                    return ToString((ushort) value);

                case PrimitiveTypeCode.Int32:
                    return ToString((int) value);

                case PrimitiveTypeCode.Byte:
                    return ToString((byte) value);

                case PrimitiveTypeCode.UInt32:
                    return ToString((uint) value);

                case PrimitiveTypeCode.Int64:
                    return ToString((long) value);

                case PrimitiveTypeCode.UInt64:
                    return ToString((ulong) value);

                case PrimitiveTypeCode.Single:
                    return ToString((float) value);

                case PrimitiveTypeCode.Double:
                    return ToString((double) value);

                case PrimitiveTypeCode.DateTime:
                    return ToString((DateTime) value);

                case PrimitiveTypeCode.DateTimeOffset:
                    return ToString((DateTimeOffset) value);

                case PrimitiveTypeCode.Decimal:
                    return ToString((decimal) value);

                case PrimitiveTypeCode.Guid:
                    return ToString((Guid) value);

                case PrimitiveTypeCode.TimeSpan:
                    return ToString((TimeSpan) value);

                case PrimitiveTypeCode.BigInteger:
                    return ToStringInternal((BigInteger) value);

                case PrimitiveTypeCode.Uri:
                    return ToString((Uri) value);

                case PrimitiveTypeCode.String:
                    return ToString((string) value);

                case PrimitiveTypeCode.DBNull:
                    return Null;
            }
            throw new ArgumentException("Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
        }

        [CLSCompliant(false)]
        public static string ToString(sbyte value) => 
            value.ToString(null, CultureInfo.InvariantCulture);

        public static string ToString(float value) => 
            EnsureDecimalPlace((double) value, value.ToString("R", CultureInfo.InvariantCulture));

        public static string ToString(string value) => 
            ToString(value, '"');

        public static string ToString(TimeSpan value) => 
            ToString(value, '"');

        [CLSCompliant(false)]
        public static string ToString(ushort value) => 
            value.ToString(null, CultureInfo.InvariantCulture);

        [CLSCompliant(false)]
        public static string ToString(uint value) => 
            value.ToString(null, CultureInfo.InvariantCulture);

        [CLSCompliant(false)]
        public static string ToString(ulong value) => 
            value.ToString(null, CultureInfo.InvariantCulture);

        public static string ToString(Uri value)
        {
            if (value == null)
            {
                return Null;
            }
            return ToString(value, '"');
        }

        public static string ToString(DateTimeOffset value, DateFormatHandling format)
        {
            using (StringWriter writer = StringUtils.CreateStringWriter(0x40))
            {
                writer.Write('"');
                DateTimeUtils.WriteDateTimeOffsetString(writer, value, format, null, CultureInfo.InvariantCulture);
                writer.Write('"');
                return writer.ToString();
            }
        }

        internal static string ToString(Guid value, char quoteChar)
        {
            string str = value.ToString("D", CultureInfo.InvariantCulture);
            string str2 = quoteChar.ToString(CultureInfo.InvariantCulture);
            return (str2 + str + str2);
        }

        public static string ToString(string value, char delimiter) => 
            ToString(value, delimiter, StringEscapeHandling.Default);

        internal static string ToString(TimeSpan value, char quoteChar) => 
            ToString(value.ToString(), quoteChar);

        internal static string ToString(Uri value, char quoteChar) => 
            ToString(value.OriginalString, quoteChar);

        public static string ToString(DateTime value, DateFormatHandling format, DateTimeZoneHandling timeZoneHandling)
        {
            DateTime time = DateTimeUtils.EnsureDateTime(value, timeZoneHandling);
            using (StringWriter writer = StringUtils.CreateStringWriter(0x40))
            {
                writer.Write('"');
                DateTimeUtils.WriteDateTimeString(writer, time, format, null, CultureInfo.InvariantCulture);
                writer.Write('"');
                return writer.ToString();
            }
        }

        public static string ToString(string value, char delimiter, StringEscapeHandling stringEscapeHandling)
        {
            if ((delimiter != '"') && (delimiter != '\''))
            {
                throw new ArgumentException("Delimiter must be a single or double quote.", "delimiter");
            }
            return JavaScriptUtils.ToEscapedJavaScriptString(value, delimiter, true, stringEscapeHandling);
        }

        internal static string ToString(double value, FloatFormatHandling floatFormatHandling, char quoteChar, bool nullable) => 
            EnsureFloatFormat(value, EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture)), floatFormatHandling, quoteChar, nullable);

        internal static string ToString(float value, FloatFormatHandling floatFormatHandling, char quoteChar, bool nullable) => 
            EnsureFloatFormat((double) value, EnsureDecimalPlace((double) value, value.ToString("R", CultureInfo.InvariantCulture)), floatFormatHandling, quoteChar, nullable);

        private static string ToStringInternal(BigInteger value) => 
            value.ToString(null, CultureInfo.InvariantCulture);

        public static Func<JsonSerializerSettings> DefaultSettings
        {
            [CompilerGenerated]
            get => 
                <DefaultSettings>k__BackingField;
            [CompilerGenerated]
            set => 
                (<DefaultSettings>k__BackingField = value);
        }
    }
}

