namespace Newtonsoft.Json.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
    public class JsonSchemaResolver
    {
        public JsonSchemaResolver()
        {
            this.LoadedSchemas = new List<JsonSchema>();
        }

        public virtual JsonSchema GetSchema(string reference)
        {
            JsonSchema schema = this.LoadedSchemas.SingleOrDefault<JsonSchema>(s => string.Equals(s.Id, reference, StringComparison.Ordinal));
            if (schema == null)
            {
                schema = this.LoadedSchemas.SingleOrDefault<JsonSchema>(s => string.Equals(s.Location, reference, StringComparison.Ordinal));
            }
            return schema;
        }

        public IList<JsonSchema> LoadedSchemas { get; protected set; }
    }
}

