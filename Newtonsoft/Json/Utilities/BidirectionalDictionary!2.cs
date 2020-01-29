namespace Newtonsoft.Json.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal class BidirectionalDictionary<TFirst, TSecond>
    {
        private readonly IDictionary<TFirst, TSecond> _firstToSecond;
        private readonly IDictionary<TSecond, TFirst> _secondToFirst;
        private readonly string _duplicateFirstErrorMessage;
        private readonly string _duplicateSecondErrorMessage;

        public BidirectionalDictionary() : this(EqualityComparer<TFirst>.Default, EqualityComparer<TSecond>.Default)
        {
        }

        public BidirectionalDictionary(IEqualityComparer<TFirst> firstEqualityComparer, IEqualityComparer<TSecond> secondEqualityComparer) : this(firstEqualityComparer, secondEqualityComparer, "Duplicate item already exists for '{0}'.", "Duplicate item already exists for '{0}'.")
        {
        }

        public BidirectionalDictionary(IEqualityComparer<TFirst> firstEqualityComparer, IEqualityComparer<TSecond> secondEqualityComparer, string duplicateFirstErrorMessage, string duplicateSecondErrorMessage)
        {
            this._firstToSecond = new Dictionary<TFirst, TSecond>(firstEqualityComparer);
            this._secondToFirst = new Dictionary<TSecond, TFirst>(secondEqualityComparer);
            this._duplicateFirstErrorMessage = duplicateFirstErrorMessage;
            this._duplicateSecondErrorMessage = duplicateSecondErrorMessage;
        }

        public void Set(TFirst first, TSecond second)
        {
            if (this._firstToSecond.TryGetValue(first, out TSecond local) && !local.Equals(second))
            {
                throw new ArgumentException(this._duplicateFirstErrorMessage.FormatWith(CultureInfo.InvariantCulture, first));
            }
            if (this._secondToFirst.TryGetValue(second, out TFirst local2) && !local2.Equals(first))
            {
                throw new ArgumentException(this._duplicateSecondErrorMessage.FormatWith(CultureInfo.InvariantCulture, second));
            }
            this._firstToSecond.Add(first, second);
            this._secondToFirst.Add(second, first);
        }

        public bool TryGetByFirst(TFirst first, out TSecond second) => 
            this._firstToSecond.TryGetValue(first, out second);

        public bool TryGetBySecond(TSecond second, out TFirst first) => 
            this._secondToFirst.TryGetValue(second, out first);
    }
}

