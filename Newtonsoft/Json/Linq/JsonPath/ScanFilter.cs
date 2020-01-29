namespace Newtonsoft.Json.Linq.JsonPath
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal class ScanFilter : PathFilter
    {
        [IteratorStateMachine(typeof(<ExecuteFilter>d__4))]
        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
        {
            foreach (JToken <root>5__1 in current)
            {
                JProperty property;
                if (this.Name == null)
                {
                    yield return <root>5__1;
                }
                JToken first = <root>5__1;
                JToken token = <root>5__1;
            Label_009F:
                if ((token == null) || !token.HasValues)
                {
                    goto Label_00D2;
                }
                first = token.First;
                goto Label_0133;
            Label_00C1:
                first = first.Parent;
            Label_00D2:
                if (((first != null) && (first != <root>5__1)) && (first == first.Parent.Last))
                {
                    goto Label_00C1;
                }
                if ((first == null) || (first == <root>5__1))
                {
                    goto Label_01CA;
                }
                first = first.Next;
            Label_0133:
                property = first as JProperty;
                if (property != null)
                {
                    if (property.Name == this.Name)
                    {
                        yield return property.Value;
                    }
                }
                else if (this.Name == null)
                {
                    yield return first;
                }
                token = first as JContainer;
                goto Label_009F;
            Label_01CA:
                first = null;
                <root>5__1 = null;
            }
        }

        public string Name { get; set; }

    }
}

