namespace Newtonsoft.Json.Linq.JsonPath
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class CompositeExpression : QueryExpression
    {
        public CompositeExpression()
        {
            this.Expressions = new List<QueryExpression>();
        }

        public override bool IsMatch(JToken t)
        {
            List<QueryExpression>.Enumerator enumerator;
            QueryOperator @operator = base.Operator;
            if (@operator != QueryOperator.And)
            {
                if (@operator != QueryOperator.Or)
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                using (enumerator = this.Expressions.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (!enumerator.Current.IsMatch(t))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            using (enumerator = this.Expressions.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsMatch(t))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<QueryExpression> Expressions { get; set; }
    }
}

