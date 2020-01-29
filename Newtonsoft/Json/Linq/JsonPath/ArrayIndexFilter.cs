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

    internal class ArrayIndexFilter : PathFilter
    {
        [IteratorStateMachine(typeof(<ExecuteFilter>d__4))]
        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
        {
            foreach (JToken <t>5__1 in current)
            {
                if (this.Index.HasValue)
                {
                    JToken token = PathFilter.GetTokenIndex(<t>5__1, errorWhenNoMatch, this.Index.GetValueOrDefault());
                    if (token != null)
                    {
                        yield return token;
                    }
                }
                else if ((<t>5__1 is JArray) || (<t>5__1 is JConstructor))
                {
                    foreach (JToken current in (IEnumerable<JToken>) <t>5__1)
                    {
                        yield return current;
                    }
                }
                else if (errorWhenNoMatch)
                {
                    throw new JsonException("Index * not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, <t>5__1.GetType().Name));
                }
                <t>5__1 = null;
            }
        }

        public int? Index { get; set; }

    }
}

