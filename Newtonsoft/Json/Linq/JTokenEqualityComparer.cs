namespace Newtonsoft.Json.Linq
{
    using System;
    using System.Collections.Generic;

    public class JTokenEqualityComparer : IEqualityComparer<JToken>
    {
        public bool Equals(JToken x, JToken y) => 
            JToken.DeepEquals(x, y);

        public int GetHashCode(JToken obj) => 
            obj?.GetDeepHashCode();
    }
}

