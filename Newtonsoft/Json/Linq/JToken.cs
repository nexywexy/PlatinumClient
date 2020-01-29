namespace Newtonsoft.Json.Linq
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq.JsonPath;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Numerics;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public abstract class JToken : IJEnumerable<JToken>, IEnumerable<JToken>, IEnumerable, IJsonLineInfo, ICloneable, IDynamicMetaObjectProvider
    {
        private static JTokenEqualityComparer _equalityComparer;
        private JContainer _parent;
        private JToken _previous;
        private JToken _next;
        private object _annotations;
        private static readonly JTokenType[] BooleanTypes = new JTokenType[] { JTokenType.Integer };
        private static readonly JTokenType[] NumberTypes = new JTokenType[] { JTokenType.Integer };
        private static readonly JTokenType[] BigIntegerTypes = new JTokenType[] { JTokenType.Integer };
        private static readonly JTokenType[] StringTypes = new JTokenType[] { JTokenType.Date };
        private static readonly JTokenType[] GuidTypes = new JTokenType[] { JTokenType.String };
        private static readonly JTokenType[] TimeSpanTypes = new JTokenType[] { JTokenType.String };
        private static readonly JTokenType[] UriTypes = new JTokenType[] { JTokenType.String };
        private static readonly JTokenType[] CharTypes = new JTokenType[] { JTokenType.Integer };
        private static readonly JTokenType[] DateTimeTypes = new JTokenType[] { JTokenType.Date };
        private static readonly JTokenType[] BytesTypes = new JTokenType[] { JTokenType.Bytes };

        internal JToken()
        {
        }

        public void AddAfterSelf(object content)
        {
            if (this._parent == null)
            {
                throw new InvalidOperationException("The parent is missing.");
            }
            int num = this._parent.IndexOfItem(this);
            this._parent.AddInternal(num + 1, content, false);
        }

        public void AddAnnotation(object annotation)
        {
            if (annotation == null)
            {
                throw new ArgumentNullException("annotation");
            }
            if (this._annotations == null)
            {
                this._annotations = (annotation is object[]) ? new object[] { annotation } : annotation;
            }
            else
            {
                object[] array = this._annotations as object[];
                if (array == null)
                {
                    object[] objArray2 = new object[] { this._annotations, annotation };
                    this._annotations = objArray2;
                }
                else
                {
                    int index = 0;
                    while ((index < array.Length) && (array[index] != null))
                    {
                        index++;
                    }
                    if (index == array.Length)
                    {
                        Array.Resize<object>(ref array, index * 2);
                        this._annotations = array;
                    }
                    array[index] = annotation;
                }
            }
        }

        public void AddBeforeSelf(object content)
        {
            if (this._parent == null)
            {
                throw new InvalidOperationException("The parent is missing.");
            }
            int index = this._parent.IndexOfItem(this);
            this._parent.AddInternal(index, content, false);
        }

        [IteratorStateMachine(typeof(<AfterSelf>d__43))]
        public IEnumerable<JToken> AfterSelf() => 
            new <AfterSelf>d__43(-2) { <>4__this = this };

        public IEnumerable<JToken> Ancestors() => 
            this.GetAncestors(false);

        public IEnumerable<JToken> AncestorsAndSelf() => 
            this.GetAncestors(true);

        public T Annotation<T>() where T: class
        {
            if (this._annotations != null)
            {
                object[] objArray = this._annotations as object[];
                if (objArray == null)
                {
                    return (this._annotations as T);
                }
                for (int i = 0; i < objArray.Length; i++)
                {
                    object obj2 = objArray[i];
                    if (obj2 == null)
                    {
                        break;
                    }
                    T local = obj2 as T;
                    if (local != null)
                    {
                        return local;
                    }
                }
            }
            return default(T);
        }

        public object Annotation(System.Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (this._annotations != null)
            {
                object[] objArray = this._annotations as object[];
                if (objArray == null)
                {
                    if (type.IsInstanceOfType(this._annotations))
                    {
                        return this._annotations;
                    }
                }
                else
                {
                    for (int i = 0; i < objArray.Length; i++)
                    {
                        object o = objArray[i];
                        if (o == null)
                        {
                            break;
                        }
                        if (type.IsInstanceOfType(o))
                        {
                            return o;
                        }
                    }
                }
            }
            return null;
        }

        [IteratorStateMachine(typeof(<Annotations>d__176))]
        public IEnumerable<T> Annotations<T>() where T: class
        {
            if (this._annotations != null)
            {
                object[] <annotations>5__1 = this._annotations as object[];
                if (<annotations>5__1 == null)
                {
                    T local2 = this._annotations as T;
                    if (local2 == null)
                    {
                    }
                    yield return local2;
                }
                for (int i = 0; i < <annotations>5__1.Length; i = num2 + 1)
                {
                    object obj2 = <annotations>5__1[i];
                    if (obj2 == null)
                    {
                        break;
                    }
                    T local = obj2 as T;
                    if (local != null)
                    {
                        yield return local;
                    }
                    int num2 = i;
                }
                yield break;
            }
        }

        [IteratorStateMachine(typeof(<Annotations>d__177))]
        public IEnumerable<object> Annotations(System.Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (this._annotations == null)
            {
            }
            object[] <annotations>5__1 = this._annotations as object[];
            if (<annotations>5__1 == null)
            {
                if (!type.IsInstanceOfType(this._annotations))
                {
                }
                yield return this._annotations;
            }
            for (int i = 0; i < <annotations>5__1.Length; i = num2 + 1)
            {
                object o = <annotations>5__1[i];
                if (o == null)
                {
                    break;
                }
                if (type.IsInstanceOfType(o))
                {
                    yield return o;
                }
                int num2 = i;
            }
        }

        [IteratorStateMachine(typeof(<BeforeSelf>d__44))]
        public IEnumerable<JToken> BeforeSelf() => 
            new <BeforeSelf>d__44(-2) { <>4__this = this };

        public virtual JEnumerable<JToken> Children() => 
            JEnumerable<JToken>.Empty;

        public JEnumerable<T> Children<T>() where T: JToken => 
            new JEnumerable<T>(this.Children().OfType<T>());

        internal abstract JToken CloneToken();
        public JsonReader CreateReader() => 
            new JTokenReader(this);

        public JToken DeepClone() => 
            this.CloneToken();

        internal abstract bool DeepEquals(JToken node);
        public static bool DeepEquals(JToken t1, JToken t2) => 
            ((t1 == t2) || (((t1 != null) && (t2 != null)) && t1.DeepEquals(t2)));

        private static JValue EnsureValue(JToken value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value is JProperty)
            {
                value = ((JProperty) value).Value;
            }
            return (value as JValue);
        }

        public static JToken FromObject(object o) => 
            FromObjectInternal(o, JsonSerializer.CreateDefault());

        public static JToken FromObject(object o, JsonSerializer jsonSerializer) => 
            FromObjectInternal(o, jsonSerializer);

        internal static JToken FromObjectInternal(object o, JsonSerializer jsonSerializer)
        {
            ValidationUtils.ArgumentNotNull(o, "o");
            ValidationUtils.ArgumentNotNull(jsonSerializer, "jsonSerializer");
            using (JTokenWriter writer = new JTokenWriter())
            {
                jsonSerializer.Serialize(writer, o);
                return writer.Token;
            }
        }

        [IteratorStateMachine(typeof(<GetAncestors>d__42))]
        internal IEnumerable<JToken> GetAncestors(bool self) => 
            new <GetAncestors>d__42(-2) { 
                <>4__this = this,
                <>3__self = self
            };

        internal abstract int GetDeepHashCode();
        protected virtual DynamicMetaObject GetMetaObject(Expression parameter) => 
            new DynamicProxyMetaObject<JToken>(parameter, this, new DynamicProxy<JToken>(), true);

        private static string GetType(JToken token)
        {
            ValidationUtils.ArgumentNotNull(token, "token");
            if (token is JProperty)
            {
                token = ((JProperty) token).Value;
            }
            return token.Type.ToString();
        }

        public static JToken Load(JsonReader reader) => 
            Load(reader, null);

        public static JToken Load(JsonReader reader, JsonLoadSettings settings) => 
            ReadFrom(reader, settings);

        bool IJsonLineInfo.HasLineInfo() => 
            (this.Annotation<LineInfoAnnotation>() > null);

        public static explicit operator bool(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, BooleanTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Boolean.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return Convert.ToBoolean((int) ((BigInteger) o.Value));
            }
            return Convert.ToBoolean(o.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator byte(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Byte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return (byte) ((BigInteger) o.Value);
            }
            return Convert.ToByte(o.Value, CultureInfo.InvariantCulture);
        }

        [CLSCompliant(false)]
        public static explicit operator char(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, CharTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Char.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return (char) ((ushort) ((BigInteger) o.Value));
            }
            return Convert.ToChar(o.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator DateTime(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, DateTimeTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to DateTime.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is DateTimeOffset)
            {
                DateTimeOffset offset = (DateTimeOffset) o.Value;
                return offset.DateTime;
            }
            return Convert.ToDateTime(o.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator DateTimeOffset(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, DateTimeTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to DateTimeOffset.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is DateTimeOffset)
            {
                return (DateTimeOffset) o.Value;
            }
            if (o.Value is string)
            {
                return DateTimeOffset.Parse((string) o.Value, CultureInfo.InvariantCulture);
            }
            return new DateTimeOffset(Convert.ToDateTime(o.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator decimal(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Decimal.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return (decimal) ((BigInteger) o.Value);
            }
            return Convert.ToDecimal(o.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator double(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Double.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return (double) ((BigInteger) o.Value);
            }
            return Convert.ToDouble(o.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator Guid(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, GuidTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Guid.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is byte[])
            {
                return new Guid((byte[]) o.Value);
            }
            if (o.Value is Guid)
            {
                return (Guid) o.Value;
            }
            return new Guid(Convert.ToString(o.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator short(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Int16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return (short) ((BigInteger) o.Value);
            }
            return Convert.ToInt16(o.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator int(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Int32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return (int) ((BigInteger) o.Value);
            }
            return Convert.ToInt32(o.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator long(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Int64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return (long) ((BigInteger) o.Value);
            }
            return Convert.ToInt64(o.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator bool?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, BooleanTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Boolean.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return new bool?(Convert.ToBoolean((int) ((BigInteger) o.Value)));
            }
            if (o.Value == null)
            {
                return null;
            }
            return new bool?(Convert.ToBoolean(o.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator byte?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Byte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return new byte?((byte) ((BigInteger) o.Value));
            }
            if (o.Value == null)
            {
                return null;
            }
            return new byte?(Convert.ToByte(o.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator char?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, CharTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Char.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return new char?((char) ((ushort) ((BigInteger) o.Value)));
            }
            if (o.Value == null)
            {
                return null;
            }
            return new char?(Convert.ToChar(o.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator DateTime?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, DateTimeTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to DateTime.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is DateTimeOffset)
            {
                DateTimeOffset offset = (DateTimeOffset) o.Value;
                return new DateTime?(offset.DateTime);
            }
            if (o.Value == null)
            {
                return null;
            }
            return new DateTime?(Convert.ToDateTime(o.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator DateTimeOffset?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, DateTimeTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to DateTimeOffset.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value == null)
            {
                return null;
            }
            if (o.Value is DateTimeOffset)
            {
                return (DateTimeOffset?) o.Value;
            }
            if (o.Value is string)
            {
                return new DateTimeOffset?(DateTimeOffset.Parse((string) o.Value, CultureInfo.InvariantCulture));
            }
            return new DateTimeOffset(Convert.ToDateTime(o.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator decimal?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Decimal.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return new decimal?((decimal) ((BigInteger) o.Value));
            }
            if (o.Value == null)
            {
                return null;
            }
            return new decimal?(Convert.ToDecimal(o.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator double?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Double.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return new double?((double) ((BigInteger) o.Value));
            }
            if (o.Value == null)
            {
                return null;
            }
            return new double?(Convert.ToDouble(o.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator Guid?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, GuidTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Guid.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value == null)
            {
                return null;
            }
            if (o.Value is byte[])
            {
                return new Guid((byte[]) o.Value);
            }
            return new Guid?((o.Value is Guid) ? ((Guid) o.Value) : new Guid(Convert.ToString(o.Value, CultureInfo.InvariantCulture)));
        }

        public static explicit operator short?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Int16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return new short?((short) ((BigInteger) o.Value));
            }
            if (o.Value == null)
            {
                return null;
            }
            return new short?(Convert.ToInt16(o.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator int?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Int32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return new int?((int) ((BigInteger) o.Value));
            }
            if (o.Value == null)
            {
                return null;
            }
            return new int?(Convert.ToInt32(o.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator long?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Int64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return new long?((long) ((BigInteger) o.Value));
            }
            if (o.Value == null)
            {
                return null;
            }
            return new long?(Convert.ToInt64(o.Value, CultureInfo.InvariantCulture));
        }

        [CLSCompliant(false)]
        public static explicit operator sbyte?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to SByte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return new sbyte?((sbyte) ((BigInteger) o.Value));
            }
            if (o.Value == null)
            {
                return null;
            }
            return new sbyte?(Convert.ToSByte(o.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator float?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Single.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return new float?((float) ((BigInteger) o.Value));
            }
            if (o.Value == null)
            {
                return null;
            }
            return new float?(Convert.ToSingle(o.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator TimeSpan?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, TimeSpanTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to TimeSpan.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value == null)
            {
                return null;
            }
            return new TimeSpan?((o.Value is TimeSpan) ? ((TimeSpan) o.Value) : ConvertUtils.ParseTimeSpan(Convert.ToString(o.Value, CultureInfo.InvariantCulture)));
        }

        [CLSCompliant(false)]
        public static explicit operator ushort?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to UInt16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return new ushort?((ushort) ((BigInteger) o.Value));
            }
            if (o.Value == null)
            {
                return null;
            }
            return new ushort?(Convert.ToUInt16(o.Value, CultureInfo.InvariantCulture));
        }

        [CLSCompliant(false)]
        public static explicit operator uint?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to UInt32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return new uint?((uint) ((BigInteger) o.Value));
            }
            if (o.Value == null)
            {
                return null;
            }
            return new uint?(Convert.ToUInt32(o.Value, CultureInfo.InvariantCulture));
        }

        [CLSCompliant(false)]
        public static explicit operator ulong?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to UInt64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return new ulong?((ulong) ((BigInteger) o.Value));
            }
            if (o.Value == null)
            {
                return null;
            }
            return new ulong?(Convert.ToUInt64(o.Value, CultureInfo.InvariantCulture));
        }

        [CLSCompliant(false)]
        public static explicit operator sbyte(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to SByte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return (sbyte) ((BigInteger) o.Value);
            }
            return Convert.ToSByte(o.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator float(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Single.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return (float) ((BigInteger) o.Value);
            }
            return Convert.ToSingle(o.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator string(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, StringTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to String.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value == null)
            {
                return null;
            }
            if (o.Value is byte[])
            {
                return Convert.ToBase64String((byte[]) o.Value);
            }
            if (o.Value is BigInteger)
            {
                BigInteger integer = (BigInteger) o.Value;
                return integer.ToString(CultureInfo.InvariantCulture);
            }
            return Convert.ToString(o.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator byte[](JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, BytesTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to byte array.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is string)
            {
                return Convert.FromBase64String(Convert.ToString(o.Value, CultureInfo.InvariantCulture));
            }
            if (o.Value is BigInteger)
            {
                BigInteger integer = (BigInteger) o.Value;
                return integer.ToByteArray();
            }
            if (!(o.Value is byte[]))
            {
                throw new ArgumentException("Can not convert {0} to byte array.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            return (byte[]) o.Value;
        }

        public static explicit operator TimeSpan(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, TimeSpanTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to TimeSpan.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is TimeSpan)
            {
                return (TimeSpan) o.Value;
            }
            return ConvertUtils.ParseTimeSpan(Convert.ToString(o.Value, CultureInfo.InvariantCulture));
        }

        [CLSCompliant(false)]
        public static explicit operator ushort(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to UInt16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return (ushort) ((BigInteger) o.Value);
            }
            return Convert.ToUInt16(o.Value, CultureInfo.InvariantCulture);
        }

        [CLSCompliant(false)]
        public static explicit operator uint(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to UInt32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return (uint) ((BigInteger) o.Value);
            }
            return Convert.ToUInt32(o.Value, CultureInfo.InvariantCulture);
        }

        [CLSCompliant(false)]
        public static explicit operator ulong(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to UInt64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value is BigInteger)
            {
                return (ulong) ((BigInteger) o.Value);
            }
            return Convert.ToUInt64(o.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator Uri(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, UriTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Uri.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value == null)
            {
                return null;
            }
            if (o.Value is Uri)
            {
                return (Uri) o.Value;
            }
            return new Uri(Convert.ToString(o.Value, CultureInfo.InvariantCulture));
        }

        public static implicit operator JToken(bool value) => 
            new JValue(value);

        public static implicit operator JToken(byte value) => 
            new JValue((long) value);

        public static implicit operator JToken(DateTime value) => 
            new JValue(value);

        public static implicit operator JToken(DateTimeOffset value) => 
            new JValue(value);

        public static implicit operator JToken(decimal value) => 
            new JValue(value);

        public static implicit operator JToken(double value) => 
            new JValue(value);

        public static implicit operator JToken(Guid value) => 
            new JValue(value);

        [CLSCompliant(false)]
        public static implicit operator JToken(short value) => 
            new JValue((long) value);

        public static implicit operator JToken(int value) => 
            new JValue((long) value);

        public static implicit operator JToken(long value) => 
            new JValue(value);

        public static implicit operator JToken(bool? value) => 
            new JValue(value);

        public static implicit operator JToken(byte? value) => 
            new JValue(value);

        public static implicit operator JToken(DateTime? value) => 
            new JValue(value);

        public static implicit operator JToken(DateTimeOffset? value) => 
            new JValue(value);

        public static implicit operator JToken(decimal? value) => 
            new JValue(value);

        public static implicit operator JToken(double? value) => 
            new JValue(value);

        public static implicit operator JToken(Guid? value) => 
            new JValue(value);

        [CLSCompliant(false)]
        public static implicit operator JToken(short? value) => 
            new JValue(value);

        public static implicit operator JToken(int? value) => 
            new JValue(value);

        public static implicit operator JToken(long? value) => 
            new JValue(value);

        [CLSCompliant(false)]
        public static implicit operator JToken(sbyte? value) => 
            new JValue(value);

        public static implicit operator JToken(float? value) => 
            new JValue(value);

        public static implicit operator JToken(TimeSpan? value) => 
            new JValue(value);

        [CLSCompliant(false)]
        public static implicit operator JToken(ushort? value) => 
            new JValue(value);

        [CLSCompliant(false)]
        public static implicit operator JToken(uint? value) => 
            new JValue(value);

        [CLSCompliant(false)]
        public static implicit operator JToken(ulong? value) => 
            new JValue(value);

        [CLSCompliant(false)]
        public static implicit operator JToken(sbyte value) => 
            new JValue((long) value);

        public static implicit operator JToken(float value) => 
            new JValue(value);

        public static implicit operator JToken(string value) => 
            new JValue(value);

        public static implicit operator JToken(byte[] value) => 
            new JValue(value);

        public static implicit operator JToken(TimeSpan value) => 
            new JValue(value);

        [CLSCompliant(false)]
        public static implicit operator JToken(ushort value) => 
            new JValue((long) value);

        [CLSCompliant(false)]
        public static implicit operator JToken(uint value) => 
            new JValue((long) value);

        [CLSCompliant(false)]
        public static implicit operator JToken(ulong value) => 
            new JValue(value);

        public static implicit operator JToken(Uri value) => 
            new JValue(value);

        public static JToken Parse(string json) => 
            Parse(json, null);

        public static JToken Parse(string json, JsonLoadSettings settings)
        {
            using (JsonReader reader = new JsonTextReader(new StringReader(json)))
            {
                if (reader.Read() && (reader.TokenType != JsonToken.Comment))
                {
                    throw JsonReaderException.Create(reader, "Additional text found in JSON string after parsing content.");
                }
                return Load(reader, settings);
            }
        }

        public static JToken ReadFrom(JsonReader reader) => 
            ReadFrom(reader, null);

        public static JToken ReadFrom(JsonReader reader, JsonLoadSettings settings)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            if ((reader.TokenType == JsonToken.None) && !(((settings != null) && (settings.CommentHandling == CommentHandling.Ignore)) ? reader.ReadAndMoveToContent() : reader.Read()))
            {
                throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader.");
            }
            IJsonLineInfo lineInfo = reader as IJsonLineInfo;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return JObject.Load(reader, settings);

                case JsonToken.StartArray:
                    return JArray.Load(reader, settings);

                case JsonToken.StartConstructor:
                    return JConstructor.Load(reader, settings);

                case JsonToken.PropertyName:
                    return JProperty.Load(reader, settings);

                case JsonToken.Comment:
                {
                    JValue value2 = JValue.CreateComment(reader.Value.ToString());
                    value2.SetLineInfo(lineInfo, settings);
                    return value2;
                }
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Date:
                case JsonToken.Bytes:
                {
                    JValue value1 = new JValue(reader.Value);
                    value1.SetLineInfo(lineInfo, settings);
                    return value1;
                }
                case JsonToken.Null:
                {
                    JValue value3 = JValue.CreateNull();
                    value3.SetLineInfo(lineInfo, settings);
                    return value3;
                }
                case JsonToken.Undefined:
                {
                    JValue value4 = JValue.CreateUndefined();
                    value4.SetLineInfo(lineInfo, settings);
                    return value4;
                }
            }
            throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader. Unexpected token: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
        }

        public void Remove()
        {
            if (this._parent == null)
            {
                throw new InvalidOperationException("The parent is missing.");
            }
            this._parent.RemoveItem(this);
        }

        public void RemoveAnnotations<T>() where T: class
        {
            if (this._annotations != null)
            {
                object[] objArray = this._annotations as object[];
                if (objArray == null)
                {
                    if (this._annotations is T)
                    {
                        this._annotations = null;
                    }
                }
                else
                {
                    int index = 0;
                    int num2 = 0;
                    while (index < objArray.Length)
                    {
                        object obj2 = objArray[index];
                        if (obj2 == null)
                        {
                            break;
                        }
                        if (!(obj2 is T))
                        {
                            objArray[num2++] = obj2;
                        }
                        index++;
                    }
                    if (num2 != 0)
                    {
                        while (num2 < index)
                        {
                            objArray[num2++] = null;
                        }
                    }
                    else
                    {
                        this._annotations = null;
                    }
                }
            }
        }

        public void RemoveAnnotations(System.Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (this._annotations != null)
            {
                object[] objArray = this._annotations as object[];
                if (objArray == null)
                {
                    if (type.IsInstanceOfType(this._annotations))
                    {
                        this._annotations = null;
                    }
                }
                else
                {
                    int index = 0;
                    int num2 = 0;
                    while (index < objArray.Length)
                    {
                        object o = objArray[index];
                        if (o == null)
                        {
                            break;
                        }
                        if (!type.IsInstanceOfType(o))
                        {
                            objArray[num2++] = o;
                        }
                        index++;
                    }
                    if (num2 != 0)
                    {
                        while (num2 < index)
                        {
                            objArray[num2++] = null;
                        }
                    }
                    else
                    {
                        this._annotations = null;
                    }
                }
            }
        }

        public void Replace(JToken value)
        {
            if (this._parent == null)
            {
                throw new InvalidOperationException("The parent is missing.");
            }
            this._parent.ReplaceItem(this, value);
        }

        public JToken SelectToken(string path) => 
            this.SelectToken(path, false);

        public JToken SelectToken(string path, bool errorWhenNoMatch)
        {
            JToken current = null;
            using (IEnumerator<JToken> enumerator = new JPath(path).Evaluate(this, errorWhenNoMatch).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (current == null)
                    {
                        current = enumerator.Current;
                    }
                    else
                    {
                        throw new JsonException("Path returned multiple tokens.");
                        current = enumerator.Current;
                    }
                }
            }
            return current;
        }

        public IEnumerable<JToken> SelectTokens(string path) => 
            this.SelectTokens(path, false);

        public IEnumerable<JToken> SelectTokens(string path, bool errorWhenNoMatch) => 
            new JPath(path).Evaluate(this, errorWhenNoMatch);

        internal void SetLineInfo(IJsonLineInfo lineInfo, JsonLoadSettings settings)
        {
            if (((settings == null) || (settings.LineInfoHandling != LineInfoHandling.Load)) && ((lineInfo != null) && lineInfo.HasLineInfo()))
            {
                this.SetLineInfo(lineInfo.LineNumber, lineInfo.LinePosition);
            }
        }

        internal void SetLineInfo(int lineNumber, int linePosition)
        {
            this.AddAnnotation(new LineInfoAnnotation(lineNumber, linePosition));
        }

        IEnumerator<JToken> IEnumerable<JToken>.GetEnumerator() => 
            this.Children().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            ((IEnumerable<JToken>) this).GetEnumerator();

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter) => 
            this.GetMetaObject(parameter);

        object ICloneable.Clone() => 
            this.DeepClone();

        private static BigInteger ToBigInteger(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, BigIntegerTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to BigInteger.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            return ConvertUtils.ToBigInteger(o.Value);
        }

        private static BigInteger? ToBigIntegerNullable(JToken value)
        {
            JValue o = EnsureValue(value);
            if ((o == null) || !ValidateToken(o, BigIntegerTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to BigInteger.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (o.Value == null)
            {
                return null;
            }
            return new BigInteger?(ConvertUtils.ToBigInteger(o.Value));
        }

        public T ToObject<T>() => 
            ((T) this.ToObject(typeof(T)));

        public T ToObject<T>(JsonSerializer jsonSerializer) => 
            ((T) this.ToObject(typeof(T), jsonSerializer));

        public object ToObject(System.Type objectType)
        {
            if (JsonConvert.DefaultSettings == null)
            {
                PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(objectType, out bool flag);
                if (flag)
                {
                    if (this.Type == JTokenType.String)
                    {
                        try
                        {
                            return this.ToObject(objectType, JsonSerializer.CreateDefault());
                        }
                        catch (Exception exception)
                        {
                            System.Type type = objectType.IsEnum() ? objectType : Nullable.GetUnderlyingType(objectType);
                            throw new ArgumentException("Could not convert '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, (string) this, type.Name), exception);
                        }
                    }
                    if (this.Type == JTokenType.Integer)
                    {
                        return Enum.ToObject(objectType.IsEnum() ? objectType : Nullable.GetUnderlyingType(objectType), ((JValue) this).Value);
                    }
                }
                switch (typeCode)
                {
                    case PrimitiveTypeCode.Char:
                        return (char) this;

                    case PrimitiveTypeCode.CharNullable:
                        return (char?) this;

                    case PrimitiveTypeCode.Boolean:
                        return (bool) this;

                    case PrimitiveTypeCode.BooleanNullable:
                        return (bool?) this;

                    case PrimitiveTypeCode.SByte:
                        return (sbyte?) this;

                    case PrimitiveTypeCode.SByteNullable:
                        return (sbyte) this;

                    case PrimitiveTypeCode.Int16:
                        return (short) this;

                    case PrimitiveTypeCode.Int16Nullable:
                        return (short?) this;

                    case PrimitiveTypeCode.UInt16:
                        return (ushort) this;

                    case PrimitiveTypeCode.UInt16Nullable:
                        return (ushort?) this;

                    case PrimitiveTypeCode.Int32:
                        return (int) this;

                    case PrimitiveTypeCode.Int32Nullable:
                        return (int?) this;

                    case PrimitiveTypeCode.Byte:
                        return (byte) this;

                    case PrimitiveTypeCode.ByteNullable:
                        return (byte?) this;

                    case PrimitiveTypeCode.UInt32:
                        return (uint) this;

                    case PrimitiveTypeCode.UInt32Nullable:
                        return (uint?) this;

                    case PrimitiveTypeCode.Int64:
                        return (long) this;

                    case PrimitiveTypeCode.Int64Nullable:
                        return (long?) this;

                    case PrimitiveTypeCode.UInt64:
                        return (ulong) this;

                    case PrimitiveTypeCode.UInt64Nullable:
                        return (ulong?) this;

                    case PrimitiveTypeCode.Single:
                        return (float) this;

                    case PrimitiveTypeCode.SingleNullable:
                        return (float?) this;

                    case PrimitiveTypeCode.Double:
                        return (double) this;

                    case PrimitiveTypeCode.DoubleNullable:
                        return (double?) this;

                    case PrimitiveTypeCode.DateTime:
                        return (DateTime) this;

                    case PrimitiveTypeCode.DateTimeNullable:
                        return (DateTime?) this;

                    case PrimitiveTypeCode.DateTimeOffset:
                        return (DateTimeOffset) this;

                    case PrimitiveTypeCode.DateTimeOffsetNullable:
                        return (DateTimeOffset?) this;

                    case PrimitiveTypeCode.Decimal:
                        return (decimal) this;

                    case PrimitiveTypeCode.DecimalNullable:
                        return (decimal?) this;

                    case PrimitiveTypeCode.Guid:
                        return (Guid) this;

                    case PrimitiveTypeCode.GuidNullable:
                        return (Guid?) this;

                    case PrimitiveTypeCode.TimeSpan:
                        return (TimeSpan) this;

                    case PrimitiveTypeCode.TimeSpanNullable:
                        return (TimeSpan?) this;

                    case PrimitiveTypeCode.BigInteger:
                        return ToBigInteger(this);

                    case PrimitiveTypeCode.BigIntegerNullable:
                        return ToBigIntegerNullable(this);

                    case PrimitiveTypeCode.Uri:
                        return (Uri) this;

                    case PrimitiveTypeCode.String:
                        return (string) this;
                }
            }
            return this.ToObject(objectType, JsonSerializer.CreateDefault());
        }

        public object ToObject(System.Type objectType, JsonSerializer jsonSerializer)
        {
            ValidationUtils.ArgumentNotNull(jsonSerializer, "jsonSerializer");
            using (JTokenReader reader = new JTokenReader(this))
            {
                return jsonSerializer.Deserialize(reader, objectType);
            }
        }

        public override string ToString() => 
            this.ToString(Formatting.Indented, new JsonConverter[0]);

        public string ToString(Formatting formatting, params JsonConverter[] converters)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonTextWriter writer2 = new JsonTextWriter(writer) {
                    Formatting = formatting
                };
                this.WriteTo(writer2, converters);
                return writer.ToString();
            }
        }

        private static bool ValidateToken(JToken o, JTokenType[] validTypes, bool nullable)
        {
            if (Array.IndexOf<JTokenType>(validTypes, o.Type) == -1)
            {
                if (!nullable)
                {
                    return false;
                }
                if (o.Type != JTokenType.Null)
                {
                    return (o.Type == JTokenType.Undefined);
                }
            }
            return true;
        }

        public virtual T Value<T>(object key)
        {
            JToken token = this[key];
            if (token != null)
            {
                return token.Convert<JToken, T>();
            }
            return default(T);
        }

        public virtual IEnumerable<T> Values<T>()
        {
            throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
        }

        public abstract void WriteTo(JsonWriter writer, params JsonConverter[] converters);

        public static JTokenEqualityComparer EqualityComparer
        {
            get
            {
                if (_equalityComparer == null)
                {
                    _equalityComparer = new JTokenEqualityComparer();
                }
                return _equalityComparer;
            }
        }

        public JContainer Parent
        {
            [DebuggerStepThrough]
            get => 
                this._parent;
            internal set => 
                (this._parent = value);
        }

        public JToken Root
        {
            get
            {
                JContainer parent = this.Parent;
                if (parent != null)
                {
                    while (parent.Parent != null)
                    {
                        parent = parent.Parent;
                    }
                    return parent;
                }
                return this;
            }
        }

        public abstract JTokenType Type { get; }

        public abstract bool HasValues { get; }

        public JToken Next
        {
            get => 
                this._next;
            internal set => 
                (this._next = value);
        }

        public JToken Previous
        {
            get => 
                this._previous;
            internal set => 
                (this._previous = value);
        }

        public string Path
        {
            get
            {
                if (this.Parent == null)
                {
                    return string.Empty;
                }
                List<JsonPosition> positions = new List<JsonPosition>();
                JToken item = null;
                for (JToken token2 = this; token2 != null; token2 = token2.Parent)
                {
                    JsonPosition position;
                    switch (token2.Type)
                    {
                        case JTokenType.Array:
                        case JTokenType.Constructor:
                            if (item != null)
                            {
                                int index = ((IList<JToken>) token2).IndexOf(item);
                                position = new JsonPosition(JsonContainerType.Array) {
                                    Position = index
                                };
                                positions.Add(position);
                            }
                            break;

                        case JTokenType.Property:
                        {
                            JProperty property = (JProperty) token2;
                            position = new JsonPosition(JsonContainerType.Object) {
                                PropertyName = property.Name
                            };
                            positions.Add(position);
                            break;
                        }
                    }
                    item = token2;
                }
                positions.Reverse();
                return JsonPosition.BuildPath(positions, null);
            }
        }

        public virtual JToken this[object key]
        {
            get
            {
                throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
            }
            set
            {
                throw new InvalidOperationException("Cannot set child value on {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
            }
        }

        public virtual JToken First
        {
            get
            {
                throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
            }
        }

        public virtual JToken Last
        {
            get
            {
                throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
            }
        }

        IJEnumerable<JToken> IJEnumerable<JToken>.this[object key] =>
            this[key];

        int IJsonLineInfo.LineNumber
        {
            get
            {
                LineInfoAnnotation annotation = this.Annotation<LineInfoAnnotation>();
                if (annotation != null)
                {
                    return annotation.LineNumber;
                }
                return 0;
            }
        }

        int IJsonLineInfo.LinePosition
        {
            get
            {
                LineInfoAnnotation annotation = this.Annotation<LineInfoAnnotation>();
                if (annotation != null)
                {
                    return annotation.LinePosition;
                }
                return 0;
            }
        }

        [CompilerGenerated]
        private sealed class <AfterSelf>d__43 : IEnumerable<JToken>, IEnumerable, IEnumerator<JToken>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private JToken <>2__current;
            private int <>l__initialThreadId;
            public JToken <>4__this;
            private JToken <o>5__1;

            [DebuggerHidden]
            public <AfterSelf>d__43(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Environment.CurrentManagedThreadId;
            }

            private bool MoveNext()
            {
                int num = this.<>1__state;
                if (num == 0)
                {
                    this.<>1__state = -1;
                    if (this.<>4__this.Parent != null)
                    {
                        this.<o>5__1 = this.<>4__this.Next;
                        while (this.<o>5__1 != null)
                        {
                            this.<>2__current = this.<o>5__1;
                            this.<>1__state = 1;
                            return true;
                        Label_005A:
                            this.<>1__state = -1;
                            this.<o>5__1 = this.<o>5__1.Next;
                        }
                        this.<o>5__1 = null;
                    }
                    return false;
                }
                if (num != 1)
                {
                    return false;
                }
                goto Label_005A;
            }

            [DebuggerHidden]
            IEnumerator<JToken> IEnumerable<JToken>.GetEnumerator()
            {
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.CurrentManagedThreadId))
                {
                    this.<>1__state = 0;
                    return this;
                }
                return new JToken.<AfterSelf>d__43(0) { <>4__this = this.<>4__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Newtonsoft.Json.Linq.JToken>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            JToken IEnumerator<JToken>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }

        [CompilerGenerated]
        private sealed class <Annotations>d__176<T> : IEnumerable<T>, IEnumerable, IEnumerator<T>, IDisposable, IEnumerator where T: class
        {
            private int <>1__state;
            private T <>2__current;
            private int <>l__initialThreadId;
            public JToken <>4__this;
            private object[] <annotations>5__1;
            private int <i>5__2;

            [DebuggerHidden]
            public <Annotations>d__176(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Environment.CurrentManagedThreadId;
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        if (this.<>4__this._annotations != null)
                        {
                            this.<annotations>5__1 = this.<>4__this._annotations as object[];
                            if (this.<annotations>5__1 == null)
                            {
                                T local2 = this.<>4__this._annotations as T;
                                if (local2 == null)
                                {
                                    return false;
                                }
                                this.<>2__current = local2;
                                this.<>1__state = 2;
                                return true;
                            }
                            this.<i>5__2 = 0;
                            while (this.<i>5__2 < this.<annotations>5__1.Length)
                            {
                                int num2;
                                object obj2 = this.<annotations>5__1[this.<i>5__2];
                                if (obj2 == null)
                                {
                                    break;
                                }
                                T local = obj2 as T;
                                if (local == null)
                                {
                                    goto Label_00A3;
                                }
                                this.<>2__current = local;
                                this.<>1__state = 1;
                                return true;
                            Label_009C:
                                this.<>1__state = -1;
                            Label_00A3:
                                num2 = this.<i>5__2;
                                this.<i>5__2 = num2 + 1;
                            }
                            break;
                        }
                        return false;

                    case 1:
                        goto Label_009C;

                    case 2:
                        this.<>1__state = -1;
                        return false;

                    default:
                        return false;
                }
                return false;
            }

            [DebuggerHidden]
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.CurrentManagedThreadId))
                {
                    this.<>1__state = 0;
                    return (JToken.<Annotations>d__176<T>) this;
                }
                return new JToken.<Annotations>d__176<T>(0) { <>4__this = this.<>4__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            T IEnumerator<T>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }


        [CompilerGenerated]
        private sealed class <BeforeSelf>d__44 : IEnumerable<JToken>, IEnumerable, IEnumerator<JToken>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private JToken <>2__current;
            private int <>l__initialThreadId;
            public JToken <>4__this;
            private JToken <o>5__1;

            [DebuggerHidden]
            public <BeforeSelf>d__44(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Environment.CurrentManagedThreadId;
            }

            private bool MoveNext()
            {
                int num = this.<>1__state;
                if (num == 0)
                {
                    this.<>1__state = -1;
                    this.<o>5__1 = this.<>4__this.Parent.First;
                    while (this.<o>5__1 != this.<>4__this)
                    {
                        this.<>2__current = this.<o>5__1;
                        this.<>1__state = 1;
                        return true;
                    Label_004D:
                        this.<>1__state = -1;
                        this.<o>5__1 = this.<o>5__1.Next;
                    }
                    this.<o>5__1 = null;
                    return false;
                }
                if (num != 1)
                {
                    return false;
                }
                goto Label_004D;
            }

            [DebuggerHidden]
            IEnumerator<JToken> IEnumerable<JToken>.GetEnumerator()
            {
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.CurrentManagedThreadId))
                {
                    this.<>1__state = 0;
                    return this;
                }
                return new JToken.<BeforeSelf>d__44(0) { <>4__this = this.<>4__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Newtonsoft.Json.Linq.JToken>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            JToken IEnumerator<JToken>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }

        [CompilerGenerated]
        private sealed class <GetAncestors>d__42 : IEnumerable<JToken>, IEnumerable, IEnumerator<JToken>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private JToken <>2__current;
            private int <>l__initialThreadId;
            private bool self;
            public bool <>3__self;
            public JToken <>4__this;
            private JToken <current>5__1;

            [DebuggerHidden]
            public <GetAncestors>d__42(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Environment.CurrentManagedThreadId;
            }

            private bool MoveNext()
            {
                int num = this.<>1__state;
                if (num == 0)
                {
                    this.<>1__state = -1;
                    this.<current>5__1 = this.self ? this.<>4__this : this.<>4__this.Parent;
                    while (this.<current>5__1 != null)
                    {
                        this.<>2__current = this.<current>5__1;
                        this.<>1__state = 1;
                        return true;
                    Label_005E:
                        this.<>1__state = -1;
                        this.<current>5__1 = this.<current>5__1.Parent;
                    }
                    this.<current>5__1 = null;
                    return false;
                }
                if (num != 1)
                {
                    return false;
                }
                goto Label_005E;
            }

            [DebuggerHidden]
            IEnumerator<JToken> IEnumerable<JToken>.GetEnumerator()
            {
                JToken.<GetAncestors>d__42 d__;
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.CurrentManagedThreadId))
                {
                    this.<>1__state = 0;
                    d__ = this;
                }
                else
                {
                    d__ = new JToken.<GetAncestors>d__42(0) {
                        <>4__this = this.<>4__this
                    };
                }
                d__.self = this.<>3__self;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Newtonsoft.Json.Linq.JToken>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            JToken IEnumerator<JToken>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }

        private class LineInfoAnnotation
        {
            internal readonly int LineNumber;
            internal readonly int LinePosition;

            public LineInfoAnnotation(int lineNumber, int linePosition)
            {
                this.LineNumber = lineNumber;
                this.LinePosition = linePosition;
            }
        }
    }
}

