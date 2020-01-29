namespace Newtonsoft.Json.Linq
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct JEnumerable<T> : IJEnumerable<T>, IEnumerable<T>, IEnumerable, IEquatable<JEnumerable<T>> where T: JToken
    {
        public static readonly JEnumerable<T> Empty;
        private readonly IEnumerable<T> _enumerable;
        public JEnumerable(IEnumerable<T> enumerable)
        {
            ValidationUtils.ArgumentNotNull(enumerable, "enumerable");
            this._enumerable = enumerable;
        }

        public IEnumerator<T> GetEnumerator() => 
            this._enumerable?.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public IJEnumerable<JToken> this[object key]
        {
            get
            {
                if (this._enumerable == null)
                {
                    return JEnumerable<JToken>.Empty;
                }
                return new JEnumerable<JToken>(this._enumerable.Values<T, JToken>(key));
            }
        }
        public bool Equals(JEnumerable<T> other) => 
            object.Equals(this._enumerable, other._enumerable);

        public override bool Equals(object obj) => 
            ((obj is JEnumerable<T>) && this.Equals((JEnumerable<T>) obj));

        public override int GetHashCode() => 
            this._enumerable?.GetHashCode();

        static JEnumerable()
        {
            JEnumerable<T>.Empty = new JEnumerable<T>(Enumerable.Empty<T>());
        }
    }
}

