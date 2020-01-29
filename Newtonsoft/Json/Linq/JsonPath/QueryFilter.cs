namespace Newtonsoft.Json.Linq.JsonPath
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal class QueryFilter : PathFilter
    {
        [IteratorStateMachine(typeof(<ExecuteFilter>d__4))]
        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch) => 
            new <ExecuteFilter>d__4(-2) { 
                <>4__this = this,
                <>3__current = current
            };

        public QueryExpression Expression { get; set; }

        [CompilerGenerated]
        private sealed class <ExecuteFilter>d__4 : IEnumerable<JToken>, IEnumerable, IEnumerator<JToken>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private JToken <>2__current;
            private int <>l__initialThreadId;
            private IEnumerable<JToken> current;
            public IEnumerable<JToken> <>3__current;
            public QueryFilter <>4__this;
            private IEnumerator<JToken> <>7__wrap1;
            private IEnumerator<JToken> <>7__wrap2;

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
                if (this.<>7__wrap2 != null)
                {
                    this.<>7__wrap2.Dispose();
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
                        this.<>7__wrap1 = this.current.GetEnumerator();
                        this.<>1__state = -3;
                        while (this.<>7__wrap1.MoveNext())
                        {
                            JToken current = this.<>7__wrap1.Current;
                            this.<>7__wrap2 = ((IEnumerable<JToken>) current).GetEnumerator();
                            this.<>1__state = -4;
                            while (this.<>7__wrap2.MoveNext())
                            {
                                JToken t = this.<>7__wrap2.Current;
                                if (!this.<>4__this.Expression.IsMatch(t))
                                {
                                    continue;
                                }
                                this.<>2__current = t;
                                this.<>1__state = 1;
                                return true;
                            Label_009C:
                                this.<>1__state = -4;
                            }
                            this.<>m__Finally2();
                            this.<>7__wrap2 = null;
                        }
                        this.<>m__Finally1();
                        this.<>7__wrap1 = null;
                        return false;
                    }
                    if (num != 1)
                    {
                        return false;
                    }
                    goto Label_009C;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
            }

            [DebuggerHidden]
            IEnumerator<JToken> IEnumerable<JToken>.GetEnumerator()
            {
                QueryFilter.<ExecuteFilter>d__4 d__;
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.CurrentManagedThreadId))
                {
                    this.<>1__state = 0;
                    d__ = this;
                }
                else
                {
                    d__ = new QueryFilter.<ExecuteFilter>d__4(0) {
                        <>4__this = this.<>4__this
                    };
                }
                d__.current = this.<>3__current;
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

