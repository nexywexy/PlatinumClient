namespace Newtonsoft.Json.Utilities
{
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.SqlTypes;
    using System.Globalization;
    using System.Numerics;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal static class ConvertUtils
    {
        private static readonly Dictionary<Type, PrimitiveTypeCode> TypeCodeMap;
        private static readonly TypeInformation[] PrimitiveTypeCodes;
        private static readonly ThreadSafeStore<TypeConvertKey, Func<object, object>> CastConverters;

        static ConvertUtils()
        {
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(char), PrimitiveTypeCode.Char);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(char?), PrimitiveTypeCode.CharNullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(bool), PrimitiveTypeCode.Boolean);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(bool?), PrimitiveTypeCode.BooleanNullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(sbyte), PrimitiveTypeCode.SByte);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(sbyte?), PrimitiveTypeCode.SByteNullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(short), PrimitiveTypeCode.Int16);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(short?), PrimitiveTypeCode.Int16Nullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(ushort), PrimitiveTypeCode.UInt16);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(ushort?), PrimitiveTypeCode.UInt16Nullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(int), PrimitiveTypeCode.Int32);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(int?), PrimitiveTypeCode.Int32Nullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(byte), PrimitiveTypeCode.Byte);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(byte?), PrimitiveTypeCode.ByteNullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(uint), PrimitiveTypeCode.UInt32);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(uint?), PrimitiveTypeCode.UInt32Nullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(long), PrimitiveTypeCode.Int64);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(long?), PrimitiveTypeCode.Int64Nullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(ulong), PrimitiveTypeCode.UInt64);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(ulong?), PrimitiveTypeCode.UInt64Nullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(float), PrimitiveTypeCode.Single);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(float?), PrimitiveTypeCode.SingleNullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(double), PrimitiveTypeCode.Double);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(double?), PrimitiveTypeCode.DoubleNullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(DateTime), PrimitiveTypeCode.DateTime);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(DateTime?), PrimitiveTypeCode.DateTimeNullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(DateTimeOffset), PrimitiveTypeCode.DateTimeOffset);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(DateTimeOffset?), PrimitiveTypeCode.DateTimeOffsetNullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(decimal), PrimitiveTypeCode.Decimal);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(decimal?), PrimitiveTypeCode.DecimalNullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(Guid), PrimitiveTypeCode.Guid);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(Guid?), PrimitiveTypeCode.GuidNullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(TimeSpan), PrimitiveTypeCode.TimeSpan);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(TimeSpan?), PrimitiveTypeCode.TimeSpanNullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(BigInteger), PrimitiveTypeCode.BigInteger);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(BigInteger?), PrimitiveTypeCode.BigIntegerNullable);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(Uri), PrimitiveTypeCode.Uri);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(string), PrimitiveTypeCode.String);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(byte[]), PrimitiveTypeCode.Bytes);
            new Dictionary<Type, PrimitiveTypeCode>().Add(typeof(DBNull), PrimitiveTypeCode.DBNull);
            TypeCodeMap = new Dictionary<Type, PrimitiveTypeCode>();
            TypeInformation[] informationArray1 = new TypeInformation[0x13];
            new TypeInformation { TypeCode = PrimitiveTypeCode.Empty }.Type = typeof(object);
            TypeInformation information1 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.Empty
            };
            informationArray1[0] = information1;
            new TypeInformation { TypeCode = PrimitiveTypeCode.Object }.Type = typeof(object);
            TypeInformation information2 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.Object
            };
            informationArray1[1] = information2;
            new TypeInformation { TypeCode = PrimitiveTypeCode.DBNull }.Type = typeof(object);
            TypeInformation information3 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.DBNull
            };
            informationArray1[2] = information3;
            new TypeInformation { TypeCode = PrimitiveTypeCode.Boolean }.Type = typeof(bool);
            TypeInformation information4 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.Boolean
            };
            informationArray1[3] = information4;
            new TypeInformation { TypeCode = PrimitiveTypeCode.Char }.Type = typeof(char);
            TypeInformation information5 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.Char
            };
            informationArray1[4] = information5;
            new TypeInformation { TypeCode = PrimitiveTypeCode.SByte }.Type = typeof(sbyte);
            TypeInformation information6 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.SByte
            };
            informationArray1[5] = information6;
            new TypeInformation { TypeCode = PrimitiveTypeCode.Byte }.Type = typeof(byte);
            TypeInformation information7 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.Byte
            };
            informationArray1[6] = information7;
            new TypeInformation { TypeCode = PrimitiveTypeCode.Int16 }.Type = typeof(short);
            TypeInformation information8 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.Int16
            };
            informationArray1[7] = information8;
            new TypeInformation { TypeCode = PrimitiveTypeCode.UInt16 }.Type = typeof(ushort);
            TypeInformation information9 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.UInt16
            };
            informationArray1[8] = information9;
            new TypeInformation { TypeCode = PrimitiveTypeCode.Int32 }.Type = typeof(int);
            TypeInformation information10 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.Int32
            };
            informationArray1[9] = information10;
            new TypeInformation { TypeCode = PrimitiveTypeCode.UInt32 }.Type = typeof(uint);
            TypeInformation information11 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.UInt32
            };
            informationArray1[10] = information11;
            new TypeInformation { TypeCode = PrimitiveTypeCode.Int64 }.Type = typeof(long);
            TypeInformation information12 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.Int64
            };
            informationArray1[11] = information12;
            new TypeInformation { TypeCode = PrimitiveTypeCode.UInt64 }.Type = typeof(ulong);
            TypeInformation information13 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.UInt64
            };
            informationArray1[12] = information13;
            new TypeInformation { TypeCode = PrimitiveTypeCode.Single }.Type = typeof(float);
            TypeInformation information14 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.Single
            };
            informationArray1[13] = information14;
            new TypeInformation { TypeCode = PrimitiveTypeCode.Double }.Type = typeof(double);
            TypeInformation information15 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.Double
            };
            informationArray1[14] = information15;
            new TypeInformation { TypeCode = PrimitiveTypeCode.Decimal }.Type = typeof(decimal);
            TypeInformation information16 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.Decimal
            };
            informationArray1[15] = information16;
            new TypeInformation { TypeCode = PrimitiveTypeCode.DateTime }.Type = typeof(DateTime);
            TypeInformation information17 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.DateTime
            };
            informationArray1[0x10] = information17;
            new TypeInformation { TypeCode = PrimitiveTypeCode.Empty }.Type = typeof(object);
            TypeInformation information18 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.Empty
            };
            informationArray1[0x11] = information18;
            new TypeInformation { TypeCode = PrimitiveTypeCode.String }.Type = typeof(string);
            TypeInformation information19 = new TypeInformation {
                TypeCode = PrimitiveTypeCode.String
            };
            informationArray1[0x12] = information19;
            PrimitiveTypeCodes = informationArray1;
            CastConverters = new ThreadSafeStore<TypeConvertKey, Func<object, object>>(new Func<TypeConvertKey, Func<object, object>>(ConvertUtils.CreateCastConverter));
        }

        public static object Convert(object initialValue, CultureInfo culture, Type targetType)
        {
            switch (TryConvertInternal(initialValue, culture, targetType, out object obj2))
            {
                case ConvertResult.Success:
                    return obj2;

                case ConvertResult.CannotConvertNull:
                    throw new Exception("Can not convert null {0} into non-nullable {1}.".FormatWith(CultureInfo.InvariantCulture, initialValue.GetType(), targetType));

                case ConvertResult.NotInstantiableType:
                    throw new ArgumentException("Target type {0} is not a value type or a non-abstract class.".FormatWith(CultureInfo.InvariantCulture, targetType), "targetType");

                case ConvertResult.NoValidConversion:
                    throw new InvalidOperationException("Can not convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, initialValue.GetType(), targetType));
            }
            throw new InvalidOperationException("Unexpected conversion result.");
        }

        public static object ConvertOrCast(object initialValue, CultureInfo culture, Type targetType)
        {
            if (targetType == typeof(object))
            {
                return initialValue;
            }
            if ((initialValue == null) && ReflectionUtils.IsNullable(targetType))
            {
                return null;
            }
            if (TryConvert(initialValue, culture, targetType, out object obj2))
            {
                return obj2;
            }
            return EnsureTypeAssignable(initialValue, ReflectionUtils.GetObjectType(initialValue), targetType);
        }

        private static Func<object, object> CreateCastConverter(TypeConvertKey t)
        {
            Type[] types = new Type[] { t.InitialType };
            MethodInfo method = t.TargetType.GetMethod("op_Implicit", types);
            if (method == null)
            {
                Type[] typeArray2 = new Type[] { t.InitialType };
                method = t.TargetType.GetMethod("op_Explicit", typeArray2);
            }
            if (method == null)
            {
                return null;
            }
            MethodCall<object, object> call = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
            return o => call(null, new object[] { o });
        }

        private static object EnsureTypeAssignable(object value, Type initialType, Type targetType)
        {
            Type c = value?.GetType();
            if (value != null)
            {
                if (targetType.IsAssignableFrom(c))
                {
                    return value;
                }
                return CastConverters.Get(new TypeConvertKey(c, targetType))?.Invoke(value);
            }
            if (!ReflectionUtils.IsNullable(targetType))
            {
                throw new ArgumentException("Could not cast or convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, (initialType != null) ? initialType.ToString() : "{null}", targetType));
            }
            return null;
        }

        public static object FromBigInteger(BigInteger i, Type targetType)
        {
            object obj2;
            if (targetType == typeof(decimal))
            {
                return (decimal) i;
            }
            if (targetType == typeof(double))
            {
                return (double) i;
            }
            if (targetType == typeof(float))
            {
                return (float) i;
            }
            if (targetType == typeof(ulong))
            {
                return (ulong) i;
            }
            if (targetType == typeof(bool))
            {
                return (i != 0L);
            }
            try
            {
                obj2 = System.Convert.ChangeType((long) i, targetType, CultureInfo.InvariantCulture);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Can not convert from BigInteger to {0}.".FormatWith(CultureInfo.InvariantCulture, targetType), exception);
            }
            return obj2;
        }

        internal static TypeConverter GetConverter(Type t) => 
            JsonTypeReflector.GetTypeConverter(t);

        public static PrimitiveTypeCode GetTypeCode(Type t) => 
            GetTypeCode(t, out _);

        public static PrimitiveTypeCode GetTypeCode(Type t, out bool isEnum)
        {
            if (TypeCodeMap.TryGetValue(t, out PrimitiveTypeCode code))
            {
                isEnum = false;
                return code;
            }
            if (t.IsEnum())
            {
                isEnum = true;
                return GetTypeCode(Enum.GetUnderlyingType(t));
            }
            if (ReflectionUtils.IsNullableType(t))
            {
                Type underlyingType = Nullable.GetUnderlyingType(t);
                if (underlyingType.IsEnum())
                {
                    Type[] typeArguments = new Type[] { Enum.GetUnderlyingType(underlyingType) };
                    isEnum = true;
                    return GetTypeCode(typeof(Nullable<>).MakeGenericType(typeArguments));
                }
            }
            isEnum = false;
            return PrimitiveTypeCode.Object;
        }

        public static TypeInformation GetTypeInformation(IConvertible convertable) => 
            PrimitiveTypeCodes[(int) convertable.GetTypeCode()];

        private static int HexCharToInt(char ch)
        {
            if ((ch <= '9') && (ch >= '0'))
            {
                return (ch - '0');
            }
            if ((ch <= 'F') && (ch >= 'A'))
            {
                return (ch - '7');
            }
            if ((ch > 'f') || (ch < 'a'))
            {
                throw new FormatException("Invalid hex character: " + ch.ToString());
            }
            return (ch - 'W');
        }

        public static int HexTextToInt(char[] text, int start, int end)
        {
            int num = 0;
            for (int i = start; i < end; i++)
            {
                num += HexCharToInt(text[i]) << (((end - 1) - i) * 4);
            }
            return num;
        }

        public static ParseResult Int32TryParse(char[] chars, int start, int length, out int value)
        {
            value = 0;
            if (length == 0)
            {
                return ParseResult.Invalid;
            }
            bool flag = chars[start] == '-';
            if (flag)
            {
                if (length == 1)
                {
                    return ParseResult.Invalid;
                }
                start++;
                length--;
            }
            int num = start + length;
            if ((length > 10) || ((length == 10) && ((chars[start] - '0') > 2)))
            {
                for (int j = start; j < num; j++)
                {
                    int num3 = chars[j] - '0';
                    if ((num3 < 0) || (num3 > 9))
                    {
                        return ParseResult.Invalid;
                    }
                }
                return ParseResult.Overflow;
            }
            for (int i = start; i < num; i++)
            {
                int num5 = chars[i] - '0';
                if ((num5 < 0) || (num5 > 9))
                {
                    return ParseResult.Invalid;
                }
                int num6 = (10 * value) - num5;
                if (num6 > value)
                {
                    i++;
                    while (i < num)
                    {
                        num5 = chars[i] - '0';
                        if ((num5 < 0) || (num5 > 9))
                        {
                            return ParseResult.Invalid;
                        }
                        i++;
                    }
                    return ParseResult.Overflow;
                }
                value = num6;
            }
            if (!flag)
            {
                if (value == -2147483648)
                {
                    return ParseResult.Overflow;
                }
                value = -value;
            }
            return ParseResult.Success;
        }

        public static ParseResult Int64TryParse(char[] chars, int start, int length, out long value)
        {
            value = 0L;
            if (length == 0)
            {
                return ParseResult.Invalid;
            }
            bool flag = chars[start] == '-';
            if (flag)
            {
                if (length == 1)
                {
                    return ParseResult.Invalid;
                }
                start++;
                length--;
            }
            int num = start + length;
            if (length > 0x13)
            {
                for (int j = start; j < num; j++)
                {
                    int num3 = chars[j] - '0';
                    if ((num3 < 0) || (num3 > 9))
                    {
                        return ParseResult.Invalid;
                    }
                }
                return ParseResult.Overflow;
            }
            for (int i = start; i < num; i++)
            {
                int num5 = chars[i] - '0';
                if ((num5 < 0) || (num5 > 9))
                {
                    return ParseResult.Invalid;
                }
                long num6 = (10L * value) - num5;
                if (num6 > value)
                {
                    i++;
                    while (i < num)
                    {
                        num5 = chars[i] - '0';
                        if ((num5 < 0) || (num5 > 9))
                        {
                            return ParseResult.Invalid;
                        }
                        i++;
                    }
                    return ParseResult.Overflow;
                }
                value = num6;
            }
            if (!flag)
            {
                if (value == -9223372036854775808L)
                {
                    return ParseResult.Overflow;
                }
                value = -value;
            }
            return ParseResult.Success;
        }

        public static bool IsConvertible(Type t) => 
            typeof(IConvertible).IsAssignableFrom(t);

        public static bool IsInteger(object value)
        {
            switch (GetTypeCode(value.GetType()))
            {
                case PrimitiveTypeCode.SByte:
                case PrimitiveTypeCode.Int16:
                case PrimitiveTypeCode.UInt16:
                case PrimitiveTypeCode.Int32:
                case PrimitiveTypeCode.Byte:
                case PrimitiveTypeCode.UInt32:
                case PrimitiveTypeCode.Int64:
                case PrimitiveTypeCode.UInt64:
                    return true;
            }
            return false;
        }

        public static TimeSpan ParseTimeSpan(string input) => 
            TimeSpan.Parse(input, CultureInfo.InvariantCulture);

        internal static BigInteger ToBigInteger(object value)
        {
            switch (value)
            {
                case (BigInteger _):
                    return (BigInteger) value;
                    break;
            }
            if (value is string)
            {
                return BigInteger.Parse((string) value, CultureInfo.InvariantCulture);
            }
            if (value is float)
            {
                return new BigInteger((float) value);
            }
            if (value is double)
            {
                return new BigInteger((double) value);
            }
            if (value is decimal)
            {
                return new BigInteger((decimal) value);
            }
            if (value is int)
            {
                return new BigInteger((int) value);
            }
            if (value is long)
            {
                return new BigInteger((long) value);
            }
            if (value is uint)
            {
                return new BigInteger((uint) value);
            }
            if (value is ulong)
            {
                return new BigInteger((ulong) value);
            }
            if (!(value is byte[]))
            {
                throw new InvalidCastException("Cannot convert {0} to BigInteger.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
            }
            return new BigInteger((byte[]) value);
        }

        public static object ToValue(INullable nullableValue)
        {
            switch (nullableValue)
            {
                case (null):
                    return null;
                    break;
            }
            if (nullableValue is SqlInt32)
            {
                return ToValue((SqlInt32) nullableValue);
            }
            if (nullableValue is SqlInt64)
            {
                return ToValue((SqlInt64) nullableValue);
            }
            if (nullableValue is SqlBoolean)
            {
                return ToValue((SqlBoolean) nullableValue);
            }
            if (nullableValue is SqlString)
            {
                return ToValue((SqlString) nullableValue);
            }
            if (!(nullableValue is SqlDateTime))
            {
                throw new ArgumentException("Unsupported INullable type: {0}".FormatWith(CultureInfo.InvariantCulture, nullableValue.GetType()));
            }
            return ToValue((SqlDateTime) nullableValue);
        }

        private static bool TryConvert(object initialValue, CultureInfo culture, Type targetType, out object value)
        {
            try
            {
                if (TryConvertInternal(initialValue, culture, targetType, out value) == ConvertResult.Success)
                {
                    return true;
                }
                value = null;
                return false;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        public static bool TryConvertGuid(string s, out Guid g) => 
            Guid.TryParseExact(s, "D", out g);

        private static ConvertResult TryConvertInternal(object initialValue, CultureInfo culture, Type targetType, out object value)
        {
            if (initialValue == null)
            {
                throw new ArgumentNullException("initialValue");
            }
            if (ReflectionUtils.IsNullableType(targetType))
            {
                targetType = Nullable.GetUnderlyingType(targetType);
            }
            Type t = initialValue.GetType();
            if (targetType == t)
            {
                value = initialValue;
                return ConvertResult.Success;
            }
            if (IsConvertible(initialValue.GetType()) && IsConvertible(targetType))
            {
                if (targetType.IsEnum())
                {
                    if (initialValue is string)
                    {
                        value = Enum.Parse(targetType, initialValue.ToString(), true);
                        return ConvertResult.Success;
                    }
                    if (IsInteger(initialValue))
                    {
                        value = Enum.ToObject(targetType, initialValue);
                        return ConvertResult.Success;
                    }
                }
                value = System.Convert.ChangeType(initialValue, targetType, culture);
                return ConvertResult.Success;
            }
            if ((initialValue is DateTime) && (targetType == typeof(DateTimeOffset)))
            {
                value = new DateTimeOffset((DateTime) initialValue);
                return ConvertResult.Success;
            }
            if ((initialValue is byte[]) && (targetType == typeof(Guid)))
            {
                value = new Guid((byte[]) initialValue);
                return ConvertResult.Success;
            }
            if ((initialValue is Guid) && (targetType == typeof(byte[])))
            {
                value = ((Guid) initialValue).ToByteArray();
                return ConvertResult.Success;
            }
            string g = initialValue as string;
            if (g != null)
            {
                if (targetType == typeof(Guid))
                {
                    value = new Guid(g);
                    return ConvertResult.Success;
                }
                if (targetType == typeof(Uri))
                {
                    value = new Uri(g, UriKind.RelativeOrAbsolute);
                    return ConvertResult.Success;
                }
                if (targetType == typeof(TimeSpan))
                {
                    value = ParseTimeSpan(g);
                    return ConvertResult.Success;
                }
                if (targetType == typeof(byte[]))
                {
                    value = System.Convert.FromBase64String(g);
                    return ConvertResult.Success;
                }
                if (targetType == typeof(Version))
                {
                    if (VersionTryParse(g, out Version version))
                    {
                        value = version;
                        return ConvertResult.Success;
                    }
                    value = null;
                    return ConvertResult.NoValidConversion;
                }
                if (typeof(Type).IsAssignableFrom(targetType))
                {
                    value = Type.GetType(g, true);
                    return ConvertResult.Success;
                }
            }
            if (targetType == typeof(BigInteger))
            {
                value = ToBigInteger(initialValue);
                return ConvertResult.Success;
            }
            if (initialValue is BigInteger)
            {
                value = FromBigInteger((BigInteger) initialValue, targetType);
                return ConvertResult.Success;
            }
            TypeConverter converter = GetConverter(t);
            if ((converter != null) && converter.CanConvertTo(targetType))
            {
                value = converter.ConvertTo(null, culture, initialValue, targetType);
                return ConvertResult.Success;
            }
            TypeConverter converter2 = GetConverter(targetType);
            if ((converter2 != null) && converter2.CanConvertFrom(t))
            {
                value = converter2.ConvertFrom(null, culture, initialValue);
                return ConvertResult.Success;
            }
            if (initialValue == DBNull.Value)
            {
                if (ReflectionUtils.IsNullable(targetType))
                {
                    value = EnsureTypeAssignable(null, t, targetType);
                    return ConvertResult.Success;
                }
                value = null;
                return ConvertResult.CannotConvertNull;
            }
            if (initialValue is INullable)
            {
                value = EnsureTypeAssignable(ToValue((INullable) initialValue), t, targetType);
                return ConvertResult.Success;
            }
            if ((targetType.IsInterface() || targetType.IsGenericTypeDefinition()) || targetType.IsAbstract())
            {
                value = null;
                return ConvertResult.NotInstantiableType;
            }
            value = null;
            return ConvertResult.NoValidConversion;
        }

        public static bool VersionTryParse(string input, out Version result) => 
            Version.TryParse(input, out result);

        internal enum ConvertResult
        {
            Success,
            CannotConvertNull,
            NotInstantiableType,
            NoValidConversion
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TypeConvertKey : IEquatable<ConvertUtils.TypeConvertKey>
        {
            private readonly Type _initialType;
            private readonly Type _targetType;
            public Type InitialType =>
                this._initialType;
            public Type TargetType =>
                this._targetType;
            public TypeConvertKey(Type initialType, Type targetType)
            {
                this._initialType = initialType;
                this._targetType = targetType;
            }

            public override int GetHashCode() => 
                (this._initialType.GetHashCode() ^ this._targetType.GetHashCode());

            public override bool Equals(object obj) => 
                ((obj is ConvertUtils.TypeConvertKey) && this.Equals((ConvertUtils.TypeConvertKey) obj));

            public bool Equals(ConvertUtils.TypeConvertKey other) => 
                ((this._initialType == other._initialType) && (this._targetType == other._targetType));
        }
    }
}

