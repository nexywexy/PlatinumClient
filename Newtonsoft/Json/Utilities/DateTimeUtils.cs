namespace Newtonsoft.Json.Utilities
{
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal static class DateTimeUtils
    {
        internal static readonly long InitialJavaScriptDateTicks = 0x89f7ff5f7b58000L;
        private const string IsoDateFormat = "yyyy-MM-ddTHH:mm:ss.FFFFFFFK";
        private const int DaysPer100Years = 0x8eac;
        private const int DaysPer400Years = 0x23ab1;
        private const int DaysPer4Years = 0x5b5;
        private const int DaysPerYear = 0x16d;
        private const long TicksPerDay = 0xc92a69c000L;
        private static readonly int[] DaysToMonth365 = new int[] { 0, 0x1f, 0x3b, 90, 120, 0x97, 0xb5, 0xd4, 0xf3, 0x111, 0x130, 0x14e, 0x16d };
        private static readonly int[] DaysToMonth366 = new int[] { 0, 0x1f, 60, 0x5b, 0x79, 0x98, 0xb6, 0xd5, 0xf4, 0x112, 0x131, 0x14f, 0x16e };

        internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime) => 
            ConvertDateTimeToJavaScriptTicks(dateTime, true);

        internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, bool convertToUtc) => 
            UniversialTicksToJavaScriptTicks(convertToUtc ? ToUniversalTicks(dateTime) : dateTime.Ticks);

        internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, TimeSpan offset) => 
            UniversialTicksToJavaScriptTicks(ToUniversalTicks(dateTime, offset));

        internal static DateTime ConvertJavaScriptTicksToDateTime(long javaScriptTicks) => 
            new DateTime((javaScriptTicks * 0x2710L) + InitialJavaScriptDateTicks, DateTimeKind.Utc);

        private static void CopyIntToCharArray(char[] chars, int start, int value, int digits)
        {
            while (digits-- != 0)
            {
                chars[start + digits] = (char) ((value % 10) + 0x30);
                value /= 10;
            }
        }

        private static DateTime CreateDateTime(DateTimeParser dateTimeParser)
        {
            bool flag;
            if (dateTimeParser.Hour == 0x18)
            {
                flag = true;
                dateTimeParser.Hour = 0;
            }
            else
            {
                flag = false;
            }
            DateTime time = new DateTime(dateTimeParser.Year, dateTimeParser.Month, dateTimeParser.Day, dateTimeParser.Hour, dateTimeParser.Minute, dateTimeParser.Second);
            time = time.AddTicks((long) dateTimeParser.Fraction);
            if (flag)
            {
                time = time.AddDays(1.0);
            }
            return time;
        }

        internal static DateTime EnsureDateTime(DateTime value, DateTimeZoneHandling timeZone)
        {
            switch (timeZone)
            {
                case DateTimeZoneHandling.Local:
                    value = SwitchToLocalTime(value);
                    return value;

                case DateTimeZoneHandling.Utc:
                    value = SwitchToUtcTime(value);
                    return value;

                case DateTimeZoneHandling.Unspecified:
                    value = new DateTime(value.Ticks, DateTimeKind.Unspecified);
                    return value;

                case DateTimeZoneHandling.RoundtripKind:
                    return value;
            }
            throw new ArgumentException("Invalid date time handling value.");
        }

        private static void GetDateValues(DateTime td, out int year, out int month, out int day)
        {
            int num = (int) (td.Ticks / 0xc92a69c000L);
            int num2 = num / 0x23ab1;
            num -= num2 * 0x23ab1;
            int num3 = num / 0x8eac;
            if (num3 == 4)
            {
                num3 = 3;
            }
            num -= num3 * 0x8eac;
            int num4 = num / 0x5b5;
            num -= num4 * 0x5b5;
            int num5 = num / 0x16d;
            if (num5 == 4)
            {
                num5 = 3;
            }
            year = ((((num2 * 400) + (num3 * 100)) + (num4 * 4)) + num5) + 1;
            num -= num5 * 0x16d;
            int[] numArray = ((num5 == 3) && ((num4 != 0x18) || (num3 == 3))) ? DaysToMonth366 : DaysToMonth365;
            int index = num >> 6;
            while (num >= numArray[index])
            {
                index++;
            }
            month = index;
            day = (num - numArray[index - 1]) + 1;
        }

        public static TimeSpan GetUtcOffset(this DateTime d) => 
            TimeZoneInfo.Local.GetUtcOffset(d);

        private static DateTime SwitchToLocalTime(DateTime value)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Unspecified:
                    return new DateTime(value.Ticks, DateTimeKind.Local);

                case DateTimeKind.Utc:
                    return value.ToLocalTime();

                case DateTimeKind.Local:
                    return value;
            }
            return value;
        }

        private static DateTime SwitchToUtcTime(DateTime value)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Unspecified:
                    return new DateTime(value.Ticks, DateTimeKind.Utc);

                case DateTimeKind.Utc:
                    return value;

                case DateTimeKind.Local:
                    return value.ToUniversalTime();
            }
            return value;
        }

        public static XmlDateTimeSerializationMode ToSerializationMode(DateTimeKind kind)
        {
            switch (kind)
            {
                case DateTimeKind.Unspecified:
                    return XmlDateTimeSerializationMode.Unspecified;

                case DateTimeKind.Utc:
                    return XmlDateTimeSerializationMode.Utc;

                case DateTimeKind.Local:
                    return XmlDateTimeSerializationMode.Local;
            }
            throw MiscellaneousUtils.CreateArgumentOutOfRangeException("kind", kind, "Unexpected DateTimeKind value.");
        }

        private static long ToUniversalTicks(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return dateTime.Ticks;
            }
            return ToUniversalTicks(dateTime, dateTime.GetUtcOffset());
        }

        private static long ToUniversalTicks(DateTime dateTime, TimeSpan offset)
        {
            if (((dateTime.Kind == DateTimeKind.Utc) || (dateTime == DateTime.MaxValue)) || (dateTime == DateTime.MinValue))
            {
                return dateTime.Ticks;
            }
            long num = dateTime.Ticks - offset.Ticks;
            if (num > 0x2bca2875f4373fffL)
            {
                return 0x2bca2875f4373fffL;
            }
            if (num < 0L)
            {
                return 0L;
            }
            return num;
        }

        internal static bool TryParseDateTime(StringReference s, DateTimeZoneHandling dateTimeZoneHandling, string dateFormatString, CultureInfo culture, out DateTime dt)
        {
            if (s.Length > 0)
            {
                int startIndex = s.StartIndex;
                if (s[startIndex] == '/')
                {
                    if (((s.Length >= 9) && s.StartsWith("/Date(")) && (s.EndsWith(")/") && TryParseDateTimeMicrosoft(s, dateTimeZoneHandling, out dt)))
                    {
                        return true;
                    }
                }
                else if ((((s.Length >= 0x13) && (s.Length <= 40)) && (char.IsDigit(s[startIndex]) && (s[startIndex + 10] == 'T'))) && TryParseDateTimeIso(s, dateTimeZoneHandling, out dt))
                {
                    return true;
                }
                if (!string.IsNullOrEmpty(dateFormatString) && TryParseDateTimeExact(s.ToString(), dateTimeZoneHandling, dateFormatString, culture, out dt))
                {
                    return true;
                }
            }
            dt = new DateTime();
            return false;
        }

        internal static bool TryParseDateTime(string s, DateTimeZoneHandling dateTimeZoneHandling, string dateFormatString, CultureInfo culture, out DateTime dt)
        {
            if (s.Length > 0)
            {
                if (s[0] == '/')
                {
                    if (((s.Length >= 9) && s.StartsWith("/Date(", StringComparison.Ordinal)) && (s.EndsWith(")/", StringComparison.Ordinal) && TryParseDateTimeMicrosoft(new StringReference(s.ToCharArray(), 0, s.Length), dateTimeZoneHandling, out dt)))
                    {
                        return true;
                    }
                }
                else if ((((s.Length >= 0x13) && (s.Length <= 40)) && (char.IsDigit(s[0]) && (s[10] == 'T'))) && DateTime.TryParseExact(s, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt))
                {
                    dt = EnsureDateTime(dt, dateTimeZoneHandling);
                    return true;
                }
                if (!string.IsNullOrEmpty(dateFormatString) && TryParseDateTimeExact(s, dateTimeZoneHandling, dateFormatString, culture, out dt))
                {
                    return true;
                }
            }
            dt = new DateTime();
            return false;
        }

        private static bool TryParseDateTimeExact(string text, DateTimeZoneHandling dateTimeZoneHandling, string dateFormatString, CultureInfo culture, out DateTime dt)
        {
            if (DateTime.TryParseExact(text, dateFormatString, culture, DateTimeStyles.RoundtripKind, out DateTime time))
            {
                time = EnsureDateTime(time, dateTimeZoneHandling);
                dt = time;
                return true;
            }
            dt = new DateTime();
            return false;
        }

        internal static bool TryParseDateTimeIso(StringReference text, DateTimeZoneHandling dateTimeZoneHandling, out DateTime dt)
        {
            long ticks;
            DateTimeParser dateTimeParser = new DateTimeParser();
            if (!dateTimeParser.Parse(text.Chars, text.StartIndex, text.Length))
            {
                dt = new DateTime();
                return false;
            }
            DateTime d = CreateDateTime(dateTimeParser);
            switch (dateTimeParser.Zone)
            {
                case ParserTimeZone.Utc:
                    d = new DateTime(d.Ticks, DateTimeKind.Utc);
                    break;

                case ParserTimeZone.LocalWestOfUtc:
                {
                    TimeSpan span = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
                    ticks = d.Ticks + span.Ticks;
                    if (ticks > DateTime.MaxValue.Ticks)
                    {
                        ticks += d.GetUtcOffset().Ticks;
                        if (ticks > DateTime.MaxValue.Ticks)
                        {
                            ticks = DateTime.MaxValue.Ticks;
                        }
                        d = new DateTime(ticks, DateTimeKind.Local);
                        break;
                    }
                    d = new DateTime(ticks, DateTimeKind.Utc).ToLocalTime();
                    break;
                }
                case ParserTimeZone.LocalEastOfUtc:
                {
                    TimeSpan span3 = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
                    ticks = d.Ticks - span3.Ticks;
                    if (ticks < DateTime.MinValue.Ticks)
                    {
                        ticks += d.GetUtcOffset().Ticks;
                        if (ticks < DateTime.MinValue.Ticks)
                        {
                            ticks = DateTime.MinValue.Ticks;
                        }
                        d = new DateTime(ticks, DateTimeKind.Local);
                        break;
                    }
                    d = new DateTime(ticks, DateTimeKind.Utc).ToLocalTime();
                    break;
                }
            }
            dt = EnsureDateTime(d, dateTimeZoneHandling);
            return true;
        }

        private static bool TryParseDateTimeMicrosoft(StringReference text, DateTimeZoneHandling dateTimeZoneHandling, out DateTime dt)
        {
            if (!TryParseMicrosoftDate(text, out long num, out _, out DateTimeKind kind))
            {
                dt = new DateTime();
                return false;
            }
            DateTime time = ConvertJavaScriptTicksToDateTime(num);
            if (kind == DateTimeKind.Unspecified)
            {
                dt = DateTime.SpecifyKind(time.ToLocalTime(), DateTimeKind.Unspecified);
            }
            else if (kind == DateTimeKind.Local)
            {
                dt = time.ToLocalTime();
            }
            else
            {
                dt = time;
            }
            dt = EnsureDateTime(dt, dateTimeZoneHandling);
            return true;
        }

        internal static bool TryParseDateTimeOffset(StringReference s, string dateFormatString, CultureInfo culture, out DateTimeOffset dt)
        {
            if (s.Length > 0)
            {
                int startIndex = s.StartIndex;
                if (s[startIndex] == '/')
                {
                    if (((s.Length >= 9) && s.StartsWith("/Date(")) && (s.EndsWith(")/") && TryParseDateTimeOffsetMicrosoft(s, out dt)))
                    {
                        return true;
                    }
                }
                else if ((((s.Length >= 0x13) && (s.Length <= 40)) && (char.IsDigit(s[startIndex]) && (s[startIndex + 10] == 'T'))) && TryParseDateTimeOffsetIso(s, out dt))
                {
                    return true;
                }
                if (!string.IsNullOrEmpty(dateFormatString) && TryParseDateTimeOffsetExact(s.ToString(), dateFormatString, culture, out dt))
                {
                    return true;
                }
            }
            dt = new DateTimeOffset();
            return false;
        }

        internal static bool TryParseDateTimeOffset(string s, string dateFormatString, CultureInfo culture, out DateTimeOffset dt)
        {
            if (s.Length > 0)
            {
                if (s[0] == '/')
                {
                    if (((s.Length >= 9) && s.StartsWith("/Date(", StringComparison.Ordinal)) && (s.EndsWith(")/", StringComparison.Ordinal) && TryParseDateTimeOffsetMicrosoft(new StringReference(s.ToCharArray(), 0, s.Length), out dt)))
                    {
                        return true;
                    }
                }
                else if ((((s.Length >= 0x13) && (s.Length <= 40)) && (char.IsDigit(s[0]) && (s[10] == 'T'))) && (DateTimeOffset.TryParseExact(s, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt) && TryParseDateTimeOffsetIso(new StringReference(s.ToCharArray(), 0, s.Length), out dt)))
                {
                    return true;
                }
                if (!string.IsNullOrEmpty(dateFormatString) && TryParseDateTimeOffsetExact(s, dateFormatString, culture, out dt))
                {
                    return true;
                }
            }
            dt = new DateTimeOffset();
            return false;
        }

        private static bool TryParseDateTimeOffsetExact(string text, string dateFormatString, CultureInfo culture, out DateTimeOffset dt)
        {
            if (DateTimeOffset.TryParseExact(text, dateFormatString, culture, DateTimeStyles.RoundtripKind, out DateTimeOffset offset))
            {
                dt = offset;
                return true;
            }
            dt = new DateTimeOffset();
            return false;
        }

        internal static bool TryParseDateTimeOffsetIso(StringReference text, out DateTimeOffset dt)
        {
            TimeSpan utcOffset;
            DateTimeParser dateTimeParser = new DateTimeParser();
            if (!dateTimeParser.Parse(text.Chars, text.StartIndex, text.Length))
            {
                dt = new DateTimeOffset();
                return false;
            }
            DateTime dateTime = CreateDateTime(dateTimeParser);
            switch (dateTimeParser.Zone)
            {
                case ParserTimeZone.Utc:
                    utcOffset = new TimeSpan(0L);
                    break;

                case ParserTimeZone.LocalWestOfUtc:
                    utcOffset = new TimeSpan(-dateTimeParser.ZoneHour, -dateTimeParser.ZoneMinute, 0);
                    break;

                case ParserTimeZone.LocalEastOfUtc:
                    utcOffset = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
                    break;

                default:
                    utcOffset = TimeZoneInfo.Local.GetUtcOffset(dateTime);
                    break;
            }
            long num = dateTime.Ticks - utcOffset.Ticks;
            if ((num < 0L) || (num > 0x2bca2875f4373fffL))
            {
                dt = new DateTimeOffset();
                return false;
            }
            dt = new DateTimeOffset(dateTime, utcOffset);
            return true;
        }

        private static bool TryParseDateTimeOffsetMicrosoft(StringReference text, out DateTimeOffset dt)
        {
            if (!TryParseMicrosoftDate(text, out long num, out TimeSpan span, out _))
            {
                dt = new DateTime();
                return false;
            }
            DateTime time2 = ConvertJavaScriptTicksToDateTime(num);
            dt = new DateTimeOffset(time2.Add(span).Ticks, span);
            return true;
        }

        private static bool TryParseMicrosoftDate(StringReference text, out long ticks, out TimeSpan offset, out DateTimeKind kind)
        {
            kind = DateTimeKind.Utc;
            int num = text.IndexOf('+', 7, text.Length - 8);
            if (num == -1)
            {
                num = text.IndexOf('-', 7, text.Length - 8);
            }
            if (num != -1)
            {
                kind = DateTimeKind.Local;
                if (!TryReadOffset(text, num + text.StartIndex, out offset))
                {
                    ticks = 0L;
                    return false;
                }
            }
            else
            {
                offset = TimeSpan.Zero;
                num = text.Length - 2;
            }
            return (ConvertUtils.Int64TryParse(text.Chars, 6 + text.StartIndex, num - 6, out ticks) == ParseResult.Success);
        }

        private static bool TryReadOffset(StringReference offsetText, int startIndex, out TimeSpan offset)
        {
            bool flag = offsetText[startIndex] == '-';
            if (ConvertUtils.Int32TryParse(offsetText.Chars, startIndex + 1, 2, out int num) != ParseResult.Success)
            {
                offset = new TimeSpan();
                return false;
            }
            int num2 = 0;
            if (((offsetText.Length - startIndex) > 5) && (ConvertUtils.Int32TryParse(offsetText.Chars, startIndex + 3, 2, out num2) != ParseResult.Success))
            {
                offset = new TimeSpan();
                return false;
            }
            offset = TimeSpan.FromHours((double) num) + TimeSpan.FromMinutes((double) num2);
            if (flag)
            {
                offset = offset.Negate();
            }
            return true;
        }

        private static long UniversialTicksToJavaScriptTicks(long universialTicks) => 
            ((universialTicks - InitialJavaScriptDateTicks) / 0x2710L);

        internal static int WriteDateTimeOffset(char[] chars, int start, TimeSpan offset, DateFormatHandling format)
        {
            chars[start++] = (offset.Ticks >= 0L) ? '+' : '-';
            int num = Math.Abs(offset.Hours);
            CopyIntToCharArray(chars, start, num, 2);
            start += 2;
            if (format == DateFormatHandling.IsoDateFormat)
            {
                chars[start++] = ':';
            }
            int num2 = Math.Abs(offset.Minutes);
            CopyIntToCharArray(chars, start, num2, 2);
            start += 2;
            return start;
        }

        internal static void WriteDateTimeOffsetString(TextWriter writer, DateTimeOffset value, DateFormatHandling format, string formatString, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(formatString))
            {
                char[] chars = new char[0x40];
                int count = WriteDateTimeString(chars, 0, (format == DateFormatHandling.IsoDateFormat) ? value.DateTime : value.UtcDateTime, new TimeSpan?(value.Offset), DateTimeKind.Local, format);
                writer.Write(chars, 0, count);
            }
            else
            {
                writer.Write(value.ToString(formatString, culture));
            }
        }

        internal static void WriteDateTimeString(TextWriter writer, DateTime value, DateFormatHandling format, string formatString, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(formatString))
            {
                char[] chars = new char[0x40];
                int count = WriteDateTimeString(chars, 0, value, null, value.Kind, format);
                writer.Write(chars, 0, count);
            }
            else
            {
                writer.Write(value.ToString(formatString, culture));
            }
        }

        internal static int WriteDateTimeString(char[] chars, int start, DateTime value, TimeSpan? offset, DateTimeKind kind, DateFormatHandling format)
        {
            TimeSpan? nullable;
            int num = start;
            if (format != DateFormatHandling.MicrosoftDateFormat)
            {
                num = WriteDefaultIsoDate(chars, num, value);
                if (kind != DateTimeKind.Utc)
                {
                    if (kind == DateTimeKind.Local)
                    {
                        nullable = offset;
                        num = WriteDateTimeOffset(chars, num, nullable.HasValue ? nullable.GetValueOrDefault() : value.GetUtcOffset(), format);
                    }
                    return num;
                }
                chars[num++] = 'Z';
                return num;
            }
            nullable = offset;
            TimeSpan span = nullable.HasValue ? nullable.GetValueOrDefault() : value.GetUtcOffset();
            long num2 = ConvertDateTimeToJavaScriptTicks(value, span);
            @"\/Date(".CopyTo(0, chars, num, 7);
            num += 7;
            string str = num2.ToString(CultureInfo.InvariantCulture);
            str.CopyTo(0, chars, num, str.Length);
            num += str.Length;
            if (kind != DateTimeKind.Unspecified)
            {
                if (kind == DateTimeKind.Local)
                {
                    num = WriteDateTimeOffset(chars, num, span, format);
                }
            }
            else if ((value != DateTime.MaxValue) && (value != DateTime.MinValue))
            {
                num = WriteDateTimeOffset(chars, num, span, format);
            }
            @")\/".CopyTo(0, chars, num, 3);
            return (num + 3);
        }

        internal static int WriteDefaultIsoDate(char[] chars, int start, DateTime dt)
        {
            int num = 0x13;
            GetDateValues(dt, out int num2, out int num3, out int num4);
            CopyIntToCharArray(chars, start, num2, 4);
            chars[start + 4] = '-';
            CopyIntToCharArray(chars, start + 5, num3, 2);
            chars[start + 7] = '-';
            CopyIntToCharArray(chars, start + 8, num4, 2);
            chars[start + 10] = 'T';
            CopyIntToCharArray(chars, start + 11, dt.Hour, 2);
            chars[start + 13] = ':';
            CopyIntToCharArray(chars, start + 14, dt.Minute, 2);
            chars[start + 0x10] = ':';
            CopyIntToCharArray(chars, start + 0x11, dt.Second, 2);
            int num5 = (int) (dt.Ticks % 0x989680L);
            if (num5 != 0)
            {
                int digits = 7;
                while ((num5 % 10) == 0)
                {
                    digits--;
                    num5 /= 10;
                }
                chars[start + 0x13] = '.';
                CopyIntToCharArray(chars, start + 20, num5, digits);
                num += digits + 1;
            }
            return (start + num);
        }
    }
}

