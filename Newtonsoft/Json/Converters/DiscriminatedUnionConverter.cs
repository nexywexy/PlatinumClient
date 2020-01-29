namespace Newtonsoft.Json.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class DiscriminatedUnionConverter : JsonConverter
    {
        private const string CasePropertyName = "Case";
        private const string FieldsPropertyName = "Fields";
        private static readonly ThreadSafeStore<Type, Union> UnionCache = new ThreadSafeStore<Type, Union>(new Func<Type, Union>(DiscriminatedUnionConverter.CreateUnion));
        private static readonly ThreadSafeStore<Type, Type> UnionTypeLookupCache = new ThreadSafeStore<Type, Type>(new Func<Type, Type>(DiscriminatedUnionConverter.CreateUnionTypeLookup));

        public override bool CanConvert(Type objectType)
        {
            if (typeof(IEnumerable).IsAssignableFrom(objectType))
            {
                return false;
            }
            bool flag = false;
            object[] customAttributes = objectType.GetCustomAttributes(true);
            for (int i = 0; i < customAttributes.Length; i++)
            {
                Type type = customAttributes[i].GetType();
                if (type.FullName == "Microsoft.FSharp.Core.CompilationMappingAttribute")
                {
                    FSharpUtils.EnsureInitialized(type.Assembly());
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                return false;
            }
            object[] args = new object[2];
            args[0] = objectType;
            return (bool) FSharpUtils.IsUnion(null, args);
        }

        private static Union CreateUnion(Type t)
        {
            Union union = new Union();
            object[] args = new object[2];
            args[0] = t;
            union.TagReader = (FSharpFunction) FSharpUtils.PreComputeUnionTagReader(null, args);
            union.Cases = new List<UnionCase>();
            object[] objArray2 = new object[2];
            objArray2[0] = t;
            foreach (object obj2 in (object[]) FSharpUtils.GetUnionCases(null, objArray2))
            {
                UnionCase item = new UnionCase {
                    Tag = (int) FSharpUtils.GetUnionCaseInfoTag(obj2),
                    Name = (string) FSharpUtils.GetUnionCaseInfoName(obj2),
                    Fields = (PropertyInfo[]) FSharpUtils.GetUnionCaseInfoFields(obj2, new object[0])
                };
                object[] objArray3 = new object[2];
                objArray3[0] = obj2;
                item.FieldReader = (FSharpFunction) FSharpUtils.PreComputeUnionReader(null, objArray3);
                object[] objArray4 = new object[2];
                objArray4[0] = obj2;
                item.Constructor = (FSharpFunction) FSharpUtils.PreComputeUnionConstructor(null, objArray4);
                union.Cases.Add(item);
            }
            return union;
        }

        private static Type CreateUnionTypeLookup(Type t)
        {
            object[] args = new object[2];
            args[0] = t;
            object arg = ((object[]) FSharpUtils.GetUnionCases(null, args)).First<object>();
            return (Type) FSharpUtils.GetUnionCaseInfoDeclaringType(arg);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            UnionCase @case = null;
            string caseName = null;
            JArray array = null;
            reader.ReadAndAssert();
            while (reader.TokenType == JsonToken.PropertyName)
            {
                string a = reader.Value.ToString();
                if (string.Equals(a, "Case", StringComparison.OrdinalIgnoreCase))
                {
                    Func<UnionCase, bool> <>9__0;
                    reader.ReadAndAssert();
                    caseName = reader.Value.ToString();
                    if (<>9__0 == null)
                    {
                    }
                    @case = UnionCache.Get(objectType).Cases.SingleOrDefault<UnionCase>(<>9__0 = c => c.Name == caseName);
                    if (@case == null)
                    {
                        throw JsonSerializationException.Create(reader, "No union type found with the name '{0}'.".FormatWith(CultureInfo.InvariantCulture, caseName));
                    }
                }
                else
                {
                    if (!string.Equals(a, "Fields", StringComparison.OrdinalIgnoreCase))
                    {
                        throw JsonSerializationException.Create(reader, "Unexpected property '{0}' found when reading union.".FormatWith(CultureInfo.InvariantCulture, a));
                    }
                    reader.ReadAndAssert();
                    if (reader.TokenType != JsonToken.StartArray)
                    {
                        throw JsonSerializationException.Create(reader, "Union fields must been an array.");
                    }
                    array = (JArray) JToken.ReadFrom(reader);
                }
                reader.ReadAndAssert();
            }
            if (@case == null)
            {
                throw JsonSerializationException.Create(reader, "No '{0}' property with union name found.".FormatWith(CultureInfo.InvariantCulture, "Case"));
            }
            object[] objArray = new object[@case.Fields.Length];
            if ((@case.Fields.Length != 0) && (array == null))
            {
                throw JsonSerializationException.Create(reader, "No '{0}' property with union fields found.".FormatWith(CultureInfo.InvariantCulture, "Fields"));
            }
            if (array != null)
            {
                if (@case.Fields.Length != array.Count)
                {
                    throw JsonSerializationException.Create(reader, "The number of field values does not match the number of properties defined by union '{0}'.".FormatWith(CultureInfo.InvariantCulture, caseName));
                }
                for (int i = 0; i < array.Count; i++)
                {
                    JToken token = array[i];
                    PropertyInfo info = @case.Fields[i];
                    objArray[i] = token.ToObject(info.PropertyType, serializer);
                }
            }
            object[] args = new object[] { objArray };
            return @case.Constructor.Invoke(args);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DefaultContractResolver contractResolver = serializer.ContractResolver as DefaultContractResolver;
            Type key = UnionTypeLookupCache.Get(value.GetType());
            Union union = UnionCache.Get(key);
            object[] args = new object[] { value };
            int tag = (int) union.TagReader.Invoke(args);
            UnionCase @case = union.Cases.Single<UnionCase>(c => c.Tag == tag);
            writer.WriteStartObject();
            writer.WritePropertyName((contractResolver != null) ? contractResolver.GetResolvedPropertyName("Case") : "Case");
            writer.WriteValue(@case.Name);
            if ((@case.Fields != null) && (@case.Fields.Length != 0))
            {
                object[] objArray2 = new object[] { value };
                writer.WritePropertyName((contractResolver != null) ? contractResolver.GetResolvedPropertyName("Fields") : "Fields");
                writer.WriteStartArray();
                foreach (object obj2 in (object[]) @case.FieldReader.Invoke(objArray2))
                {
                    serializer.Serialize(writer, obj2);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }

        internal class Union
        {
            public List<DiscriminatedUnionConverter.UnionCase> Cases;

            public FSharpFunction TagReader { get; set; }
        }

        internal class UnionCase
        {
            public int Tag;
            public string Name;
            public PropertyInfo[] Fields;
            public FSharpFunction FieldReader;
            public FSharpFunction Constructor;
        }
    }
}

