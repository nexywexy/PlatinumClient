namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal abstract class JsonSerializerInternalBase
    {
        private ErrorContext _currentErrorContext;
        private BidirectionalDictionary<string, object> _mappings;
        internal readonly JsonSerializer Serializer;
        internal readonly ITraceWriter TraceWriter;
        protected JsonSerializerProxy InternalSerializer;

        protected JsonSerializerInternalBase(JsonSerializer serializer)
        {
            ValidationUtils.ArgumentNotNull(serializer, "serializer");
            this.Serializer = serializer;
            this.TraceWriter = serializer.TraceWriter;
        }

        protected void ClearErrorContext()
        {
            if (this._currentErrorContext == null)
            {
                throw new InvalidOperationException("Could not clear error context. Error context is already null.");
            }
            this._currentErrorContext = null;
        }

        private ErrorContext GetErrorContext(object currentObject, object member, string path, Exception error)
        {
            if (this._currentErrorContext == null)
            {
                this._currentErrorContext = new ErrorContext(currentObject, member, path, error);
            }
            if (this._currentErrorContext.Error != error)
            {
                throw new InvalidOperationException("Current error context error is different to requested error.");
            }
            return this._currentErrorContext;
        }

        protected bool IsErrorHandled(object currentObject, JsonContract contract, object keyValue, IJsonLineInfo lineInfo, string path, Exception ex)
        {
            ErrorContext errorContext = this.GetErrorContext(currentObject, keyValue, path, ex);
            if (((this.TraceWriter != null) && (this.TraceWriter.LevelFilter >= TraceLevel.Error)) && !errorContext.Traced)
            {
                errorContext.Traced = true;
                string message = (base.GetType() == typeof(JsonSerializerInternalWriter)) ? "Error serializing" : "Error deserializing";
                if (contract != null)
                {
                    message = message + " " + contract.UnderlyingType;
                }
                message = message + ". " + ex.Message;
                if (!(ex is JsonException))
                {
                    message = JsonPosition.FormatMessage(lineInfo, path, message);
                }
                this.TraceWriter.Trace(TraceLevel.Error, message, ex);
            }
            if ((contract != null) && (currentObject != null))
            {
                contract.InvokeOnError(currentObject, this.Serializer.Context, errorContext);
            }
            if (!errorContext.Handled)
            {
                this.Serializer.OnError(new ErrorEventArgs(currentObject, errorContext));
            }
            return errorContext.Handled;
        }

        internal BidirectionalDictionary<string, object> DefaultReferenceMappings
        {
            get
            {
                if (this._mappings == null)
                {
                    this._mappings = new BidirectionalDictionary<string, object>(EqualityComparer<string>.Default, new ReferenceEqualsEqualityComparer(), "A different value already has the Id '{0}'.", "A different Id has already been assigned for value '{0}'.");
                }
                return this._mappings;
            }
        }

        private class ReferenceEqualsEqualityComparer : IEqualityComparer<object>
        {
            bool IEqualityComparer<object>.Equals(object x, object y) => 
                (x == y);

            int IEqualityComparer<object>.GetHashCode(object obj) => 
                RuntimeHelpers.GetHashCode(obj);
        }
    }
}

