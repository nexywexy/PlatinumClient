namespace Newtonsoft.Json.Serialization
{
    using Newtonsoft.Json.Utilities;
    using System;

    internal static class CachedAttributeGetter<T> where T: Attribute
    {
        private static readonly ThreadSafeStore<object, T> TypeAttributeCache;

        static CachedAttributeGetter()
        {
            CachedAttributeGetter<T>.TypeAttributeCache = new ThreadSafeStore<object, T>(new Func<object, T>(JsonTypeReflector.GetAttribute<T>));
        }

        public static T GetAttribute(object type) => 
            CachedAttributeGetter<T>.TypeAttributeCache.Get(type);
    }
}

