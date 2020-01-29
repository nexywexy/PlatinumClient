﻿namespace Newtonsoft.Json.Linq.JsonPath
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal class FieldMultipleFilter : PathFilter
    {
        [IteratorStateMachine(typeof(<ExecuteFilter>d__4))]
        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch) => 
            new <ExecuteFilter>d__4(-2) { 
                <>4__this = this,
                <>3__current = current,
                <>3__errorWhenNoMatch = errorWhenNoMatch
            };

        public List<string> Names { get; set; }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly FieldMultipleFilter.<>c <>9 = new FieldMultipleFilter.<>c();
            public static Func<string, string> <>9__4_0;

            internal string <ExecuteFilter>b__4_0(string n) => 
                ("'" + n + "'");
        }

        [CompilerGenerated]
        private sealed class <ExecuteFilter>d__4 : IEnumerable<JToken>, IEnumerable, IEnumerator<JToken>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private JToken <>2__current;
            private int <>l__initialThreadId;
            private IEnumerable<JToken> current;
            public IEnumerable<JToken> <>3__current;
            public FieldMultipleFilter <>4__this;
            private JObject <o>5__1;
            private bool errorWhenNoMatch;
            public bool <>3__errorWhenNoMatch;
            private string <name>5__2;
            private JToken <t>5__3;
            private IEnumerator<JToken> <>7__wrap1;
            private List<string>.Enumerator <>7__wrap2;

            [DebuggerHidden]
            public <ExecuteFilter>d__4(int <>1__state)
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
                this.<>7__wrap2.Dispose();
            }

            private bool MoveNext()
            {
                try
                {
                    int num = this.<>1__state;
                    if (num == 0)
                    {
                        this.<>1__state = -1;
                        this.<>7__wrap1 = this.current.GetEnumerator();
                        this.<>1__state = -3;
                        while (this.<>7__wrap1.MoveNext())
                        {
                            this.<t>5__3 = this.<>7__wrap1.Current;
                            this.<o>5__1 = this.<t>5__3 as JObject;
                            if (this.<o>5__1 != null)
                            {
                                this.<>7__wrap2 = this.<>4__this.Names.GetEnumerator();
                                this.<>1__state = -4;
                                while (this.<>7__wrap2.MoveNext())
                                {
                                    this.<name>5__2 = this.<>7__wrap2.Current;
                                    JToken token = this.<o>5__1[this.<name>5__2];
                                    if (token == null)
                                    {
                                        goto Label_00D6;
                                    }
                                    this.<>2__current = token;
                                    this.<>1__state = 1;
                                    return true;
                                Label_00CE:
                                    this.<>1__state = -4;
                                Label_00D6:
                                    if (this.errorWhenNoMatch)
                                    {
                                        throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, this.<name>5__2));
                                    }
                                    this.<name>5__2 = null;
                                }
                                this.<>m__Finally2();
                                this.<>7__wrap2 = new List<string>.Enumerator();
                            }
                            else if (this.errorWhenNoMatch)
                            {
                                if (FieldMultipleFilter.<>c.<>9__4_0 == null)
                                {
                                }
                                throw new JsonException("Properties {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", this.<>4__this.Names.Select<string, string>((FieldMultipleFilter.<>c.<>9__4_0 = new Func<string, string>(FieldMultipleFilter.<>c.<>9.<ExecuteFilter>b__4_0))).ToArray<string>()), this.<t>5__3.GetType().Name));
                            }
                            this.<o>5__1 = null;
                            this.<t>5__3 = null;
                        }
                        this.<>m__Finally1();
                        this.<>7__wrap1 = null;
                        return false;
                    }
                    if (num != 1)
                    {
                        return false;
                    }
                    goto Label_00CE;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
            }

            [DebuggerHidden]
            IEnumerator<JToken> IEnumerable<JToken>.GetEnumerator()
            {
                FieldMultipleFilter.<ExecuteFilter>d__4 d__;
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.CurrentManagedThreadId))
                {
                    this.<>1__state = 0;
                    d__ = this;
                }
                else
                {
                    d__ = new FieldMultipleFilter.<ExecuteFilter>d__4(0) {
                        <>4__this = this.<>4__this
                    };
                }
                d__.current = this.<>3__current;
                d__.errorWhenNoMatch = this.<>3__errorWhenNoMatch;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Newtonsoft.Json.Linq.JToken>.GetEnumerator();

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
                        try
                        {
                            switch (num)
                            {
                                case -4:
                                case 1:
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
                        break;
                }
            }

            JToken IEnumerator<JToken>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}
