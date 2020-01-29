namespace Newtonsoft.Json.Schema
{
    using System;
    using System.Collections.ObjectModel;

    [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
    internal class JsonSchemaNodeCollection : KeyedCollection<string, JsonSchemaNode>
    {
        protected override string GetKeyForItem(JsonSchemaNode item) => 
            item.Id;
    }
}

