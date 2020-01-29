namespace Newtonsoft.Json
{
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Schema;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading;

    [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
    public class JsonValidatingReader : JsonReader, IJsonLineInfo
    {
        private readonly JsonReader _reader;
        private readonly Stack<SchemaScope> _stack;
        private JsonSchema _schema;
        private JsonSchemaModel _model;
        private SchemaScope _currentScope;
        private static readonly IList<JsonSchemaModel> EmptySchemaList = new List<JsonSchemaModel>();

        [field: CompilerGenerated]
        public event Newtonsoft.Json.Schema.ValidationEventHandler ValidationEventHandler;

        public JsonValidatingReader(JsonReader reader)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            this._reader = reader;
            this._stack = new Stack<SchemaScope>();
        }

        private static double FloatingPointRemainder(double dividend, double divisor) => 
            (dividend - (Math.Floor((double) (dividend / divisor)) * divisor));

        private JsonSchemaType? GetCurrentNodeSchemaType()
        {
            switch (this._reader.TokenType)
            {
                case JsonToken.StartObject:
                    return 0x10;

                case JsonToken.StartArray:
                    return 0x20;

                case JsonToken.Integer:
                    return 4;

                case JsonToken.Float:
                    return 2;

                case JsonToken.String:
                    return 1;

                case JsonToken.Boolean:
                    return 8;

                case JsonToken.Null:
                    return 0x40;
            }
            return null;
        }

        private bool IsPropertyDefinied(JsonSchemaModel schema, string propertyName)
        {
            if ((schema.Properties != null) && schema.Properties.ContainsKey(propertyName))
            {
                return true;
            }
            if (schema.PatternProperties != null)
            {
                foreach (string str in schema.PatternProperties.Keys)
                {
                    if (Regex.IsMatch(propertyName, str))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsZero(double value) => 
            (Math.Abs(value) < 4.4408920985006262E-15);

        bool IJsonLineInfo.HasLineInfo()
        {
            IJsonLineInfo info = this._reader as IJsonLineInfo;
            return ((info != null) && info.HasLineInfo());
        }

        private void OnValidationEvent(JsonSchemaException exception)
        {
            Newtonsoft.Json.Schema.ValidationEventHandler validationEventHandler = this.ValidationEventHandler;
            if (validationEventHandler == null)
            {
                throw exception;
            }
            validationEventHandler(this, new ValidationEventArgs(exception));
        }

        private SchemaScope Pop()
        {
            this._currentScope = (this._stack.Count != 0) ? this._stack.Peek() : null;
            return this._stack.Pop();
        }

        private void ProcessValue()
        {
            if ((this._currentScope != null) && (this._currentScope.TokenType == JTokenType.Array))
            {
                this._currentScope.ArrayItemCount++;
                foreach (JsonSchemaModel model in this.CurrentSchemas)
                {
                    if ((((model != null) && model.PositionalItemsValidation) && !model.AllowAdditionalItems) && ((model.Items == null) || ((this._currentScope.ArrayItemCount - 1) >= model.Items.Count)))
                    {
                        this.RaiseError("Index {0} has not been defined and the schema does not allow additional items.".FormatWith(CultureInfo.InvariantCulture, this._currentScope.ArrayItemCount), model);
                    }
                }
            }
        }

        private void Push(SchemaScope scope)
        {
            this._stack.Push(scope);
            this._currentScope = scope;
        }

        private void RaiseError(string message, JsonSchemaModel schema)
        {
            IJsonLineInfo info = this;
            string str = info.HasLineInfo() ? (message + " Line {0}, position {1}.".FormatWith(CultureInfo.InvariantCulture, info.LineNumber, info.LinePosition)) : message;
            this.OnValidationEvent(new JsonSchemaException(str, null, this.Path, info.LineNumber, info.LinePosition));
        }

        public override bool Read()
        {
            if (!this._reader.Read())
            {
                return false;
            }
            if (this._reader.TokenType != JsonToken.Comment)
            {
                this.ValidateCurrentToken();
            }
            return true;
        }

        public override bool? ReadAsBoolean()
        {
            this.ValidateCurrentToken();
            return this._reader.ReadAsBoolean();
        }

        public override byte[] ReadAsBytes()
        {
            this.ValidateCurrentToken();
            return this._reader.ReadAsBytes();
        }

        public override DateTime? ReadAsDateTime()
        {
            this.ValidateCurrentToken();
            return this._reader.ReadAsDateTime();
        }

        public override DateTimeOffset? ReadAsDateTimeOffset()
        {
            this.ValidateCurrentToken();
            return this._reader.ReadAsDateTimeOffset();
        }

        public override decimal? ReadAsDecimal()
        {
            this.ValidateCurrentToken();
            return this._reader.ReadAsDecimal();
        }

        public override double? ReadAsDouble()
        {
            this.ValidateCurrentToken();
            return this._reader.ReadAsDouble();
        }

        public override int? ReadAsInt32()
        {
            this.ValidateCurrentToken();
            return this._reader.ReadAsInt32();
        }

        public override string ReadAsString()
        {
            this.ValidateCurrentToken();
            return this._reader.ReadAsString();
        }

        private bool TestType(JsonSchemaModel currentSchema, JsonSchemaType currentType)
        {
            if (!JsonSchemaGenerator.HasFlag(new JsonSchemaType?(currentSchema.Type), currentType))
            {
                this.RaiseError("Invalid type. Expected {0} but got {1}.".FormatWith(CultureInfo.InvariantCulture, currentSchema.Type, currentType), currentSchema);
                return false;
            }
            return true;
        }

        private bool ValidateArray(JsonSchemaModel schema) => 
            ((schema == null) || this.TestType(schema, JsonSchemaType.Array));

        private void ValidateBoolean(JsonSchemaModel schema)
        {
            if ((schema != null) && this.TestType(schema, JsonSchemaType.Boolean))
            {
                this.ValidateNotDisallowed(schema);
            }
        }

        private void ValidateCurrentToken()
        {
            if (this._model == null)
            {
                this._model = new JsonSchemaModelBuilder().Build(this._schema);
                if (!JsonTokenUtils.IsStartToken(this._reader.TokenType))
                {
                    this.Push(new SchemaScope(JTokenType.None, this.CurrentMemberSchemas));
                }
            }
            switch (this._reader.TokenType)
            {
                case JsonToken.None:
                    return;

                case JsonToken.StartObject:
                {
                    this.ProcessValue();
                    IList<JsonSchemaModel> schemas = this.CurrentMemberSchemas.Where<JsonSchemaModel>(new Func<JsonSchemaModel, bool>(this.ValidateObject)).ToList<JsonSchemaModel>();
                    this.Push(new SchemaScope(JTokenType.Object, schemas));
                    this.WriteToken(this.CurrentSchemas);
                    return;
                }
                case JsonToken.StartArray:
                {
                    this.ProcessValue();
                    IList<JsonSchemaModel> schemas = this.CurrentMemberSchemas.Where<JsonSchemaModel>(new Func<JsonSchemaModel, bool>(this.ValidateArray)).ToList<JsonSchemaModel>();
                    this.Push(new SchemaScope(JTokenType.Array, schemas));
                    this.WriteToken(this.CurrentSchemas);
                    return;
                }
                case JsonToken.StartConstructor:
                    this.ProcessValue();
                    this.Push(new SchemaScope(JTokenType.Constructor, null));
                    this.WriteToken(this.CurrentSchemas);
                    return;

                case JsonToken.PropertyName:
                    this.WriteToken(this.CurrentSchemas);
                    foreach (JsonSchemaModel model in this.CurrentSchemas)
                    {
                        this.ValidatePropertyName(model);
                    }
                    return;

                case JsonToken.Raw:
                    this.ProcessValue();
                    return;

                case JsonToken.Integer:
                    this.ProcessValue();
                    this.WriteToken(this.CurrentMemberSchemas);
                    foreach (JsonSchemaModel model2 in this.CurrentMemberSchemas)
                    {
                        this.ValidateInteger(model2);
                    }
                    return;

                case JsonToken.Float:
                    this.ProcessValue();
                    this.WriteToken(this.CurrentMemberSchemas);
                    foreach (JsonSchemaModel model3 in this.CurrentMemberSchemas)
                    {
                        this.ValidateFloat(model3);
                    }
                    return;

                case JsonToken.String:
                    this.ProcessValue();
                    this.WriteToken(this.CurrentMemberSchemas);
                    foreach (JsonSchemaModel model4 in this.CurrentMemberSchemas)
                    {
                        this.ValidateString(model4);
                    }
                    return;

                case JsonToken.Boolean:
                    this.ProcessValue();
                    this.WriteToken(this.CurrentMemberSchemas);
                    foreach (JsonSchemaModel model5 in this.CurrentMemberSchemas)
                    {
                        this.ValidateBoolean(model5);
                    }
                    return;

                case JsonToken.Null:
                    this.ProcessValue();
                    this.WriteToken(this.CurrentMemberSchemas);
                    foreach (JsonSchemaModel model6 in this.CurrentMemberSchemas)
                    {
                        this.ValidateNull(model6);
                    }
                    return;

                case JsonToken.Undefined:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    this.WriteToken(this.CurrentMemberSchemas);
                    return;

                case JsonToken.EndObject:
                    this.WriteToken(this.CurrentSchemas);
                    foreach (JsonSchemaModel model7 in this.CurrentSchemas)
                    {
                        this.ValidateEndObject(model7);
                    }
                    this.Pop();
                    return;

                case JsonToken.EndArray:
                    this.WriteToken(this.CurrentSchemas);
                    foreach (JsonSchemaModel model8 in this.CurrentSchemas)
                    {
                        this.ValidateEndArray(model8);
                    }
                    this.Pop();
                    return;

                case JsonToken.EndConstructor:
                    this.WriteToken(this.CurrentSchemas);
                    this.Pop();
                    return;
            }
            throw new ArgumentOutOfRangeException();
        }

        private void ValidateEndArray(JsonSchemaModel schema)
        {
            if (schema != null)
            {
                int? maximumItems;
                int arrayItemCount = this._currentScope.ArrayItemCount;
                if (schema.MaximumItems.HasValue)
                {
                    maximumItems = schema.MaximumItems;
                    if ((arrayItemCount > maximumItems.GetValueOrDefault()) ? maximumItems.HasValue : false)
                    {
                        this.RaiseError("Array item count {0} exceeds maximum count of {1}.".FormatWith(CultureInfo.InvariantCulture, arrayItemCount, schema.MaximumItems), schema);
                    }
                }
                if (schema.MinimumItems.HasValue)
                {
                    maximumItems = schema.MinimumItems;
                    if ((arrayItemCount < maximumItems.GetValueOrDefault()) ? maximumItems.HasValue : false)
                    {
                        this.RaiseError("Array item count {0} is less than minimum count of {1}.".FormatWith(CultureInfo.InvariantCulture, arrayItemCount, schema.MinimumItems), schema);
                    }
                }
            }
        }

        private void ValidateEndObject(JsonSchemaModel schema)
        {
            if (schema != null)
            {
                Dictionary<string, bool> requiredProperties = this._currentScope.RequiredProperties;
                if (requiredProperties != null)
                {
                    if (<>c.<>9__50_0 == null)
                    {
                    }
                    if (<>c.<>9__50_1 == null)
                    {
                    }
                    List<string> list = requiredProperties.Where<KeyValuePair<string, bool>>((<>c.<>9__50_0 = new Func<KeyValuePair<string, bool>, bool>(<>c.<>9.<ValidateEndObject>b__50_0))).Select<KeyValuePair<string, bool>, string>((<>c.<>9__50_1 = new Func<KeyValuePair<string, bool>, string>(<>c.<>9.<ValidateEndObject>b__50_1))).ToList<string>();
                    if (list.Count > 0)
                    {
                        this.RaiseError("Required properties are missing from object: {0}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", list.ToArray())), schema);
                    }
                }
            }
        }

        private void ValidateFloat(JsonSchemaModel schema)
        {
            if ((schema != null) && this.TestType(schema, JsonSchemaType.Float))
            {
                double? maximum;
                this.ValidateNotDisallowed(schema);
                double num = Convert.ToDouble(this._reader.Value, CultureInfo.InvariantCulture);
                if (schema.Maximum.HasValue)
                {
                    maximum = schema.Maximum;
                    if ((num > maximum.GetValueOrDefault()) ? maximum.HasValue : false)
                    {
                        this.RaiseError("Float {0} exceeds maximum value of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.Maximum), schema);
                    }
                    if (schema.ExclusiveMaximum)
                    {
                        maximum = schema.Maximum;
                        if ((num == maximum.GetValueOrDefault()) ? maximum.HasValue : false)
                        {
                            this.RaiseError("Float {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.Maximum), schema);
                        }
                    }
                }
                if (schema.Minimum.HasValue)
                {
                    maximum = schema.Minimum;
                    if ((num < maximum.GetValueOrDefault()) ? maximum.HasValue : false)
                    {
                        this.RaiseError("Float {0} is less than minimum value of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.Minimum), schema);
                    }
                    if (schema.ExclusiveMinimum)
                    {
                        maximum = schema.Minimum;
                        if ((num == maximum.GetValueOrDefault()) ? maximum.HasValue : false)
                        {
                            this.RaiseError("Float {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.Minimum), schema);
                        }
                    }
                }
                if (schema.DivisibleBy.HasValue && !IsZero(FloatingPointRemainder(num, schema.DivisibleBy.GetValueOrDefault())))
                {
                    this.RaiseError("Float {0} is not evenly divisible by {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.DivisibleBy), schema);
                }
            }
        }

        private void ValidateInteger(JsonSchemaModel schema)
        {
            if ((schema != null) && this.TestType(schema, JsonSchemaType.Integer))
            {
                this.ValidateNotDisallowed(schema);
                object objA = this._reader.Value;
                if (schema.Maximum.HasValue)
                {
                    if (JValue.Compare(JTokenType.Integer, objA, schema.Maximum) > 0)
                    {
                        this.RaiseError("Integer {0} exceeds maximum value of {1}.".FormatWith(CultureInfo.InvariantCulture, objA, schema.Maximum), schema);
                    }
                    if (schema.ExclusiveMaximum && (JValue.Compare(JTokenType.Integer, objA, schema.Maximum) == 0))
                    {
                        this.RaiseError("Integer {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith(CultureInfo.InvariantCulture, objA, schema.Maximum), schema);
                    }
                }
                if (schema.Minimum.HasValue)
                {
                    if (JValue.Compare(JTokenType.Integer, objA, schema.Minimum) < 0)
                    {
                        this.RaiseError("Integer {0} is less than minimum value of {1}.".FormatWith(CultureInfo.InvariantCulture, objA, schema.Minimum), schema);
                    }
                    if (schema.ExclusiveMinimum && (JValue.Compare(JTokenType.Integer, objA, schema.Minimum) == 0))
                    {
                        this.RaiseError("Integer {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith(CultureInfo.InvariantCulture, objA, schema.Minimum), schema);
                    }
                }
                if (schema.DivisibleBy.HasValue)
                {
                    bool flag;
                    if (objA is BigInteger)
                    {
                        BigInteger integer = (BigInteger) objA;
                        if (!Math.Abs((double) (schema.DivisibleBy.Value - Math.Truncate(schema.DivisibleBy.Value))).Equals((double) 0.0))
                        {
                            flag = integer != 0L;
                        }
                        else
                        {
                            flag = (integer % new BigInteger(schema.DivisibleBy.Value)) != 0L;
                        }
                    }
                    else
                    {
                        flag = !IsZero(((double) Convert.ToInt64(objA, CultureInfo.InvariantCulture)) % schema.DivisibleBy.GetValueOrDefault());
                    }
                    if (flag)
                    {
                        this.RaiseError("Integer {0} is not evenly divisible by {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(objA), schema.DivisibleBy), schema);
                    }
                }
            }
        }

        private void ValidateNotDisallowed(JsonSchemaModel schema)
        {
            if (schema != null)
            {
                JsonSchemaType? currentNodeSchemaType = this.GetCurrentNodeSchemaType();
                if (currentNodeSchemaType.HasValue && JsonSchemaGenerator.HasFlag(new JsonSchemaType?(schema.Disallow), currentNodeSchemaType.GetValueOrDefault()))
                {
                    this.RaiseError("Type {0} is disallowed.".FormatWith(CultureInfo.InvariantCulture, currentNodeSchemaType), schema);
                }
            }
        }

        private void ValidateNull(JsonSchemaModel schema)
        {
            if ((schema != null) && this.TestType(schema, JsonSchemaType.Null))
            {
                this.ValidateNotDisallowed(schema);
            }
        }

        private bool ValidateObject(JsonSchemaModel schema) => 
            ((schema == null) || this.TestType(schema, JsonSchemaType.Object));

        private void ValidatePropertyName(JsonSchemaModel schema)
        {
            if (schema != null)
            {
                string key = Convert.ToString(this._reader.Value, CultureInfo.InvariantCulture);
                if (this._currentScope.RequiredProperties.ContainsKey(key))
                {
                    this._currentScope.RequiredProperties[key] = true;
                }
                if (!schema.AllowAdditionalProperties && !this.IsPropertyDefinied(schema, key))
                {
                    this.RaiseError("Property '{0}' has not been defined and the schema does not allow additional properties.".FormatWith(CultureInfo.InvariantCulture, key), schema);
                }
                this._currentScope.CurrentPropertyName = key;
            }
        }

        private void ValidateString(JsonSchemaModel schema)
        {
            if ((schema != null) && this.TestType(schema, JsonSchemaType.String))
            {
                int? maximumLength;
                this.ValidateNotDisallowed(schema);
                string str = this._reader.Value.ToString();
                if (schema.MaximumLength.HasValue)
                {
                    maximumLength = schema.MaximumLength;
                    if ((str.Length > maximumLength.GetValueOrDefault()) ? maximumLength.HasValue : false)
                    {
                        this.RaiseError("String '{0}' exceeds maximum length of {1}.".FormatWith(CultureInfo.InvariantCulture, str, schema.MaximumLength), schema);
                    }
                }
                if (schema.MinimumLength.HasValue)
                {
                    maximumLength = schema.MinimumLength;
                    if ((str.Length < maximumLength.GetValueOrDefault()) ? maximumLength.HasValue : false)
                    {
                        this.RaiseError("String '{0}' is less than minimum length of {1}.".FormatWith(CultureInfo.InvariantCulture, str, schema.MinimumLength), schema);
                    }
                }
                if (schema.Patterns != null)
                {
                    foreach (string str2 in schema.Patterns)
                    {
                        if (!Regex.IsMatch(str, str2))
                        {
                            this.RaiseError("String '{0}' does not match regex pattern '{1}'.".FormatWith(CultureInfo.InvariantCulture, str, str2), schema);
                        }
                    }
                }
            }
        }

        private void WriteToken(IList<JsonSchemaModel> schemas)
        {
            foreach (SchemaScope scope in this._stack)
            {
                bool flag = ((scope.TokenType == JTokenType.Array) && scope.IsUniqueArray) && (scope.ArrayItemCount > 0);
                if (!flag)
                {
                    if (<>c.<>9__49_0 == null)
                    {
                    }
                    if (!schemas.Any<JsonSchemaModel>((<>c.<>9__49_0 = new Func<JsonSchemaModel, bool>(<>c.<>9.<WriteToken>b__49_0))))
                    {
                        continue;
                    }
                }
                if (scope.CurrentItemWriter == null)
                {
                    if (JsonTokenUtils.IsEndToken(this._reader.TokenType))
                    {
                        continue;
                    }
                    scope.CurrentItemWriter = new JTokenWriter();
                }
                scope.CurrentItemWriter.WriteToken(this._reader, false);
                if ((scope.CurrentItemWriter.Top == 0) && (this._reader.TokenType != JsonToken.PropertyName))
                {
                    JToken token = scope.CurrentItemWriter.Token;
                    scope.CurrentItemWriter = null;
                    if (flag)
                    {
                        if (scope.UniqueArrayItems.Contains<JToken>(token, JToken.EqualityComparer))
                        {
                            if (<>c.<>9__49_1 == null)
                            {
                            }
                            this.RaiseError("Non-unique array item at index {0}.".FormatWith(CultureInfo.InvariantCulture, scope.ArrayItemCount - 1), scope.Schemas.First<JsonSchemaModel>(<>c.<>9__49_1 = new Func<JsonSchemaModel, bool>(<>c.<>9.<WriteToken>b__49_1)));
                        }
                        scope.UniqueArrayItems.Add(token);
                    }
                    else
                    {
                        if (<>c.<>9__49_2 == null)
                        {
                        }
                        if (schemas.Any<JsonSchemaModel>(<>c.<>9__49_2 = new Func<JsonSchemaModel, bool>(<>c.<>9.<WriteToken>b__49_2)))
                        {
                            foreach (JsonSchemaModel model in schemas)
                            {
                                if ((model.Enum != null) && !model.Enum.ContainsValue<JToken>(token, JToken.EqualityComparer))
                                {
                                    StringWriter textWriter = new StringWriter(CultureInfo.InvariantCulture);
                                    token.WriteTo(new JsonTextWriter(textWriter), new JsonConverter[0]);
                                    this.RaiseError("Value {0} is not defined in enum.".FormatWith(CultureInfo.InvariantCulture, textWriter.ToString()), model);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override object Value =>
            this._reader.Value;

        public override int Depth =>
            this._reader.Depth;

        public override string Path =>
            this._reader.Path;

        public override char QuoteChar
        {
            get => 
                this._reader.QuoteChar;
            protected internal set
            {
            }
        }

        public override JsonToken TokenType =>
            this._reader.TokenType;

        public override Type ValueType =>
            this._reader.ValueType;

        private IList<JsonSchemaModel> CurrentSchemas =>
            this._currentScope.Schemas;

        private IList<JsonSchemaModel> CurrentMemberSchemas
        {
            get
            {
                if (this._currentScope == null)
                {
                    return new List<JsonSchemaModel>(new JsonSchemaModel[] { this._model });
                }
                if ((this._currentScope.Schemas == null) || (this._currentScope.Schemas.Count == 0))
                {
                    return EmptySchemaList;
                }
                switch (this._currentScope.TokenType)
                {
                    case JTokenType.None:
                        return this._currentScope.Schemas;

                    case JTokenType.Object:
                    {
                        if (this._currentScope.CurrentPropertyName == null)
                        {
                            throw new JsonReaderException("CurrentPropertyName has not been set on scope.");
                        }
                        IList<JsonSchemaModel> list = new List<JsonSchemaModel>();
                        foreach (JsonSchemaModel model in this.CurrentSchemas)
                        {
                            if ((model.Properties != null) && model.Properties.TryGetValue(this._currentScope.CurrentPropertyName, out JsonSchemaModel model2))
                            {
                                list.Add(model2);
                            }
                            if (model.PatternProperties != null)
                            {
                                foreach (KeyValuePair<string, JsonSchemaModel> pair in model.PatternProperties)
                                {
                                    if (Regex.IsMatch(this._currentScope.CurrentPropertyName, pair.Key))
                                    {
                                        list.Add(pair.Value);
                                    }
                                }
                            }
                            if (((list.Count == 0) && model.AllowAdditionalProperties) && (model.AdditionalProperties != null))
                            {
                                list.Add(model.AdditionalProperties);
                            }
                        }
                        return list;
                    }
                    case JTokenType.Array:
                    {
                        IList<JsonSchemaModel> list2 = new List<JsonSchemaModel>();
                        foreach (JsonSchemaModel model3 in this.CurrentSchemas)
                        {
                            if (!model3.PositionalItemsValidation)
                            {
                                if ((model3.Items != null) && (model3.Items.Count > 0))
                                {
                                    list2.Add(model3.Items[0]);
                                }
                            }
                            else
                            {
                                if (((model3.Items != null) && (model3.Items.Count > 0)) && (model3.Items.Count > (this._currentScope.ArrayItemCount - 1)))
                                {
                                    list2.Add(model3.Items[this._currentScope.ArrayItemCount - 1]);
                                }
                                if (model3.AllowAdditionalItems && (model3.AdditionalItems != null))
                                {
                                    list2.Add(model3.AdditionalItems);
                                }
                            }
                        }
                        return list2;
                    }
                    case JTokenType.Constructor:
                        return EmptySchemaList;
                }
                throw new ArgumentOutOfRangeException("TokenType", "Unexpected token type: {0}".FormatWith(CultureInfo.InvariantCulture, this._currentScope.TokenType));
            }
        }

        public JsonSchema Schema
        {
            get => 
                this._schema;
            set
            {
                if (this.TokenType != JsonToken.None)
                {
                    throw new InvalidOperationException("Cannot change schema while validating JSON.");
                }
                this._schema = value;
                this._model = null;
            }
        }

        public JsonReader Reader =>
            this._reader;

        int IJsonLineInfo.LineNumber
        {
            get
            {
                IJsonLineInfo info = this._reader as IJsonLineInfo;
                return info?.LineNumber;
            }
        }

        int IJsonLineInfo.LinePosition
        {
            get
            {
                IJsonLineInfo info = this._reader as IJsonLineInfo;
                return info?.LinePosition;
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly JsonValidatingReader.<>c <>9 = new JsonValidatingReader.<>c();
            public static Func<JsonSchemaModel, bool> <>9__49_0;
            public static Func<JsonSchemaModel, bool> <>9__49_1;
            public static Func<JsonSchemaModel, bool> <>9__49_2;
            public static Func<KeyValuePair<string, bool>, bool> <>9__50_0;
            public static Func<KeyValuePair<string, bool>, string> <>9__50_1;

            internal bool <ValidateEndObject>b__50_0(KeyValuePair<string, bool> kv) => 
                !kv.Value;

            internal string <ValidateEndObject>b__50_1(KeyValuePair<string, bool> kv) => 
                kv.Key;

            internal bool <WriteToken>b__49_0(JsonSchemaModel s) => 
                (s.Enum > null);

            internal bool <WriteToken>b__49_1(JsonSchemaModel s) => 
                s.UniqueItems;

            internal bool <WriteToken>b__49_2(JsonSchemaModel s) => 
                (s.Enum > null);
        }

        private class SchemaScope
        {
            private readonly JTokenType _tokenType;
            private readonly IList<JsonSchemaModel> _schemas;
            private readonly Dictionary<string, bool> _requiredProperties;

            public SchemaScope(JTokenType tokenType, IList<JsonSchemaModel> schemas)
            {
                this._tokenType = tokenType;
                this._schemas = schemas;
                if (<>c.<>9__29_0 == null)
                {
                }
                if (<>c.<>9__29_1 == null)
                {
                }
                this._requiredProperties = schemas.SelectMany<JsonSchemaModel, string>(new Func<JsonSchemaModel, IEnumerable<string>>(this.GetRequiredProperties)).Distinct<string>().ToDictionary<string, string, bool>(<>c.<>9__29_0 = new Func<string, string>(<>c.<>9.<.ctor>b__29_0), <>c.<>9__29_1 = new Func<string, bool>(<>c.<>9.<.ctor>b__29_1));
                if (tokenType == JTokenType.Array)
                {
                    if (<>c.<>9__29_2 == null)
                    {
                    }
                    if (schemas.Any<JsonSchemaModel>(<>c.<>9__29_2 = new Func<JsonSchemaModel, bool>(<>c.<>9.<.ctor>b__29_2)))
                    {
                        this.IsUniqueArray = true;
                        this.UniqueArrayItems = new List<JToken>();
                    }
                }
            }

            private IEnumerable<string> GetRequiredProperties(JsonSchemaModel schema)
            {
                if ((schema == null) || (schema.Properties == null))
                {
                    return Enumerable.Empty<string>();
                }
                if (<>c.<>9__30_0 == null)
                {
                }
                if (<>c.<>9__30_1 == null)
                {
                }
                return schema.Properties.Where<KeyValuePair<string, JsonSchemaModel>>((<>c.<>9__30_0 = new Func<KeyValuePair<string, JsonSchemaModel>, bool>(<>c.<>9.<GetRequiredProperties>b__30_0))).Select<KeyValuePair<string, JsonSchemaModel>, string>((<>c.<>9__30_1 = new Func<KeyValuePair<string, JsonSchemaModel>, string>(<>c.<>9.<GetRequiredProperties>b__30_1)));
            }

            public string CurrentPropertyName { get; set; }

            public int ArrayItemCount { get; set; }

            public bool IsUniqueArray { get; set; }

            public IList<JToken> UniqueArrayItems { get; set; }

            public JTokenWriter CurrentItemWriter { get; set; }

            public IList<JsonSchemaModel> Schemas =>
                this._schemas;

            public Dictionary<string, bool> RequiredProperties =>
                this._requiredProperties;

            public JTokenType TokenType =>
                this._tokenType;

            [Serializable, CompilerGenerated]
            private sealed class <>c
            {
                public static readonly JsonValidatingReader.SchemaScope.<>c <>9 = new JsonValidatingReader.SchemaScope.<>c();
                public static Func<string, string> <>9__29_0;
                public static Func<string, bool> <>9__29_1;
                public static Func<JsonSchemaModel, bool> <>9__29_2;
                public static Func<KeyValuePair<string, JsonSchemaModel>, bool> <>9__30_0;
                public static Func<KeyValuePair<string, JsonSchemaModel>, string> <>9__30_1;

                internal string <.ctor>b__29_0(string p) => 
                    p;

                internal bool <.ctor>b__29_1(string p) => 
                    false;

                internal bool <.ctor>b__29_2(JsonSchemaModel s) => 
                    s.UniqueItems;

                internal bool <GetRequiredProperties>b__30_0(KeyValuePair<string, JsonSchemaModel> p) => 
                    p.Value.Required;

                internal string <GetRequiredProperties>b__30_1(KeyValuePair<string, JsonSchemaModel> p) => 
                    p.Key;
            }
        }
    }
}

