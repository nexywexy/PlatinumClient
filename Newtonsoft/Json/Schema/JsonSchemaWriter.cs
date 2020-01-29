﻿namespace Newtonsoft.Json.Schema
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
    internal class JsonSchemaWriter
    {
        private readonly JsonWriter _writer;
        private readonly JsonSchemaResolver _resolver;

        public JsonSchemaWriter(JsonWriter writer, JsonSchemaResolver resolver)
        {
            ValidationUtils.ArgumentNotNull(writer, "writer");
            this._writer = writer;
            this._resolver = resolver;
        }

        private void ReferenceOrWriteSchema(JsonSchema schema)
        {
            if ((schema.Id != null) && (this._resolver.GetSchema(schema.Id) != null))
            {
                this._writer.WriteStartObject();
                this._writer.WritePropertyName("$ref");
                this._writer.WriteValue(schema.Id);
                this._writer.WriteEndObject();
            }
            else
            {
                this.WriteSchema(schema);
            }
        }

        private void WriteItems(JsonSchema schema)
        {
            if ((schema.Items != null) || schema.PositionalItemsValidation)
            {
                this._writer.WritePropertyName("items");
                if (!schema.PositionalItemsValidation)
                {
                    if ((schema.Items != null) && (schema.Items.Count > 0))
                    {
                        this.ReferenceOrWriteSchema(schema.Items[0]);
                    }
                    else
                    {
                        this._writer.WriteStartObject();
                        this._writer.WriteEndObject();
                    }
                }
                else
                {
                    this._writer.WriteStartArray();
                    if (schema.Items != null)
                    {
                        foreach (JsonSchema schema2 in schema.Items)
                        {
                            this.ReferenceOrWriteSchema(schema2);
                        }
                    }
                    this._writer.WriteEndArray();
                }
            }
        }

        private void WritePropertyIfNotNull(JsonWriter writer, string propertyName, object value)
        {
            if (value != null)
            {
                writer.WritePropertyName(propertyName);
                writer.WriteValue(value);
            }
        }

        public void WriteSchema(JsonSchema schema)
        {
            ValidationUtils.ArgumentNotNull(schema, "schema");
            if (!this._resolver.LoadedSchemas.Contains(schema))
            {
                this._resolver.LoadedSchemas.Add(schema);
            }
            this._writer.WriteStartObject();
            this.WritePropertyIfNotNull(this._writer, "id", schema.Id);
            this.WritePropertyIfNotNull(this._writer, "title", schema.Title);
            this.WritePropertyIfNotNull(this._writer, "description", schema.Description);
            this.WritePropertyIfNotNull(this._writer, "required", schema.Required);
            this.WritePropertyIfNotNull(this._writer, "readonly", schema.ReadOnly);
            this.WritePropertyIfNotNull(this._writer, "hidden", schema.Hidden);
            this.WritePropertyIfNotNull(this._writer, "transient", schema.Transient);
            if (schema.Type.HasValue)
            {
                this.WriteType("type", this._writer, schema.Type.GetValueOrDefault());
            }
            if (!schema.AllowAdditionalProperties)
            {
                this._writer.WritePropertyName("additionalProperties");
                this._writer.WriteValue(schema.AllowAdditionalProperties);
            }
            else if (schema.AdditionalProperties != null)
            {
                this._writer.WritePropertyName("additionalProperties");
                this.ReferenceOrWriteSchema(schema.AdditionalProperties);
            }
            if (!schema.AllowAdditionalItems)
            {
                this._writer.WritePropertyName("additionalItems");
                this._writer.WriteValue(schema.AllowAdditionalItems);
            }
            else if (schema.AdditionalItems != null)
            {
                this._writer.WritePropertyName("additionalItems");
                this.ReferenceOrWriteSchema(schema.AdditionalItems);
            }
            this.WriteSchemaDictionaryIfNotNull(this._writer, "properties", schema.Properties);
            this.WriteSchemaDictionaryIfNotNull(this._writer, "patternProperties", schema.PatternProperties);
            this.WriteItems(schema);
            this.WritePropertyIfNotNull(this._writer, "minimum", schema.Minimum);
            this.WritePropertyIfNotNull(this._writer, "maximum", schema.Maximum);
            this.WritePropertyIfNotNull(this._writer, "exclusiveMinimum", schema.ExclusiveMinimum);
            this.WritePropertyIfNotNull(this._writer, "exclusiveMaximum", schema.ExclusiveMaximum);
            this.WritePropertyIfNotNull(this._writer, "minLength", schema.MinimumLength);
            this.WritePropertyIfNotNull(this._writer, "maxLength", schema.MaximumLength);
            this.WritePropertyIfNotNull(this._writer, "minItems", schema.MinimumItems);
            this.WritePropertyIfNotNull(this._writer, "maxItems", schema.MaximumItems);
            this.WritePropertyIfNotNull(this._writer, "divisibleBy", schema.DivisibleBy);
            this.WritePropertyIfNotNull(this._writer, "format", schema.Format);
            this.WritePropertyIfNotNull(this._writer, "pattern", schema.Pattern);
            if (schema.Enum != null)
            {
                this._writer.WritePropertyName("enum");
                this._writer.WriteStartArray();
                using (IEnumerator<JToken> enumerator = schema.Enum.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.WriteTo(this._writer, new JsonConverter[0]);
                    }
                }
                this._writer.WriteEndArray();
            }
            if (schema.Default != null)
            {
                this._writer.WritePropertyName("default");
                schema.Default.WriteTo(this._writer, new JsonConverter[0]);
            }
            if (schema.Disallow.HasValue)
            {
                this.WriteType("disallow", this._writer, schema.Disallow.GetValueOrDefault());
            }
            if ((schema.Extends != null) && (schema.Extends.Count > 0))
            {
                this._writer.WritePropertyName("extends");
                if (schema.Extends.Count == 1)
                {
                    this.ReferenceOrWriteSchema(schema.Extends[0]);
                }
                else
                {
                    this._writer.WriteStartArray();
                    foreach (JsonSchema schema2 in schema.Extends)
                    {
                        this.ReferenceOrWriteSchema(schema2);
                    }
                    this._writer.WriteEndArray();
                }
            }
            this._writer.WriteEndObject();
        }

        private void WriteSchemaDictionaryIfNotNull(JsonWriter writer, string propertyName, IDictionary<string, JsonSchema> properties)
        {
            if (properties != null)
            {
                writer.WritePropertyName(propertyName);
                writer.WriteStartObject();
                foreach (KeyValuePair<string, JsonSchema> pair in properties)
                {
                    writer.WritePropertyName(pair.Key);
                    this.ReferenceOrWriteSchema(pair.Value);
                }
                writer.WriteEndObject();
            }
        }

        private void WriteType(string propertyName, JsonWriter writer, JsonSchemaType type)
        {
            IList<JsonSchemaType> list;
            if (Enum.IsDefined(typeof(JsonSchemaType), type))
            {
                List<JsonSchemaType> list1 = new List<JsonSchemaType> {
                    type
                };
                list = list1;
            }
            else
            {
                if (<>c.<>9__7_0 == null)
                {
                }
                list = EnumUtils.GetFlagsValues<JsonSchemaType>(type).Where<JsonSchemaType>((<>c.<>9__7_0 = new Func<JsonSchemaType, bool>(<>c.<>9.<WriteType>b__7_0))).ToList<JsonSchemaType>();
            }
            if (list.Count != 0)
            {
                writer.WritePropertyName(propertyName);
                if (list.Count == 1)
                {
                    writer.WriteValue(JsonSchemaBuilder.MapType(list[0]));
                }
                else
                {
                    writer.WriteStartArray();
                    foreach (JsonSchemaType type2 in list)
                    {
                        writer.WriteValue(JsonSchemaBuilder.MapType(type2));
                    }
                    writer.WriteEndArray();
                }
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly JsonSchemaWriter.<>c <>9 = new JsonSchemaWriter.<>c();
            public static Func<JsonSchemaType, bool> <>9__7_0;

            internal bool <WriteType>b__7_0(JsonSchemaType v) => 
                (v > JsonSchemaType.None);
        }
    }
}

