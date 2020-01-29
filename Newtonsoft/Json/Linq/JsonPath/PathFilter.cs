namespace Newtonsoft.Json.Linq.JsonPath
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    internal abstract class PathFilter
    {
        protected PathFilter()
        {
        }

        public abstract IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch);
        protected static JToken GetTokenIndex(JToken t, bool errorWhenNoMatch, int index)
        {
            JArray array = t as JArray;
            JConstructor constructor = t as JConstructor;
            if (array != null)
            {
                if (array.Count > index)
                {
                    return array[index];
                }
                if (errorWhenNoMatch)
                {
                    throw new JsonException("Index {0} outside the bounds of JArray.".FormatWith(CultureInfo.InvariantCulture, index));
                }
                return null;
            }
            if (constructor != null)
            {
                if (constructor.Count > index)
                {
                    return constructor[index];
                }
                if (errorWhenNoMatch)
                {
                    throw new JsonException("Index {0} outside the bounds of JConstructor.".FormatWith(CultureInfo.InvariantCulture, index));
                }
                return null;
            }
            if (errorWhenNoMatch)
            {
                throw new JsonException("Index {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, index, t.GetType().Name));
            }
            return null;
        }
    }
}

