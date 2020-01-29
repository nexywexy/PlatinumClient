namespace Newtonsoft.Json.Utilities
{
    using System;

    internal class PropertyNameTable
    {
        private static readonly int HashCodeRandomizer = Environment.TickCount;
        private int _count;
        private Entry[] _entries;
        private int _mask = 0x1f;

        public PropertyNameTable()
        {
            this._entries = new Entry[this._mask + 1];
        }

        public string Add(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            int length = key.Length;
            if (length == 0)
            {
                return string.Empty;
            }
            int hashCode = length + HashCodeRandomizer;
            for (int i = 0; i < key.Length; i++)
            {
                hashCode += (hashCode << 7) ^ key[i];
            }
            hashCode -= hashCode >> 0x11;
            hashCode -= hashCode >> 11;
            hashCode -= hashCode >> 5;
            for (Entry entry = this._entries[hashCode & this._mask]; entry != null; entry = entry.Next)
            {
                if ((entry.HashCode == hashCode) && entry.Value.Equals(key))
                {
                    return entry.Value;
                }
            }
            return this.AddEntry(key, hashCode);
        }

        private string AddEntry(string str, int hashCode)
        {
            int index = hashCode & this._mask;
            Entry entry = new Entry(str, hashCode, this._entries[index]);
            this._entries[index] = entry;
            int num2 = this._count;
            this._count = num2 + 1;
            if (num2 == this._mask)
            {
                this.Grow();
            }
            return entry.Value;
        }

        public string Get(char[] key, int start, int length)
        {
            if (length == 0)
            {
                return string.Empty;
            }
            int num = length + HashCodeRandomizer;
            num += (num << 7) ^ key[start];
            int num2 = start + length;
            for (int i = start + 1; i < num2; i++)
            {
                num += (num << 7) ^ key[i];
            }
            num -= num >> 0x11;
            num -= num >> 11;
            num -= num >> 5;
            for (Entry entry = this._entries[num & this._mask]; entry != null; entry = entry.Next)
            {
                if ((entry.HashCode == num) && TextEquals(entry.Value, key, start, length))
                {
                    return entry.Value;
                }
            }
            return null;
        }

        private void Grow()
        {
            Entry[] entryArray = this._entries;
            int num = (this._mask * 2) + 1;
            Entry[] entryArray2 = new Entry[num + 1];
            for (int i = 0; i < entryArray.Length; i++)
            {
                Entry next;
                for (Entry entry = entryArray[i]; entry != null; entry = next)
                {
                    int index = entry.HashCode & num;
                    next = entry.Next;
                    entry.Next = entryArray2[index];
                    entryArray2[index] = entry;
                }
            }
            this._entries = entryArray2;
            this._mask = num;
        }

        private static bool TextEquals(string str1, char[] str2, int str2Start, int str2Length)
        {
            if (str1.Length != str2Length)
            {
                return false;
            }
            for (int i = 0; i < str1.Length; i++)
            {
                if (str1[i] != str2[str2Start + i])
                {
                    return false;
                }
            }
            return true;
        }

        private class Entry
        {
            internal readonly string Value;
            internal readonly int HashCode;
            internal PropertyNameTable.Entry Next;

            internal Entry(string value, int hashCode, PropertyNameTable.Entry next)
            {
                this.Value = value;
                this.HashCode = hashCode;
                this.Next = next;
            }
        }
    }
}

