namespace Newtonsoft.Json.Schema
{
    using Newtonsoft.Json.Utilities;
    using System;

    [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
    public class ValidationEventArgs : EventArgs
    {
        private readonly JsonSchemaException _ex;

        internal ValidationEventArgs(JsonSchemaException ex)
        {
            ValidationUtils.ArgumentNotNull(ex, "ex");
            this._ex = ex;
        }

        public JsonSchemaException Exception =>
            this._ex;

        public string Path =>
            this._ex.Path;

        public string Message =>
            this._ex.Message;
    }
}

