namespace Newtonsoft.Json.Linq
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Numerics;
    using System.Runtime.InteropServices;

    public class JValue : JToken, IEquatable<JValue>, IFormattable, IComparable, IComparable<JValue>, IConvertible
    {
        private JTokenType _valueType;
        private object _value;

        public JValue(JValue other) : this(other.Value, other.Type)
        {
        }

        public JValue(bool value) : this(value, JTokenType.Boolean)
        {
        }

        public JValue(char value) : this(value, JTokenType.String)
        {
        }

        public JValue(DateTime value) : this(value, JTokenType.Date)
        {
        }

        public JValue(DateTimeOffset value) : this(value, JTokenType.Date)
        {
        }

        public JValue(decimal value) : this(value, JTokenType.Float)
        {
        }

        public JValue(double value) : this(value, JTokenType.Float)
        {
        }

        public JValue(Guid value) : this(value, JTokenType.Guid)
        {
        }

        public JValue(long value) : this(value, JTokenType.Integer)
        {
        }

        public JValue(object value) : this(value, GetValueType(null, value))
        {
        }

        public JValue(float value) : this(value, JTokenType.Float)
        {
        }

        public JValue(string value) : this(value, JTokenType.String)
        {
        }

        public JValue(TimeSpan value) : this(value, JTokenType.TimeSpan)
        {
        }

        [CLSCompliant(false)]
        public JValue(ulong value) : this(value, JTokenType.Integer)
        {
        }

        public JValue(Uri value) : this(value, (value != null) ? JTokenType.Uri : JTokenType.Null)
        {
        }

        internal JValue(object value, JTokenType type)
        {
            this._value = value;
            this._valueType = type;
        }

        internal override JToken CloneToken() => 
            new JValue(this);

        internal static int Compare(JTokenType valueType, object objA, object objB)
        {
            DateTime time;
            DateTime dateTime;
            if ((objA == null) && (objB == null))
            {
                return 0;
            }
            if ((objA != null) && (objB == null))
            {
                return 1;
            }
            if ((objA == null) && (objB != null))
            {
                return -1;
            }
            switch (valueType)
            {
                case JTokenType.Comment:
                case JTokenType.String:
                case JTokenType.Raw:
                    return string.CompareOrdinal(Convert.ToString(objA, CultureInfo.InvariantCulture), Convert.ToString(objB, CultureInfo.InvariantCulture));

                case JTokenType.Integer:
                    switch (objA)
                    {
                        case (BigInteger _):
                            return CompareBigInteger((BigInteger) objA, objB);
                            break;
                    }
                    if (objB is BigInteger)
                    {
                        return -CompareBigInteger((BigInteger) objB, objA);
                    }
                    if (((objA is ulong) || (objB is ulong)) || ((objA is decimal) || (objB is decimal)))
                    {
                        return Convert.ToDecimal(objA, CultureInfo.InvariantCulture).CompareTo(Convert.ToDecimal(objB, CultureInfo.InvariantCulture));
                    }
                    if (((objA is float) || (objB is float)) || ((objA is double) || (objB is double)))
                    {
                        return CompareFloat(objA, objB);
                    }
                    return Convert.ToInt64(objA, CultureInfo.InvariantCulture).CompareTo(Convert.ToInt64(objB, CultureInfo.InvariantCulture));

                case JTokenType.Float:
                    if (objA is BigInteger)
                    {
                        return CompareBigInteger((BigInteger) objA, objB);
                    }
                    if (objB is BigInteger)
                    {
                        return -CompareBigInteger((BigInteger) objB, objA);
                    }
                    return CompareFloat(objA, objB);

                case JTokenType.Boolean:
                {
                    bool flag = Convert.ToBoolean(objA, CultureInfo.InvariantCulture);
                    bool flag2 = Convert.ToBoolean(objB, CultureInfo.InvariantCulture);
                    return flag.CompareTo(flag2);
                }
                case JTokenType.Date:
                {
                    if (!(objA is DateTime))
                    {
                        DateTimeOffset offset3;
                        DateTimeOffset offset2 = (DateTimeOffset) objA;
                        if (objB is DateTimeOffset)
                        {
                            offset3 = (DateTimeOffset) objB;
                        }
                        else
                        {
                            offset3 = new DateTimeOffset(Convert.ToDateTime(objB, CultureInfo.InvariantCulture));
                        }
                        return offset2.CompareTo(offset3);
                    }
                    time = (DateTime) objA;
                    if (!(objB is DateTimeOffset))
                    {
                        dateTime = Convert.ToDateTime(objB, CultureInfo.InvariantCulture);
                        break;
                    }
                    DateTimeOffset offset = (DateTimeOffset) objB;
                    dateTime = offset.DateTime;
                    break;
                }
                case JTokenType.Bytes:
                {
                    if (!(objB is byte[]))
                    {
                        throw new ArgumentException("Object must be of type byte[].");
                    }
                    byte[] buffer = objA as byte[];
                    byte[] buffer2 = objB as byte[];
                    if (buffer == null)
                    {
                        return -1;
                    }
                    if (buffer2 == null)
                    {
                        return 1;
                    }
                    return MiscellaneousUtils.ByteArrayCompare(buffer, buffer2);
                }
                case JTokenType.Guid:
                {
                    if (!(objB is Guid))
                    {
                        throw new ArgumentException("Object must be of type Guid.");
                    }
                    Guid guid = (Guid) objA;
                    Guid guid2 = (Guid) objB;
                    return guid.CompareTo(guid2);
                }
                case JTokenType.Uri:
                {
                    if (!(objB is Uri))
                    {
                        throw new ArgumentException("Object must be of type Uri.");
                    }
                    Uri uri = (Uri) objA;
                    Uri uri2 = (Uri) objB;
                    return Comparer<string>.Default.Compare(uri.ToString(), uri2.ToString());
                }
                case JTokenType.TimeSpan:
                {
                    if (!(objB is TimeSpan))
                    {
                        throw new ArgumentException("Object must be of type TimeSpan.");
                    }
                    TimeSpan span = (TimeSpan) objA;
                    TimeSpan span2 = (TimeSpan) objB;
                    return span.CompareTo(span2);
                }
                default:
                    throw MiscellaneousUtils.CreateArgumentOutOfRangeException("valueType", valueType, "Unexpected value type: {0}".FormatWith(CultureInfo.InvariantCulture, valueType));
            }
            return time.CompareTo(dateTime);
        }

        private static int CompareBigInteger(BigInteger i1, object i2)
        {
            int num = i1.CompareTo(ConvertUtils.ToBigInteger(i2));
            if (num != 0)
            {
                return num;
            }
            if (i2 is decimal)
            {
                decimal num2 = (decimal) i2;
                return decimal.Zero.CompareTo(Math.Abs((decimal) (num2 - Math.Truncate(num2))));
            }
            if (!(i2 is double) && !(i2 is float))
            {
                return num;
            }
            double d = Convert.ToDouble(i2, CultureInfo.InvariantCulture);
            double num5 = 0.0;
            return num5.CompareTo(Math.Abs((double) (d - Math.Truncate(d))));
        }

        private static int CompareFloat(object objA, object objB)
        {
            double num = Convert.ToDouble(objA, CultureInfo.InvariantCulture);
            double num2 = Convert.ToDouble(objB, CultureInfo.InvariantCulture);
            if (MathUtils.ApproxEquals(num, num2))
            {
                return 0;
            }
            return num.CompareTo(num2);
        }

        public int CompareTo(JValue obj)
        {
            if (obj == null)
            {
                return 1;
            }
            return Compare(this._valueType, this._value, obj._value);
        }

        public static JValue CreateComment(string value) => 
            new JValue(value, JTokenType.Comment);

        public static JValue CreateNull() => 
            new JValue(null, JTokenType.Null);

        public static JValue CreateString(string value) => 
            new JValue(value, JTokenType.String);

        public static JValue CreateUndefined() => 
            new JValue(null, JTokenType.Undefined);

        internal override bool DeepEquals(JToken node)
        {
            JValue value2 = node as JValue;
            if (value2 == null)
            {
                return false;
            }
            return ((value2 == this) || ValuesEquals(this, value2));
        }

        public bool Equals(JValue other)
        {
            if (other == null)
            {
                return false;
            }
            return ValuesEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            JValue other = obj as JValue;
            if (other != null)
            {
                return this.Equals(other);
            }
            return base.Equals(obj);
        }

        internal override int GetDeepHashCode()
        {
            int num = (this._value != null) ? this._value.GetHashCode() : 0;
            return (((int) this._valueType).GetHashCode() ^ num);
        }

        public override int GetHashCode() => 
            this._value?.GetHashCode();

        protected override DynamicMetaObject GetMetaObject(Expression parameter) => 
            new DynamicProxyMetaObject<JValue>(parameter, this, new JValueDynamicProxy(), true);

        private static JTokenType GetStringValueType(JTokenType? current)
        {
            if (!current.HasValue)
            {
                return JTokenType.String;
            }
            JTokenType valueOrDefault = current.GetValueOrDefault();
            if (((valueOrDefault != JTokenType.Comment) && (valueOrDefault != JTokenType.String)) && (valueOrDefault != JTokenType.Raw))
            {
                return JTokenType.String;
            }
            return current.GetValueOrDefault();
        }

        private static JTokenType GetValueType(JTokenType? current, object value)
        {
            switch (value)
            {
                case (null):
                    return JTokenType.Null;
                    break;
            }
            if (value == DBNull.Value)
            {
                return JTokenType.Null;
            }
            if (value is string)
            {
                return GetStringValueType(current);
            }
            if ((((value is long) || (value is int)) || ((value is short) || (value is sbyte))) || (((value is ulong) || (value is uint)) || ((value is ushort) || (value is byte))))
            {
                return JTokenType.Integer;
            }
            if (value is Enum)
            {
                return JTokenType.Integer;
            }
            if (value is BigInteger)
            {
                return JTokenType.Integer;
            }
            if (((value is double) || (value is float)) || (value is decimal))
            {
                return JTokenType.Float;
            }
            if (value is DateTime)
            {
                return JTokenType.Date;
            }
            if (value is DateTimeOffset)
            {
                return JTokenType.Date;
            }
            if (value is byte[])
            {
                return JTokenType.Bytes;
            }
            if (value is bool)
            {
                return JTokenType.Boolean;
            }
            if (value is Guid)
            {
                return JTokenType.Guid;
            }
            if (value is Uri)
            {
                return JTokenType.Uri;
            }
            if (!(value is TimeSpan))
            {
                throw new ArgumentException("Could not determine JSON object type for type {0}.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
            }
            return JTokenType.TimeSpan;
        }

        private static bool Operation(ExpressionType operation, object objA, object objB, out object result)
        {
            BigInteger integer;
            BigInteger integer2;
            decimal num;
            decimal num2;
            double num3;
            double num4;
            long num5;
            long num6;
            if (((objA is string) || (objB is string)) && ((operation == ExpressionType.Add) || (operation == ExpressionType.AddAssign)))
            {
                result = objA?.ToString() + objB?.ToString();
                return true;
            }
            if ((objA is BigInteger) || (objB is BigInteger))
            {
                if ((objA == null) || (objB == null))
                {
                    result = null;
                    return true;
                }
                integer = ConvertUtils.ToBigInteger(objA);
                integer2 = ConvertUtils.ToBigInteger(objB);
                if (operation <= ExpressionType.Subtract)
                {
                    if (operation <= ExpressionType.Divide)
                    {
                        if (operation == ExpressionType.Add)
                        {
                            goto Label_00F0;
                        }
                        if (operation == ExpressionType.Divide)
                        {
                            goto Label_0120;
                        }
                    }
                    else
                    {
                        if (operation == ExpressionType.Multiply)
                        {
                            goto Label_0110;
                        }
                        if (operation == ExpressionType.Subtract)
                        {
                            goto Label_0100;
                        }
                    }
                }
                else if (operation <= ExpressionType.DivideAssign)
                {
                    if (operation == ExpressionType.AddAssign)
                    {
                        goto Label_00F0;
                    }
                    if (operation == ExpressionType.DivideAssign)
                    {
                        goto Label_0120;
                    }
                }
                else
                {
                    if (operation == ExpressionType.MultiplyAssign)
                    {
                        goto Label_0110;
                    }
                    if (operation == ExpressionType.SubtractAssign)
                    {
                        goto Label_0100;
                    }
                }
            }
            else if ((!(objA is ulong) && !(objB is ulong)) && (!(objA is decimal) && !(objB is decimal)))
            {
                if ((!(objA is float) && !(objB is float)) && (!(objA is double) && !(objB is double)))
                {
                    if (((((objA is int) || (objA is uint)) || ((objA is long) || (objA is short))) || (((objA is ushort) || (objA is sbyte)) || ((objA is byte) || (objB is int)))) || ((((objB is uint) || (objB is long)) || ((objB is short) || (objB is ushort))) || ((objB is sbyte) || (objB is byte))))
                    {
                        if ((objA == null) || (objB == null))
                        {
                            result = null;
                            return true;
                        }
                        num5 = Convert.ToInt64(objA, CultureInfo.InvariantCulture);
                        num6 = Convert.ToInt64(objB, CultureInfo.InvariantCulture);
                        if (operation <= ExpressionType.Subtract)
                        {
                            if (operation <= ExpressionType.Divide)
                            {
                                if (operation == ExpressionType.Add)
                                {
                                    goto Label_0457;
                                }
                                if (operation == ExpressionType.Divide)
                                {
                                    goto Label_0481;
                                }
                            }
                            else
                            {
                                if (operation == ExpressionType.Multiply)
                                {
                                    goto Label_0473;
                                }
                                if (operation == ExpressionType.Subtract)
                                {
                                    goto Label_0465;
                                }
                            }
                        }
                        else if (operation <= ExpressionType.DivideAssign)
                        {
                            if (operation == ExpressionType.AddAssign)
                            {
                                goto Label_0457;
                            }
                            if (operation == ExpressionType.DivideAssign)
                            {
                                goto Label_0481;
                            }
                        }
                        else
                        {
                            if (operation == ExpressionType.MultiplyAssign)
                            {
                                goto Label_0473;
                            }
                            if (operation == ExpressionType.SubtractAssign)
                            {
                                goto Label_0465;
                            }
                        }
                    }
                }
                else
                {
                    if ((objA == null) || (objB == null))
                    {
                        result = null;
                        return true;
                    }
                    num3 = Convert.ToDouble(objA, CultureInfo.InvariantCulture);
                    num4 = Convert.ToDouble(objB, CultureInfo.InvariantCulture);
                    if (operation <= ExpressionType.Subtract)
                    {
                        if (operation <= ExpressionType.Divide)
                        {
                            if (operation == ExpressionType.Add)
                            {
                                goto Label_02F0;
                            }
                            if (operation == ExpressionType.Divide)
                            {
                                goto Label_031A;
                            }
                        }
                        else
                        {
                            if (operation == ExpressionType.Multiply)
                            {
                                goto Label_030C;
                            }
                            if (operation == ExpressionType.Subtract)
                            {
                                goto Label_02FE;
                            }
                        }
                    }
                    else if (operation <= ExpressionType.DivideAssign)
                    {
                        if (operation == ExpressionType.AddAssign)
                        {
                            goto Label_02F0;
                        }
                        if (operation == ExpressionType.DivideAssign)
                        {
                            goto Label_031A;
                        }
                    }
                    else
                    {
                        if (operation == ExpressionType.MultiplyAssign)
                        {
                            goto Label_030C;
                        }
                        if (operation == ExpressionType.SubtractAssign)
                        {
                            goto Label_02FE;
                        }
                    }
                }
            }
            else
            {
                if ((objA == null) || (objB == null))
                {
                    result = null;
                    return true;
                }
                num = Convert.ToDecimal(objA, CultureInfo.InvariantCulture);
                num2 = Convert.ToDecimal(objB, CultureInfo.InvariantCulture);
                if (operation <= ExpressionType.Subtract)
                {
                    if (operation <= ExpressionType.Divide)
                    {
                        if (operation == ExpressionType.Add)
                        {
                            goto Label_01EF;
                        }
                        if (operation == ExpressionType.Divide)
                        {
                            goto Label_021F;
                        }
                    }
                    else
                    {
                        if (operation == ExpressionType.Multiply)
                        {
                            goto Label_020F;
                        }
                        if (operation == ExpressionType.Subtract)
                        {
                            goto Label_01FF;
                        }
                    }
                }
                else if (operation <= ExpressionType.DivideAssign)
                {
                    if (operation == ExpressionType.AddAssign)
                    {
                        goto Label_01EF;
                    }
                    if (operation == ExpressionType.DivideAssign)
                    {
                        goto Label_021F;
                    }
                }
                else
                {
                    if (operation == ExpressionType.MultiplyAssign)
                    {
                        goto Label_020F;
                    }
                    if (operation == ExpressionType.SubtractAssign)
                    {
                        goto Label_01FF;
                    }
                }
            }
            result = null;
            return false;
        Label_00F0:
            result = integer + integer2;
            return true;
        Label_0100:
            result = integer - integer2;
            return true;
        Label_0110:
            result = integer * integer2;
            return true;
        Label_0120:
            result = integer / integer2;
            return true;
        Label_01EF:
            result = num + num2;
            return true;
        Label_01FF:
            result = num - num2;
            return true;
        Label_020F:
            result = num * num2;
            return true;
        Label_021F:
            result = num / num2;
            return true;
        Label_02F0:
            result = num3 + num4;
            return true;
        Label_02FE:
            result = num3 - num4;
            return true;
        Label_030C:
            result = num3 * num4;
            return true;
        Label_031A:
            result = num3 / num4;
            return true;
        Label_0457:
            result = num5 + num6;
            return true;
        Label_0465:
            result = num5 - num6;
            return true;
        Label_0473:
            result = num5 * num6;
            return true;
        Label_0481:
            result = num5 / num6;
            return true;
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            object objB = (obj is JValue) ? ((JValue) obj).Value : obj;
            return Compare(this._valueType, this._value, objB);
        }

        TypeCode IConvertible.GetTypeCode()
        {
            if (this._value == null)
            {
                return TypeCode.Empty;
            }
            IConvertible convertible = this._value as IConvertible;
            return convertible?.GetTypeCode();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider) => 
            ((bool) this);

        byte IConvertible.ToByte(IFormatProvider provider) => 
            ((byte) this);

        char IConvertible.ToChar(IFormatProvider provider) => 
            ((char) this);

        DateTime IConvertible.ToDateTime(IFormatProvider provider) => 
            ((DateTime) this);

        decimal IConvertible.ToDecimal(IFormatProvider provider) => 
            ((decimal) this);

        double IConvertible.ToDouble(IFormatProvider provider) => 
            ((double) this);

        short IConvertible.ToInt16(IFormatProvider provider) => 
            ((short) this);

        int IConvertible.ToInt32(IFormatProvider provider) => 
            ((int) this);

        long IConvertible.ToInt64(IFormatProvider provider) => 
            ((long) this);

        sbyte IConvertible.ToSByte(IFormatProvider provider) => 
            ((sbyte) this);

        float IConvertible.ToSingle(IFormatProvider provider) => 
            ((float) this);

        object IConvertible.ToType(System.Type conversionType, IFormatProvider provider) => 
            base.ToObject(conversionType);

        ushort IConvertible.ToUInt16(IFormatProvider provider) => 
            ((ushort) this);

        uint IConvertible.ToUInt32(IFormatProvider provider) => 
            ((uint) this);

        ulong IConvertible.ToUInt64(IFormatProvider provider) => 
            ((ulong) this);

        public override string ToString() => 
            this._value?.ToString();

        public string ToString(IFormatProvider formatProvider) => 
            this.ToString(null, formatProvider);

        public string ToString(string format) => 
            this.ToString(format, CultureInfo.CurrentCulture);

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (this._value == null)
            {
                return string.Empty;
            }
            IFormattable formattable = this._value as IFormattable;
            if (formattable != null)
            {
                return formattable.ToString(format, formatProvider);
            }
            return this._value.ToString();
        }

        private static bool ValuesEquals(JValue v1, JValue v2) => 
            ((v1 == v2) || ((v1._valueType == v2._valueType) && (Compare(v1._valueType, v1._value, v2._value) == 0)));

        public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        {
            if (((converters != null) && (converters.Length != 0)) && (this._value != null))
            {
                JsonConverter matchingConverter = JsonSerializer.GetMatchingConverter(converters, this._value.GetType());
                if ((matchingConverter != null) && matchingConverter.CanWrite)
                {
                    matchingConverter.WriteJson(writer, this._value, JsonSerializer.CreateDefault());
                    return;
                }
            }
            switch (this._valueType)
            {
                case JTokenType.Comment:
                    writer.WriteComment(this._value?.ToString());
                    return;

                case JTokenType.Integer:
                    switch (this._value)
                    {
                        case (int _):
                            break;

                        case (long _):
                            writer.WriteValue((long) this._value);
                            return;
                            break;

                        case (ulong _):
                            writer.WriteValue((ulong) this._value);
                            return;
                            break;

                        case (BigInteger _):
                            writer.WriteValue((BigInteger) this._value);
                            return;
                            break;
                    }
                    writer.WriteValue((int) this._value);
                    return;

                case JTokenType.Float:
                    if (!(this._value is decimal))
                    {
                        if (this._value is double)
                        {
                            writer.WriteValue((double) this._value);
                            return;
                        }
                        if (this._value is float)
                        {
                            writer.WriteValue((float) this._value);
                            return;
                        }
                        writer.WriteValue(Convert.ToDouble(this._value, CultureInfo.InvariantCulture));
                        return;
                    }
                    writer.WriteValue((decimal) this._value);
                    return;

                case JTokenType.String:
                    writer.WriteValue(this._value?.ToString());
                    return;

                case JTokenType.Boolean:
                    writer.WriteValue(Convert.ToBoolean(this._value, CultureInfo.InvariantCulture));
                    return;

                case JTokenType.Null:
                    writer.WriteNull();
                    return;

                case JTokenType.Undefined:
                    writer.WriteUndefined();
                    return;

                case JTokenType.Date:
                    if (!(this._value is DateTimeOffset))
                    {
                        writer.WriteValue(Convert.ToDateTime(this._value, CultureInfo.InvariantCulture));
                        return;
                    }
                    writer.WriteValue((DateTimeOffset) this._value);
                    return;

                case JTokenType.Raw:
                    writer.WriteRawValue(this._value?.ToString());
                    return;

                case JTokenType.Bytes:
                    writer.WriteValue((byte[]) this._value);
                    return;

                case JTokenType.Guid:
                    writer.WriteValue((this._value != null) ? ((Guid?) this._value) : null);
                    return;

                case JTokenType.Uri:
                    writer.WriteValue((Uri) this._value);
                    return;

                case JTokenType.TimeSpan:
                    writer.WriteValue((this._value != null) ? ((TimeSpan?) this._value) : null);
                    return;
            }
            throw MiscellaneousUtils.CreateArgumentOutOfRangeException("TokenType", this._valueType, "Unexpected token type.");
        }

        public override bool HasValues =>
            false;

        public override JTokenType Type =>
            this._valueType;

        public object Value
        {
            get => 
                this._value;
            set
            {
                System.Type type = value?.GetType();
                if (this._value?.GetType() != type)
                {
                    this._valueType = GetValueType(new JTokenType?(this._valueType), value);
                }
                this._value = value;
            }
        }

        private class JValueDynamicProxy : DynamicProxy<JValue>
        {
            public override bool TryBinaryOperation(JValue instance, BinaryOperationBinder binder, object arg, out object result)
            {
                object objB = (arg is JValue) ? ((JValue) arg).Value : arg;
                switch (binder.Operation)
                {
                    case ExpressionType.Multiply:
                    case ExpressionType.Divide:
                    case ExpressionType.Add:
                    case ExpressionType.Subtract:
                    case ExpressionType.AddAssign:
                    case ExpressionType.DivideAssign:
                    case ExpressionType.MultiplyAssign:
                    case ExpressionType.SubtractAssign:
                        if (JValue.Operation(binder.Operation, instance.Value, objB, out result))
                        {
                            result = new JValue(result);
                            return true;
                        }
                        break;

                    case ExpressionType.NotEqual:
                        result = JValue.Compare(instance.Type, instance.Value, objB) > 0;
                        return true;

                    case ExpressionType.Equal:
                        result = JValue.Compare(instance.Type, instance.Value, objB) == 0;
                        return true;

                    case ExpressionType.GreaterThan:
                        result = JValue.Compare(instance.Type, instance.Value, objB) > 0;
                        return true;

                    case ExpressionType.GreaterThanOrEqual:
                        result = JValue.Compare(instance.Type, instance.Value, objB) >= 0;
                        return true;

                    case ExpressionType.LessThan:
                        result = JValue.Compare(instance.Type, instance.Value, objB) < 0;
                        return true;

                    case ExpressionType.LessThanOrEqual:
                        result = JValue.Compare(instance.Type, instance.Value, objB) <= 0;
                        return true;
                }
                result = null;
                return false;
            }

            public override bool TryConvert(JValue instance, ConvertBinder binder, out object result)
            {
                if ((binder.Type == typeof(JValue)) || (binder.Type == typeof(JToken)))
                {
                    result = instance;
                    return true;
                }
                object initialValue = instance.Value;
                if (initialValue == null)
                {
                    result = null;
                    return ReflectionUtils.IsNullable(binder.Type);
                }
                result = ConvertUtils.Convert(initialValue, CultureInfo.InvariantCulture, binder.Type);
                return true;
            }
        }
    }
}

