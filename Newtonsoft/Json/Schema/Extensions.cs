namespace Newtonsoft.Json.Schema
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
    public static class Extensions
    {
        [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
        public static bool IsValid(this JToken source, JsonSchema schema)
        {
            bool valid = true;
            source.Validate(schema, delegate (object sender, ValidationEventArgs args) {
                valid = false;
            });
            return valid;
        }

        [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
        public static bool IsValid(this JToken source, JsonSchema schema, out IList<string> errorMessages)
        {
            IList<string> errors = new List<string>();
            source.Validate(schema, delegate (object sender, ValidationEventArgs args) {
                errors.Add(args.Message);
            });
            errorMessages = errors;
            return (errorMessages.Count == 0);
        }

        [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
        public static void Validate(this JToken source, JsonSchema schema)
        {
            source.Validate(schema, null);
        }

        [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
        public static void Validate(this JToken source, JsonSchema schema, ValidationEventHandler validationEventHandler)
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            ValidationUtils.ArgumentNotNull(schema, "schema");
            using (JsonValidatingReader reader = new JsonValidatingReader(source.CreateReader()))
            {
                reader.Schema = schema;
                if (validationEventHandler != null)
                {
                    reader.ValidationEventHandler += validationEventHandler;
                }
                while (reader.Read())
                {
                }
            }
        }
    }
}

