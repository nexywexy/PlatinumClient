namespace Newtonsoft.Json.Schema
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
    internal class JsonSchemaBuilder
    {
        private readonly IList<JsonSchema> _stack = new List<JsonSchema>();
        private readonly JsonSchemaResolver _resolver;
        private readonly IDictionary<string, JsonSchema> _documentSchemas = new Dictionary<string, JsonSchema>();
        private JsonSchema _currentSchema;
        private JObject _rootSchema;

        public JsonSchemaBuilder(JsonSchemaResolver resolver)
        {
            this._resolver = resolver;
        }

        private JsonSchema BuildSchema(JToken token)
        {
            JObject schemaObject = token as JObject;
            if (schemaObject == null)
            {
                throw JsonException.Create(token, token.Path, "Expected object while parsing schema object, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));
            }
            if (schemaObject.TryGetValue("$ref", out JToken token2))
            {
                return new JsonSchema { DeferredReference = (string) token2 };
            }
            string str = token.Path.Replace(".", "/").Replace("[", "/").Replace("]", string.Empty);
            if (!string.IsNullOrEmpty(str))
            {
                str = "/" + str;
            }
            str = "#" + str;
            if (this._documentSchemas.TryGetValue(str, out JsonSchema schema))
            {
                return schema;
            }
            JsonSchema schema2 = new JsonSchema {
                Location = str
            };
            this.Push(schema2);
            this.ProcessSchemaProperties(schemaObject);
            return this.Pop();
        }

        internal static string MapType(JsonSchemaType type) => 
            JsonSchemaConstants.JsonSchemaTypeMapping.Single<KeyValuePair<string, JsonSchemaType>>(kv => (((JsonSchemaType) kv.Value) == type)).Key;

        internal static JsonSchemaType MapType(string type)
        {
            if (!JsonSchemaConstants.JsonSchemaTypeMapping.TryGetValue(type, out JsonSchemaType type2))
            {
                throw new JsonException("Invalid JSON schema type: {0}".FormatWith(CultureInfo.InvariantCulture, type));
            }
            return type2;
        }

        private JsonSchema Pop()
        {
            this._stack.RemoveAt(this._stack.Count - 1);
            this._currentSchema = this._stack.LastOrDefault<JsonSchema>();
            return this._currentSchema;
        }

        private void ProcessAdditionalItems(JToken token)
        {
            if (token.Type == JTokenType.Boolean)
            {
                this.CurrentSchema.AllowAdditionalItems = (bool) token;
            }
            else
            {
                this.CurrentSchema.AdditionalItems = this.BuildSchema(token);
            }
        }

        private void ProcessAdditionalProperties(JToken token)
        {
            if (token.Type == JTokenType.Boolean)
            {
                this.CurrentSchema.AllowAdditionalProperties = (bool) token;
            }
            else
            {
                this.CurrentSchema.AdditionalProperties = this.BuildSchema(token);
            }
        }

        private void ProcessEnum(JToken token)
        {
            if (token.Type != JTokenType.Array)
            {
                throw JsonException.Create(token, token.Path, "Expected Array token while parsing enum values, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));
            }
            this.CurrentSchema.Enum = new List<JToken>();
            foreach (JToken token2 in (IEnumerable<JToken>) token)
            {
                this.CurrentSchema.Enum.Add(token2.DeepClone());
            }
        }

        private void ProcessExtends(JToken token)
        {
            IList<JsonSchema> list = new List<JsonSchema>();
            if (token.Type == JTokenType.Array)
            {
                foreach (JToken token2 in (IEnumerable<JToken>) token)
                {
                    list.Add(this.BuildSchema(token2));
                }
            }
            else
            {
                JsonSchema item = this.BuildSchema(token);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            if (list.Count > 0)
            {
                this.CurrentSchema.Extends = list;
            }
        }

        private void ProcessItems(JToken token)
        {
            this.CurrentSchema.Items = new List<JsonSchema>();
            JTokenType type = token.Type;
            if (type != JTokenType.Object)
            {
                if (type != JTokenType.Array)
                {
                    throw JsonException.Create(token, token.Path, "Expected array or JSON schema object, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));
                }
            }
            else
            {
                this.CurrentSchema.Items.Add(this.BuildSchema(token));
                this.CurrentSchema.PositionalItemsValidation = false;
                return;
            }
            this.CurrentSchema.PositionalItemsValidation = true;
            foreach (JToken token2 in (IEnumerable<JToken>) token)
            {
                this.CurrentSchema.Items.Add(this.BuildSchema(token2));
            }
        }

        private IDictionary<string, JsonSchema> ProcessProperties(JToken token)
        {
            IDictionary<string, JsonSchema> dictionary = new Dictionary<string, JsonSchema>();
            if (token.Type != JTokenType.Object)
            {
                throw JsonException.Create(token, token.Path, "Expected Object token while parsing schema properties, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));
            }
            foreach (JProperty property in (IEnumerable<JToken>) token)
            {
                if (dictionary.ContainsKey(property.Name))
                {
                    throw new JsonException("Property {0} has already been defined in schema.".FormatWith(CultureInfo.InvariantCulture, property.Name));
                }
                dictionary.Add(property.Name, this.BuildSchema(property.Value));
            }
            return dictionary;
        }

        private void ProcessSchemaProperties(JObject schemaObject)
        {
            foreach (KeyValuePair<string, JToken> pair in schemaObject)
            {
                string key = pair.Key;
                switch (<PrivateImplementationDetails>.ComputeStringHash(key))
                {
                    case 0x11de6cdc:
                    {
                        if (key == "properties")
                        {
                            goto Label_04F8;
                        }
                        continue;
                    }
                    case 0x13f0fb79:
                    {
                        if (key == "minItems")
                        {
                            goto Label_068E;
                        }
                        continue;
                    }
                    case 0x150efe0d:
                    {
                        if (key == "divisibleBy")
                        {
                            goto Label_06AF;
                        }
                        continue;
                    }
                    case 0x37386ae0:
                    {
                        if (key == "id")
                        {
                            goto Label_04A4;
                        }
                        continue;
                    }
                    case 0x3a79338f:
                    {
                        if (key == "items")
                        {
                            goto Label_0515;
                        }
                        continue;
                    }
                    case 0x1c9c30e1:
                    {
                        if (key == "additionalProperties")
                        {
                            goto Label_0527;
                        }
                        continue;
                    }
                    case 0x346f3b69:
                    {
                        if (key == "description")
                        {
                            goto Label_04DC;
                        }
                        continue;
                    }
                    case 0x5127f14d:
                    {
                        if (key == "type")
                        {
                            break;
                        }
                        continue;
                    }
                    case 0x5bf2f681:
                    {
                        if (key == "maximum")
                        {
                            goto Label_05C7;
                        }
                        continue;
                    }
                    case 0x64f7c28b:
                    {
                        if (key == "exclusiveMaximum")
                        {
                            goto Label_060A;
                        }
                        continue;
                    }
                    case 0x816cb000:
                    {
                        if (key == "enum")
                        {
                            goto Label_0784;
                        }
                        continue;
                    }
                    case 0x848c8620:
                    {
                        if (key == "required")
                        {
                            goto Label_0568;
                        }
                        continue;
                    }
                    case 0x720625cd:
                    {
                        if (key == "exclusiveMinimum")
                        {
                            goto Label_05E9;
                        }
                        continue;
                    }
                    case 0x7a472400:
                    {
                        if (key == "additionalItems")
                        {
                            goto Label_0539;
                        }
                        continue;
                    }
                    case 0x873d0129:
                    {
                        if (key == "pattern")
                        {
                            goto Label_0768;
                        }
                        continue;
                    }
                    case 0x933b5bde:
                    {
                        if (key == "default")
                        {
                            goto Label_06EE;
                        }
                        continue;
                    }
                    case 0x938122f7:
                    {
                        if (key == "minimum")
                        {
                            goto Label_05A5;
                        }
                        continue;
                    }
                    case 0x9d85d64e:
                    {
                        if (key == "extends")
                        {
                            goto Label_0796;
                        }
                        continue;
                    }
                    case 0xa07863c0:
                    {
                        if (key == "disallow")
                        {
                            goto Label_06D1;
                        }
                        continue;
                    }
                    case 0x9865b509:
                    {
                        if (key == "title")
                        {
                            goto Label_04C0;
                        }
                        continue;
                    }
                    case 0x9b8caa55:
                    {
                        if (key == "requires")
                        {
                            goto Label_0589;
                        }
                        continue;
                    }
                    case 0xce0beff7:
                    {
                        if (key == "readonly")
                        {
                            goto Label_072B;
                        }
                        continue;
                    }
                    case 0xd1f6a662:
                    {
                        if (key == "uniqueItems")
                        {
                            goto Label_07A8;
                        }
                        continue;
                    }
                    case 0xb0443bf7:
                    {
                        if (key == "minLength")
                        {
                            goto Label_064C;
                        }
                        continue;
                    }
                    case 0xb99d8552:
                    {
                        if (key == "format")
                        {
                            goto Label_074C;
                        }
                        continue;
                    }
                    case 0xd23308c1:
                    {
                        if (key == "maxLength")
                        {
                            goto Label_062B;
                        }
                        continue;
                    }
                    case 0xeb4bb270:
                    {
                        if (key == "patternProperties")
                        {
                            goto Label_054B;
                        }
                        continue;
                    }
                    case 0xf618f139:
                    {
                        if (key == "hidden")
                        {
                            goto Label_070A;
                        }
                        continue;
                    }
                    case 0xfcfb3733:
                    {
                        if (key == "maxItems")
                        {
                            goto Label_066D;
                        }
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
                this.CurrentSchema.Type = this.ProcessType(pair.Value);
                continue;
            Label_04A4:
                this.CurrentSchema.Id = (string) pair.Value;
                continue;
            Label_04C0:
                this.CurrentSchema.Title = (string) pair.Value;
                continue;
            Label_04DC:
                this.CurrentSchema.Description = (string) pair.Value;
                continue;
            Label_04F8:
                this.CurrentSchema.Properties = this.ProcessProperties(pair.Value);
                continue;
            Label_0515:
                this.ProcessItems(pair.Value);
                continue;
            Label_0527:
                this.ProcessAdditionalProperties(pair.Value);
                continue;
            Label_0539:
                this.ProcessAdditionalItems(pair.Value);
                continue;
            Label_054B:
                this.CurrentSchema.PatternProperties = this.ProcessProperties(pair.Value);
                continue;
            Label_0568:
                this.CurrentSchema.Required = new bool?((bool) pair.Value);
                continue;
            Label_0589:
                this.CurrentSchema.Requires = (string) pair.Value;
                continue;
            Label_05A5:
                this.CurrentSchema.Minimum = new double?((double) pair.Value);
                continue;
            Label_05C7:
                this.CurrentSchema.Maximum = new double?((double) pair.Value);
                continue;
            Label_05E9:
                this.CurrentSchema.ExclusiveMinimum = new bool?((bool) pair.Value);
                continue;
            Label_060A:
                this.CurrentSchema.ExclusiveMaximum = new bool?((bool) pair.Value);
                continue;
            Label_062B:
                this.CurrentSchema.MaximumLength = new int?((int) pair.Value);
                continue;
            Label_064C:
                this.CurrentSchema.MinimumLength = new int?((int) pair.Value);
                continue;
            Label_066D:
                this.CurrentSchema.MaximumItems = new int?((int) pair.Value);
                continue;
            Label_068E:
                this.CurrentSchema.MinimumItems = new int?((int) pair.Value);
                continue;
            Label_06AF:
                this.CurrentSchema.DivisibleBy = new double?((double) pair.Value);
                continue;
            Label_06D1:
                this.CurrentSchema.Disallow = this.ProcessType(pair.Value);
                continue;
            Label_06EE:
                this.CurrentSchema.Default = pair.Value.DeepClone();
                continue;
            Label_070A:
                this.CurrentSchema.Hidden = new bool?((bool) pair.Value);
                continue;
            Label_072B:
                this.CurrentSchema.ReadOnly = new bool?((bool) pair.Value);
                continue;
            Label_074C:
                this.CurrentSchema.Format = (string) pair.Value;
                continue;
            Label_0768:
                this.CurrentSchema.Pattern = (string) pair.Value;
                continue;
            Label_0784:
                this.ProcessEnum(pair.Value);
                continue;
            Label_0796:
                this.ProcessExtends(pair.Value);
                continue;
            Label_07A8:
                this.CurrentSchema.UniqueItems = (bool) pair.Value;
            }
        }

        private JsonSchemaType? ProcessType(JToken token)
        {
            JTokenType type = token.Type;
            if (type != JTokenType.Array)
            {
                if (type != JTokenType.String)
                {
                    throw JsonException.Create(token, token.Path, "Expected array or JSON schema type string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));
                }
                return new JsonSchemaType?(MapType((string) token));
            }
            JsonSchemaType? nullable = new JsonSchemaType?(JsonSchemaType.None);
            foreach (JToken token2 in (IEnumerable<JToken>) token)
            {
                if (token2.Type != JTokenType.String)
                {
                    throw JsonException.Create(token2, token2.Path, "Exception JSON schema type string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));
                }
                JsonSchemaType? nullable2 = nullable;
                JsonSchemaType type2 = MapType((string) token2);
                nullable = nullable2.HasValue ? new JsonSchemaType?(((JsonSchemaType) nullable2.GetValueOrDefault()) | type2) : null;
            }
            return nullable;
        }

        private void Push(JsonSchema value)
        {
            this._currentSchema = value;
            this._stack.Add(value);
            this._resolver.LoadedSchemas.Add(value);
            this._documentSchemas.Add(value.Location, value);
        }

        internal JsonSchema Read(JsonReader reader)
        {
            JToken token = JToken.ReadFrom(reader);
            this._rootSchema = token as JObject;
            JsonSchema schema = this.BuildSchema(token);
            this.ResolveReferences(schema);
            return schema;
        }

        private JsonSchema ResolveReferences(JsonSchema schema)
        {
            if (schema.DeferredReference != null)
            {
                string deferredReference = schema.DeferredReference;
                bool flag = deferredReference.StartsWith("#", StringComparison.Ordinal);
                if (flag)
                {
                    deferredReference = this.UnescapeReference(deferredReference);
                }
                JsonSchema schema2 = this._resolver.GetSchema(deferredReference);
                if (schema2 == null)
                {
                    if (flag)
                    {
                        char[] trimChars = new char[] { '#' };
                        char[] separator = new char[] { '/' };
                        JToken source = this._rootSchema;
                        foreach (string str2 in schema.DeferredReference.TrimStart(trimChars).Split(separator, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string s = this.UnescapeReference(str2);
                            if (source.Type == JTokenType.Object)
                            {
                                source = source[s];
                            }
                            else if ((source.Type == JTokenType.Array) || (source.Type == JTokenType.Constructor))
                            {
                                if ((int.TryParse(s, out int num2) && (num2 >= 0)) && (num2 < source.Count<JToken>()))
                                {
                                    source = source[num2];
                                }
                                else
                                {
                                    source = null;
                                }
                            }
                            if (source == null)
                            {
                                break;
                            }
                        }
                        if (source != null)
                        {
                            schema2 = this.BuildSchema(source);
                        }
                    }
                    if (schema2 == null)
                    {
                        throw new JsonException("Could not resolve schema reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, schema.DeferredReference));
                    }
                }
                schema = schema2;
            }
            if (!schema.ReferencesResolved)
            {
                schema.ReferencesResolved = true;
                if (schema.Extends != null)
                {
                    for (int i = 0; i < schema.Extends.Count; i++)
                    {
                        schema.Extends[i] = this.ResolveReferences(schema.Extends[i]);
                    }
                }
                if (schema.Items != null)
                {
                    for (int i = 0; i < schema.Items.Count; i++)
                    {
                        schema.Items[i] = this.ResolveReferences(schema.Items[i]);
                    }
                }
                if (schema.AdditionalItems != null)
                {
                    schema.AdditionalItems = this.ResolveReferences(schema.AdditionalItems);
                }
                if (schema.PatternProperties != null)
                {
                    foreach (KeyValuePair<string, JsonSchema> pair in schema.PatternProperties.ToList<KeyValuePair<string, JsonSchema>>())
                    {
                        schema.PatternProperties[pair.Key] = this.ResolveReferences(pair.Value);
                    }
                }
                if (schema.Properties != null)
                {
                    foreach (KeyValuePair<string, JsonSchema> pair2 in schema.Properties.ToList<KeyValuePair<string, JsonSchema>>())
                    {
                        schema.Properties[pair2.Key] = this.ResolveReferences(pair2.Value);
                    }
                }
                if (schema.AdditionalProperties != null)
                {
                    schema.AdditionalProperties = this.ResolveReferences(schema.AdditionalProperties);
                }
            }
            return schema;
        }

        private string UnescapeReference(string reference) => 
            Uri.UnescapeDataString(reference).Replace("~1", "/").Replace("~0", "~");

        private JsonSchema CurrentSchema =>
            this._currentSchema;
    }
}

