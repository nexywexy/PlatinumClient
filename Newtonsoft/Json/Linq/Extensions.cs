namespace Newtonsoft.Json.Linq
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public static class Extensions
    {
        public static IJEnumerable<JToken> Ancestors<T>(this IEnumerable<T> source) where T: JToken
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            if (<>c__0<T>.<>9__0_0 == null)
            {
            }
            return source.SelectMany<T, JToken>((<>c__0<T>.<>9__0_0 = new Func<T, IEnumerable<JToken>>(<>c__0<T>.<>9.<Ancestors>b__0_0))).AsJEnumerable();
        }

        public static IJEnumerable<JToken> AncestorsAndSelf<T>(this IEnumerable<T> source) where T: JToken
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            if (<>c__1<T>.<>9__1_0 == null)
            {
            }
            return source.SelectMany<T, JToken>((<>c__1<T>.<>9__1_0 = new Func<T, IEnumerable<JToken>>(<>c__1<T>.<>9.<AncestorsAndSelf>b__1_0))).AsJEnumerable();
        }

        public static IJEnumerable<JToken> AsJEnumerable(this IEnumerable<JToken> source) => 
            source.AsJEnumerable<JToken>();

        public static IJEnumerable<T> AsJEnumerable<T>(this IEnumerable<T> source) where T: JToken
        {
            if (source == null)
            {
                return null;
            }
            if (source is IJEnumerable<T>)
            {
                return (IJEnumerable<T>) source;
            }
            return new JEnumerable<T>(source);
        }

        public static IJEnumerable<JToken> Children<T>(this IEnumerable<T> source) where T: JToken => 
            source.Children<T, JToken>().AsJEnumerable();

        public static IEnumerable<U> Children<T, U>(this IEnumerable<T> source) where T: JToken
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            if (<>c__13<T, U>.<>9__13_0 == null)
            {
            }
            return source.SelectMany<T, JToken>((<>c__13<T, U>.<>9__13_0 = new Func<T, IEnumerable<JToken>>(<>c__13<T, U>.<>9.<Children>b__13_0))).Convert<JToken, U>();
        }

        [IteratorStateMachine(typeof(<Convert>d__14))]
        internal static IEnumerable<U> Convert<T, U>(this IEnumerable<T> source) where T: JToken => 
            new <Convert>d__14<T, U>(-2) { <>3__source = source };

        internal static U Convert<T, U>(this T token) where T: JToken
        {
            if (token == null)
            {
                return default(U);
            }
            if (((token is U) && (typeof(U) != typeof(IComparable))) && (typeof(U) != typeof(IFormattable)))
            {
                return (U) token;
            }
            JValue value2 = token as JValue;
            if (value2 == null)
            {
                throw new InvalidCastException("Cannot cast {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, token.GetType(), typeof(T)));
            }
            if (value2.Value is U)
            {
                return (U) value2.Value;
            }
            Type t = typeof(U);
            if (ReflectionUtils.IsNullableType(t))
            {
                if (value2.Value == null)
                {
                    return default(U);
                }
                t = Nullable.GetUnderlyingType(t);
            }
            return (U) System.Convert.ChangeType(value2.Value, t, CultureInfo.InvariantCulture);
        }

        public static IJEnumerable<JToken> Descendants<T>(this IEnumerable<T> source) where T: JContainer
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            if (<>c__2<T>.<>9__2_0 == null)
            {
            }
            return source.SelectMany<T, JToken>((<>c__2<T>.<>9__2_0 = new Func<T, IEnumerable<JToken>>(<>c__2<T>.<>9.<Descendants>b__2_0))).AsJEnumerable();
        }

        public static IJEnumerable<JToken> DescendantsAndSelf<T>(this IEnumerable<T> source) where T: JContainer
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            if (<>c__3<T>.<>9__3_0 == null)
            {
            }
            return source.SelectMany<T, JToken>((<>c__3<T>.<>9__3_0 = new Func<T, IEnumerable<JToken>>(<>c__3<T>.<>9.<DescendantsAndSelf>b__3_0))).AsJEnumerable();
        }

        public static IJEnumerable<JProperty> Properties(this IEnumerable<JObject> source)
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            if (<>c.<>9__4_0 == null)
            {
            }
            return source.SelectMany<JObject, JProperty>((<>c.<>9__4_0 = new Func<JObject, IEnumerable<JProperty>>(<>c.<>9.<Properties>b__4_0))).AsJEnumerable<JProperty>();
        }

        public static U Value<U>(this IEnumerable<JToken> value) => 
            value.Value<JToken, U>();

        public static U Value<T, U>(this IEnumerable<T> value) where T: JToken
        {
            ValidationUtils.ArgumentNotNull(value, "value");
            JToken token = value as JToken;
            if (token == null)
            {
                throw new ArgumentException("Source value must be a JToken.");
            }
            return token.Convert<JToken, U>();
        }

        public static IJEnumerable<JToken> Values(this IEnumerable<JToken> source) => 
            source.Values(null);

        public static IEnumerable<U> Values<U>(this IEnumerable<JToken> source) => 
            source.Values<JToken, U>(null);

        public static IJEnumerable<JToken> Values(this IEnumerable<JToken> source, object key) => 
            source.Values<JToken, JToken>(key).AsJEnumerable();

        public static IEnumerable<U> Values<U>(this IEnumerable<JToken> source, object key) => 
            source.Values<JToken, U>(key);

        [IteratorStateMachine(typeof(<Values>d__11))]
        internal static IEnumerable<U> Values<T, U>(this IEnumerable<T> source, object key) where T: JToken
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            foreach (JToken <token>5__1 in source)
            {
                if (key == null)
                {
                    if (<token>5__1 is JValue)
                    {
                        yield return ((JValue) <token>5__1).Convert<JValue, U>();
                    }
                    else
                    {
                        IEnumerator<JToken> <>7__wrap2;
                        using (<>7__wrap2 = <token>5__1.Children().GetEnumerator())
                        {
                            while (<>7__wrap2.MoveNext())
                            {
                                yield return <>7__wrap2.Current.Convert<JToken, U>();
                            }
                        }
                        <>7__wrap2 = null;
                    }
                }
                else
                {
                    JToken token = <token>5__1[key];
                    if (token != null)
                    {
                        yield return token.Convert<JToken, U>();
                    }
                }
                <token>5__1 = null;
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly Extensions.<>c <>9 = new Extensions.<>c();
            public static Func<JObject, IEnumerable<JProperty>> <>9__4_0;

            internal IEnumerable<JProperty> <Properties>b__4_0(JObject d) => 
                d.Properties();
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__0<T> where T: JToken
        {
            public static readonly Extensions.<>c__0<T> <>9;
            public static Func<T, IEnumerable<JToken>> <>9__0_0;

            static <>c__0()
            {
                Extensions.<>c__0<T>.<>9 = new Extensions.<>c__0<T>();
            }

            internal IEnumerable<JToken> <Ancestors>b__0_0(T j) => 
                j.Ancestors();
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__1<T> where T: JToken
        {
            public static readonly Extensions.<>c__1<T> <>9;
            public static Func<T, IEnumerable<JToken>> <>9__1_0;

            static <>c__1()
            {
                Extensions.<>c__1<T>.<>9 = new Extensions.<>c__1<T>();
            }

            internal IEnumerable<JToken> <AncestorsAndSelf>b__1_0(T j) => 
                j.AncestorsAndSelf();
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__13<T, U> where T: JToken
        {
            public static readonly Extensions.<>c__13<T, U> <>9;
            public static Func<T, IEnumerable<JToken>> <>9__13_0;

            static <>c__13()
            {
                Extensions.<>c__13<T, U>.<>9 = new Extensions.<>c__13<T, U>();
            }

            internal IEnumerable<JToken> <Children>b__13_0(T c) => 
                c.Children();
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__2<T> where T: JContainer
        {
            public static readonly Extensions.<>c__2<T> <>9;
            public static Func<T, IEnumerable<JToken>> <>9__2_0;

            static <>c__2()
            {
                Extensions.<>c__2<T>.<>9 = new Extensions.<>c__2<T>();
            }

            internal IEnumerable<JToken> <Descendants>b__2_0(T j) => 
                j.Descendants();
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__3<T> where T: JContainer
        {
            public static readonly Extensions.<>c__3<T> <>9;
            public static Func<T, IEnumerable<JToken>> <>9__3_0;

            static <>c__3()
            {
                Extensions.<>c__3<T>.<>9 = new Extensions.<>c__3<T>();
            }

            internal IEnumerable<JToken> <DescendantsAndSelf>b__3_0(T j) => 
                j.DescendantsAndSelf();
        }

        [CompilerGenerated]
        private sealed class <Convert>d__14<T, U> : IEnumerable<U>, IEnumerable, IEnumerator<U>, IDisposable, IEnumerator where T: JToken
        {
            private int <>1__state;
            private U <>2__current;
            private int <>l__initialThreadId;
            private IEnumerable<T> source;
            public IEnumerable<T> <>3__source;
            private IEnumerator<T> <>7__wrap1;

            [DebuggerHidden]
            public <Convert>d__14(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Environment.CurrentManagedThreadId;
            }

            private void <>m__Finally1()
            {
                this.<>1__state = -1;
                if (this.<>7__wrap1 != null)
                {
                    this.<>7__wrap1.Dispose();
                }
            }

            private bool MoveNext()
            {
                try
                {
                    int num = this.<>1__state;
                    if (num == 0)
                    {
                        this.<>1__state = -1;
                        ValidationUtils.ArgumentNotNull(this.source, "source");
                        this.<>7__wrap1 = this.source.GetEnumerator();
                        this.<>1__state = -3;
                        while (this.<>7__wrap1.MoveNext())
                        {
                            this.<>2__current = this.<>7__wrap1.Current.Convert<JToken, U>();
                            this.<>1__state = 1;
                            return true;
                        Label_007B:
                            this.<>1__state = -3;
                        }
                        this.<>m__Finally1();
                        this.<>7__wrap1 = null;
                        return false;
                    }
                    if (num != 1)
                    {
                        return false;
                    }
                    goto Label_007B;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
            }

            [DebuggerHidden]
            IEnumerator<U> IEnumerable<U>.GetEnumerator()
            {
                Extensions.<Convert>d__14<T, U> d__;
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.CurrentManagedThreadId))
                {
                    this.<>1__state = 0;
                    d__ = (Extensions.<Convert>d__14<T, U>) this;
                }
                else
                {
                    d__ = new Extensions.<Convert>d__14<T, U>(0);
                }
                d__.source = this.<>3__source;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<U>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
                switch (this.<>1__state)
                {
                    case -3:
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            this.<>m__Finally1();
                        }
                        break;
                }
            }

            U IEnumerator<U>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }

        [CompilerGenerated]
        private sealed class <Values>d__11<T, U> : IEnumerable<U>, IEnumerable, IEnumerator<U>, IDisposable, IEnumerator where T: JToken
        {
            private int <>1__state;
            private U <>2__current;
            private int <>l__initialThreadId;
            private IEnumerable<T> source;
            public IEnumerable<T> <>3__source;
            private object key;
            public object <>3__key;
            private JToken <token>5__1;
            private IEnumerator<T> <>7__wrap1;
            private IEnumerator<JToken> <>7__wrap2;

            [DebuggerHidden]
            public <Values>d__11(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Environment.CurrentManagedThreadId;
            }

            private void <>m__Finally1()
            {
                this.<>1__state = -1;
                if (this.<>7__wrap1 != null)
                {
                    this.<>7__wrap1.Dispose();
                }
            }

            private void <>m__Finally2()
            {
                this.<>1__state = -3;
                if (this.<>7__wrap2 != null)
                {
                    this.<>7__wrap2.Dispose();
                }
            }

            private bool MoveNext()
            {
                try
                {
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            ValidationUtils.ArgumentNotNull(this.source, "source");
                            this.<>7__wrap1 = this.source.GetEnumerator();
                            this.<>1__state = -3;
                            goto Label_0172;

                        case 1:
                            this.<>1__state = -3;
                            goto Label_016B;

                        case 2:
                            goto Label_0107;

                        case 3:
                            this.<>1__state = -3;
                            goto Label_016B;

                        default:
                            return false;
                    }
                Label_0059:
                    this.<token>5__1 = this.<>7__wrap1.Current;
                    if (this.key == null)
                    {
                        if (this.<token>5__1 is JValue)
                        {
                            this.<>2__current = ((JValue) this.<token>5__1).Convert<JValue, U>();
                            this.<>1__state = 1;
                            return true;
                        }
                        this.<>7__wrap2 = this.<token>5__1.Children().GetEnumerator();
                        this.<>1__state = -4;
                        while (this.<>7__wrap2.MoveNext())
                        {
                            this.<>2__current = this.<>7__wrap2.Current.Convert<JToken, U>();
                            this.<>1__state = 2;
                            return true;
                        Label_0107:
                            this.<>1__state = -4;
                        }
                        this.<>m__Finally2();
                        this.<>7__wrap2 = null;
                    }
                    else
                    {
                        JToken token = this.<token>5__1[this.key];
                        if (token != null)
                        {
                            this.<>2__current = token.Convert<JToken, U>();
                            this.<>1__state = 3;
                            return true;
                        }
                    }
                Label_016B:
                    this.<token>5__1 = null;
                Label_0172:
                    if (this.<>7__wrap1.MoveNext())
                    {
                        goto Label_0059;
                    }
                    this.<>m__Finally1();
                    this.<>7__wrap1 = null;
                    return false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
            }

            [DebuggerHidden]
            IEnumerator<U> IEnumerable<U>.GetEnumerator()
            {
                Extensions.<Values>d__11<T, U> d__;
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.CurrentManagedThreadId))
                {
                    this.<>1__state = 0;
                    d__ = (Extensions.<Values>d__11<T, U>) this;
                }
                else
                {
                    d__ = new Extensions.<Values>d__11<T, U>(0);
                }
                d__.source = this.<>3__source;
                d__.key = this.<>3__key;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<U>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
                int num = this.<>1__state;
                switch (num)
                {
                    case -4:
                    case -3:
                    case 1:
                    case 2:
                    case 3:
                        try
                        {
                            switch (num)
                            {
                                case -4:
                                case 2:
                                    try
                                    {
                                    }
                                    finally
                                    {
                                        this.<>m__Finally2();
                                    }
                                    break;
                            }
                        }
                        finally
                        {
                            this.<>m__Finally1();
                        }
                        return;

                    case -2:
                    case -1:
                    case 0:
                        return;
                }
            }

            U IEnumerator<U>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

