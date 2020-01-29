namespace Newtonsoft.Json.Linq.JsonPath
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Runtime.CompilerServices;

    internal abstract class QueryExpression
    {
        protected QueryExpression()
        {
        }

        public abstract bool IsMatch(JToken t);

        public QueryOperator Operator { get; set; }
    }
}

