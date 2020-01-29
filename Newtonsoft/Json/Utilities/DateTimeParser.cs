namespace Newtonsoft.Json.Utilities
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct DateTimeParser
    {
        public int Year;
        public int Month;
        public int Day;
        public int Hour;
        public int Minute;
        public int Second;
        public int Fraction;
        public int ZoneHour;
        public int ZoneMinute;
        public ParserTimeZone Zone;
        private char[] _text;
        private int _end;
        private static readonly int[] Power10;
        private static readonly int Lzyyyy;
        private static readonly int Lzyyyy_;
        private static readonly int Lzyyyy_MM;
        private static readonly int Lzyyyy_MM_;
        private static readonly int Lzyyyy_MM_dd;
        private static readonly int Lzyyyy_MM_ddT;
        private static readonly int LzHH;
        private static readonly int LzHH_;
        private static readonly int LzHH_mm;
        private static readonly int LzHH_mm_;
        private static readonly int LzHH_mm_ss;
        private static readonly int Lz_;
        private static readonly int Lz_zz;
        private const short MaxFractionDigits = 7;
        static DateTimeParser()
        {
            Power10 = new int[] { -1, 10, 100, 0x3e8, 0x2710, 0x186a0, 0xf4240 };
            Lzyyyy = "yyyy".Length;
            Lzyyyy_ = "yyyy-".Length;
            Lzyyyy_MM = "yyyy-MM".Length;
            Lzyyyy_MM_ = "yyyy-MM-".Length;
            Lzyyyy_MM_dd = "yyyy-MM-dd".Length;
            Lzyyyy_MM_ddT = "yyyy-MM-ddT".Length;
            LzHH = "HH".Length;
            LzHH_ = "HH:".Length;
            LzHH_mm = "HH:mm".Length;
            LzHH_mm_ = "HH:mm:".Length;
            LzHH_mm_ss = "HH:mm:ss".Length;
            Lz_ = "-".Length;
            Lz_zz = "-zz".Length;
        }

        public bool Parse(char[] text, int startIndex, int length)
        {
            this._text = text;
            this._end = startIndex + length;
            return ((this.ParseDate(startIndex) && this.ParseChar(Lzyyyy_MM_dd + startIndex, 'T')) && this.ParseTimeAndZoneAndWhitespace(Lzyyyy_MM_ddT + startIndex));
        }

        private bool ParseDate(int start) => 
            (((((this.Parse4Digit(start, out this.Year) && (1 <= this.Year)) && (this.ParseChar(start + Lzyyyy, '-') && this.Parse2Digit(start + Lzyyyy_, out this.Month))) && (((1 <= this.Month) && (this.Month <= 12)) && (this.ParseChar(start + Lzyyyy_MM, '-') && this.Parse2Digit(start + Lzyyyy_MM_, out this.Day)))) && (1 <= this.Day)) && (this.Day <= DateTime.DaysInMonth(this.Year, this.Month)));

        private bool ParseTimeAndZoneAndWhitespace(int start) => 
            (this.ParseTime(ref start) && this.ParseZone(start));

        private bool ParseTime(ref int start)
        {
            int num3;
            if (((!this.Parse2Digit(start, out this.Hour) || (this.Hour > 0x18)) || (!this.ParseChar(start + LzHH, ':') || !this.Parse2Digit(start + LzHH_, out this.Minute))) || ((((this.Minute >= 60) || !this.ParseChar(start + LzHH_mm, ':')) || (!this.Parse2Digit(start + LzHH_mm_, out this.Second) || (this.Second >= 60))) || ((this.Hour == 0x18) && ((this.Minute != 0) || (this.Second != 0)))))
            {
                return false;
            }
            start += LzHH_mm_ss;
            if (!this.ParseChar(start, '.'))
            {
                goto Label_016A;
            }
            this.Fraction = 0;
            int num = 0;
        Label_0113:
            num3 = start + 1;
            start = num3;
            if ((num3 < this._end) && (num < 7))
            {
                int num2 = this._text[start] - '0';
                if ((num2 >= 0) && (num2 <= 9))
                {
                    this.Fraction = (this.Fraction * 10) + num2;
                    num++;
                    goto Label_0113;
                }
            }
            if (num < 7)
            {
                if (num == 0)
                {
                    return false;
                }
                this.Fraction *= Power10[7 - num];
            }
            if ((this.Hour == 0x18) && (this.Fraction != 0))
            {
                return false;
            }
        Label_016A:
            return true;
        }

        private bool ParseZone(int start)
        {
            if (start < this._end)
            {
                char ch = this._text[start];
                switch (ch)
                {
                    case 'Z':
                    case 'z':
                        this.Zone = ParserTimeZone.Utc;
                        start++;
                        goto Label_0126;
                }
                if ((((start + 2) < this._end) && this.Parse2Digit(start + Lz_, out this.ZoneHour)) && (this.ZoneHour <= 0x63))
                {
                    switch (ch)
                    {
                        case '+':
                            this.Zone = ParserTimeZone.LocalEastOfUtc;
                            start += Lz_zz;
                            break;

                        case '-':
                            this.Zone = ParserTimeZone.LocalWestOfUtc;
                            start += Lz_zz;
                            break;
                    }
                }
                if (start < this._end)
                {
                    if (this.ParseChar(start, ':'))
                    {
                        start++;
                        if ((((start + 1) < this._end) && this.Parse2Digit(start, out this.ZoneMinute)) && (this.ZoneMinute <= 0x63))
                        {
                            start += 2;
                        }
                    }
                    else if ((((start + 1) < this._end) && this.Parse2Digit(start, out this.ZoneMinute)) && (this.ZoneMinute <= 0x63))
                    {
                        start += 2;
                    }
                }
            }
        Label_0126:
            return (start == this._end);
        }

        private bool Parse4Digit(int start, out int num)
        {
            if ((start + 3) < this._end)
            {
                int num2 = this._text[start] - '0';
                int num3 = this._text[start + 1] - '0';
                int num4 = this._text[start + 2] - '0';
                int num5 = this._text[start + 3] - '0';
                if ((((0 <= num2) && (num2 < 10)) && ((0 <= num3) && (num3 < 10))) && (((0 <= num4) && (num4 < 10)) && ((0 <= num5) && (num5 < 10))))
                {
                    num = (((((num2 * 10) + num3) * 10) + num4) * 10) + num5;
                    return true;
                }
            }
            num = 0;
            return false;
        }

        private bool Parse2Digit(int start, out int num)
        {
            if ((start + 1) < this._end)
            {
                int num2 = this._text[start] - '0';
                int num3 = this._text[start + 1] - '0';
                if (((0 <= num2) && (num2 < 10)) && ((0 <= num3) && (num3 < 10)))
                {
                    num = (num2 * 10) + num3;
                    return true;
                }
            }
            num = 0;
            return false;
        }

        private bool ParseChar(int start, char ch) => 
            ((start < this._end) && (this._text[start] == ch));
    }
}

