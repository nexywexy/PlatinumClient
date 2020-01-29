namespace Newtonsoft.Json.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    internal class ThreadSafeStore<TKey, TValue>
    {
        private readonly object _lock;
        private Dictionary<TKey, TValue> _store;
        private readonly Func<TKey, TValue> _creator;

        public ThreadSafeStore(Func<TKey, TValue> creator)
        {
            this._lock = new object();
            if (creator == null)
            {
                throw new ArgumentNullException("creator");
            }
            this._creator = creator;
            this._store = new Dictionary<TKey, TValue>();
        }

        private TValue AddValue(TKey key)
        {
            TValue local = this._creator(key);
            object obj2 = this._lock;
            lock (obj2)
            {
                if (this._store == null)
                {
                    this._store = new Dictionary<TKey, TValue>();
                    this._store[key] = local;
                }
                else
                {
                    if (this._store.TryGetValue(key, out TValue local2))
                    {
                        return local2;
                    }
                    Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(this._store) {
                        [key] = local
                    };
                    Thread.MemoryBarrier();
                    this._store = dictionary;
                }
                return local;
            }
        }

        public TValue Get(TKey key)
        {
            if (!this._store.TryGetValue(key, out TValue local))
            {
                return this.AddValue(key);
            }
            return local;
        }
    }
}

