namespace Newtonsoft.Json.Linq.JsonPath
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class JPath
    {
        private readonly string _expression;
        private int _currentIndex;

        public JPath(string expression)
        {
            ValidationUtils.ArgumentNotNull(expression, "expression");
            this._expression = expression;
            this.Filters = new List<PathFilter>();
            this.ParseMain();
        }

        private void EatWhitespace()
        {
            while (this._currentIndex < this._expression.Length)
            {
                if (this._expression[this._currentIndex] != ' ')
                {
                    break;
                }
                this._currentIndex++;
            }
        }

        private void EnsureLength(string message)
        {
            if (this._currentIndex >= this._expression.Length)
            {
                throw new JsonException(message);
            }
        }

        internal IEnumerable<JToken> Evaluate(JToken t, bool errorWhenNoMatch) => 
            Evaluate(this.Filters, t, errorWhenNoMatch);

        internal static IEnumerable<JToken> Evaluate(List<PathFilter> filters, JToken t, bool errorWhenNoMatch)
        {
            JToken[] tokenArray1 = new JToken[] { t };
            IEnumerable<JToken> current = tokenArray1;
            using (List<PathFilter>.Enumerator enumerator = filters.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    current = enumerator.Current.ExecuteFilter(current, errorWhenNoMatch);
                }
            }
            return current;
        }

        private bool Match(string s)
        {
            int num = this._currentIndex;
            foreach (char ch in s)
            {
                if ((num < this._expression.Length) && (this._expression[num] == ch))
                {
                    num++;
                }
                else
                {
                    return false;
                }
            }
            this._currentIndex = num;
            return true;
        }

        private PathFilter ParseArrayIndexer(char indexerCloseChar)
        {
            int startIndex = this._currentIndex;
            int? nullable = null;
            List<int> list = null;
            int num2 = 0;
            int? nullable2 = null;
            int? nullable3 = null;
            int? nullable4 = null;
            while (this._currentIndex < this._expression.Length)
            {
                int? nullable5;
                char c = this._expression[this._currentIndex];
                if (c == ' ')
                {
                    nullable = new int?(this._currentIndex);
                    this.EatWhitespace();
                    continue;
                }
                if (c == indexerCloseChar)
                {
                    nullable5 = nullable;
                    int length = (nullable5.HasValue ? nullable5.GetValueOrDefault() : this._currentIndex) - startIndex;
                    if (list != null)
                    {
                        if (length == 0)
                        {
                            throw new JsonException("Array index expected.");
                        }
                        int item = Convert.ToInt32(this._expression.Substring(startIndex, length), CultureInfo.InvariantCulture);
                        list.Add(item);
                        return new ArrayMultipleIndexFilter { Indexes = list };
                    }
                    if (num2 > 0)
                    {
                        if (length > 0)
                        {
                            int num5 = Convert.ToInt32(this._expression.Substring(startIndex, length), CultureInfo.InvariantCulture);
                            if (num2 == 1)
                            {
                                nullable3 = new int?(num5);
                            }
                            else
                            {
                                nullable4 = new int?(num5);
                            }
                        }
                        return new ArraySliceFilter { 
                            Start = nullable2,
                            End = nullable3,
                            Step = nullable4
                        };
                    }
                    if (length == 0)
                    {
                        throw new JsonException("Array index expected.");
                    }
                    int num6 = Convert.ToInt32(this._expression.Substring(startIndex, length), CultureInfo.InvariantCulture);
                    return new ArrayIndexFilter { Index = new int?(num6) };
                }
                switch (c)
                {
                    case ',':
                    {
                        nullable5 = nullable;
                        int length = (nullable5.HasValue ? nullable5.GetValueOrDefault() : this._currentIndex) - startIndex;
                        if (length == 0)
                        {
                            throw new JsonException("Array index expected.");
                        }
                        if (list == null)
                        {
                            list = new List<int>();
                        }
                        string str = this._expression.Substring(startIndex, length);
                        list.Add(Convert.ToInt32(str, CultureInfo.InvariantCulture));
                        this._currentIndex++;
                        this.EatWhitespace();
                        startIndex = this._currentIndex;
                        nullable = null;
                        continue;
                    }
                    case '*':
                        this._currentIndex++;
                        this.EnsureLength("Path ended with open indexer.");
                        this.EatWhitespace();
                        if (this._expression[this._currentIndex] != indexerCloseChar)
                        {
                            throw new JsonException("Unexpected character while parsing path indexer: " + c.ToString());
                        }
                        return new ArrayIndexFilter();

                    default:
                    {
                        if (c != ':')
                        {
                            goto Label_031C;
                        }
                        nullable5 = nullable;
                        int length = (nullable5.HasValue ? nullable5.GetValueOrDefault() : this._currentIndex) - startIndex;
                        if (length > 0)
                        {
                            int num9 = Convert.ToInt32(this._expression.Substring(startIndex, length), CultureInfo.InvariantCulture);
                            switch (num2)
                            {
                                case 0:
                                    nullable2 = new int?(num9);
                                    goto Label_02F0;

                                case 1:
                                    nullable3 = new int?(num9);
                                    goto Label_02F0;
                            }
                            nullable4 = new int?(num9);
                        }
                        break;
                    }
                }
            Label_02F0:
                num2++;
                this._currentIndex++;
                this.EatWhitespace();
                startIndex = this._currentIndex;
                nullable = null;
                continue;
            Label_031C:
                if (!char.IsDigit(c) && (c != '-'))
                {
                    throw new JsonException("Unexpected character while parsing path indexer: " + c.ToString());
                }
                if (nullable.HasValue)
                {
                    throw new JsonException("Unexpected character while parsing path indexer: " + c.ToString());
                }
                this._currentIndex++;
            }
            throw new JsonException("Path ended with open indexer.");
        }

        private QueryExpression ParseExpression()
        {
            QueryExpression expression = null;
            CompositeExpression expression2 = null;
            while (this._currentIndex < this._expression.Length)
            {
                QueryOperator exists;
                this.EatWhitespace();
                if (this._expression[this._currentIndex] != '@')
                {
                    char ch = this._expression[this._currentIndex];
                    throw new JsonException("Unexpected character while parsing path query: " + ch.ToString());
                }
                this._currentIndex++;
                List<PathFilter> filters = new List<PathFilter>();
                if (this.ParsePath(filters, this._currentIndex, true))
                {
                    throw new JsonException("Path ended with open query.");
                }
                this.EatWhitespace();
                this.EnsureLength("Path ended with open query.");
                object obj2 = null;
                if (((this._expression[this._currentIndex] == ')') || (this._expression[this._currentIndex] == '|')) || (this._expression[this._currentIndex] == '&'))
                {
                    exists = QueryOperator.Exists;
                }
                else
                {
                    exists = this.ParseOperator();
                    this.EatWhitespace();
                    this.EnsureLength("Path ended with open query.");
                    obj2 = this.ParseValue();
                    this.EatWhitespace();
                    this.EnsureLength("Path ended with open query.");
                }
                BooleanQueryExpression item = new BooleanQueryExpression {
                    Path = filters,
                    Operator = exists,
                    Value = (exists != QueryOperator.Exists) ? new JValue(obj2) : null
                };
                if (this._expression[this._currentIndex] == ')')
                {
                    if (expression2 != null)
                    {
                        expression2.Expressions.Add(item);
                        return expression;
                    }
                    return item;
                }
                if ((this._expression[this._currentIndex] == '&') && this.Match("&&"))
                {
                    if ((expression2 == null) || (expression2.Operator != QueryOperator.And))
                    {
                        CompositeExpression expression4 = new CompositeExpression {
                            Operator = QueryOperator.And
                        };
                        if (expression2 != null)
                        {
                            expression2.Expressions.Add(expression4);
                        }
                        expression2 = expression4;
                        if (expression == null)
                        {
                            expression = expression2;
                        }
                    }
                    expression2.Expressions.Add(item);
                }
                if ((this._expression[this._currentIndex] == '|') && this.Match("||"))
                {
                    if ((expression2 == null) || (expression2.Operator != QueryOperator.Or))
                    {
                        CompositeExpression expression5 = new CompositeExpression {
                            Operator = QueryOperator.Or
                        };
                        if (expression2 != null)
                        {
                            expression2.Expressions.Add(expression5);
                        }
                        expression2 = expression5;
                        if (expression == null)
                        {
                            expression = expression2;
                        }
                    }
                    expression2.Expressions.Add(item);
                }
            }
            throw new JsonException("Path ended with open query.");
        }

        private PathFilter ParseIndexer(char indexerOpenChar)
        {
            this._currentIndex++;
            char indexerCloseChar = (indexerOpenChar == '[') ? ']' : ')';
            this.EnsureLength("Path ended with open indexer.");
            this.EatWhitespace();
            if (this._expression[this._currentIndex] == '\'')
            {
                return this.ParseQuotedField(indexerCloseChar);
            }
            if (this._expression[this._currentIndex] == '?')
            {
                return this.ParseQuery(indexerCloseChar);
            }
            return this.ParseArrayIndexer(indexerCloseChar);
        }

        private void ParseMain()
        {
            int currentPartStartIndex = this._currentIndex;
            this.EatWhitespace();
            if (this._expression.Length != this._currentIndex)
            {
                if (this._expression[this._currentIndex] == '$')
                {
                    if (this._expression.Length == 1)
                    {
                        return;
                    }
                    switch (this._expression[this._currentIndex + 1])
                    {
                        case '.':
                        case '[':
                            this._currentIndex++;
                            currentPartStartIndex = this._currentIndex;
                            break;
                    }
                }
                if (!this.ParsePath(this.Filters, currentPartStartIndex, false))
                {
                    int num2 = this._currentIndex;
                    this.EatWhitespace();
                    if (this._currentIndex < this._expression.Length)
                    {
                        char ch2 = this._expression[num2];
                        throw new JsonException("Unexpected character while parsing path: " + ch2.ToString());
                    }
                }
            }
        }

        private QueryOperator ParseOperator()
        {
            if ((this._currentIndex + 1) >= this._expression.Length)
            {
                throw new JsonException("Path ended with open query.");
            }
            if (this.Match("=="))
            {
                return QueryOperator.Equals;
            }
            if (this.Match("!=") || this.Match("<>"))
            {
                return QueryOperator.NotEquals;
            }
            if (this.Match("<="))
            {
                return QueryOperator.LessThanOrEquals;
            }
            if (this.Match("<"))
            {
                return QueryOperator.LessThan;
            }
            if (this.Match(">="))
            {
                return QueryOperator.GreaterThanOrEquals;
            }
            if (!this.Match(">"))
            {
                throw new JsonException("Could not read query operator.");
            }
            return QueryOperator.GreaterThan;
        }

        private bool ParsePath(List<PathFilter> filters, int currentPartStartIndex, bool query)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            while ((this._currentIndex < this._expression.Length) && !flag4)
            {
                char indexerOpenChar = this._expression[this._currentIndex];
                switch (indexerOpenChar)
                {
                    case '.':
                    {
                        if (this._currentIndex > currentPartStartIndex)
                        {
                            string str2 = this._expression.Substring(currentPartStartIndex, this._currentIndex - currentPartStartIndex);
                            if (str2 == "*")
                            {
                                str2 = null;
                            }
                            PathFilter item = flag ? ((PathFilter) new ScanFilter()) : ((PathFilter) new FieldFilter());
                            filters.Add(item);
                            flag = false;
                        }
                        if (((this._currentIndex + 1) < this._expression.Length) && (this._expression[this._currentIndex + 1] == '.'))
                        {
                            flag = true;
                            this._currentIndex++;
                        }
                        this._currentIndex++;
                        currentPartStartIndex = this._currentIndex;
                        flag2 = false;
                        flag3 = true;
                        continue;
                    }
                    case '[':
                    case '(':
                    {
                        if (this._currentIndex > currentPartStartIndex)
                        {
                            string str = this._expression.Substring(currentPartStartIndex, this._currentIndex - currentPartStartIndex);
                            if (str == "*")
                            {
                                str = null;
                            }
                            PathFilter item = flag ? ((PathFilter) new ScanFilter()) : ((PathFilter) new FieldFilter());
                            filters.Add(item);
                            flag = false;
                        }
                        filters.Add(this.ParseIndexer(indexerOpenChar));
                        this._currentIndex++;
                        currentPartStartIndex = this._currentIndex;
                        flag2 = true;
                        flag3 = false;
                        continue;
                    }
                    case ']':
                    case ')':
                    {
                        flag4 = true;
                        continue;
                    }
                    case ' ':
                    {
                        if (this._currentIndex < this._expression.Length)
                        {
                            flag4 = true;
                        }
                        continue;
                    }
                }
                if (query && ((((indexerOpenChar == '=') || (indexerOpenChar == '<')) || ((indexerOpenChar == '!') || (indexerOpenChar == '>'))) || ((indexerOpenChar == '|') || (indexerOpenChar == '&'))))
                {
                    flag4 = true;
                }
                else
                {
                    if (flag2)
                    {
                        throw new JsonException("Unexpected character following indexer: " + indexerOpenChar.ToString());
                    }
                    this._currentIndex++;
                }
            }
            bool flag5 = this._currentIndex == this._expression.Length;
            if (this._currentIndex > currentPartStartIndex)
            {
                string str3 = this._expression.Substring(currentPartStartIndex, this._currentIndex - currentPartStartIndex).TrimEnd(new char[0]);
                if (str3 == "*")
                {
                    str3 = null;
                }
                PathFilter item = flag ? ((PathFilter) new ScanFilter()) : ((PathFilter) new FieldFilter());
                filters.Add(item);
                return flag5;
            }
            if (flag3 && (flag5 | query))
            {
                throw new JsonException("Unexpected end while parsing path.");
            }
            return flag5;
        }

        private PathFilter ParseQuery(char indexerCloseChar)
        {
            char ch;
            this._currentIndex++;
            this.EnsureLength("Path ended with open indexer.");
            if (this._expression[this._currentIndex] != '(')
            {
                ch = this._expression[this._currentIndex];
                throw new JsonException("Unexpected character while parsing path indexer: " + ch.ToString());
            }
            this._currentIndex++;
            QueryExpression expression = this.ParseExpression();
            this._currentIndex++;
            this.EnsureLength("Path ended with open indexer.");
            this.EatWhitespace();
            if (this._expression[this._currentIndex] != indexerCloseChar)
            {
                ch = this._expression[this._currentIndex];
                throw new JsonException("Unexpected character while parsing path indexer: " + ch.ToString());
            }
            return new QueryFilter { Expression = expression };
        }

        private PathFilter ParseQuotedField(char indexerCloseChar)
        {
            List<string> list = null;
            while (this._currentIndex < this._expression.Length)
            {
                string item = this.ReadQuotedString();
                this.EatWhitespace();
                this.EnsureLength("Path ended with open indexer.");
                if (this._expression[this._currentIndex] == indexerCloseChar)
                {
                    if (list != null)
                    {
                        list.Add(item);
                        return new FieldMultipleFilter { Names = list };
                    }
                    return new FieldFilter { Name = item };
                }
                if (this._expression[this._currentIndex] != ',')
                {
                    char ch = this._expression[this._currentIndex];
                    throw new JsonException("Unexpected character while parsing path indexer: " + ch.ToString());
                }
                this._currentIndex++;
                this.EatWhitespace();
                if (list == null)
                {
                    list = new List<string>();
                }
                list.Add(item);
            }
            throw new JsonException("Path ended with open indexer.");
        }

        private object ParseValue()
        {
            char c = this._expression[this._currentIndex];
            if (c == '\'')
            {
                return this.ReadQuotedString();
            }
            if (char.IsDigit(c) || (c == '-'))
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(c);
                this._currentIndex++;
                while (this._currentIndex < this._expression.Length)
                {
                    c = this._expression[this._currentIndex];
                    if ((c == ' ') || (c == ')'))
                    {
                        string s = builder.ToString();
                        if (s.IndexOfAny(new char[] { '.', 'E', 'e' }) != -1)
                        {
                            if (!double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double num))
                            {
                                throw new JsonException("Could not read query value.");
                            }
                            return num;
                        }
                        if (!long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out long num2))
                        {
                            throw new JsonException("Could not read query value.");
                        }
                        return num2;
                    }
                    builder.Append(c);
                    this._currentIndex++;
                }
            }
            else
            {
                switch (c)
                {
                    case 't':
                        if (this.Match("true"))
                        {
                            return true;
                        }
                        goto Label_0173;

                    case 'f':
                        if (this.Match("false"))
                        {
                            return false;
                        }
                        goto Label_0173;
                }
                if ((c == 'n') && this.Match("null"))
                {
                    return null;
                }
            }
        Label_0173:
            throw new JsonException("Could not read query value.");
        }

        private string ReadQuotedString()
        {
            StringBuilder builder = new StringBuilder();
            this._currentIndex++;
            while (this._currentIndex < this._expression.Length)
            {
                char ch = this._expression[this._currentIndex];
                if ((ch == '\\') && ((this._currentIndex + 1) < this._expression.Length))
                {
                    this._currentIndex++;
                    if (this._expression[this._currentIndex] == '\'')
                    {
                        builder.Append('\'');
                    }
                    else if (this._expression[this._currentIndex] == '\\')
                    {
                        builder.Append('\\');
                    }
                    else
                    {
                        char ch2 = this._expression[this._currentIndex];
                        throw new JsonException(@"Unknown escape chracter: \" + ch2.ToString());
                    }
                    this._currentIndex++;
                }
                else
                {
                    if (ch == '\'')
                    {
                        this._currentIndex++;
                        return builder.ToString();
                    }
                    this._currentIndex++;
                    builder.Append(ch);
                }
            }
            throw new JsonException("Path ended with an open string.");
        }

        public List<PathFilter> Filters { get; private set; }
    }
}

