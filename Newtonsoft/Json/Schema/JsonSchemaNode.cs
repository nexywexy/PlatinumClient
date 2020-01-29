namespace Newtonsoft.Json.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
    internal class JsonSchemaNode
    {
        public JsonSchemaNode(JsonSchema schema)
        {
            JsonSchema[] list = new JsonSchema[] { schema };
            this.Schemas = new ReadOnlyCollection<JsonSchema>(list);
            this.Properties = new Dictionary<string, JsonSchemaNode>();
            this.PatternProperties = new Dictionary<string, JsonSchemaNode>();
            this.Items = new List<JsonSchemaNode>();
            this.Id = GetId(this.Schemas);
        }

        private JsonSchemaNode(JsonSchemaNode source, JsonSchema schema)
        {
            JsonSchema[] second = new JsonSchema[] { schema };
            this.Schemas = new ReadOnlyCollection<JsonSchema>(source.Schemas.Union<JsonSchema>(second).ToList<JsonSchema>());
            this.Properties = new Dictionary<string, JsonSchemaNode>(source.Properties);
            this.PatternProperties = new Dictionary<string, JsonSchemaNode>(source.PatternProperties);
            this.Items = new List<JsonSchemaNode>(source.Items);
            this.AdditionalProperties = source.AdditionalProperties;
            this.AdditionalItems = source.AdditionalItems;
            this.Id = GetId(this.Schemas);
        }

        public JsonSchemaNode Combine(JsonSchema schema) => 
            new JsonSchemaNode(this, schema);

        public static string GetId(IEnumerable<JsonSchema> schemata)
        {
            if (<>c.<>9__31_0 == null)
            {
            }
            if (<>c.<>9__31_1 == null)
            {
            }
            return string.Join("-", schemata.Select<JsonSchema, string>((<>c.<>9__31_0 = new Func<JsonSchema, string>(<>c.<>9.<GetId>b__31_0))).OrderBy<string, string>((<>c.<>9__31_1 = new Func<string, string>(<>c.<>9.<GetId>b__31_1)), StringComparer.Ordinal).ToArray<string>());
        }

        public string Id { get; private set; }

        public ReadOnlyCollection<JsonSchema> Schemas { get; private set; }

        public Dictionary<string, JsonSchemaNode> Properties { get; private set; }

        public Dictionary<string, JsonSchemaNode> PatternProperties { get; private set; }

        public List<JsonSchemaNode> Items { get; private set; }

        public JsonSchemaNode AdditionalProperties { get; set; }

        public JsonSchemaNode AdditionalItems { get; set; }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly JsonSchemaNode.<>c <>9 = new JsonSchemaNode.<>c();
            public static Func<JsonSchema, string> <>9__31_0;
            public static Func<string, string> <>9__31_1;

            internal string <GetId>b__31_0(JsonSchema s) => 
                s.InternalId;

            internal string <GetId>b__31_1(string id) => 
                id;
        }
    }
}

