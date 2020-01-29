namespace Newtonsoft.Json.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal static class StringUtils
    {
        public const string CarriageReturnLineFeed = "\r\n";
        public const string Empty = "";
        public const char CarriageReturn = '\r';
        public const char LineFeed = '\n';
        public const char Tab = '\t';

        public static StringWriter CreateStringWriter(int capacity) => 
            new StringWriter(new StringBuilder(capacity), CultureInfo.InvariantCulture);

        public static bool EndsWith(this string source, char value) => 
            ((source.Length > 0) && (source[source.Length - 1] == value));

        public static TSource ForgivingCaseSensitiveFind<TSource>(this IEnumerable<TSource> source, Func<TSource, string> valueSelector, string testValue)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (valueSelector == null)
            {
                throw new ArgumentNullException("valueSelector");
            }
            IEnumerable<TSource> enumerable = from s in source
                where string.Equals(valueSelector(s), testValue, StringComparison.OrdinalIgnoreCase)
                select s;
            if (enumerable.Count<TSource>() <= 1)
            {
                return enumerable.SingleOrDefault<TSource>();
            }
            return (from s in source
                where string.Equals(valueSelector(s), testValue, StringComparison.Ordinal)
                select s).SingleOrDefault<TSource>();
        }

        public static string FormatWith(this string format, IFormatProvider provider, object arg0)
        {
            object[] args = new object[] { arg0 };
            return format.FormatWith(provider, args);
        }

        private static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            ValidationUtils.ArgumentNotNull(format, "format");
            return string.Format(provider, format, args);
        }

        public static string FormatWith(this string format, IFormatProvider provider, object arg0, object arg1)
        {
            object[] args = new object[] { arg0, arg1 };
            return format.FormatWith(provider, args);
        }

        public static string FormatWith(this string format, IFormatProvider provider, object arg0, object arg1, object arg2)
        {
            object[] args = new object[] { arg0, arg1, arg2 };
            return format.FormatWith(provider, args);
        }

        public static string FormatWith(this string format, IFormatProvider provider, object arg0, object arg1, object arg2, object arg3)
        {
            object[] args = new object[] { arg0, arg1, arg2, arg3 };
            return format.FormatWith(provider, args);
        }

        public static bool IsHighSurrogate(char c) => 
            char.IsHighSurrogate(c);

        public static bool IsLowSurrogate(char c) => 
            char.IsLowSurrogate(c);

        public static bool IsWhiteSpace(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            if (s.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsWhiteSpace(s[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool StartsWith(this string source, char value) => 
            ((source.Length > 0) && (source[0] == value));

        public static string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
            {
                return s;
            }
            char[] chArray = s.ToCharArray();
            for (int i = 0; i < chArray.Length; i++)
            {
                if ((i == 1) && !char.IsUpper(chArray[i]))
                {
                    break;
                }
                bool flag = (i + 1) < chArray.Length;
                if (((i > 0) & flag) && !char.IsUpper(chArray[i + 1]))
                {
                    break;
                }
                chArray[i] = char.ToLower(chArray[i], CultureInfo.InvariantCulture);
            }
            return new string(chArray);
        }

        public static void ToCharAsUnicode(char c, char[] buffer)
        {
            buffer[0] = '\\';
            buffer[1] = 'u';
            buffer[2] = MathUtils.IntToHex((c >> 12) & '\x000f');
            buffer[3] = MathUtils.IntToHex((c >> 8) & '\x000f');
            buffer[4] = MathUtils.IntToHex((c >> 4) & '\x000f');
            buffer[5] = MathUtils.IntToHex(c & '\x000f');
        }

        public static string ToSnakeCase(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            StringBuilder builder = new StringBuilder();
            SnakeCaseState start = SnakeCaseState.Start;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ')
                {
                    if (start != SnakeCaseState.Start)
                    {
                        start = SnakeCaseState.NewWord;
                    }
                    continue;
                }
                if (char.IsUpper(s[i]))
                {
                    switch (start)
                    {
                        case SnakeCaseState.Lower:
                        case SnakeCaseState.NewWord:
                            builder.Append('_');
                            break;

                        case SnakeCaseState.Upper:
                        {
                            bool flag = (i + 1) < s.Length;
                            if ((i > 0) & flag)
                            {
                                char c = s[i + 1];
                                if (!char.IsUpper(c) && (c != '_'))
                                {
                                    builder.Append('_');
                                }
                            }
                            break;
                        }
                    }
                    char ch2 = char.ToLower(s[i], CultureInfo.InvariantCulture);
                    builder.Append(ch2);
                    start = SnakeCaseState.Upper;
                    continue;
                }
                if (s[i] == '_')
                {
                    builder.Append('_');
                    start = SnakeCaseState.Start;
                }
                else
                {
                    if (start == SnakeCaseState.NewWord)
                    {
                        builder.Append('_');
                    }
                    builder.Append(s[i]);
                    start = SnakeCaseState.Lower;
                }
            }
            return builder.ToString();
        }

        internal enum SnakeCaseState
        {
            Start,
            Lower,
            Upper,
            NewWord
        }
    }
}

