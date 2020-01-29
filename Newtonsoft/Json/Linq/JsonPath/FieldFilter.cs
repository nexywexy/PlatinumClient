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

    internal class FieldFilter : PathFilter
    {
        [IteratorStateMachine(typeof(<ExecuteFilter>d__4))]
        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
        {
            foreach (JToken <t>5__2 in current)
            {
                JObject <o>5__1 = <t>5__2 as JObject;
                if (<o>5__1 != null)
                {
                    if (this.Name != null)
                    {
                        JToken token = <o>5__1[this.Name];
                        if (token != null)
                        {
                            yield return token;
                        }
                        else if (errorWhenNoMatch)
                        {
                            throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, this.Name));
                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<string, JToken> current in <o>5__1)
                        {
                            yield return current.Value;
                        }
                    }
                }
                else if (errorWhenNoMatch)
                {
                    if (this.Name == null)
                    {
                    }
                    throw new JsonException("Property '{0}' not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, "*", <t>5__2.GetType().Name));
                }
                <o>5__1 = null;
                <t>5__2 = null;
            }
        }

        public string Name { get; set; }

    }
}

