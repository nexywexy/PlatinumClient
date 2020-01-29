namespace Newtonsoft.Json.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class CollectionUtils
    {
        public static bool AddDistinct<T>(this IList<T> list, T value) => 
            list.AddDistinct<T>(value, EqualityComparer<T>.Default);

        public static bool AddDistinct<T>(this IList<T> list, T value, IEqualityComparer<T> comparer)
        {
            if (list.ContainsValue<T>(value, comparer))
            {
                return false;
            }
            list.Add(value);
            return true;
        }

        public static void AddRange<T>(this IList<T> initial, IEnumerable<T> collection)
        {
            if (initial == null)
            {
                throw new ArgumentNullException("initial");
            }
            if (collection != null)
            {
                foreach (T local in collection)
                {
                    initial.Add(local);
                }
            }
        }

        public static bool AddRangeDistinct<T>(this IList<T> list, IEnumerable<T> values, IEqualityComparer<T> comparer)
        {
            bool flag = true;
            foreach (T local in values)
            {
                if (!list.AddDistinct<T>(local, comparer))
                {
                    flag = false;
                }
            }
            return flag;
        }

        public static bool Contains<T>(this List<T> list, T value, IEqualityComparer comparer)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (comparer.Equals(value, list[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsValue<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<TSource>.Default;
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            foreach (TSource local in source)
            {
                if (comparer.Equals(local, value))
                {
                    return true;
                }
            }
            return false;
        }

        private static void CopyFromJaggedToMultidimensionalArray(IList values, Array multidimensionalArray, int[] indices)
        {
            int length = indices.Length;
            if (length == multidimensionalArray.Rank)
            {
                multidimensionalArray.SetValue(JaggedArrayGetValue(values, indices), indices);
            }
            else
            {
                int num2 = multidimensionalArray.GetLength(length);
                if (((IList) JaggedArrayGetValue(values, indices)).Count != num2)
                {
                    throw new Exception("Cannot deserialize non-cubical array as multidimensional array.");
                }
                int[] numArray = new int[length + 1];
                for (int i = 0; i < length; i++)
                {
                    numArray[i] = indices[i];
                }
                for (int j = 0; j < multidimensionalArray.GetLength(length); j++)
                {
                    numArray[length] = j;
                    CopyFromJaggedToMultidimensionalArray(values, multidimensionalArray, numArray);
                }
            }
        }

        private static IList<int> GetDimensions(IList values, int dimensionsCount)
        {
            IList<int> list = new List<int>();
            IList list2 = values;
            while (true)
            {
                list.Add(list2.Count);
                if ((list.Count == dimensionsCount) || (list2.Count == 0))
                {
                    return list;
                }
                object obj2 = list2[0];
                if (!(obj2 is IList))
                {
                    return list;
                }
                list2 = (IList) obj2;
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            int num = 0;
            foreach (T local in collection)
            {
                if (predicate(local))
                {
                    return num;
                }
                num++;
            }
            return -1;
        }

        public static int IndexOfReference<T>(this List<T> list, T item)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (item == list[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool IsDictionaryType(Type type)
        {
            ValidationUtils.ArgumentNotNull(type, "type");
            return (typeof(IDictionary).IsAssignableFrom(type) || (ReflectionUtils.ImplementsGenericDefinition(type, typeof(IDictionary<,>)) || ReflectionUtils.ImplementsGenericDefinition(type, typeof(IReadOnlyDictionary<,>))));
        }

        public static bool IsNullOrEmpty<T>(ICollection<T> collection)
        {
            if (collection != null)
            {
                return (collection.Count == 0);
            }
            return true;
        }

        private static object JaggedArrayGetValue(IList values, int[] indices)
        {
            IList list = values;
            for (int i = 0; i < indices.Length; i++)
            {
                int num2 = indices[i];
                if (i == (indices.Length - 1))
                {
                    return list[num2];
                }
                list = (IList) list[num2];
            }
            return list;
        }

        public static ConstructorInfo ResolveEnumerableCollectionConstructor(Type collectionType, Type collectionItemType)
        {
            Type[] typeArguments = new Type[] { collectionItemType };
            Type constructorArgumentType = typeof(IList<>).MakeGenericType(typeArguments);
            return ResolveEnumerableCollectionConstructor(collectionType, collectionItemType, constructorArgumentType);
        }

        public static ConstructorInfo ResolveEnumerableCollectionConstructor(Type collectionType, Type collectionItemType, Type constructorArgumentType)
        {
            Type[] typeArguments = new Type[] { collectionItemType };
            Type type = typeof(IEnumerable<>).MakeGenericType(typeArguments);
            ConstructorInfo info = null;
            foreach (ConstructorInfo info2 in collectionType.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            {
                IList<ParameterInfo> parameters = info2.GetParameters();
                if (parameters.Count == 1)
                {
                    Type parameterType = parameters[0].ParameterType;
                    if (type == parameterType)
                    {
                        return info2;
                    }
                    if ((info == null) && parameterType.IsAssignableFrom(constructorArgumentType))
                    {
                        info = info2;
                    }
                }
            }
            return info;
        }

        public static Array ToMultidimensionalArray(IList values, Type type, int rank)
        {
            IList<int> dimensions = GetDimensions(values, rank);
            while (dimensions.Count < rank)
            {
                dimensions.Add(0);
            }
            Array multidimensionalArray = Array.CreateInstance(type, dimensions.ToArray<int>());
            CopyFromJaggedToMultidimensionalArray(values, multidimensionalArray, new int[0]);
            return multidimensionalArray;
        }
    }
}

