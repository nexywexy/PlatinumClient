namespace Newtonsoft.Json.Utilities
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal static class JavaScriptUtils
    {
        internal static readonly bool[] SingleQuoteCharEscapeFlags = new bool[0x80];
        internal static readonly bool[] DoubleQuoteCharEscapeFlags = new bool[0x80];
        internal static readonly bool[] HtmlCharEscapeFlags = new bool[0x80];
        private const int UnicodeTextLength = 6;
        private const string EscapedUnicodeText = "!";

        static JavaScriptUtils()
        {
            List<char> list1 = new List<char> { 
                '\n',
                '\r',
                '\t',
                '\\',
                '\f',
                '\b'
            };
            IList<char> first = list1;
            for (int i = 0; i < 0x20; i++)
            {
                first.Add((char) ((ushort) i));
            }
            char[] second = new char[] { '\'' };
            foreach (char ch in first.Union<char>(second))
            {
                SingleQuoteCharEscapeFlags[ch] = true;
            }
            char[] chArray2 = new char[] { '"' };
            foreach (char ch2 in first.Union<char>(chArray2))
            {
                DoubleQuoteCharEscapeFlags[ch2] = true;
            }
            foreach (char ch3 in first.Union<char>(new char[] { '"', '\'', '<', '>', '&' }))
            {
                HtmlCharEscapeFlags[ch3] = true;
            }
        }

        public static bool[] GetCharEscapeFlags(StringEscapeHandling stringEscapeHandling, char quoteChar)
        {
            if (stringEscapeHandling == StringEscapeHandling.EscapeHtml)
            {
                return HtmlCharEscapeFlags;
            }
            if (quoteChar == '"')
            {
                return DoubleQuoteCharEscapeFlags;
            }
            return SingleQuoteCharEscapeFlags;
        }

        public static bool ShouldEscapeJavaScriptString(string s, bool[] charEscapeFlags)
        {
            if (s != null)
            {
                foreach (char ch in s)
                {
                    if ((ch >= charEscapeFlags.Length) || charEscapeFlags[ch])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static string ToEscapedJavaScriptString(string value, char delimiter, bool appendDelimiters, StringEscapeHandling stringEscapeHandling)
        {
            bool[] charEscapeFlags = GetCharEscapeFlags(stringEscapeHandling, delimiter);
            using (StringWriter writer = StringUtils.CreateStringWriter((value != null) ? value.Length : 0x10))
            {
                char[] writeBuffer = null;
                WriteEscapedJavaScriptString(writer, value, delimiter, appendDelimiters, charEscapeFlags, stringEscapeHandling, null, ref writeBuffer);
                return writer.ToString();
            }
        }

        public static void WriteEscapedJavaScriptString(TextWriter writer, string s, char delimiter, bool appendDelimiters, bool[] charEscapeFlags, StringEscapeHandling stringEscapeHandling, IArrayPool<char> bufferPool, ref char[] writeBuffer)
        {
            if (appendDelimiters)
            {
                writer.Write(delimiter);
            }
            if (s != null)
            {
                int sourceIndex = 0;
                for (int i = 0; i < s.Length; i++)
                {
                    char index = s[i];
                    if ((index >= charEscapeFlags.Length) || charEscapeFlags[index])
                    {
                        string str;
                        switch (index)
                        {
                            case '\x0085':
                                str = @"\u0085";
                                break;

                            case '\u2028':
                                str = @"\u2028";
                                break;

                            case '\u2029':
                                str = @"\u2029";
                                break;

                            case '\b':
                                str = @"\b";
                                break;

                            case '\t':
                                str = @"\t";
                                break;

                            case '\n':
                                str = @"\n";
                                break;

                            case '\f':
                                str = @"\f";
                                break;

                            case '\r':
                                str = @"\r";
                                break;

                            case '\\':
                                str = @"\\";
                                break;

                            default:
                                if ((index < charEscapeFlags.Length) || (stringEscapeHandling == StringEscapeHandling.EscapeNonAscii))
                                {
                                    if ((index == '\'') && (stringEscapeHandling != StringEscapeHandling.EscapeHtml))
                                    {
                                        str = @"\'";
                                    }
                                    else if ((index == '"') && (stringEscapeHandling != StringEscapeHandling.EscapeHtml))
                                    {
                                        str = "\\\"";
                                    }
                                    else
                                    {
                                        if ((writeBuffer == null) || (writeBuffer.Length < 6))
                                        {
                                            writeBuffer = BufferUtils.EnsureBufferSize(bufferPool, 6, writeBuffer);
                                        }
                                        StringUtils.ToCharAsUnicode(index, writeBuffer);
                                        str = "!";
                                    }
                                }
                                else
                                {
                                    str = null;
                                }
                                break;
                        }
                        if (str != null)
                        {
                            bool flag = string.Equals(str, "!");
                            if (i > sourceIndex)
                            {
                                int minSize = (i - sourceIndex) + (flag ? 6 : 0);
                                int destinationIndex = flag ? 6 : 0;
                                if ((writeBuffer == null) || (writeBuffer.Length < minSize))
                                {
                                    char[] destinationArray = BufferUtils.RentBuffer(bufferPool, minSize);
                                    if (flag)
                                    {
                                        Array.Copy(writeBuffer, destinationArray, 6);
                                    }
                                    BufferUtils.ReturnBuffer(bufferPool, writeBuffer);
                                    writeBuffer = destinationArray;
                                }
                                s.CopyTo(sourceIndex, writeBuffer, destinationIndex, minSize - destinationIndex);
                                writer.Write(writeBuffer, destinationIndex, minSize - destinationIndex);
                            }
                            sourceIndex = i + 1;
                            if (!flag)
                            {
                                writer.Write(str);
                            }
                            else
                            {
                                writer.Write(writeBuffer, 0, 6);
                            }
                        }
                    }
                }
                if (sourceIndex == 0)
                {
                    writer.Write(s);
                }
                else
                {
                    int size = s.Length - sourceIndex;
                    if ((writeBuffer == null) || (writeBuffer.Length < size))
                    {
                        writeBuffer = BufferUtils.EnsureBufferSize(bufferPool, size, writeBuffer);
                    }
                    s.CopyTo(sourceIndex, writeBuffer, 0, size);
                    writer.Write(writeBuffer, 0, size);
                }
            }
            if (appendDelimiters)
            {
                writer.Write(delimiter);
            }
        }
    }
}

