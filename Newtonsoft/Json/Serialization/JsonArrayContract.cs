namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class JsonArrayContract : JsonContainerContract
    {
        private readonly Type _genericCollectionDefinitionType;
        private Type _genericWrapperType;
        private ObjectConstructor<object> _genericWrapperCreator;
        private Func<object> _genericTemporaryCollectionCreator;
        private readonly ConstructorInfo _parameterizedConstructor;
        private ObjectConstructor<object> _parameterizedCreator;
        private ObjectConstructor<object> _overrideCreator;

        public JsonArrayContract(Type underlyingType) : base(underlyingType)
        {
            bool hasParameterizedCreatorInternal;
            base.ContractType = JsonContractType.Array;
            this.IsArray = base.CreatedType.IsArray;
            if (this.IsArray)
            {
                this.CollectionItemType = ReflectionUtils.GetCollectionItemType(base.UnderlyingType);
                base.IsReadOnlyOrFixedSize = true;
                Type[] typeArguments = new Type[] { this.CollectionItemType };
                this._genericCollectionDefinitionType = typeof(List<>).MakeGenericType(typeArguments);
                hasParameterizedCreatorInternal = true;
                this.IsMultidimensionalArray = this.IsArray && (base.UnderlyingType.GetArrayRank() > 1);
            }
            else if (typeof(IList).IsAssignableFrom(underlyingType))
            {
                if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(ICollection<>), out this._genericCollectionDefinitionType))
                {
                    this.CollectionItemType = this._genericCollectionDefinitionType.GetGenericArguments()[0];
                }
                else
                {
                    this.CollectionItemType = ReflectionUtils.GetCollectionItemType(underlyingType);
                }
                if (underlyingType == typeof(IList))
                {
                    base.CreatedType = typeof(List<object>);
                }
                if (this.CollectionItemType != null)
                {
                    this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(underlyingType, this.CollectionItemType);
                }
                base.IsReadOnlyOrFixedSize = ReflectionUtils.InheritsGenericDefinition(underlyingType, typeof(ReadOnlyCollection<>));
                hasParameterizedCreatorInternal = true;
            }
            else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(ICollection<>), out this._genericCollectionDefinitionType))
            {
                this.CollectionItemType = this._genericCollectionDefinitionType.GetGenericArguments()[0];
                if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof(ICollection<>)) || ReflectionUtils.IsGenericDefinition(underlyingType, typeof(IList<>)))
                {
                    Type[] typeArguments = new Type[] { this.CollectionItemType };
                    base.CreatedType = typeof(List<>).MakeGenericType(typeArguments);
                }
                if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof(ISet<>)))
                {
                    Type[] typeArguments = new Type[] { this.CollectionItemType };
                    base.CreatedType = typeof(HashSet<>).MakeGenericType(typeArguments);
                }
                this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(underlyingType, this.CollectionItemType);
                hasParameterizedCreatorInternal = true;
                this.ShouldCreateWrapper = true;
            }
            else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IReadOnlyCollection<>), out Type type))
            {
                this.CollectionItemType = type.GetGenericArguments()[0];
                if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof(IReadOnlyCollection<>)) || ReflectionUtils.IsGenericDefinition(underlyingType, typeof(IReadOnlyList<>)))
                {
                    Type[] typeArray4 = new Type[] { this.CollectionItemType };
                    base.CreatedType = typeof(ReadOnlyCollection<>).MakeGenericType(typeArray4);
                }
                Type[] typeArguments = new Type[] { this.CollectionItemType };
                this._genericCollectionDefinitionType = typeof(List<>).MakeGenericType(typeArguments);
                this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(base.CreatedType, this.CollectionItemType);
                base.IsReadOnlyOrFixedSize = true;
                hasParameterizedCreatorInternal = this.HasParameterizedCreatorInternal;
            }
            else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IEnumerable<>), out type))
            {
                this.CollectionItemType = type.GetGenericArguments()[0];
                if (ReflectionUtils.IsGenericDefinition(base.UnderlyingType, typeof(IEnumerable<>)))
                {
                    Type[] typeArguments = new Type[] { this.CollectionItemType };
                    base.CreatedType = typeof(List<>).MakeGenericType(typeArguments);
                }
                this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(underlyingType, this.CollectionItemType);
                if (!this.HasParameterizedCreatorInternal && (underlyingType.Name == "FSharpList`1"))
                {
                    FSharpUtils.EnsureInitialized(underlyingType.Assembly());
                    this._parameterizedCreator = FSharpUtils.CreateSeq(this.CollectionItemType);
                }
                if (underlyingType.IsGenericType() && (underlyingType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    this._genericCollectionDefinitionType = type;
                    base.IsReadOnlyOrFixedSize = false;
                    this.ShouldCreateWrapper = false;
                    hasParameterizedCreatorInternal = true;
                }
                else
                {
                    Type[] typeArguments = new Type[] { this.CollectionItemType };
                    this._genericCollectionDefinitionType = typeof(List<>).MakeGenericType(typeArguments);
                    base.IsReadOnlyOrFixedSize = true;
                    this.ShouldCreateWrapper = true;
                    hasParameterizedCreatorInternal = this.HasParameterizedCreatorInternal;
                }
            }
            else
            {
                hasParameterizedCreatorInternal = false;
                this.ShouldCreateWrapper = true;
            }
            this.CanDeserialize = hasParameterizedCreatorInternal;
            if (ImmutableCollectionsUtils.TryBuildImmutableForArrayContract(underlyingType, this.CollectionItemType, out Type type2, out ObjectConstructor<object> constructor))
            {
                base.CreatedType = type2;
                this._parameterizedCreator = constructor;
                base.IsReadOnlyOrFixedSize = true;
                this.CanDeserialize = true;
            }
        }

        internal IList CreateTemporaryCollection()
        {
            if (this._genericTemporaryCollectionCreator == null)
            {
                Type type = (this.IsMultidimensionalArray || (this.CollectionItemType == null)) ? typeof(object) : this.CollectionItemType;
                Type[] typeArguments = new Type[] { type };
                Type type2 = typeof(List<>).MakeGenericType(typeArguments);
                this._genericTemporaryCollectionCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type2);
            }
            return (IList) this._genericTemporaryCollectionCreator();
        }

        internal IWrappedCollection CreateWrapper(object list)
        {
            if (this._genericWrapperCreator == null)
            {
                Type type;
                Type[] typeArguments = new Type[] { this.CollectionItemType };
                this._genericWrapperType = typeof(CollectionWrapper<>).MakeGenericType(typeArguments);
                if (ReflectionUtils.InheritsGenericDefinition(this._genericCollectionDefinitionType, typeof(List<>)) || (this._genericCollectionDefinitionType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    Type[] typeArray2 = new Type[] { this.CollectionItemType };
                    type = typeof(ICollection<>).MakeGenericType(typeArray2);
                }
                else
                {
                    type = this._genericCollectionDefinitionType;
                }
                Type[] types = new Type[] { type };
                ConstructorInfo constructor = this._genericWrapperType.GetConstructor(types);
                this._genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor);
            }
            object[] args = new object[] { list };
            return (IWrappedCollection) this._genericWrapperCreator(args);
        }

        public Type CollectionItemType { get; private set; }

        public bool IsMultidimensionalArray { get; private set; }

        internal bool IsArray { get; private set; }

        internal bool ShouldCreateWrapper { get; private set; }

        internal bool CanDeserialize { get; private set; }

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
            set
            {
                this._overrideCreator = value;
                this.CanDeserialize = true;
            }
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

