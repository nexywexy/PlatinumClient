namespace Newtonsoft.Json.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    internal static class EnumUtils
    {
        private static readonly ThreadSafeStore<Type, BidirectionalDictionary<string, string>> EnumMemberNamesPerType = new ThreadSafeStore<Type, BidirectionalDictionary<string, string>>(new Func<Type, BidirectionalDictionary<string, string>>(EnumUtils.InitializeEnumType));

        public static IList<T> GetFlagsValues<T>(T value) where T: struct
        {
            Type type = typeof(T);
            if (!type.IsDefined(typeof(FlagsAttribute), false))
            {
                throw new ArgumentException("Enum type {0} is not a set of flags.".FormatWith(CultureInfo.InvariantCulture, type));
            }
            Type underlyingType = Enum.GetUnderlyingType(value.GetType());
            ulong num = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            IList<EnumValue<ulong>> namesAndValues = GetNamesAndValues<T>();
            IList<T> list2 = new List<T>();
            foreach (EnumValue<ulong> value2 in namesAndValues)
            {
                if (((num & value2.Value) == value2.Value) && (value2.Value != 0))
                {
                    list2.Add((T) Convert.ChangeType(value2.Value, underlyingType, CultureInfo.CurrentCulture));
                }
            }
            if (list2.Count == 0)
            {
                if (<>c__2<T>.<>9__2_0 == null)
                {
                }
                if (namesAndValues.SingleOrDefault<EnumValue<ulong>>((<>c__2<T>.<>9__2_0 = new Func<EnumValue<ulong>, bool>(<>c__2<T>.<>9.<GetFlagsValues>b__2_0))) != null)
                {
                    list2.Add(default(T));
                }
            }
            return list2;
        }

        public static IList<string> GetNames(Type enumType)
        {
            if (!enumType.IsEnum())
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum.");
            }
            List<string> list = new List<string>();
            if (<>c.<>9__6_0 == null)
            {
            }
            foreach (FieldInfo info in enumType.GetFields().Where<FieldInfo>(<>c.<>9__6_0 = new Func<FieldInfo, bool>(<>c.<>9.<GetNames>b__6_0)))
            {
                list.Add(info.Name);
            }
            return list;
        }

        public static IList<EnumValue<ulong>> GetNamesAndValues<T>() where T: struct => 
            GetNamesAndValues<ulong>(typeof(T));

        public static IList<EnumValue<TUnderlyingType>> GetNamesAndValues<TUnderlyingType>(Type enumType) where TUnderlyingType: struct
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }
            if (!enumType.IsEnum())
            {
                throw new ArgumentException("Type {0} is not an Enum.".FormatWith(CultureInfo.InvariantCulture, enumType), "enumType");
            }
            IList<object> values = GetValues(enumType);
            IList<string> names = GetNames(enumType);
            IList<EnumValue<TUnderlyingType>> list3 = new List<EnumValue<TUnderlyingType>>();
            for (int i = 0; i < values.Count; i++)
            {
                try
                {
                    list3.Add(new EnumValue<TUnderlyingType>(names[i], (TUnderlyingType) Convert.ChangeType(values[i], typeof(TUnderlyingType), CultureInfo.CurrentCulture)));
                }
                catch (OverflowException exception)
                {
                    object[] args = new object[] { Enum.GetUnderlyingType(enumType), typeof(TUnderlyingType), Convert.ToUInt64(values[i], CultureInfo.InvariantCulture) };
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Value from enum with the underlying type of {0} cannot be added to dictionary with a value type of {1}. Value was too large: {2}", args), exception);
                }
            }
            return list3;
        }

        public static IList<object> GetValues(Type enumType)
        {
            if (!enumType.IsEnum())
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum.");
            }
            List<object> list = new List<object>();
            if (<>c.<>9__5_0 == null)
            {
            }
            using (IEnumerator<FieldInfo> enumerator = enumType.GetFields().Where<FieldInfo>((<>c.<>9__5_0 = new Func<FieldInfo, bool>(<>c.<>9.<GetValues>b__5_0))).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    object item = enumerator.Current.GetValue(enumType);
                    list.Add(item);
                }
            }
            return list;
        }

        private static BidirectionalDictionary<string, string> InitializeEnumType(Type type)
        {
            BidirectionalDictionary<string, string> dictionary = new BidirectionalDictionary<string, string>(StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);
            foreach (FieldInfo info in type.GetFields())
            {
                string str2;
                string name = info.Name;
                if (<>c.<>9__1_0 == null)
                {
                }
                string local1 = info.GetCustomAttributes(typeof(EnumMemberAttribute), true).Cast<EnumMemberAttribute>().Select<EnumMemberAttribute, string>((<>c.<>9__1_0 = new Func<EnumMemberAttribute, string>(<>c.<>9.<InitializeEnumType>b__1_0))).SingleOrDefault<string>();
                if (local1 != null)
                {
                    str2 = local1;
                }
                else
                {
                    str2 = info.Name;
                }
                if (dictionary.TryGetBySecond(str2, out _))
                {
                    throw new InvalidOperationException("Enum name '{0}' already exists on enum '{1}'.".FormatWith(CultureInfo.InvariantCulture, str2, type.Name));
                }
                dictionary.Set(name, str2);
            }
            return dictionary;
        }

        public static object ParseEnumName(string enumText, bool isNullable, Type t)
        {
            string str2;
            if ((enumText == string.Empty) & isNullable)
            {
                return null;
            }
            BidirectionalDictionary<string, string> map = EnumMemberNamesPerType.Get(t);
            if (enumText.IndexOf(',') != -1)
            {
                char[] separator = new char[] { ',' };
                string[] strArray = enumText.Split(separator);
                for (int i = 0; i < strArray.Length; i++)
                {
                    string str = strArray[i].Trim();
                    strArray[i] = ResolvedEnumName(map, str);
                }
                str2 = string.Join(", ", strArray);
            }
            else
            {
                str2 = ResolvedEnumName(map, enumText);
            }
            return Enum.Parse(t, str2, true);
        }

        private static string ResolvedEnumName(BidirectionalDictionary<string, string> map, string enumText)
        {
            map.TryGetBySecond(enumText, out string str);
            if (str != null)
            {
                return str;
            }
            return enumText;
        }

        public static string ToEnumName(Type enumType, string enumText, bool camelCaseText)
        {
            BidirectionalDictionary<string, string> dictionary = EnumMemberNamesPerType.Get(enumType);
            char[] separator = new char[] { ',' };
            string[] strArray = enumText.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                string first = strArray[i].Trim();
                dictionary.TryGetByFirst(first, out string str2);
                if (str2 != null)
                {
                    str2 = str2;
                }
                else
                {
                    str2 = first;
                }
                if (camelCaseText)
                {
                    str2 = StringUtils.ToCamelCase(str2);
                }
                strArray[i] = str2;
            }
            return string.Join(", ", strArray);
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly EnumUtils.<>c <>9 = new EnumUtils.<>c();
            public static Func<EnumMemberAttribute, string> <>9__1_0;
            public static Func<FieldInfo, bool> <>9__5_0;
            public static Func<FieldInfo, bool> <>9__6_0;

            internal bool <GetNames>b__6_0(FieldInfo f) => 
                f.IsLiteral;

            internal bool <GetValues>b__5_0(FieldInfo f) => 
                f.IsLiteral;

            internal string <InitializeEnumType>b__1_0(EnumMemberAttribute a) => 
                a.Value;
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__2<T> where T: struct
        {
            public static readonly EnumUtils.<>c__2<T> <>9;
            public static Func<EnumValue<ulong>, bool> <>9__2_0;

            static <>c__2()
            {
                EnumUtils.<>c__2<T>.<>9 = new EnumUtils.<>c__2<T>();
            }

            internal bool <GetFlagsValues>b__2_0(EnumValue<ulong> v) => 
                (v.Value == 0L);
        }
    }
}

