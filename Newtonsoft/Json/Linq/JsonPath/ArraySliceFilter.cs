namespace Newtonsoft.Json.Linq.JsonPath
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;

    internal class ArraySliceFilter : PathFilter
    {
        [IteratorStateMachine(typeof(<ExecuteFilter>d__12))]
        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch) => 
            new <ExecuteFilter>d__12(-2) { 
                <>4__this = this,
                <>3__current = current,
                <>3__errorWhenNoMatch = errorWhenNoMatch
            };

        private bool IsValid(int index, int stopIndex, bool positiveStep)
        {
            if (positiveStep)
            {
                return (index < stopIndex);
            }
            return (index > stopIndex);
        }

        public int? Start { get; set; }

        public int? End { get; set; }

        public int? Step { get; set; }

        [CompilerGenerated]
        private sealed class <ExecuteFilter>d__12 : IEnumerable<JToken>, IEnumerable, IEnumerator<JToken>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private JToken <>2__current;
            private int <>l__initialThreadId;
            public ArraySliceFilter <>4__this;
            private IEnumerable<JToken> current;
            public IEnumerable<JToken> <>3__current;
            private JArray <a>5__1;
            private int <i>5__2;
            private int <stepCount>5__3;
            private int <stopIndex>5__4;
            private bool <positiveStep>5__5;
            private bool errorWhenNoMatch;
            public bool <>3__errorWhenNoMatch;
            private JToken <t>5__6;
            private IEnumerator<JToken> <>7__wrap1;

            [DebuggerHidden]
            public <ExecuteFilter>d__12(int <>1__state)
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
                        int? step = this.<>4__this.Step;
                        int num2 = 0;
                        if ((step.GetValueOrDefault() == num2) ? step.HasValue : false)
                        {
                            throw new JsonException("Step cannot be zero.");
                        }
                        this.<>7__wrap1 = this.current.GetEnumerator();
                        this.<>1__state = -3;
                        while (this.<>7__wrap1.MoveNext())
                        {
                            this.<t>5__6 = this.<>7__wrap1.Current;
                            this.<a>5__1 = this.<t>5__6 as JArray;
                            if (this.<a>5__1 != null)
                            {
                                step = this.<>4__this.Step;
                                this.<stepCount>5__3 = step.HasValue ? step.GetValueOrDefault() : 1;
                                step = this.<>4__this.Start;
                                int num3 = step.HasValue ? step.GetValueOrDefault() : ((this.<stepCount>5__3 > 0) ? 0 : (this.<a>5__1.Count - 1));
                                step = this.<>4__this.End;
                                this.<stopIndex>5__4 = step.HasValue ? step.GetValueOrDefault() : ((this.<stepCount>5__3 > 0) ? this.<a>5__1.Count : -1);
                                step = this.<>4__this.Start;
                                num2 = 0;
                                if ((step.GetValueOrDefault() < num2) ? step.HasValue : false)
                                {
                                    num3 = this.<a>5__1.Count + num3;
                                }
                                step = this.<>4__this.End;
                                num2 = 0;
                                if ((step.GetValueOrDefault() < num2) ? step.HasValue : false)
                                {
                                    this.<stopIndex>5__4 = this.<a>5__1.Count + this.<stopIndex>5__4;
                                }
                                num3 = Math.Min(Math.Max(num3, (this.<stepCount>5__3 > 0) ? 0 : -2147483648), (this.<stepCount>5__3 > 0) ? this.<a>5__1.Count : (this.<a>5__1.Count - 1));
                                this.<stopIndex>5__4 = Math.Max(this.<stopIndex>5__4, -1);
                                this.<stopIndex>5__4 = Math.Min(this.<stopIndex>5__4, this.<a>5__1.Count);
                                this.<positiveStep>5__5 = this.<stepCount>5__3 > 0;
                                if (!this.<>4__this.IsValid(num3, this.<stopIndex>5__4, this.<positiveStep>5__5))
                                {
                                    if (this.errorWhenNoMatch)
                                    {
                                        throw new JsonException("Array slice of {0} to {1} returned no results.".FormatWith(CultureInfo.InvariantCulture, this.<>4__this.Start.HasValue ? this.<>4__this.Start.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) : "*", this.<>4__this.End.HasValue ? this.<>4__this.End.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) : "*"));
                                    }
                                }
                                else
                                {
                                    this.<i>5__2 = num3;
                                    while (this.<>4__this.IsValid(this.<i>5__2, this.<stopIndex>5__4, this.<positiveStep>5__5))
                                    {
                                        this.<>2__current = this.<a>5__1[this.<i>5__2];
                                        this.<>1__state = 1;
                                        return true;
                                    Label_02BD:
                                        this.<>1__state = -3;
                                        this.<i>5__2 += this.<stepCount>5__3;
                                    }
                                }
                            }
                            else if (this.errorWhenNoMatch)
                            {
                                throw new JsonException("Array slice is not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, this.<t>5__6.GetType().Name));
                            }
                            this.<a>5__1 = null;
                            this.<t>5__6 = null;
                        }
                        this.<>m__Finally1();
                        this.<>7__wrap1 = null;
                        return false;
                    }
                    if (num != 1)
                    {
                        return false;
                    }
                    goto Label_02BD;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
            }

            [DebuggerHidden]
            IEnumerator<JToken> IEnumerable<JToken>.GetEnumerator()
            {
                ArraySliceFilter.<ExecuteFilter>d__12 d__;
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.CurrentManagedThreadId))
                {
                    this.<>1__state = 0;
                    d__ = this;
                }
                else
                {
                    d__ = new ArraySliceFilter.<ExecuteFilter>d__12(0) {
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

            JToken IEnumerator<JToken>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

