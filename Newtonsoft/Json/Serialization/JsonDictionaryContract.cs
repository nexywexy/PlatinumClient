namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class JsonDictionaryContract : JsonContainerContract
    {
        private readonly Type _genericCollectionDefinitionType;
        private Type _genericWrapperType;
        private ObjectConstructor<object> _genericWrapperCreator;
        private Func<object> _genericTemporaryDictionaryCreator;
        private readonly ConstructorInfo _parameterizedConstructor;
        private ObjectConstructor<object> _overrideCreator;
        private ObjectConstructor<object> _parameterizedCreator;

        public JsonDictionaryContract(Type underlyingType) : base(underlyingType)
        {
            Type type;
            Type type2;
            base.ContractType = JsonContractType.Dictionary;
            if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IDictionary<,>), out this._genericCollectionDefinitionType))
            {
                type = this._genericCollectionDefinitionType.GetGenericArguments()[0];
                type2 = this._genericCollectionDefinitionType.GetGenericArguments()[1];
                if (ReflectionUtils.IsGenericDefinition(base.UnderlyingType, typeof(IDictionary<,>)))
                {
                    Type[] typeArguments = new Type[] { type, type2 };
                    base.CreatedType = typeof(Dictionary<,>).MakeGenericType(typeArguments);
                }
                base.IsReadOnlyOrFixedSize = ReflectionUtils.InheritsGenericDefinition(underlyingType, typeof(ReadOnlyDictionary<,>));
            }
            else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IReadOnlyDictionary<,>), out this._genericCollectionDefinitionType))
            {
                type = this._genericCollectionDefinitionType.GetGenericArguments()[0];
                type2 = this._genericCollectionDefinitionType.GetGenericArguments()[1];
                if (ReflectionUtils.IsGenericDefinition(base.UnderlyingType, typeof(IReadOnlyDictionary<,>)))
                {
                    Type[] typeArguments = new Type[] { type, type2 };
                    base.CreatedType = typeof(ReadOnlyDictionary<,>).MakeGenericType(typeArguments);
                }
                base.IsReadOnlyOrFixedSize = true;
            }
            else
            {
                ReflectionUtils.GetDictionaryKeyValueTypes(base.UnderlyingType, out type, out type2);
                if (base.UnderlyingType == typeof(IDictionary))
                {
                    base.CreatedType = typeof(Dictionary<object, object>);
                }
            }
            if ((type != null) && (type2 != null))
            {
                Type[] typeArguments = new Type[] { type, type2 };
                Type[] typeArray4 = new Type[] { type, type2 };
                this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(base.CreatedType, typeof(KeyValuePair<,>).MakeGenericType(typeArguments), typeof(IDictionary<,>).MakeGenericType(typeArray4));
                if (!this.HasParameterizedCreatorInternal && (underlyingType.Name == "FSharpMap`2"))
                {
                    FSharpUtils.EnsureInitialized(underlyingType.Assembly());
                    this._parameterizedCreator = FSharpUtils.CreateMap(type, type2);
                }
            }
            this.ShouldCreateWrapper = !typeof(IDictionary).IsAssignableFrom(base.CreatedType);
            this.DictionaryKeyType = type;
            this.DictionaryValueType = type2;
            if (ImmutableCollectionsUtils.TryBuildImmutableForDictionaryContract(underlyingType, this.DictionaryKeyType, this.DictionaryValueType, out Type type3, out ObjectConstructor<object> constructor))
            {
                base.CreatedType = type3;
                this._parameterizedCreator = constructor;
                base.IsReadOnlyOrFixedSize = true;
            }
        }

        internal IDictionary CreateTemporaryDictionary()
        {
            Type type;
            Type expressionStack_34_0;
            int expressionStack_34_1;
            Type[] expressionStack_34_2;
            Type[] expressionStack_34_3;
            Type expressionStack_34_4;
            Type expressionStack_29_0;
            int expressionStack_29_1;
            Type[] expressionStack_29_2;
            Type[] expressionStack_29_3;
            Type expressionStack_29_4;
            Type expressionStack_4E_0;
            int expressionStack_4E_1;
            Type[] expressionStack_4E_2;
            Type[] expressionStack_4E_3;
            Type expressionStack_4E_4;
            Type expressionStack_43_0;
            int expressionStack_43_1;
            Type[] expressionStack_43_2;
            Type[] expressionStack_43_3;
            Type expressionStack_43_4;
            if (this._genericTemporaryDictionaryCreator != null)
            {
                goto Label_0066;
            }
            Type[] typeArray2 = new Type[2];
            Type dictionaryKeyType = this.DictionaryKeyType;
            if (dictionaryKeyType != null)
            {
                expressionStack_34_4 = typeof(Dictionary<,>);
                expressionStack_34_3 = typeArray2;
                expressionStack_34_2 = typeArray2;
                expressionStack_34_1 = 0;
                expressionStack_34_0 = dictionaryKeyType;
                goto Label_0034;
            }
            else
            {
                expressionStack_29_4 = typeof(Dictionary<,>);
                expressionStack_29_3 = typeArray2;
                expressionStack_29_2 = typeArray2;
                expressionStack_29_1 = 0;
                expressionStack_29_0 = dictionaryKeyType;
            }
            expressionStack_34_4 = expressionStack_29_4;
            expressionStack_34_3 = expressionStack_29_3;
            expressionStack_34_2 = expressionStack_29_2;
            expressionStack_34_1 = expressionStack_29_1;
            expressionStack_34_0 = typeof(object);
        Label_0034:
            expressionStack_34_2[expressionStack_34_1] = expressionStack_34_0;
            Type dictionaryValueType = this.DictionaryValueType;
            if (dictionaryValueType != null)
            {
                expressionStack_4E_4 = expressionStack_34_4;
                expressionStack_4E_3 = expressionStack_34_3;
                expressionStack_4E_2 = expressionStack_34_3;
                expressionStack_4E_1 = 1;
                expressionStack_4E_0 = dictionaryValueType;
                goto Label_004E;
            }
            else
            {
                expressionStack_43_4 = expressionStack_34_4;
                expressionStack_43_3 = expressionStack_34_3;
                expressionStack_43_2 = expressionStack_34_3;
                expressionStack_43_1 = 1;
                expressionStack_43_0 = dictionaryValueType;
            }
            expressionStack_4E_4 = expressionStack_43_4;
            expressionStack_4E_3 = expressionStack_43_3;
            expressionStack_4E_2 = expressionStack_43_2;
            expressionStack_4E_1 = expressionStack_43_1;
            expressionStack_4E_0 = typeof(object);
        Label_004E:
            expressionStack_4E_2[expressionStack_4E_1] = expressionStack_4E_0;
            type = expressionStack_4E_4.MakeGenericType(expressionStack_4E_3);
            this._genericTemporaryDictionaryCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type);
        Label_0066:
            return (IDictionary) this._genericTemporaryDictionaryCreator();
        }

        internal IWrappedDictionary CreateWrapper(object dictionary)
        {
            if (this._genericWrapperCreator == null)
            {
                Type[] typeArguments = new Type[] { this.DictionaryKeyType, this.DictionaryValueType };
                this._genericWrapperType = typeof(DictionaryWrapper<,>).MakeGenericType(typeArguments);
                Type[] types = new Type[] { this._genericCollectionDefinitionType };
                ConstructorInfo constructor = this._genericWrapperType.GetConstructor(types);
                this._genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor);
            }
            object[] args = new object[] { dictionary };
            return (IWrappedDictionary) this._genericWrapperCreator(args);
        }

        [Obsolete("PropertyNameResolver is obsolete. Use DictionaryKeyResolver instead.")]
        public Func<string, string> PropertyNameResolver
        {
            get => 
                this.DictionaryKeyResolver;
            set => 
                (this.DictionaryKeyResolver = value);
        }

        public Func<string, string> DictionaryKeyResolver { get; set; }

        public Type DictionaryKeyType { get; private set; }

        public Type DictionaryValueType { get; private set; }

        internal JsonContract KeyContract { get; set; }

        internal bool ShouldCreateWrapper { get; private set; }

        internal ObjectConstructor<object> ParameterizedCreator
        {
            get
            {
                if (this._parameterizedCreator == null)
                {
                    this._parameterizedCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(this._parameterizedConstructor);
                }
                return this._parameterizedCreator;
            }
        }

        public ObjectConstructor<object> OverrideCreator
        {
            get => 
                this._overrideCreator;
            set => 
                (this._overrideCreator = value);
        }

        public bool HasParameterizedCreator { get; set; }

        internal bool HasParameterizedCreatorInternal
        {
            get
            {
                if (!this.HasParameterizedCreator && (this._parameterizedCreator == null))
                {
                    return (this._parameterizedConstructor != null);
                }
                return true;
            }
        }
    }
}

