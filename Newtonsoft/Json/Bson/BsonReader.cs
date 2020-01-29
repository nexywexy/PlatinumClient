namespace Newtonsoft.Json.Bson
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public class BsonReader : JsonReader
    {
        private const int MaxCharBytesSize = 0x80;
        private static readonly byte[] SeqRange1;
        private static readonly byte[] SeqRange2;
        private static readonly byte[] SeqRange3;
        private static readonly byte[] SeqRange4;
        private readonly BinaryReader _reader;
        private readonly List<ContainerContext> _stack;
        private byte[] _byteBuffer;
        private char[] _charBuffer;
        private BsonType _currentElementType;
        private BsonReaderState _bsonReaderState;
        private ContainerContext _currentContext;
        private bool _readRootValueAsArray;
        private bool _jsonNet35BinaryCompatibility;
        private DateTimeKind _dateTimeKindHandling;

        static BsonReader()
        {
            byte[] buffer1 = new byte[2];
            buffer1[1] = 0x7f;
            SeqRange1 = buffer1;
            SeqRange2 = new byte[] { 0xc2, 0xdf };
            SeqRange3 = new byte[] { 0xe0, 0xef };
            SeqRange4 = new byte[] { 240, 0xf4 };
        }

        public BsonReader(BinaryReader reader) : this(reader, false, DateTimeKind.Local)
        {
        }

        public BsonReader(Stream stream) : this(stream, false, DateTimeKind.Local)
        {
        }

        public BsonReader(BinaryReader reader, bool readRootValueAsArray, DateTimeKind dateTimeKindHandling)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            this._reader = reader;
            this._stack = new List<ContainerContext>();
            this._readRootValueAsArray = readRootValueAsArray;
            this._dateTimeKindHandling = dateTimeKindHandling;
        }

        public BsonReader(Stream stream, bool readRootValueAsArray, DateTimeKind dateTimeKindHandling)
        {
            ValidationUtils.ArgumentNotNull(stream, "stream");
            this._reader = new BinaryReader(stream);
            this._stack = new List<ContainerContext>();
            this._readRootValueAsArray = readRootValueAsArray;
            this._dateTimeKindHandling = dateTimeKindHandling;
        }

        private int BytesInSequence(byte b)
        {
            if (b <= SeqRange1[1])
            {
                return 1;
            }
            if ((b >= SeqRange2[0]) && (b <= SeqRange2[1]))
            {
                return 2;
            }
            if ((b >= SeqRange3[0]) && (b <= SeqRange3[1]))
            {
                return 3;
            }
            if ((b >= SeqRange4[0]) && (b <= SeqRange4[1]))
            {
                return 4;
            }
            return 0;
        }

        public override void Close()
        {
            base.Close();
            if (base.CloseInput && (this._reader != null))
            {
                this._reader.Close();
            }
        }

        private void EnsureBuffers()
        {
            if (this._byteBuffer == null)
            {
                this._byteBuffer = new byte[0x80];
            }
            if (this._charBuffer == null)
            {
                int maxCharCount = Encoding.UTF8.GetMaxCharCount(0x80);
                this._charBuffer = new char[maxCharCount];
            }
        }

        private int GetLastFullCharStop(int start)
        {
            int index = start;
            int num2 = 0;
            while (index >= 0)
            {
                num2 = this.BytesInSequence(this._byteBuffer[index]);
                if (num2 == 0)
                {
                    index--;
                }
                else
                {
                    if (num2 != 1)
                    {
                        index--;
                    }
                    break;
                }
            }
            if (num2 == (start - index))
            {
                return start;
            }
            return index;
        }

        private string GetString(int length)
        {
            if (length == 0)
            {
                return string.Empty;
            }
            this.EnsureBuffers();
            StringBuilder builder = null;
            int num = 0;
            int index = 0;
            do
            {
                int count = ((length - num) > (0x80 - index)) ? (0x80 - index) : (length - num);
                int byteCount = this._reader.Read(this._byteBuffer, index, count);
                if (byteCount == 0)
                {
                    throw new EndOfStreamException("Unable to read beyond the end of the stream.");
                }
                num += byteCount;
                byteCount += index;
                if (byteCount == length)
                {
                    return new string(this._charBuffer, 0, Encoding.UTF8.GetChars(this._byteBuffer, 0, byteCount, this._charBuffer, 0));
                }
                int lastFullCharStop = this.GetLastFullCharStop(byteCount - 1);
                if (builder == null)
                {
                    builder = new StringBuilder(length);
                }
                int charCount = Encoding.UTF8.GetChars(this._byteBuffer, 0, lastFullCharStop + 1, this._charBuffer, 0);
                builder.Append(this._charBuffer, 0, charCount);
                if (lastFullCharStop < (byteCount - 1))
                {
                    index = (byteCount - lastFullCharStop) - 1;
                    Array.Copy(this._byteBuffer, lastFullCharStop + 1, this._byteBuffer, 0, index);
                }
                else
                {
                    index = 0;
                }
            }
            while (num < length);
            return builder.ToString();
        }

        private void MovePosition(int count)
        {
            this._currentContext.Position += count;
        }

        private void PopContext()
        {
            this._stack.RemoveAt(this._stack.Count - 1);
            if (this._stack.Count == 0)
            {
                this._currentContext = null;
            }
            else
            {
                this._currentContext = this._stack[this._stack.Count - 1];
            }
        }

        private void PushContext(ContainerContext newContext)
        {
            this._stack.Add(newContext);
            this._currentContext = newContext;
        }

        public override bool Read()
        {
            try
            {
                bool flag;
                switch (this._bsonReaderState)
                {
                    case BsonReaderState.Normal:
                        flag = this.ReadNormal();
                        break;

                    case BsonReaderState.ReferenceStart:
                    case BsonReaderState.ReferenceRef:
                    case BsonReaderState.ReferenceId:
                        flag = this.ReadReference();
                        break;

                    case BsonReaderState.CodeWScopeStart:
                    case BsonReaderState.CodeWScopeCode:
                    case BsonReaderState.CodeWScopeScope:
                    case BsonReaderState.CodeWScopeScopeObject:
                    case BsonReaderState.CodeWScopeScopeEnd:
                        flag = this.ReadCodeWScope();
                        break;

                    default:
                        throw JsonReaderException.Create(this, "Unexpected state: {0}".FormatWith(CultureInfo.InvariantCulture, this._bsonReaderState));
                }
                if (!flag)
                {
                    base.SetToken(JsonToken.None);
                    return false;
                }
                return true;
            }
            catch (EndOfStreamException)
            {
                base.SetToken(JsonToken.None);
                return false;
            }
        }

        private byte[] ReadBinary(out BsonBinaryType binaryType)
        {
            int count = this.ReadInt32();
            binaryType = (BsonBinaryType) this.ReadByte();
            if ((binaryType == BsonBinaryType.BinaryOld) && !this._jsonNet35BinaryCompatibility)
            {
                count = this.ReadInt32();
            }
            return this.ReadBytes(count);
        }

        private byte ReadByte()
        {
            this.MovePosition(1);
            return this._reader.ReadByte();
        }

        private byte[] ReadBytes(int count)
        {
            this.MovePosition(count);
            return this._reader.ReadBytes(count);
        }

        private bool ReadCodeWScope()
        {
            switch (this._bsonReaderState)
            {
                case BsonReaderState.CodeWScopeStart:
                    base.SetToken(JsonToken.PropertyName, "$code");
                    this._bsonReaderState = BsonReaderState.CodeWScopeCode;
                    return true;

                case BsonReaderState.CodeWScopeCode:
                    this.ReadInt32();
                    base.SetToken(JsonToken.String, this.ReadLengthString());
                    this._bsonReaderState = BsonReaderState.CodeWScopeScope;
                    return true;

                case BsonReaderState.CodeWScopeScope:
                    if (base.CurrentState != JsonReader.State.PostValue)
                    {
                        base.SetToken(JsonToken.StartObject);
                        this._bsonReaderState = BsonReaderState.CodeWScopeScopeObject;
                        ContainerContext newContext = new ContainerContext(BsonType.Object);
                        this.PushContext(newContext);
                        newContext.Length = this.ReadInt32();
                        return true;
                    }
                    base.SetToken(JsonToken.PropertyName, "$scope");
                    return true;

                case BsonReaderState.CodeWScopeScopeObject:
                {
                    bool flag1 = this.ReadNormal();
                    if (flag1 && (this.TokenType == JsonToken.EndObject))
                    {
                        this._bsonReaderState = BsonReaderState.CodeWScopeScopeEnd;
                    }
                    return flag1;
                }
                case BsonReaderState.CodeWScopeScopeEnd:
                    base.SetToken(JsonToken.EndObject);
                    this._bsonReaderState = BsonReaderState.Normal;
                    return true;
            }
            throw new ArgumentOutOfRangeException();
        }

        private double ReadDouble()
        {
            this.MovePosition(8);
            return this._reader.ReadDouble();
        }

        private string ReadElement()
        {
            this._currentElementType = this.ReadType();
            return this.ReadString();
        }

        private int ReadInt32()
        {
            this.MovePosition(4);
            return this._reader.ReadInt32();
        }

        private long ReadInt64()
        {
            this.MovePosition(8);
            return this._reader.ReadInt64();
        }

        private string ReadLengthString()
        {
            int count = this.ReadInt32();
            this.MovePosition(count);
            this._reader.ReadByte();
            return this.GetString(count - 1);
        }

        private bool ReadNormal()
        {
            switch (base.CurrentState)
            {
                case JsonReader.State.Start:
                {
                    JsonToken newToken = !this._readRootValueAsArray ? JsonToken.StartObject : JsonToken.StartArray;
                    base.SetToken(newToken);
                    ContainerContext newContext = new ContainerContext(!this._readRootValueAsArray ? BsonType.Object : BsonType.Array);
                    this.PushContext(newContext);
                    newContext.Length = this.ReadInt32();
                    return true;
                }
                case JsonReader.State.Complete:
                case JsonReader.State.Closed:
                    return false;

                case JsonReader.State.Property:
                    this.ReadType(this._currentElementType);
                    return true;

                case JsonReader.State.ObjectStart:
                case JsonReader.State.ArrayStart:
                case JsonReader.State.PostValue:
                {
                    ContainerContext context2 = this._currentContext;
                    if (context2 != null)
                    {
                        int num = context2.Length - 1;
                        if (context2.Position < num)
                        {
                            if (context2.Type == BsonType.Array)
                            {
                                this.ReadElement();
                                this.ReadType(this._currentElementType);
                                return true;
                            }
                            base.SetToken(JsonToken.PropertyName, this.ReadElement());
                            return true;
                        }
                        if (context2.Position != num)
                        {
                            throw JsonReaderException.Create(this, "Read past end of current container context.");
                        }
                        if (this.ReadByte() != 0)
                        {
                            throw JsonReaderException.Create(this, "Unexpected end of object byte value.");
                        }
                        this.PopContext();
                        if (this._currentContext != null)
                        {
                            this.MovePosition(context2.Length);
                        }
                        JsonToken newToken = (context2.Type == BsonType.Object) ? JsonToken.EndObject : JsonToken.EndArray;
                        base.SetToken(newToken);
                        return true;
                    }
                    return false;
                }
                case JsonReader.State.ConstructorStart:
                case JsonReader.State.Constructor:
                case JsonReader.State.Error:
                case JsonReader.State.Finished:
                    return false;
            }
            throw new ArgumentOutOfRangeException();
        }

        private bool ReadReference()
        {
            JsonReader.State currentState = base.CurrentState;
            if (currentState != JsonReader.State.Property)
            {
                if (currentState == JsonReader.State.ObjectStart)
                {
                    base.SetToken(JsonToken.PropertyName, "$ref");
                    this._bsonReaderState = BsonReaderState.ReferenceRef;
                    return true;
                }
                if (currentState != JsonReader.State.PostValue)
                {
                    throw JsonReaderException.Create(this, "Unexpected state when reading BSON reference: " + base.CurrentState);
                }
            }
            else
            {
                if (this._bsonReaderState == BsonReaderState.ReferenceRef)
                {
                    base.SetToken(JsonToken.String, this.ReadLengthString());
                    return true;
                }
                if (this._bsonReaderState != BsonReaderState.ReferenceId)
                {
                    throw JsonReaderException.Create(this, "Unexpected state when reading BSON reference: " + this._bsonReaderState);
                }
                base.SetToken(JsonToken.Bytes, this.ReadBytes(12));
                return true;
            }
            if (this._bsonReaderState == BsonReaderState.ReferenceRef)
            {
                base.SetToken(JsonToken.PropertyName, "$id");
                this._bsonReaderState = BsonReaderState.ReferenceId;
                return true;
            }
            if (this._bsonReaderState != BsonReaderState.ReferenceId)
            {
                throw JsonReaderException.Create(this, "Unexpected state when reading BSON reference: " + this._bsonReaderState);
            }
            base.SetToken(JsonToken.EndObject);
            this._bsonReaderState = BsonReaderState.Normal;
            return true;
        }

        private string ReadString()
        {
            this.EnsureBuffers();
            StringBuilder builder = null;
            int num = 0;
            int length = 0;
            while (true)
            {
                byte num4;
                int num3 = length;
                while ((num3 < 0x80) && ((num4 = this._reader.ReadByte()) > 0))
                {
                    this._byteBuffer[num3++] = num4;
                }
                int byteCount = num3 - length;
                num += byteCount;
                if ((num3 < 0x80) && (builder == null))
                {
                    int num6 = Encoding.UTF8.GetChars(this._byteBuffer, 0, byteCount, this._charBuffer, 0);
                    this.MovePosition(num + 1);
                    return new string(this._charBuffer, 0, num6);
                }
                int lastFullCharStop = this.GetLastFullCharStop(num3 - 1);
                int charCount = Encoding.UTF8.GetChars(this._byteBuffer, 0, lastFullCharStop + 1, this._charBuffer, 0);
                if (builder == null)
                {
                    builder = new StringBuilder(0x100);
                }
                builder.Append(this._charBuffer, 0, charCount);
                if (lastFullCharStop < (byteCount - 1))
                {
                    length = (byteCount - lastFullCharStop) - 1;
                    Array.Copy(this._byteBuffer, lastFullCharStop + 1, this._byteBuffer, 0, length);
                }
                else
                {
                    if (num3 < 0x80)
                    {
                        this.MovePosition(num + 1);
                        return builder.ToString();
                    }
                    length = 0;
                }
            }
        }

        private BsonType ReadType()
        {
            this.MovePosition(1);
            return (BsonType) this._reader.ReadSByte();
        }

        private void ReadType(BsonType type)
        {
            DateTime time2;
            switch (type)
            {
                case BsonType.Number:
                {
                    double num = this.ReadDouble();
                    if (base._floatParseHandling != FloatParseHandling.Decimal)
                    {
                        base.SetToken(JsonToken.Float, num);
                        return;
                    }
                    base.SetToken(JsonToken.Float, Convert.ToDecimal(num, CultureInfo.InvariantCulture));
                    return;
                }
                case BsonType.String:
                case BsonType.Symbol:
                    base.SetToken(JsonToken.String, this.ReadLengthString());
                    return;

                case BsonType.Object:
                {
                    base.SetToken(JsonToken.StartObject);
                    ContainerContext newContext = new ContainerContext(BsonType.Object);
                    this.PushContext(newContext);
                    newContext.Length = this.ReadInt32();
                    return;
                }
                case BsonType.Array:
                {
                    base.SetToken(JsonToken.StartArray);
                    ContainerContext newContext = new ContainerContext(BsonType.Array);
                    this.PushContext(newContext);
                    newContext.Length = this.ReadInt32();
                    return;
                }
                case BsonType.Binary:
                {
                    byte[] b = this.ReadBinary(out BsonBinaryType type2);
                    object obj2 = (type2 != BsonBinaryType.Uuid) ? ((object) b) : ((object) new Guid(b));
                    base.SetToken(JsonToken.Bytes, obj2);
                    return;
                }
                case BsonType.Undefined:
                    base.SetToken(JsonToken.Undefined);
                    return;

                case BsonType.Oid:
                {
                    byte[] buffer2 = this.ReadBytes(12);
                    base.SetToken(JsonToken.Bytes, buffer2);
                    return;
                }
                case BsonType.Boolean:
                {
                    bool flag = Convert.ToBoolean(this.ReadByte());
                    base.SetToken(JsonToken.Boolean, flag);
                    return;
                }
                case BsonType.Date:
                {
                    DateTime time = DateTimeUtils.ConvertJavaScriptTicksToDateTime(this.ReadInt64());
                    switch (this.DateTimeKindHandling)
                    {
                        case DateTimeKind.Unspecified:
                            time2 = DateTime.SpecifyKind(time, DateTimeKind.Unspecified);
                            goto Label_019C;

                        case DateTimeKind.Local:
                            time2 = time.ToLocalTime();
                            goto Label_019C;
                    }
                    time2 = time;
                    break;
                }
                case BsonType.Null:
                    base.SetToken(JsonToken.Null);
                    return;

                case BsonType.Regex:
                {
                    string str = this.ReadString();
                    string str2 = this.ReadString();
                    string str3 = "/" + str + "/" + str2;
                    base.SetToken(JsonToken.String, str3);
                    return;
                }
                case BsonType.Reference:
                    base.SetToken(JsonToken.StartObject);
                    this._bsonReaderState = BsonReaderState.ReferenceStart;
                    return;

                case BsonType.Code:
                    base.SetToken(JsonToken.String, this.ReadLengthString());
                    return;

                case BsonType.CodeWScope:
                    base.SetToken(JsonToken.StartObject);
                    this._bsonReaderState = BsonReaderState.CodeWScopeStart;
                    return;

                case BsonType.Integer:
                    base.SetToken(JsonToken.Integer, (long) this.ReadInt32());
                    return;

                case BsonType.TimeStamp:
                case BsonType.Long:
                    base.SetToken(JsonToken.Integer, this.ReadInt64());
                    return;

                default:
                    throw new ArgumentOutOfRangeException("type", "Unexpected BsonType value: " + type);
            }
        Label_019C:
            base.SetToken(JsonToken.Date, time2);
        }

        [Obsolete("JsonNet35BinaryCompatibility will be removed in a future version of Json.NET.")]
        public bool JsonNet35BinaryCompatibility
        {
            get => 
                this._jsonNet35BinaryCompatibility;
            set => 
                (this._jsonNet35BinaryCompatibility = value);
        }

        public bool ReadRootValueAsArray
        {
            get => 
                this._readRootValueAsArray;
            set => 
                (this._readRootValueAsArray = value);
        }

        public DateTimeKind DateTimeKindHandling
        {
            get => 
                this._dateTimeKindHandling;
            set => 
                (this._dateTimeKindHandling = value);
        }

        private enum BsonReaderState
        {
            Normal,
            ReferenceStart,
            ReferenceRef,
            ReferenceId,
            CodeWScopeStart,
            CodeWScopeCode,
            CodeWScopeScope,
            CodeWScopeScopeObject,
            CodeWScopeScopeEnd
        }

        private class ContainerContext
        {
            public readonly BsonType Type;
            public int Length;
            public int Position;

            public ContainerContext(BsonType type)
            {
                this.Type = type;
            }
        }
    }
}

