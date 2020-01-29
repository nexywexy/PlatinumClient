namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    public abstract class JsonContract
    {
        internal bool IsNullable;
        internal bool IsConvertable;
        internal bool IsEnum;
        internal Type NonNullableUnderlyingType;
        internal ReadType InternalReadType;
        internal JsonContractType ContractType;
        internal bool IsReadOnlyOrFixedSize;
        internal bool IsSealed;
        internal bool IsInstantiable;
        private List<SerializationCallback> _onDeserializedCallbacks;
        private IList<SerializationCallback> _onDeserializingCallbacks;
        private IList<SerializationCallback> _onSerializedCallbacks;
        private IList<SerializationCallback> _onSerializingCallbacks;
        private IList<SerializationErrorCallback> _onErrorCallbacks;
        private Type _createdType;

        internal JsonContract(Type underlyingType)
        {
            ValidationUtils.ArgumentNotNull(underlyingType, "underlyingType");
            this.UnderlyingType = underlyingType;
            this.IsNullable = ReflectionUtils.IsNullable(underlyingType);
            this.NonNullableUnderlyingType = (this.IsNullable && ReflectionUtils.IsNullableType(underlyingType)) ? Nullable.GetUnderlyingType(underlyingType) : underlyingType;
            this.CreatedType = this.NonNullableUnderlyingType;
            this.IsConvertable = ConvertUtils.IsConvertible(this.NonNullableUnderlyingType);
            this.IsEnum = this.NonNullableUnderlyingType.IsEnum();
            this.InternalReadType = ReadType.Read;
        }

        internal static SerializationCallback CreateSerializationCallback(MethodInfo callbackMethodInfo) => 
            delegate (object o, StreamingContext context) {
                object[] parameters = new object[] { context };
                callbackMethodInfo.Invoke(o, parameters);
            };

        internal static SerializationErrorCallback CreateSerializationErrorCallback(MethodInfo callbackMethodInfo) => 
            delegate (object o, StreamingContext context, ErrorContext econtext) {
                object[] parameters = new object[] { context, econtext };
                callbackMethodInfo.Invoke(o, parameters);
            };

        internal void InvokeOnDeserialized(object o, StreamingContext context)
        {
            if (this._onDeserializedCallbacks != null)
            {
                using (List<SerializationCallback>.Enumerator enumerator = this._onDeserializedCallbacks.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current(o, context);
                    }
                }
            }
        }

        internal void InvokeOnDeserializing(object o, StreamingContext context)
        {
            if (this._onDeserializingCallbacks != null)
            {
                using (IEnumerator<SerializationCallback> enumerator = this._onDeserializingCallbacks.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current(o, context);
                    }
                }
            }
        }

        internal void InvokeOnError(object o, StreamingContext context, ErrorContext errorContext)
        {
            if (this._onErrorCallbacks != null)
            {
                using (IEnumerator<SerializationErrorCallback> enumerator = this._onErrorCallbacks.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current(o, context, errorContext);
                    }
                }
            }
        }

        internal void InvokeOnSerialized(object o, StreamingContext context)
        {
            if (this._onSerializedCallbacks != null)
            {
                using (IEnumerator<SerializationCallback> enumerator = this._onSerializedCallbacks.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current(o, context);
                    }
                }
            }
        }

        internal void InvokeOnSerializing(object o, StreamingContext context)
        {
            if (this._onSerializingCallbacks != null)
            {
                using (IEnumerator<SerializationCallback> enumerator = this._onSerializingCallbacks.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current(o, context);
                    }
                }
            }
        }

        public Type UnderlyingType { get; private set; }

        public Type CreatedType
        {
            get => 
                this._createdType;
            set
            {
                this._createdType = value;
                this.IsSealed = this._createdType.IsSealed();
                this.IsInstantiable = !this._createdType.IsInterface() && !this._createdType.IsAbstract();
            }
        }

        public bool? IsReference { get; set; }

        public JsonConverter Converter { get; set; }

        internal JsonConverter InternalConverter { get; set; }

        public IList<SerializationCallback> OnDeserializedCallbacks
        {
            get
            {
                if (this._onDeserializedCallbacks == null)
                {
                    this._onDeserializedCallbacks = new List<SerializationCallback>();
                }
                return this._onDeserializedCallbacks;
            }
        }

        public IList<SerializationCallback> OnDeserializingCallbacks
        {
            get
            {
                if (this._onDeserializingCallbacks == null)
                {
                    this._onDeserializingCallbacks = new List<SerializationCallback>();
                }
                return this._onDeserializingCallbacks;
            }
        }

        public IList<SerializationCallback> OnSerializedCallbacks
        {
            get
            {
                if (this._onSerializedCallbacks == null)
                {
                    this._onSerializedCallbacks = new List<SerializationCallback>();
                }
                return this._onSerializedCallbacks;
            }
        }

        public IList<SerializationCallback> OnSerializingCallbacks
        {
            get
            {
                if (this._onSerializingCallbacks == null)
                {
                    this._onSerializingCallbacks = new List<SerializationCallback>();
                }
                return this._onSerializingCallbacks;
            }
        }

        public IList<SerializationErrorCallback> OnErrorCallbacks
        {
            get
            {
                if (this._onErrorCallbacks == null)
                {
                    this._onErrorCallbacks = new List<SerializationErrorCallback>();
                }
                return this._onErrorCallbacks;
            }
        }

        [Obsolete("This property is obsolete and has been replaced by the OnDeserializedCallbacks collection.")]
        public MethodInfo OnDeserialized
        {
            get
            {
                if (this.OnDeserializedCallbacks.Count <= 0)
                {
                    return null;
                }
                return this.OnDeserializedCallbacks[0].Method();
            }
            set
            {
                this.OnDeserializedCallbacks.Clear();
                this.OnDeserializedCallbacks.Add(CreateSerializationCallback(value));
            }
        }

        [Obsolete("This property is obsolete and has been replaced by the OnDeserializingCallbacks collection.")]
        public MethodInfo OnDeserializing
        {
            get
            {
                if (this.OnDeserializingCallbacks.Count <= 0)
                {
                    return null;
                }
                return this.OnDeserializingCallbacks[0].Method();
            }
            set
            {
                this.OnDeserializingCallbacks.Clear();
                this.OnDeserializingCallbacks.Add(CreateSerializationCallback(value));
            }
        }

        [Obsolete("This property is obsolete and has been replaced by the OnSerializedCallbacks collection.")]
        public MethodInfo OnSerialized
        {
            get
            {
                if (this.OnSerializedCallbacks.Count <= 0)
                {
                    return null;
                }
                return this.OnSerializedCallbacks[0].Method();
            }
            set
            {
                this.OnSerializedCallbacks.Clear();
                this.OnSerializedCallbacks.Add(CreateSerializationCallback(value));
            }
        }

        [Obsolete("This property is obsolete and has been replaced by the OnSerializingCallbacks collection.")]
        public MethodInfo OnSerializing
        {
            get
            {
                if (this.OnSerializingCallbacks.Count <= 0)
                {
                    return null;
                }
                return this.OnSerializingCallbacks[0].Method();
            }
            set
            {
                this.OnSerializingCallbacks.Clear();
                this.OnSerializingCallbacks.Add(CreateSerializationCallback(value));
            }
        }

        [Obsolete("This property is obsolete and has been replaced by the OnErrorCallbacks collection.")]
        public MethodInfo OnError
        {
            get
            {
                if (this.OnErrorCallbacks.Count <= 0)
                {
                    return null;
                }
                return this.OnErrorCallbacks[0].Method();
            }
            set
            {
                this.OnErrorCallbacks.Clear();
                this.OnErrorCallbacks.Add(CreateSerializationErrorCallback(value));
            }
        }

        public Func<object> DefaultCreator { get; set; }

        public bool DefaultCreatorNonPublic { get; set; }
    }
}

