namespace Newtonsoft.Json
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Numerics;
    using System.Runtime.CompilerServices;

    public class JsonTextReader : JsonReader, IJsonLineInfo
    {
        private const char UnicodeReplacementChar = '�';
        private const int MaximumJavascriptIntegerCharacterLength = 380;
        private readonly TextReader _reader;
        private char[] _chars;
        private int _charsUsed;
        private int _charPos;
        private int _lineStartPos;
        private int _lineNumber;
        private bool _isEndOfFile;
        private StringBuffer _stringBuffer;
        private StringReference _stringReference;
        private IArrayPool<char> _arrayPool;
        internal PropertyNameTable NameTable;

        public JsonTextReader(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            this._reader = reader;
            this._lineNumber = 1;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static object BigIntegerParse(string number, CultureInfo culture) => 
            BigInteger.Parse(number, culture);

        private static void BlockCopyChars(char[] src, int srcOffset, char[] dst, int dstOffset, int count)
        {
            Buffer.BlockCopy(src, srcOffset * 2, dst, dstOffset * 2, count * 2);
        }

        private void ClearRecentString()
        {
            this._stringBuffer.Position = 0;
            this._stringReference = new StringReference();
        }

        public override void Close()
        {
            base.Close();
            if (this._chars != null)
            {
                BufferUtils.ReturnBuffer(this._arrayPool, this._chars);
                this._chars = null;
            }
            if (base.CloseInput && (this._reader != null))
            {
                this._reader.Close();
            }
            this._stringBuffer.Clear(this._arrayPool);
        }

        private JsonReaderException CreateUnexpectedCharacterException(char c) => 
            JsonReaderException.Create(this, "Unexpected character encountered while parsing value: {0}.".FormatWith(CultureInfo.InvariantCulture, c));

        private bool EatWhitespace(bool oneOrMore)
        {
            bool flag = false;
            bool flag2 = false;
            while (!flag)
            {
                char c = this._chars[this._charPos];
                switch (c)
                {
                    case '\0':
                    {
                        if (this._charsUsed == this._charPos)
                        {
                            if (this.ReadData(false) == 0)
                            {
                                flag = true;
                            }
                        }
                        else
                        {
                            this._charPos++;
                        }
                        continue;
                    }
                    case '\n':
                    {
                        this.ProcessLineFeed();
                        continue;
                    }
                    case '\r':
                    {
                        this.ProcessCarriageReturn(false);
                        continue;
                    }
                }
                if ((c == ' ') || char.IsWhiteSpace(c))
                {
                    flag2 = true;
                    this._charPos++;
                }
                else
                {
                    flag = true;
                }
            }
            return (!oneOrMore | flag2);
        }

        private void EndComment(bool setToken, int initialPosition, int endPosition)
        {
            if (setToken)
            {
                base.SetToken(JsonToken.Comment, new string(this._chars, initialPosition, endPosition - initialPosition));
            }
        }

        private void EnsureBuffer()
        {
            if (this._chars == null)
            {
                this._chars = BufferUtils.RentBuffer(this._arrayPool, 0x400);
                this._chars[0] = '\0';
            }
        }

        private void EnsureBufferNotEmpty()
        {
            if (this._stringBuffer.IsEmpty)
            {
                this._stringBuffer = new StringBuffer(this._arrayPool, 0x400);
            }
        }

        private bool EnsureChars(int relativePosition, bool append)
        {
            if ((this._charPos + relativePosition) >= this._charsUsed)
            {
                return this.ReadChars(relativePosition, append);
            }
            return true;
        }

        private void HandleNull()
        {
            if (this.EnsureChars(1, true))
            {
                if (this._chars[this._charPos + 1] == 'u')
                {
                    this.ParseNull();
                    return;
                }
                this._charPos += 2;
                throw this.CreateUnexpectedCharacterException(this._chars[this._charPos - 1]);
            }
            this._charPos = this._charsUsed;
            throw base.CreateUnexpectedEndException();
        }

        public bool HasLineInfo() => 
            true;

        private bool IsSeparator(char c)
        {
            if (c <= ')')
            {
                switch (c)
                {
                    case '\t':
                    case '\n':
                    case '\r':
                    case ' ':
                        return true;

                    case ')':
                        if ((base.CurrentState != JsonReader.State.Constructor) && (base.CurrentState != JsonReader.State.ConstructorStart))
                        {
                            goto Label_00C3;
                        }
                        return true;
                }
                goto Label_00B6;
            }
            if (c <= '/')
            {
                if (c == ',')
                {
                    goto Label_0067;
                }
                if (c == '/')
                {
                    if (!this.EnsureChars(1, false))
                    {
                        return false;
                    }
                    char ch = this._chars[this._charPos + 1];
                    if (ch != '*')
                    {
                        return (ch == '/');
                    }
                    return true;
                }
                goto Label_00B6;
            }
            if ((c != ']') && (c != '}'))
            {
                goto Label_00B6;
            }
        Label_0067:
            return true;
        Label_00B6:
            if (char.IsWhiteSpace(c))
            {
                return true;
            }
        Label_00C3:
            return false;
        }

        private bool MatchValue(string value)
        {
            if (!this.EnsureChars(value.Length - 1, true))
            {
                this._charPos = this._charsUsed;
                throw base.CreateUnexpectedEndException();
            }
            for (int i = 0; i < value.Length; i++)
            {
                if (this._chars[this._charPos + i] != value[i])
                {
                    this._charPos += i;
                    return false;
                }
            }
            this._charPos += value.Length;
            return true;
        }

        private bool MatchValueWithTrailingSeparator(string value)
        {
            if (!this.MatchValue(value))
            {
                return false;
            }
            if (this.EnsureChars(0, false) && !this.IsSeparator(this._chars[this._charPos]))
            {
                return (this._chars[this._charPos] == '\0');
            }
            return true;
        }

        private void OnNewLine(int pos)
        {
            this._lineNumber++;
            this._lineStartPos = pos;
        }

        private void ParseComment(bool setToken)
        {
            bool flag;
            this._charPos++;
            if (!this.EnsureChars(1, false))
            {
                throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
            }
            if (this._chars[this._charPos] == '*')
            {
                flag = false;
            }
            else
            {
                if (this._chars[this._charPos] != '/')
                {
                    throw JsonReaderException.Create(this, "Error parsing comment. Expected: *, got {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
                }
                flag = true;
            }
            this._charPos++;
            int initialPosition = this._charPos;
        Label_009A:
            switch (this._chars[this._charPos])
            {
                case '\0':
                    if (this._charsUsed == this._charPos)
                    {
                        if (this.ReadData(true) == 0)
                        {
                            if (!flag)
                            {
                                throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
                            }
                            this.EndComment(setToken, initialPosition, this._charPos);
                            return;
                        }
                    }
                    else
                    {
                        this._charPos++;
                    }
                    goto Label_009A;

                case '\n':
                    if (flag)
                    {
                        this.EndComment(setToken, initialPosition, this._charPos);
                        return;
                    }
                    this.ProcessLineFeed();
                    goto Label_009A;

                case '\r':
                    if (flag)
                    {
                        this.EndComment(setToken, initialPosition, this._charPos);
                        return;
                    }
                    this.ProcessCarriageReturn(true);
                    goto Label_009A;

                case '*':
                    this._charPos++;
                    if ((flag || !this.EnsureChars(0, true)) || (this._chars[this._charPos] != '/'))
                    {
                        goto Label_009A;
                    }
                    this.EndComment(setToken, initialPosition, this._charPos - 1);
                    this._charPos++;
                    return;
            }
            this._charPos++;
            goto Label_009A;
        }

        private void ParseConstructor()
        {
            char ch;
            int num2;
            if (!this.MatchValueWithTrailingSeparator("new"))
            {
                throw JsonReaderException.Create(this, "Unexpected content while parsing JSON.");
            }
            this.EatWhitespace(false);
            int startIndex = this._charPos;
        Label_001F:
            ch = this._chars[this._charPos];
            if (ch == '\0')
            {
                if (this._charsUsed == this._charPos)
                {
                    if (this.ReadData(true) == 0)
                    {
                        throw JsonReaderException.Create(this, "Unexpected end while parsing constructor.");
                    }
                    goto Label_001F;
                }
                num2 = this._charPos;
                this._charPos++;
            }
            else
            {
                if (char.IsLetterOrDigit(ch))
                {
                    this._charPos++;
                    goto Label_001F;
                }
                switch (ch)
                {
                    case '\r':
                        num2 = this._charPos;
                        this.ProcessCarriageReturn(true);
                        goto Label_0118;

                    case '\n':
                        num2 = this._charPos;
                        this.ProcessLineFeed();
                        goto Label_0118;
                }
                if (char.IsWhiteSpace(ch))
                {
                    num2 = this._charPos;
                    this._charPos++;
                }
                else
                {
                    if (ch != '(')
                    {
                        throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, ch));
                    }
                    num2 = this._charPos;
                }
            }
        Label_0118:
            this._stringReference = new StringReference(this._chars, startIndex, num2 - startIndex);
            string str = this._stringReference.ToString();
            this.EatWhitespace(false);
            if (this._chars[this._charPos] != '(')
            {
                throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
            }
            this._charPos++;
            this.ClearRecentString();
            base.SetToken(JsonToken.StartConstructor, str);
        }

        private void ParseFalse()
        {
            if (!this.MatchValueWithTrailingSeparator(JsonConvert.False))
            {
                throw JsonReaderException.Create(this, "Error parsing boolean value.");
            }
            base.SetToken(JsonToken.Boolean, false);
        }

        private void ParseNull()
        {
            if (!this.MatchValueWithTrailingSeparator(JsonConvert.Null))
            {
                throw JsonReaderException.Create(this, "Error parsing null value.");
            }
            base.SetToken(JsonToken.Null);
        }

        private void ParseNumber(ReadType readType)
        {
            JsonToken integer;
            object obj2;
            this.ShiftBufferIfNeeded();
            char c = this._chars[this._charPos];
            int startIndex = this._charPos;
            this.ReadNumberIntoBuffer();
            base.SetPostValueState(true);
            this._stringReference = new StringReference(this._chars, startIndex, this._charPos - startIndex);
            bool flag = char.IsDigit(c) && (this._stringReference.Length == 1);
            bool flag2 = (((c == '0') && (this._stringReference.Length > 1)) && ((this._stringReference.Chars[this._stringReference.StartIndex + 1] != '.') && (this._stringReference.Chars[this._stringReference.StartIndex + 1] != 'e'))) && (this._stringReference.Chars[this._stringReference.StartIndex + 1] != 'E');
            if (readType != ReadType.ReadAsString)
            {
                if (readType != ReadType.ReadAsInt32)
                {
                    if (readType != ReadType.ReadAsDecimal)
                    {
                        if (readType != ReadType.ReadAsDouble)
                        {
                            if (flag)
                            {
                                obj2 = ((long) c) - 0x30L;
                                integer = JsonToken.Integer;
                            }
                            else if (flag2)
                            {
                                string str5 = this._stringReference.ToString();
                                try
                                {
                                    obj2 = str5.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(str5, 0x10) : Convert.ToInt64(str5, 8);
                                }
                                catch (Exception exception5)
                                {
                                    throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, str5), exception5);
                                }
                                integer = JsonToken.Integer;
                            }
                            else
                            {
                                switch (ConvertUtils.Int64TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out long num6))
                                {
                                    case ParseResult.Success:
                                        obj2 = num6;
                                        integer = JsonToken.Integer;
                                        goto Label_0682;

                                    case ParseResult.Overflow:
                                    {
                                        string number = this._stringReference.ToString();
                                        if (number.Length > 380)
                                        {
                                            throw JsonReaderException.Create(this, "JSON integer {0} is too large to parse.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
                                        }
                                        obj2 = BigIntegerParse(number, CultureInfo.InvariantCulture);
                                        integer = JsonToken.Integer;
                                        goto Label_0682;
                                    }
                                }
                                string s = this._stringReference.ToString();
                                if (base._floatParseHandling == FloatParseHandling.Decimal)
                                {
                                    if (!decimal.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands | NumberStyles.AllowTrailingSign, CultureInfo.InvariantCulture, out decimal num7))
                                    {
                                        throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, s));
                                    }
                                    obj2 = num7;
                                }
                                else
                                {
                                    if (!double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double num8))
                                    {
                                        throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, s));
                                    }
                                    obj2 = num8;
                                }
                                integer = JsonToken.Float;
                            }
                            goto Label_0682;
                        }
                        if (flag)
                        {
                            obj2 = ((double) c) - 48.0;
                        }
                        else
                        {
                            if (flag2)
                            {
                                string str4 = this._stringReference.ToString();
                                try
                                {
                                    obj2 = Convert.ToDouble(str4.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(str4, 0x10) : Convert.ToInt64(str4, 8));
                                    goto Label_04A9;
                                }
                                catch (Exception exception4)
                                {
                                    throw JsonReaderException.Create(this, "Input string '{0}' is not a valid double.".FormatWith(CultureInfo.InvariantCulture, str4), exception4);
                                }
                            }
                            if (!double.TryParse(this._stringReference.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double num5))
                            {
                                throw JsonReaderException.Create(this, "Input string '{0}' is not a valid double.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
                            }
                            obj2 = num5;
                        }
                        goto Label_04A9;
                    }
                    if (flag)
                    {
                        obj2 = c - 48M;
                    }
                    else
                    {
                        if (flag2)
                        {
                            string str3 = this._stringReference.ToString();
                            try
                            {
                                obj2 = Convert.ToDecimal(str3.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(str3, 0x10) : Convert.ToInt64(str3, 8));
                                goto Label_03B2;
                            }
                            catch (Exception exception3)
                            {
                                throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, str3), exception3);
                            }
                        }
                        if (!decimal.TryParse(this._stringReference.ToString(), NumberStyles.Float | NumberStyles.AllowThousands | NumberStyles.AllowTrailingSign, CultureInfo.InvariantCulture, out decimal num4))
                        {
                            throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
                        }
                        obj2 = num4;
                    }
                    goto Label_03B2;
                }
                if (flag)
                {
                    obj2 = c - '0';
                }
                else
                {
                    if (flag2)
                    {
                        string str2 = this._stringReference.ToString();
                        try
                        {
                            obj2 = str2.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt32(str2, 0x10) : Convert.ToInt32(str2, 8);
                            goto Label_02B5;
                        }
                        catch (Exception exception2)
                        {
                            throw JsonReaderException.Create(this, "Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, str2), exception2);
                        }
                    }
                    switch (ConvertUtils.Int32TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out int num3))
                    {
                        case ParseResult.Success:
                            obj2 = num3;
                            goto Label_02B5;

                        case ParseResult.Overflow:
                            throw JsonReaderException.Create(this, "JSON integer {0} is too large or small for an Int32.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
                    }
                    throw JsonReaderException.Create(this, "Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
                }
                goto Label_02B5;
            }
            string str = this._stringReference.ToString();
            if (flag2)
            {
                try
                {
                    if (str.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        Convert.ToInt64(str, 0x10);
                    }
                    else
                    {
                        Convert.ToInt64(str, 8);
                    }
                    goto Label_018C;
                }
                catch (Exception exception)
                {
                    throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, str), exception);
                }
            }
            if (!double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            {
                throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
            }
        Label_018C:
            integer = JsonToken.String;
            obj2 = str;
            goto Label_0682;
        Label_02B5:
            integer = JsonToken.Integer;
            goto Label_0682;
        Label_03B2:
            integer = JsonToken.Float;
            goto Label_0682;
        Label_04A9:
            integer = JsonToken.Float;
        Label_0682:
            this.ClearRecentString();
            base.SetToken(integer, obj2, false);
        }

        private object ParseNumberNaN(ReadType readType)
        {
            if (!this.MatchValueWithTrailingSeparator(JsonConvert.NaN))
            {
                throw JsonReaderException.Create(this, "Error parsing NaN value.");
            }
            if (readType != ReadType.Read)
            {
                if (readType == ReadType.ReadAsString)
                {
                    base.SetToken(JsonToken.String, JsonConvert.NaN);
                    return JsonConvert.NaN;
                }
                if (readType != ReadType.ReadAsDouble)
                {
                    goto Label_0066;
                }
            }
            if (base._floatParseHandling == FloatParseHandling.Double)
            {
                base.SetToken(JsonToken.Float, (double) 1.0 / (double) 0.0);
                return (double) 1.0 / (double) 0.0;
            }
        Label_0066:
            throw JsonReaderException.Create(this, "Cannot read NaN value.");
        }

        private object ParseNumberNegativeInfinity(ReadType readType)
        {
            if (!this.MatchValueWithTrailingSeparator(JsonConvert.NegativeInfinity))
            {
                throw JsonReaderException.Create(this, "Error parsing -Infinity value.");
            }
            if (readType != ReadType.Read)
            {
                if (readType == ReadType.ReadAsString)
                {
                    base.SetToken(JsonToken.String, JsonConvert.NegativeInfinity);
                    return JsonConvert.NegativeInfinity;
                }
                if (readType != ReadType.ReadAsDouble)
                {
                    goto Label_0066;
                }
            }
            if (base._floatParseHandling == FloatParseHandling.Double)
            {
                base.SetToken(JsonToken.Float, (double) -1.0 / (double) 0.0);
                return (double) -1.0 / (double) 0.0;
            }
        Label_0066:
            throw JsonReaderException.Create(this, "Cannot read -Infinity value.");
        }

        private object ParseNumberPositiveInfinity(ReadType readType)
        {
            if (!this.MatchValueWithTrailingSeparator(JsonConvert.PositiveInfinity))
            {
                throw JsonReaderException.Create(this, "Error parsing Infinity value.");
            }
            if (readType != ReadType.Read)
            {
                if (readType == ReadType.ReadAsString)
                {
                    base.SetToken(JsonToken.String, JsonConvert.PositiveInfinity);
                    return JsonConvert.PositiveInfinity;
                }
                if (readType != ReadType.ReadAsDouble)
                {
                    goto Label_0066;
                }
            }
            if (base._floatParseHandling == FloatParseHandling.Double)
            {
                base.SetToken(JsonToken.Float, (double) 1.0 / (double) 0.0);
                return (double) 1.0 / (double) 0.0;
            }
        Label_0066:
            throw JsonReaderException.Create(this, "Cannot read Infinity value.");
        }

        private bool ParseObject()
        {
            char ch;
        Label_0000:
            ch = this._chars[this._charPos];
            switch (ch)
            {
                case '\t':
                case ' ':
                    this._charPos++;
                    goto Label_0000;

                case '\n':
                    this.ProcessLineFeed();
                    goto Label_0000;

                case '\r':
                    this.ProcessCarriageReturn(false);
                    goto Label_0000;

                case '\0':
                    if (this._charsUsed == this._charPos)
                    {
                        if (this.ReadData(false) == 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        this._charPos++;
                    }
                    goto Label_0000;

                case '/':
                    this.ParseComment(true);
                    return true;

                case '}':
                    base.SetToken(JsonToken.EndObject);
                    this._charPos++;
                    return true;
            }
            if (char.IsWhiteSpace(ch))
            {
                this._charPos++;
                goto Label_0000;
            }
            return this.ParseProperty();
        }

        private bool ParsePostValue()
        {
            char ch;
        Label_0000:
            ch = this._chars[this._charPos];
            switch (ch)
            {
                case '\t':
                case ' ':
                    this._charPos++;
                    goto Label_0000;

                case '\n':
                    this.ProcessLineFeed();
                    goto Label_0000;

                case '\r':
                    this.ProcessCarriageReturn(false);
                    goto Label_0000;

                case '\0':
                    if (this._charsUsed == this._charPos)
                    {
                        if (this.ReadData(false) == 0)
                        {
                            base._currentState = JsonReader.State.Finished;
                            return false;
                        }
                    }
                    else
                    {
                        this._charPos++;
                    }
                    goto Label_0000;

                case ')':
                    this._charPos++;
                    base.SetToken(JsonToken.EndConstructor);
                    return true;

                case ',':
                    this._charPos++;
                    base.SetStateBasedOnCurrent();
                    return false;

                case '/':
                    this.ParseComment(true);
                    return true;

                case ']':
                    this._charPos++;
                    base.SetToken(JsonToken.EndArray);
                    return true;

                case '}':
                    this._charPos++;
                    base.SetToken(JsonToken.EndObject);
                    return true;
            }
            if (!char.IsWhiteSpace(ch))
            {
                throw JsonReaderException.Create(this, "After parsing a value an unexpected character was encountered: {0}.".FormatWith(CultureInfo.InvariantCulture, ch));
            }
            this._charPos++;
            goto Label_0000;
        }

        private bool ParseProperty()
        {
            char ch2;
            string str;
            char ch = this._chars[this._charPos];
            switch (ch)
            {
                case '"':
                case '\'':
                    this._charPos++;
                    ch2 = ch;
                    this.ShiftBufferIfNeeded();
                    this.ReadStringIntoBuffer(ch2);
                    break;

                default:
                    if (!this.ValidIdentifierChar(ch))
                    {
                        throw JsonReaderException.Create(this, "Invalid property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
                    }
                    ch2 = '\0';
                    this.ShiftBufferIfNeeded();
                    this.ParseUnquotedProperty();
                    break;
            }
            if (this.NameTable != null)
            {
                str = this.NameTable.Get(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length);
                if (str == null)
                {
                    str = this._stringReference.ToString();
                }
            }
            else
            {
                str = this._stringReference.ToString();
            }
            this.EatWhitespace(false);
            if (this._chars[this._charPos] != ':')
            {
                throw JsonReaderException.Create(this, "Invalid character after parsing property name. Expected ':' but got: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
            }
            this._charPos++;
            base.SetToken(JsonToken.PropertyName, str);
            base._quoteChar = ch2;
            this.ClearRecentString();
            return true;
        }

        private void ParseString(char quote, ReadType readType)
        {
            byte[] buffer;
            this._charPos++;
            this.ShiftBufferIfNeeded();
            this.ReadStringIntoBuffer(quote);
            base.SetPostValueState(true);
            switch (readType)
            {
                case ReadType.ReadAsInt32:
                case ReadType.ReadAsDecimal:
                case ReadType.ReadAsBoolean:
                    return;

                case ReadType.ReadAsBytes:
                    if (this._stringReference.Length != 0)
                    {
                        if ((this._stringReference.Length == 0x24) && ConvertUtils.TryConvertGuid(this._stringReference.ToString(), out Guid guid))
                        {
                            buffer = guid.ToByteArray();
                        }
                        else
                        {
                            buffer = Convert.FromBase64CharArray(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length);
                        }
                        break;
                    }
                    buffer = new byte[0];
                    break;

                case ReadType.ReadAsString:
                {
                    string str = this._stringReference.ToString();
                    base.SetToken(JsonToken.String, str, false);
                    base._quoteChar = quote;
                    return;
                }
                default:
                    if (base._dateParseHandling != DateParseHandling.None)
                    {
                        DateParseHandling dateTime;
                        if (readType == ReadType.ReadAsDateTime)
                        {
                            dateTime = DateParseHandling.DateTime;
                        }
                        else if (readType == ReadType.ReadAsDateTimeOffset)
                        {
                            dateTime = DateParseHandling.DateTimeOffset;
                        }
                        else
                        {
                            dateTime = base._dateParseHandling;
                        }
                        if (dateTime == DateParseHandling.DateTime)
                        {
                            if (DateTimeUtils.TryParseDateTime(this._stringReference, base.DateTimeZoneHandling, base.DateFormatString, base.Culture, out DateTime time))
                            {
                                base.SetToken(JsonToken.Date, time, false);
                                return;
                            }
                        }
                        else if (DateTimeUtils.TryParseDateTimeOffset(this._stringReference, base.DateFormatString, base.Culture, out DateTimeOffset offset))
                        {
                            base.SetToken(JsonToken.Date, offset, false);
                            return;
                        }
                    }
                    base.SetToken(JsonToken.String, this._stringReference.ToString(), false);
                    base._quoteChar = quote;
                    return;
            }
            base.SetToken(JsonToken.Bytes, buffer, false);
        }

        private void ParseTrue()
        {
            if (!this.MatchValueWithTrailingSeparator(JsonConvert.True))
            {
                throw JsonReaderException.Create(this, "Error parsing boolean value.");
            }
            base.SetToken(JsonToken.Boolean, true);
        }

        private void ParseUndefined()
        {
            if (!this.MatchValueWithTrailingSeparator(JsonConvert.Undefined))
            {
                throw JsonReaderException.Create(this, "Error parsing undefined value.");
            }
            base.SetToken(JsonToken.Undefined);
        }

        private char ParseUnicode()
        {
            if (!this.EnsureChars(4, true))
            {
                throw JsonReaderException.Create(this, "Unexpected end while parsing unicode character.");
            }
            char ch = Convert.ToChar(ConvertUtils.HexTextToInt(this._chars, this._charPos, this._charPos + 4));
            this._charPos += 4;
            return ch;
        }

        private void ParseUnquotedProperty()
        {
            int startIndex = this._charPos;
        Label_0007:
            while (this._chars[this._charPos] == '\0')
            {
                if (this._charsUsed != this._charPos)
                {
                    this._stringReference = new StringReference(this._chars, startIndex, this._charPos - startIndex);
                    return;
                }
                if (this.ReadData(true) == 0)
                {
                    throw JsonReaderException.Create(this, "Unexpected end while parsing unquoted property name.");
                }
            }
            char ch2 = this._chars[this._charPos];
            if (this.ValidIdentifierChar(ch2))
            {
                this._charPos++;
                goto Label_0007;
            }
            if (!char.IsWhiteSpace(ch2) && (ch2 != ':'))
            {
                throw JsonReaderException.Create(this, "Invalid JavaScript property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, ch2));
            }
            this._stringReference = new StringReference(this._chars, startIndex, this._charPos - startIndex);
        }

        private bool ParseValue()
        {
            char ch;
        Label_0000:
            ch = this._chars[this._charPos];
            switch (ch)
            {
                case '\t':
                case ' ':
                    this._charPos++;
                    goto Label_0000;

                case '\n':
                    this.ProcessLineFeed();
                    goto Label_0000;

                case '\r':
                    this.ProcessCarriageReturn(false);
                    goto Label_0000;

                case '\0':
                    if (this._charsUsed == this._charPos)
                    {
                        if (this.ReadData(false) == 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        this._charPos++;
                    }
                    goto Label_0000;

                case 'I':
                    this.ParseNumberPositiveInfinity(ReadType.Read);
                    return true;

                case 'N':
                    this.ParseNumberNaN(ReadType.Read);
                    return true;

                case '\'':
                case '"':
                    this.ParseString(ch, ReadType.Read);
                    return true;

                case ')':
                    this._charPos++;
                    base.SetToken(JsonToken.EndConstructor);
                    return true;

                case ',':
                    base.SetToken(JsonToken.Undefined);
                    return true;

                case '-':
                    if (!this.EnsureChars(1, true) || (this._chars[this._charPos + 1] != 'I'))
                    {
                        this.ParseNumber(ReadType.Read);
                    }
                    else
                    {
                        this.ParseNumberNegativeInfinity(ReadType.Read);
                    }
                    return true;

                case '/':
                    this.ParseComment(true);
                    return true;

                case '[':
                    this._charPos++;
                    base.SetToken(JsonToken.StartArray);
                    return true;

                case ']':
                    this._charPos++;
                    base.SetToken(JsonToken.EndArray);
                    return true;

                case 'f':
                    this.ParseFalse();
                    return true;

                case 'u':
                    this.ParseUndefined();
                    return true;

                case '{':
                    this._charPos++;
                    base.SetToken(JsonToken.StartObject);
                    return true;

                case 'n':
                    if (this.EnsureChars(1, true))
                    {
                        switch (this._chars[this._charPos + 1])
                        {
                            case 'u':
                                this.ParseNull();
                                goto Label_01B4;

                            case 'e':
                                this.ParseConstructor();
                                goto Label_01B4;
                        }
                        throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
                    }
                    this._charPos++;
                    throw base.CreateUnexpectedEndException();

                case 't':
                    this.ParseTrue();
                    return true;

                default:
                    if (char.IsWhiteSpace(ch))
                    {
                        this._charPos++;
                        goto Label_0000;
                    }
                    if ((!char.IsNumber(ch) && (ch != '-')) && (ch != '.'))
                    {
                        throw this.CreateUnexpectedCharacterException(ch);
                    }
                    this.ParseNumber(ReadType.Read);
                    return true;
            }
        Label_01B4:
            return true;
        }

        private void ProcessCarriageReturn(bool append)
        {
            this._charPos++;
            if (this.EnsureChars(1, append) && (this._chars[this._charPos] == '\n'))
            {
                this._charPos++;
            }
            this.OnNewLine(this._charPos);
        }

        private void ProcessLineFeed()
        {
            this._charPos++;
            this.OnNewLine(this._charPos);
        }

        private void ProcessValueComma()
        {
            this._charPos++;
            if (base._currentState != JsonReader.State.PostValue)
            {
                base.SetToken(JsonToken.Undefined);
                throw this.CreateUnexpectedCharacterException(',');
            }
            base.SetStateBasedOnCurrent();
        }

        public override bool Read()
        {
            this.EnsureBuffer();
        Label_0006:
            switch (base._currentState)
            {
                case JsonReader.State.Start:
                case JsonReader.State.Property:
                case JsonReader.State.ArrayStart:
                case JsonReader.State.Array:
                case JsonReader.State.ConstructorStart:
                case JsonReader.State.Constructor:
                    return this.ParseValue();

                case JsonReader.State.ObjectStart:
                case JsonReader.State.Object:
                    return this.ParseObject();

                case JsonReader.State.PostValue:
                    if (!this.ParsePostValue())
                    {
                        goto Label_0006;
                    }
                    return true;

                case JsonReader.State.Finished:
                    if (!this.EnsureChars(0, false))
                    {
                        base.SetToken(JsonToken.None);
                        return false;
                    }
                    this.EatWhitespace(false);
                    if (!this._isEndOfFile)
                    {
                        if (this._chars[this._charPos] != '/')
                        {
                            throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
                        }
                        this.ParseComment(true);
                        return true;
                    }
                    base.SetToken(JsonToken.None);
                    return false;
            }
            throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
        }

        public override bool? ReadAsBoolean()
        {
            char ch;
            bool flag;
            this.EnsureBuffer();
            switch (base._currentState)
            {
                case JsonReader.State.Start:
                case JsonReader.State.Property:
                case JsonReader.State.ArrayStart:
                case JsonReader.State.Array:
                case JsonReader.State.PostValue:
                case JsonReader.State.ConstructorStart:
                case JsonReader.State.Constructor:
                    break;

                case JsonReader.State.Finished:
                    this.ReadFinished();
                    return null;

                default:
                    throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
            }
        Label_004C:
            ch = this._chars[this._charPos];
            switch (ch)
            {
                case '\t':
                case ' ':
                    this._charPos++;
                    goto Label_004C;

                case '\n':
                    this.ProcessLineFeed();
                    goto Label_004C;

                case '\r':
                    this.ProcessCarriageReturn(false);
                    goto Label_004C;

                case '\0':
                    if (!this.ReadNullChar())
                    {
                        goto Label_004C;
                    }
                    base.SetToken(JsonToken.None, null, false);
                    return null;

                case '"':
                case '\'':
                    this.ParseString(ch, ReadType.Read);
                    return base.ReadBooleanString(this._stringReference.ToString());

                case ',':
                    this.ProcessValueComma();
                    goto Label_004C;

                case '-':
                case '.':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    this.ParseNumber(ReadType.Read);
                    if (!(this.Value is BigInteger))
                    {
                        flag = Convert.ToBoolean(this.Value, CultureInfo.InvariantCulture);
                        break;
                    }
                    flag = ((BigInteger) this.Value) != 0L;
                    break;

                case '/':
                    this.ParseComment(false);
                    goto Label_004C;

                case 'n':
                    this.HandleNull();
                    return null;

                case 't':
                case 'f':
                {
                    bool flag2 = ch == 't';
                    string str = flag2 ? JsonConvert.True : JsonConvert.False;
                    if (!this.MatchValueWithTrailingSeparator(str))
                    {
                        throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
                    }
                    base.SetToken(JsonToken.Boolean, flag2);
                    return new bool?(flag2);
                }
                case ']':
                    this._charPos++;
                    if (((base._currentState != JsonReader.State.Array) && (base._currentState != JsonReader.State.ArrayStart)) && (base._currentState != JsonReader.State.PostValue))
                    {
                        throw this.CreateUnexpectedCharacterException(ch);
                    }
                    base.SetToken(JsonToken.EndArray);
                    return null;

                default:
                    this._charPos++;
                    if (!char.IsWhiteSpace(ch))
                    {
                        throw this.CreateUnexpectedCharacterException(ch);
                    }
                    goto Label_004C;
            }
            base.SetToken(JsonToken.Boolean, flag, false);
            return new bool?(flag);
        }

        public override byte[] ReadAsBytes()
        {
            char ch;
            this.EnsureBuffer();
            bool flag = false;
            switch (base._currentState)
            {
                case JsonReader.State.Start:
                case JsonReader.State.Property:
                case JsonReader.State.ArrayStart:
                case JsonReader.State.Array:
                case JsonReader.State.PostValue:
                case JsonReader.State.ConstructorStart:
                case JsonReader.State.Constructor:
                    break;

                case JsonReader.State.Finished:
                    this.ReadFinished();
                    return null;

                default:
                    goto Label_0257;
            }
        Label_004E:
            ch = this._chars[this._charPos];
            switch (ch)
            {
                case '\t':
                case ' ':
                    this._charPos++;
                    goto Label_004E;

                case '\n':
                    this.ProcessLineFeed();
                    goto Label_004E;

                case '\r':
                    this.ProcessCarriageReturn(false);
                    goto Label_004E;

                case '\0':
                    if (!this.ReadNullChar())
                    {
                        goto Label_004E;
                    }
                    base.SetToken(JsonToken.None, null, false);
                    return null;

                case '"':
                case '\'':
                {
                    this.ParseString(ch, ReadType.ReadAsBytes);
                    byte[] buffer = (byte[]) this.Value;
                    if (flag)
                    {
                        base.ReaderReadAndAssert();
                        if (this.TokenType != JsonToken.EndObject)
                        {
                            throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
                        }
                        base.SetToken(JsonToken.Bytes, buffer, false);
                    }
                    return buffer;
                }
                case ',':
                    this.ProcessValueComma();
                    goto Label_004E;

                case '/':
                    this.ParseComment(false);
                    goto Label_004E;

                case '[':
                    this._charPos++;
                    base.SetToken(JsonToken.StartArray);
                    return base.ReadArrayIntoByteArray();

                case ']':
                    this._charPos++;
                    if (((base._currentState != JsonReader.State.Array) && (base._currentState != JsonReader.State.ArrayStart)) && (base._currentState != JsonReader.State.PostValue))
                    {
                        throw this.CreateUnexpectedCharacterException(ch);
                    }
                    base.SetToken(JsonToken.EndArray);
                    return null;

                case 'n':
                    this.HandleNull();
                    return null;

                case '{':
                    this._charPos++;
                    base.SetToken(JsonToken.StartObject);
                    base.ReadIntoWrappedTypeObject();
                    flag = true;
                    goto Label_004E;

                default:
                    this._charPos++;
                    if (!char.IsWhiteSpace(ch))
                    {
                        throw this.CreateUnexpectedCharacterException(ch);
                    }
                    goto Label_004E;
            }
        Label_0257:
            throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
        }

        public override DateTime? ReadAsDateTime() => 
            ((DateTime?) this.ReadStringValue(ReadType.ReadAsDateTime));

        public override DateTimeOffset? ReadAsDateTimeOffset() => 
            ((DateTimeOffset?) this.ReadStringValue(ReadType.ReadAsDateTimeOffset));

        public override decimal? ReadAsDecimal() => 
            ((decimal?) this.ReadNumberValue(ReadType.ReadAsDecimal));

        public override double? ReadAsDouble() => 
            ((double?) this.ReadNumberValue(ReadType.ReadAsDouble));

        public override int? ReadAsInt32() => 
            ((int?) this.ReadNumberValue(ReadType.ReadAsInt32));

        public override string ReadAsString() => 
            ((string) this.ReadStringValue(ReadType.ReadAsString));

        private bool ReadChars(int relativePosition, bool append)
        {
            if (this._isEndOfFile)
            {
                return false;
            }
            int num = ((this._charPos + relativePosition) - this._charsUsed) + 1;
            int num2 = 0;
            do
            {
                int num3 = this.ReadData(append, num - num2);
                if (num3 == 0)
                {
                    break;
                }
                num2 += num3;
            }
            while (num2 < num);
            if (num2 < num)
            {
                return false;
            }
            return true;
        }

        private int ReadData(bool append) => 
            this.ReadData(append, 0);

        private int ReadData(bool append, int charsRequired)
        {
            if (this._isEndOfFile)
            {
                return 0;
            }
            if ((this._charsUsed + charsRequired) >= (this._chars.Length - 1))
            {
                if (append)
                {
                    int minSize = Math.Max((int) (this._chars.Length * 2), (int) ((this._charsUsed + charsRequired) + 1));
                    char[] dst = BufferUtils.RentBuffer(this._arrayPool, minSize);
                    BlockCopyChars(this._chars, 0, dst, 0, this._chars.Length);
                    BufferUtils.ReturnBuffer(this._arrayPool, this._chars);
                    this._chars = dst;
                }
                else
                {
                    int num2 = this._charsUsed - this._charPos;
                    if (((num2 + charsRequired) + 1) >= this._chars.Length)
                    {
                        char[] dst = BufferUtils.RentBuffer(this._arrayPool, (num2 + charsRequired) + 1);
                        if (num2 > 0)
                        {
                            BlockCopyChars(this._chars, this._charPos, dst, 0, num2);
                        }
                        BufferUtils.ReturnBuffer(this._arrayPool, this._chars);
                        this._chars = dst;
                    }
                    else if (num2 > 0)
                    {
                        BlockCopyChars(this._chars, this._charPos, this._chars, 0, num2);
                    }
                    this._lineStartPos -= this._charPos;
                    this._charPos = 0;
                    this._charsUsed = num2;
                }
            }
            int count = (this._chars.Length - this._charsUsed) - 1;
            int num4 = this._reader.Read(this._chars, this._charsUsed, count);
            this._charsUsed += num4;
            if (num4 == 0)
            {
                this._isEndOfFile = true;
            }
            this._chars[this._charsUsed] = '\0';
            return num4;
        }

        private void ReadFinished()
        {
            if (this.EnsureChars(0, false))
            {
                this.EatWhitespace(false);
                if (this._isEndOfFile)
                {
                    return;
                }
                if (this._chars[this._charPos] != '/')
                {
                    throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
                }
                this.ParseComment(false);
            }
            base.SetToken(JsonToken.None);
        }

        private bool ReadNullChar()
        {
            if (this._charsUsed == this._charPos)
            {
                if (this.ReadData(false) == 0)
                {
                    this._isEndOfFile = true;
                    return true;
                }
            }
            else
            {
                this._charPos++;
            }
            return false;
        }

        private void ReadNumberIntoBuffer()
        {
            int index = this._charPos;
        Label_0007:
            switch (this._chars[index])
            {
                case '+':
                case '-':
                case '.':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'x':
                case 'X':
                    index++;
                    goto Label_0007;

                case '\0':
                    this._charPos = index;
                    if (this._charsUsed == index)
                    {
                        if (this.ReadData(true) == 0)
                        {
                            return;
                        }
                        goto Label_0007;
                    }
                    return;
            }
            this._charPos = index;
            char c = this._chars[this._charPos];
            if (((!char.IsWhiteSpace(c) && (c != ',')) && ((c != '}') && (c != ']'))) && ((c != ')') && (c != '/')))
            {
                throw JsonReaderException.Create(this, "Unexpected character encountered while parsing number: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
            }
        }

        private object ReadNumberValue(ReadType readType)
        {
            char ch;
            this.EnsureBuffer();
            switch (base._currentState)
            {
                case JsonReader.State.Start:
                case JsonReader.State.Property:
                case JsonReader.State.ArrayStart:
                case JsonReader.State.Array:
                case JsonReader.State.PostValue:
                case JsonReader.State.ConstructorStart:
                case JsonReader.State.Constructor:
                    break;

                case JsonReader.State.Finished:
                    this.ReadFinished();
                    return null;

                default:
                    goto Label_02D4;
            }
        Label_004C:
            ch = this._chars[this._charPos];
            switch (ch)
            {
                case '\t':
                case ' ':
                    this._charPos++;
                    goto Label_004C;

                case '\n':
                    this.ProcessLineFeed();
                    goto Label_004C;

                case '\r':
                    this.ProcessCarriageReturn(false);
                    goto Label_004C;

                case '\0':
                    if (!this.ReadNullChar())
                    {
                        goto Label_004C;
                    }
                    base.SetToken(JsonToken.None, null, false);
                    return null;

                case '"':
                case '\'':
                    this.ParseString(ch, readType);
                    if (readType == ReadType.ReadAsInt32)
                    {
                        return base.ReadInt32String(this._stringReference.ToString());
                    }
                    if (readType == ReadType.ReadAsDecimal)
                    {
                        return base.ReadDecimalString(this._stringReference.ToString());
                    }
                    if (readType != ReadType.ReadAsDouble)
                    {
                        throw new ArgumentOutOfRangeException("readType");
                    }
                    return base.ReadDoubleString(this._stringReference.ToString());

                case ',':
                    this.ProcessValueComma();
                    goto Label_004C;

                case '-':
                    if (!this.EnsureChars(1, true) || (this._chars[this._charPos + 1] != 'I'))
                    {
                        this.ParseNumber(readType);
                        return this.Value;
                    }
                    return this.ParseNumberNegativeInfinity(readType);

                case '.':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    this.ParseNumber(readType);
                    return this.Value;

                case '/':
                    this.ParseComment(false);
                    goto Label_004C;

                case ']':
                    this._charPos++;
                    if (((base._currentState != JsonReader.State.Array) && (base._currentState != JsonReader.State.ArrayStart)) && (base._currentState != JsonReader.State.PostValue))
                    {
                        throw this.CreateUnexpectedCharacterException(ch);
                    }
                    base.SetToken(JsonToken.EndArray);
                    return null;

                case 'n':
                    this.HandleNull();
                    return null;

                case 'I':
                    return this.ParseNumberPositiveInfinity(readType);

                case 'N':
                    return this.ParseNumberNaN(readType);

                default:
                    this._charPos++;
                    if (!char.IsWhiteSpace(ch))
                    {
                        throw this.CreateUnexpectedCharacterException(ch);
                    }
                    goto Label_004C;
            }
        Label_02D4:
            throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
        }

        private void ReadStringIntoBuffer(char quote)
        {
            int num4;
            char ch3;
            int index = this._charPos;
            int startIndex = this._charPos;
            int num3 = this._charPos;
            this._stringBuffer.Position = 0;
        Label_0021:
            switch (this._chars[index++])
            {
                case '\0':
                    if (this._charsUsed == (index - 1))
                    {
                        index--;
                        if (this.ReadData(true) == 0)
                        {
                            this._charPos = index;
                            throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
                        }
                    }
                    goto Label_0021;

                case '\n':
                    this._charPos = index - 1;
                    this.ProcessLineFeed();
                    index = this._charPos;
                    goto Label_0021;

                case '\r':
                    this._charPos = index - 1;
                    this.ProcessCarriageReturn(true);
                    index = this._charPos;
                    goto Label_0021;

                case '"':
                case '\'':
                    if (this._chars[index - 1] != quote)
                    {
                        goto Label_0021;
                    }
                    index--;
                    if (startIndex == num3)
                    {
                        this._stringReference = new StringReference(this._chars, startIndex, index - startIndex);
                    }
                    else
                    {
                        this.EnsureBufferNotEmpty();
                        if (index > num3)
                        {
                            this._stringBuffer.Append(this._arrayPool, this._chars, num3, index - num3);
                        }
                        this._stringReference = new StringReference(this._stringBuffer.InternalBuffer, 0, this._stringBuffer.Position);
                    }
                    index++;
                    this._charPos = index;
                    return;

                case '\\':
                {
                    this._charPos = index;
                    if (!this.EnsureChars(0, true))
                    {
                        throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
                    }
                    num4 = index - 1;
                    char ch2 = this._chars[index];
                    index++;
                    switch (ch2)
                    {
                        case '/':
                        case '"':
                        case '\'':
                            ch3 = ch2;
                            goto Label_02C6;

                        case '\\':
                            ch3 = '\\';
                            goto Label_02C6;

                        case 'b':
                            ch3 = '\b';
                            goto Label_02C6;

                        case 'f':
                            ch3 = '\f';
                            goto Label_02C6;

                        case 'r':
                            ch3 = '\r';
                            goto Label_02C6;

                        case 't':
                            ch3 = '\t';
                            goto Label_02C6;

                        case 'u':
                            this._charPos = index;
                            ch3 = this.ParseUnicode();
                            if (!StringUtils.IsLowSurrogate(ch3))
                            {
                                if (StringUtils.IsHighSurrogate(ch3))
                                {
                                    bool flag;
                                    do
                                    {
                                        flag = false;
                                        if ((this.EnsureChars(2, true) && (this._chars[this._charPos] == '\\')) && (this._chars[this._charPos + 1] == 'u'))
                                        {
                                            char writeChar = ch3;
                                            this._charPos += 2;
                                            ch3 = this.ParseUnicode();
                                            if (!StringUtils.IsLowSurrogate(ch3))
                                            {
                                                if (StringUtils.IsHighSurrogate(ch3))
                                                {
                                                    writeChar = 0xfffd;
                                                    flag = true;
                                                }
                                                else
                                                {
                                                    writeChar = 0xfffd;
                                                }
                                            }
                                            this.EnsureBufferNotEmpty();
                                            this.WriteCharToBuffer(writeChar, num3, num4);
                                            num3 = this._charPos;
                                        }
                                        else
                                        {
                                            ch3 = 0xfffd;
                                        }
                                    }
                                    while (flag);
                                }
                            }
                            else
                            {
                                ch3 = 0xfffd;
                            }
                            goto Label_028C;

                        case 'n':
                            ch3 = '\n';
                            goto Label_02C6;
                    }
                    this._charPos = index;
                    throw JsonReaderException.Create(this, "Bad JSON escape sequence: {0}.".FormatWith(CultureInfo.InvariantCulture, @"\" + ch2.ToString()));
                }
                default:
                    goto Label_0021;
            }
        Label_028C:
            index = this._charPos;
        Label_02C6:
            this.EnsureBufferNotEmpty();
            this.WriteCharToBuffer(ch3, num3, num4);
            num3 = index;
            goto Label_0021;
        }

        private object ReadStringValue(ReadType readType)
        {
            char ch;
            this.EnsureBuffer();
            switch (base._currentState)
            {
                case JsonReader.State.Start:
                case JsonReader.State.Property:
                case JsonReader.State.ArrayStart:
                case JsonReader.State.Array:
                case JsonReader.State.PostValue:
                case JsonReader.State.ConstructorStart:
                case JsonReader.State.Constructor:
                    break;

                case JsonReader.State.Finished:
                    this.ReadFinished();
                    return null;

                default:
                    throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
            }
        Label_004C:
            ch = this._chars[this._charPos];
            switch (ch)
            {
                case '\t':
                case ' ':
                    this._charPos++;
                    goto Label_004C;

                case '\n':
                    this.ProcessLineFeed();
                    goto Label_004C;

                case '\r':
                    this.ProcessCarriageReturn(false);
                    goto Label_004C;

                case '\0':
                    if (!this.ReadNullChar())
                    {
                        goto Label_004C;
                    }
                    base.SetToken(JsonToken.None, null, false);
                    return null;

                case '"':
                case '\'':
                    this.ParseString(ch, readType);
                    switch (readType)
                    {
                        case ReadType.ReadAsBytes:
                            return this.Value;

                        case ReadType.ReadAsString:
                            return this.Value;

                        case ReadType.ReadAsDateTime:
                            if (this.Value is DateTime)
                            {
                                return (DateTime) this.Value;
                            }
                            return base.ReadDateTimeString((string) this.Value);

                        case ReadType.ReadAsDateTimeOffset:
                            if (this.Value is DateTimeOffset)
                            {
                                return (DateTimeOffset) this.Value;
                            }
                            return base.ReadDateTimeOffsetString((string) this.Value);
                    }
                    break;

                case ',':
                    this.ProcessValueComma();
                    goto Label_004C;

                case '-':
                    if (!this.EnsureChars(1, true) || (this._chars[this._charPos + 1] != 'I'))
                    {
                        this.ParseNumber(readType);
                        return this.Value;
                    }
                    return this.ParseNumberNegativeInfinity(readType);

                case '.':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    if (readType != ReadType.ReadAsString)
                    {
                        this._charPos++;
                        throw this.CreateUnexpectedCharacterException(ch);
                    }
                    this.ParseNumber(ReadType.ReadAsString);
                    return this.Value;

                case '/':
                    this.ParseComment(false);
                    goto Label_004C;

                case 'I':
                    return this.ParseNumberPositiveInfinity(readType);

                case 'N':
                    return this.ParseNumberNaN(readType);

                case ']':
                    this._charPos++;
                    if (((base._currentState != JsonReader.State.Array) && (base._currentState != JsonReader.State.ArrayStart)) && (base._currentState != JsonReader.State.PostValue))
                    {
                        throw this.CreateUnexpectedCharacterException(ch);
                    }
                    base.SetToken(JsonToken.EndArray);
                    return null;

                case 'f':
                case 't':
                {
                    if (readType != ReadType.ReadAsString)
                    {
                        this._charPos++;
                        throw this.CreateUnexpectedCharacterException(ch);
                    }
                    string str = (ch == 't') ? JsonConvert.True : JsonConvert.False;
                    if (!this.MatchValueWithTrailingSeparator(str))
                    {
                        throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
                    }
                    base.SetToken(JsonToken.String, str);
                    return str;
                }
                case 'n':
                    this.HandleNull();
                    return null;

                default:
                    this._charPos++;
                    if (!char.IsWhiteSpace(ch))
                    {
                        throw this.CreateUnexpectedCharacterException(ch);
                    }
                    goto Label_004C;
            }
            throw new ArgumentOutOfRangeException("readType");
        }

        private void ShiftBufferIfNeeded()
        {
            int length = this._chars.Length;
            if ((length - this._charPos) <= (length * 0.1))
            {
                int count = this._charsUsed - this._charPos;
                if (count > 0)
                {
                    BlockCopyChars(this._chars, this._charPos, this._chars, 0, count);
                }
                this._lineStartPos -= this._charPos;
                this._charPos = 0;
                this._charsUsed = count;
                this._chars[this._charsUsed] = '\0';
            }
        }

        private bool ValidIdentifierChar(char value)
        {
            if (!char.IsLetterOrDigit(value) && (value != '_'))
            {
                return (value == '$');
            }
            return true;
        }

        private void WriteCharToBuffer(char writeChar, int lastWritePosition, int writeToPosition)
        {
            if (writeToPosition > lastWritePosition)
            {
                this._stringBuffer.Append(this._arrayPool, this._chars, lastWritePosition, writeToPosition - lastWritePosition);
            }
            this._stringBuffer.Append(this._arrayPool, writeChar);
        }

        public IArrayPool<char> ArrayPool
        {
            get => 
                this._arrayPool;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this._arrayPool = value;
            }
        }

        public int LineNumber
        {
            get
            {
                if (((base.CurrentState == JsonReader.State.Start) && (this.LinePosition == 0)) && (this.TokenType != JsonToken.Comment))
                {
                    return 0;
                }
                return this._lineNumber;
            }
        }

        public int LinePosition =>
            (this._charPos - this._lineStartPos);
    }
}

