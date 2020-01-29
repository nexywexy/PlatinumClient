namespace Newtonsoft.Json.Linq.JsonPath
{
    using System;

    internal enum QueryOperator
    {
        None,
        Equals,
        NotEquals,
        Exists,
        LessThan,
        LessThanOrEquals,
        GreaterThan,
        GreaterThanOrEquals,
        And,
        Or
    }
}

