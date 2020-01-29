namespace Newtonsoft.Json.Bson
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class BsonBinaryWriter
    {
        private static readonly System.Text.Encoding Encoding = new UTF8Encoding(false);
        private readonly BinaryWriter _writer;
        private byte[] _largeByteBuffer;

        public BsonBinaryWriter(BinaryWriter writer)
        {
            this.DateTimeKindHandling = DateTimeKind.Utc;
            this._writer = writer;
        }

        private int CalculateSize(BsonToken t)
        {
            switch (t.Type)
            {
                case BsonType.Number:
                    return 8;

                case BsonType.String:
                {
                    BsonString str = (BsonString) t;
                    string s = (string) str.Value;
                    str.ByteCount = (s != null) ? Encoding.GetByteCount(s) : 0;
                    str.CalculatedSize = this.CalculateSizeWithLength(str.ByteCount, str.IncludeLength);
                    return str.CalculatedSize;
                }
                case BsonType.Object:
                {
                    BsonObject obj2 = (BsonObject) t;
                    int num = 4;
                    foreach (BsonProperty property in obj2)
                    {
                        int num2 = 1;
                        num2 += this.CalculateSize(property.Name);
                        num2 += this.CalculateSize(property.Value);
                        num += num2;
                    }
                    num++;
                    obj2.CalculatedSize = num;
                    return num;
                }
                case BsonType.Array:
                {
                    BsonArray array = (BsonArray) t;
                    int num3 = 4;
                    ulong i = 0L;
                    foreach (BsonToken token in array)
                    {
                        num3++;
                        num3 += this.CalculateSize(MathUtils.IntLength(i));
                        num3 += this.CalculateSize(token);
                        i += (ulong) 1L;
                    }
                    num3++;
                    array.CalculatedSize = num3;
                    return array.CalculatedSize;
                }
                case BsonType.Binary:
                {
                    BsonBinary binary1 = (BsonBinary) t;
                    byte[] buffer = (byte[]) binary1.Value;
                    binary1.CalculatedSize = 5 + buffer.Length;
                    return binary1.CalculatedSize;
                }
                case BsonType.Undefined:
                case BsonType.Null:
                    return 0;

                case BsonType.Oid:
                    return 12;

                case BsonType.Boolean:
                    return 1;

                case BsonType.Date:
                    return 8;

                case BsonType.Regex:
                {
                    BsonRegex regex = (BsonRegex) t;
                    int num5 = 0;
                    num5 += this.CalculateSize(regex.Pattern);
                    num5 += this.CalculateSize(regex.Options);
                    regex.CalculatedSize = num5;
                    return regex.CalculatedSize;
                }
                case BsonType.Integer:
                    return 4;

                case BsonType.Long:
                    return 8;
            }
            throw new ArgumentOutOfRangeException("t", "Unexpected token when writing BSON: {0}".FormatWith(CultureInfo.InvariantCulture, t.Type));
        }

        private int CalculateSize(int stringByteCount) => 
            (stringByteCount + 1);

        private int CalculateSizeWithLength(int stringByteCount, bool includeSize) => 
            ((includeSize ? 5 : 1) + stringByteCount);

        public void Close()
        {
            this._writer.Close();
        }

        public void Flush()
        {
            this._writer.Flush();
        }

        private void WriteString(string s, int byteCount, int? calculatedlengthPrefix)
        {
            if (calculatedlengthPrefix.HasValue)
            {
                this._writer.Write(calculatedlengthPrefix.GetValueOrDefault());
            }
            this.WriteUtf8Bytes(s, byteCount);
            this._writer.Write((byte) 0);
        }

        public void WriteToken(BsonToken t)
        {
            this.CalculateSize(t);
            this.WriteTokenInternal(t);
        }

        private void WriteTokenInternal(BsonToken t)
        {
            int? nullable;
            long num2;
            DateTime time;
            switch (t.Type)
            {
                case BsonType.Number:
                {
                    BsonValue value4 = (BsonValue) t;
                    this._writer.Write(Convert.ToDouble(value4.Value, CultureInfo.InvariantCulture));
                    return;
                }
                case BsonType.String:
                {
                    BsonString str = (BsonString) t;
                    this.WriteString((string) str.Value, str.ByteCount, new int?(str.CalculatedSize - 4));
                    return;
                }
                case BsonType.Object:
                {
                    BsonObject obj2 = (BsonObject) t;
                    this._writer.Write(obj2.CalculatedSize);
                    foreach (BsonProperty property in obj2)
                    {
                        this._writer.Write((sbyte) property.Value.Type);
                        nullable = null;
                        this.WriteString((string) property.Name.Value, property.Name.ByteCount, nullable);
                        this.WriteTokenInternal(property.Value);
                    }
                    this._writer.Write((byte) 0);
                    return;
                }
                case BsonType.Array:
                {
                    BsonArray array = (BsonArray) t;
                    this._writer.Write(array.CalculatedSize);
                    ulong i = 0L;
                    foreach (BsonToken token in array)
                    {
                        this._writer.Write((sbyte) token.Type);
                        nullable = null;
                        this.WriteString(i.ToString(CultureInfo.InvariantCulture), MathUtils.IntLength(i), nullable);
                        this.WriteTokenInternal(token);
                        i += (ulong) 1L;
                    }
                    this._writer.Write((byte) 0);
                    return;
                }
                case BsonType.Binary:
                {
                    BsonBinary binary = (BsonBinary) t;
                    byte[] buffer = (byte[]) binary.Value;
                    this._writer.Write(buffer.Length);
                    this._writer.Write((byte) binary.BinaryType);
                    this._writer.Write(buffer);
                    return;
                }
                case BsonType.Undefined:
                case BsonType.Null:
                    return;

                case BsonType.Oid:
                {
                    byte[] buffer2 = (byte[]) ((BsonValue) t).Value;
                    this._writer.Write(buffer2);
                    return;
                }
                case BsonType.Boolean:
                {
                    BsonValue value5 = (BsonValue) t;
                    this._writer.Write((bool) value5.Value);
                    return;
                }
                case BsonType.Date:
                {
                    BsonValue value6 = (BsonValue) t;
                    num2 = 0L;
                    if (!(value6.Value is DateTime))
                    {
                        DateTimeOffset offset = (DateTimeOffset) value6.Value;
                        num2 = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(offset.UtcDateTime, offset.Offset);
                        goto Label_02E9;
                    }
                    time = (DateTime) value6.Value;
                    if (this.DateTimeKindHandling != DateTimeKind.Utc)
                    {
                        if (this.DateTimeKindHandling == DateTimeKind.Local)
                        {
                            time = time.ToLocalTime();
                        }
                        break;
                    }
                    time = time.ToUniversalTime();
                    break;
                }
                case BsonType.Regex:
                {
                    BsonRegex regex = (BsonRegex) t;
                    nullable = null;
                    this.WriteString((string) regex.Pattern.Value, regex.Pattern.ByteCount, nullable);
                    this.WriteString((string) regex.Options.Value, regex.Options.ByteCount, null);
                    return;
                }
                case BsonType.Integer:
                {
                    BsonValue value2 = (BsonValue) t;
                    this._writer.Write(Convert.ToInt32(value2.Value, CultureInfo.InvariantCulture));
                    return;
                }
                case BsonType.Long:
                {
                    BsonValue value3 = (BsonValue) t;
                    this._writer.Write(Convert.ToInt64(value3.Value, CultureInfo.InvariantCulture));
                    return;
                }
                default:
                    throw new ArgumentOutOfRangeException("t", "Unexpected token when writing BSON: {0}".FormatWith(CultureInfo.InvariantCulture, t.Type));
            }
            num2 = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(time, false);
        Label_02E9:
            this._writer.Write(num2);
        }

        public void WriteUtf8Bytes(string s, int byteCount)
        {
            if (s != null)
            {
                if (this._largeByteBuffer == null)
                {
                    this._largeByteBuffer = new byte[0x100];
                }
                if (byteCount <= 0x100)
                {
                    Encoding.GetBytes(s, 0, s.Length, this._largeByteBuffer, 0);
                    this._writer.Write(this._largeByteBuffer, 0, byteCount);
                }
                else
                {
                    byte[] bytes = Encoding.GetBytes(s);
                    this._writer.Write(bytes);
                }
            }
        }

        public DateTimeKind DateTimeKindHandling { get; set; }
    }
}

