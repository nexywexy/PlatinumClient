namespace Newtonsoft.Json.Linq.JsonPath
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal class BooleanQueryExpression : QueryExpression
    {
        private bool EqualsWithStringCoercion(JValue value, JValue queryValue)
        {
            string str;
            string originalString;
            if (value.Equals(queryValue))
            {
                return true;
            }
            if (queryValue.Type == JTokenType.String)
            {
                str = (string) queryValue.Value;
                switch (value.Type)
                {
                    case JTokenType.Date:
                    {
                        using (StringWriter writer = StringUtils.CreateStringWriter(0x40))
                        {
                            if (value.Value is DateTimeOffset)
                            {
                                DateTimeUtils.WriteDateTimeOffsetString(writer, (DateTimeOffset) value.Value, DateFormatHandling.IsoDateFormat, null, CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                DateTimeUtils.WriteDateTimeString(writer, (DateTime) value.Value, DateFormatHandling.IsoDateFormat, null, CultureInfo.InvariantCulture);
                            }
                            originalString = writer.ToString();
                            goto Label_00FA;
                        }
                    }
                    case JTokenType.Bytes:
                        originalString = Convert.ToBase64String((byte[]) value.Value);
                        goto Label_00FA;

                    case JTokenType.Guid:
                    case JTokenType.TimeSpan:
                        originalString = value.Value.ToString();
                        goto Label_00FA;

                    case JTokenType.Uri:
                        originalString = ((Uri) value.Value).OriginalString;
                        goto Label_00FA;
                }
            }
            return false;
        Label_00FA:
            return string.Equals(originalString, str, StringComparison.Ordinal);
        }

        public override bool IsMatch(JToken t)
        {
            foreach (JValue value2 in JPath.Evaluate(this.Path, t, false))
            {
                if (value2 != null)
                {
                    switch (base.Operator)
                    {
                        case QueryOperator.Equals:
                            if (!this.EqualsWithStringCoercion(value2, this.Value))
                            {
                                break;
                            }
                            return true;

                        case QueryOperator.NotEquals:
                            if (this.EqualsWithStringCoercion(value2, this.Value))
                            {
                                break;
                            }
                            return true;

                        case QueryOperator.Exists:
                            return true;

                        case QueryOperator.LessThan:
                            if (value2.CompareTo(this.Value) >= 0)
                            {
                                break;
                            }
                            return true;

                        case QueryOperator.LessThanOrEquals:
                            if (value2.CompareTo(this.Value) > 0)
                            {
                                break;
                            }
                            return true;

                        case QueryOperator.GreaterThan:
                            if (value2.CompareTo(this.Value) <= 0)
                            {
                                break;
                            }
                            return true;

                        case QueryOperator.GreaterThanOrEquals:
                            if (value2.CompareTo(this.Value) < 0)
                            {
                                break;
                            }
                            return true;
                    }
                }
                else
                {
                    switch (base.Operator)
                    {
                        case QueryOperator.NotEquals:
                        case QueryOperator.Exists:
                            return true;
                    }
                }
            }
            return false;
        }

        public List<PathFilter> Path { get; set; }

        public JValue Value { get; set; }
    }
}

