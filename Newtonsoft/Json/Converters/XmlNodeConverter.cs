namespace Newtonsoft.Json.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.Linq;

    public class XmlNodeConverter : JsonConverter
    {
        private const string TextName = "#text";
        private const string CommentName = "#comment";
        private const string CDataName = "#cdata-section";
        private const string WhitespaceName = "#whitespace";
        private const string SignificantWhitespaceName = "#significant-whitespace";
        private const string DeclarationName = "?xml";
        private const string JsonNamespaceUri = "http://james.newtonking.com/projects/json";

        private static void AddAttribute(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string attributeName, XmlNamespaceManager manager, string attributePrefix)
        {
            string qualifiedName = XmlConvert.EncodeName(attributeName);
            string str2 = reader.Value.ToString();
            IXmlNode attribute = !string.IsNullOrEmpty(attributePrefix) ? document.CreateAttribute(qualifiedName, manager.LookupNamespace(attributePrefix), str2) : document.CreateAttribute(qualifiedName, str2);
            ((IXmlElement) currentNode).SetAttributeNode(attribute);
        }

        private void AddJsonArrayAttribute(IXmlElement element, IXmlDocument document)
        {
            element.SetAttributeNode(document.CreateAttribute("json:Array", "http://james.newtonking.com/projects/json", "true"));
            if ((element is XElementWrapper) && (element.GetPrefixOfNamespace("http://james.newtonking.com/projects/json") == null))
            {
                element.SetAttributeNode(document.CreateAttribute("xmlns:json", "http://www.w3.org/2000/xmlns/", "http://james.newtonking.com/projects/json"));
            }
        }

        private static bool AllSameName(IXmlNode node)
        {
            using (List<IXmlNode>.Enumerator enumerator = node.ChildNodes.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.LocalName != node.LocalName)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool CanConvert(Type valueType) => 
            (typeof(XObject).IsAssignableFrom(valueType) || typeof(System.Xml.XmlNode).IsAssignableFrom(valueType));

        private string ConvertTokenToXmlValue(JsonReader reader)
        {
            switch (reader.TokenType)
            {
                case (reader.TokenType == JsonToken.String):
                    return reader.Value?.ToString();
                    break;
            }
            if (reader.TokenType == JsonToken.Integer)
            {
                if (reader.Value is BigInteger)
                {
                    BigInteger integer = (BigInteger) reader.Value;
                    return integer.ToString(CultureInfo.InvariantCulture);
                }
                return XmlConvert.ToString(Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture));
            }
            if (reader.TokenType == JsonToken.Float)
            {
                if (reader.Value is decimal)
                {
                    return XmlConvert.ToString((decimal) reader.Value);
                }
                if (reader.Value is float)
                {
                    return XmlConvert.ToString((float) reader.Value);
                }
                return XmlConvert.ToString(Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture));
            }
            if (reader.TokenType == JsonToken.Boolean)
            {
                return XmlConvert.ToString(Convert.ToBoolean(reader.Value, CultureInfo.InvariantCulture));
            }
            if (reader.TokenType == JsonToken.Date)
            {
                if (reader.Value is DateTimeOffset)
                {
                    return XmlConvert.ToString((DateTimeOffset) reader.Value);
                }
                DateTime time = Convert.ToDateTime(reader.Value, CultureInfo.InvariantCulture);
                return XmlConvert.ToString(time, DateTimeUtils.ToSerializationMode(time.Kind));
            }
            if (reader.TokenType != JsonToken.Null)
            {
                throw JsonSerializationException.Create(reader, "Cannot get an XML string value from token type '{0}'.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
            return null;
        }

        private void CreateDocumentType(JsonReader reader, IXmlDocument document, IXmlNode currentNode)
        {
            string name = null;
            string publicId = null;
            string systemId = null;
            string internalSubset = null;
            while (reader.Read() && (reader.TokenType != JsonToken.EndObject))
            {
                string str5 = reader.Value.ToString();
                if (str5 != "@name")
                {
                    if (str5 != "@public")
                    {
                        if (str5 == "@system")
                        {
                            goto Label_0093;
                        }
                        if (str5 != "@internalSubset")
                        {
                            throw JsonSerializationException.Create(reader, "Unexpected property name encountered while deserializing XmlDeclaration: " + reader.Value);
                        }
                        goto Label_00AB;
                    }
                }
                else
                {
                    reader.Read();
                    name = reader.Value.ToString();
                    continue;
                }
                reader.Read();
                publicId = reader.Value.ToString();
                continue;
            Label_0093:
                reader.Read();
                systemId = reader.Value.ToString();
                continue;
            Label_00AB:
                reader.Read();
                internalSubset = reader.Value.ToString();
            }
            IXmlNode newChild = document.CreateXmlDocumentType(name, publicId, systemId, internalSubset);
            currentNode.AppendChild(newChild);
        }

        private IXmlElement CreateElement(string elementName, IXmlDocument document, string elementPrefix, XmlNamespaceManager manager)
        {
            string str = XmlConvert.EncodeName(elementName);
            string str2 = string.IsNullOrEmpty(elementPrefix) ? manager.DefaultNamespace : manager.LookupNamespace(elementPrefix);
            if (string.IsNullOrEmpty(str2))
            {
                return document.CreateElement(str);
            }
            return document.CreateElement(str, str2);
        }

        private void CreateElement(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string elementName, XmlNamespaceManager manager, string elementPrefix, Dictionary<string, string> attributeNameValues)
        {
            IXmlElement newChild = this.CreateElement(elementName, document, elementPrefix, manager);
            currentNode.AppendChild(newChild);
            foreach (KeyValuePair<string, string> pair in attributeNameValues)
            {
                string qualifiedName = XmlConvert.EncodeName(pair.Key);
                string prefix = MiscellaneousUtils.GetPrefix(pair.Key);
                string text1 = manager.LookupNamespace(prefix);
                if (text1 == null)
                {
                }
                IXmlNode attribute = !string.IsNullOrEmpty(prefix) ? document.CreateAttribute(qualifiedName, string.Empty, pair.Value) : document.CreateAttribute(qualifiedName, pair.Value);
                newChild.SetAttributeNode(attribute);
            }
            if (((reader.TokenType == JsonToken.String) || (reader.TokenType == JsonToken.Integer)) || (((reader.TokenType == JsonToken.Float) || (reader.TokenType == JsonToken.Boolean)) || (reader.TokenType == JsonToken.Date)))
            {
                string text = this.ConvertTokenToXmlValue(reader);
                if (text != null)
                {
                    newChild.AppendChild(document.CreateTextNode(text));
                }
            }
            else if (reader.TokenType != JsonToken.Null)
            {
                if (reader.TokenType != JsonToken.EndObject)
                {
                    manager.PushScope();
                    this.DeserializeNode(reader, document, manager, newChild);
                    manager.PopScope();
                }
                manager.RemoveNamespace(string.Empty, manager.DefaultNamespace);
            }
        }

        private void CreateInstruction(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string propertyName)
        {
            if (propertyName != "?xml")
            {
                IXmlNode newChild = document.CreateProcessingInstruction(propertyName.Substring(1), reader.Value.ToString());
                currentNode.AppendChild(newChild);
            }
            else
            {
                string version = null;
                string encoding = null;
                string standalone = null;
                while (reader.Read() && (reader.TokenType != JsonToken.EndObject))
                {
                    string str4 = reader.Value.ToString();
                    if (str4 != "@version")
                    {
                        if (str4 != "@encoding")
                        {
                            if (str4 != "@standalone")
                            {
                                throw JsonSerializationException.Create(reader, "Unexpected property name encountered while deserializing XmlDeclaration: " + reader.Value);
                            }
                            goto Label_008D;
                        }
                    }
                    else
                    {
                        reader.Read();
                        version = reader.Value.ToString();
                        continue;
                    }
                    reader.Read();
                    encoding = reader.Value.ToString();
                    continue;
                Label_008D:
                    reader.Read();
                    standalone = reader.Value.ToString();
                }
                IXmlNode newChild = document.CreateXmlDeclaration(version, encoding, standalone);
                currentNode.AppendChild(newChild);
            }
        }

        private void DeserializeNode(JsonReader reader, IXmlDocument document, XmlNamespaceManager manager, IXmlNode currentNode)
        {
            do
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartConstructor:
                    {
                        string propertyName = reader.Value.ToString();
                        while (reader.Read() && (reader.TokenType != JsonToken.EndConstructor))
                        {
                            this.DeserializeValue(reader, document, manager, propertyName, currentNode);
                        }
                        break;
                    }
                    case JsonToken.PropertyName:
                    {
                        if ((currentNode.NodeType == XmlNodeType.Document) && (document.DocumentElement != null))
                        {
                            throw JsonSerializationException.Create(reader, "JSON root object has multiple properties. The root object must have a single property in order to create a valid XML document. Consider specifing a DeserializeRootElementName.");
                        }
                        string propertyName = reader.Value.ToString();
                        reader.Read();
                        if (reader.TokenType == JsonToken.StartArray)
                        {
                            int num = 0;
                            while (reader.Read() && (reader.TokenType != JsonToken.EndArray))
                            {
                                this.DeserializeValue(reader, document, manager, propertyName, currentNode);
                                num++;
                            }
                            if ((num == 1) && this.WriteArrayAttribute)
                            {
                                foreach (IXmlElement element in currentNode.ChildNodes)
                                {
                                    if ((element != null) && (element.LocalName == propertyName))
                                    {
                                        this.AddJsonArrayAttribute(element, document);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.DeserializeValue(reader, document, manager, propertyName, currentNode);
                        }
                        break;
                    }
                    case JsonToken.Comment:
                        currentNode.AppendChild(document.CreateComment((string) reader.Value));
                        break;

                    case JsonToken.EndObject:
                    case JsonToken.EndArray:
                        return;

                    default:
                        throw JsonSerializationException.Create(reader, "Unexpected JsonToken when deserializing node: " + reader.TokenType);
                }
            }
            while ((reader.TokenType == JsonToken.PropertyName) || reader.Read());
        }

        private void DeserializeValue(JsonReader reader, IXmlDocument document, XmlNamespaceManager manager, string propertyName, IXmlNode currentNode)
        {
            if (propertyName != "#text")
            {
                if (propertyName == "#cdata-section")
                {
                    currentNode.AppendChild(document.CreateCDataSection(reader.Value.ToString()));
                }
                else if (propertyName == "#whitespace")
                {
                    currentNode.AppendChild(document.CreateWhitespace(reader.Value.ToString()));
                }
                else if (propertyName == "#significant-whitespace")
                {
                    currentNode.AppendChild(document.CreateSignificantWhitespace(reader.Value.ToString()));
                }
                else if (!string.IsNullOrEmpty(propertyName) && (propertyName[0] == '?'))
                {
                    this.CreateInstruction(reader, document, currentNode, propertyName);
                }
                else if (string.Equals(propertyName, "!DOCTYPE", StringComparison.OrdinalIgnoreCase))
                {
                    this.CreateDocumentType(reader, document, currentNode);
                }
                else if (reader.TokenType == JsonToken.StartArray)
                {
                    this.ReadArrayElements(reader, document, propertyName, currentNode, manager);
                }
                else
                {
                    this.ReadElement(reader, document, currentNode, propertyName, manager);
                }
            }
            else
            {
                currentNode.AppendChild(document.CreateTextNode(reader.Value.ToString()));
            }
        }

        private string GetPropertyName(IXmlNode node, XmlNamespaceManager manager)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                    if (node.NamespaceUri != "http://james.newtonking.com/projects/json")
                    {
                        return this.ResolveFullName(node, manager);
                    }
                    return ("$" + node.LocalName);

                case XmlNodeType.Attribute:
                    if (node.NamespaceUri != "http://james.newtonking.com/projects/json")
                    {
                        return ("@" + this.ResolveFullName(node, manager));
                    }
                    return ("$" + node.LocalName);

                case XmlNodeType.Text:
                    return "#text";

                case XmlNodeType.CDATA:
                    return "#cdata-section";

                case XmlNodeType.ProcessingInstruction:
                    return ("?" + this.ResolveFullName(node, manager));

                case XmlNodeType.Comment:
                    return "#comment";

                case XmlNodeType.DocumentType:
                    return ("!" + this.ResolveFullName(node, manager));

                case XmlNodeType.Whitespace:
                    return "#whitespace";

                case XmlNodeType.SignificantWhitespace:
                    return "#significant-whitespace";

                case XmlNodeType.XmlDeclaration:
                    return "?xml";
            }
            throw new JsonSerializationException("Unexpected XmlNodeType when getting node name: " + node.NodeType);
        }

        private bool IsArray(IXmlNode node)
        {
            if (node.Attributes != null)
            {
                foreach (IXmlNode node2 in node.Attributes)
                {
                    if ((node2.LocalName == "Array") && (node2.NamespaceUri == "http://james.newtonking.com/projects/json"))
                    {
                        return XmlConvert.ToBoolean(node2.Value);
                    }
                }
            }
            return false;
        }

        private bool IsNamespaceAttribute(string attributeName, out string prefix)
        {
            if (attributeName.StartsWith("xmlns", StringComparison.Ordinal))
            {
                if (attributeName.Length == 5)
                {
                    prefix = string.Empty;
                    return true;
                }
                if (attributeName[5] == ':')
                {
                    prefix = attributeName.Substring(6, attributeName.Length - 6);
                    return true;
                }
            }
            prefix = null;
            return false;
        }

        private void PushParentNamespaces(IXmlNode node, XmlNamespaceManager manager)
        {
            List<IXmlNode> list = null;
            IXmlNode item = node;
            while ((item = item.ParentNode) != null)
            {
                if (item.NodeType == XmlNodeType.Element)
                {
                    if (list == null)
                    {
                        list = new List<IXmlNode>();
                    }
                    list.Add(item);
                }
            }
            if (list != null)
            {
                list.Reverse();
                using (List<IXmlNode>.Enumerator enumerator = list.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        manager.PushScope();
                        foreach (IXmlNode node3 in enumerator.Current.Attributes)
                        {
                            if ((node3.NamespaceUri == "http://www.w3.org/2000/xmlns/") && (node3.LocalName != "xmlns"))
                            {
                                manager.AddNamespace(node3.LocalName, node3.Value);
                            }
                        }
                    }
                }
            }
        }

        private void ReadArrayElements(JsonReader reader, IXmlDocument document, string propertyName, IXmlNode currentNode, XmlNamespaceManager manager)
        {
            string prefix = MiscellaneousUtils.GetPrefix(propertyName);
            IXmlElement newChild = this.CreateElement(propertyName, document, prefix, manager);
            currentNode.AppendChild(newChild);
            int num = 0;
            while (reader.Read() && (reader.TokenType != JsonToken.EndArray))
            {
                this.DeserializeValue(reader, document, manager, propertyName, newChild);
                num++;
            }
            if (this.WriteArrayAttribute)
            {
                this.AddJsonArrayAttribute(newChild, document);
            }
            if ((num == 1) && this.WriteArrayAttribute)
            {
                foreach (IXmlElement element2 in newChild.ChildNodes)
                {
                    if ((element2 != null) && (element2.LocalName == propertyName))
                    {
                        this.AddJsonArrayAttribute(element2, document);
                        break;
                    }
                }
            }
        }

        private Dictionary<string, string> ReadAttributeElements(JsonReader reader, XmlNamespaceManager manager)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            bool flag = false;
            bool flag2 = false;
            if ((((reader.TokenType != JsonToken.String) && (reader.TokenType != JsonToken.Null)) && ((reader.TokenType != JsonToken.Boolean) && (reader.TokenType != JsonToken.Integer))) && (((reader.TokenType != JsonToken.Float) && (reader.TokenType != JsonToken.Date)) && (reader.TokenType != JsonToken.StartConstructor)))
            {
                while ((!flag && !flag2) && reader.Read())
                {
                    string str2;
                    JsonToken tokenType = reader.TokenType;
                    if (tokenType != JsonToken.PropertyName)
                    {
                        if (tokenType != JsonToken.Comment)
                        {
                            if (tokenType != JsonToken.EndObject)
                            {
                                throw JsonSerializationException.Create(reader, "Unexpected JsonToken: " + reader.TokenType);
                            }
                            goto Label_0279;
                        }
                        goto Label_0280;
                    }
                    string str = reader.Value.ToString();
                    if (string.IsNullOrEmpty(str))
                    {
                        goto Label_0272;
                    }
                    char ch = str[0];
                    if (ch != '$')
                    {
                        if (ch != '@')
                        {
                            goto Label_026B;
                        }
                        str = str.Substring(1);
                        reader.Read();
                        str2 = this.ConvertTokenToXmlValue(reader);
                        dictionary.Add(str, str2);
                        if (this.IsNamespaceAttribute(str, out string str3))
                        {
                            manager.AddNamespace(str3, str2);
                        }
                    }
                    else if (((str == "$values") || (str == "$id")) || (((str == "$ref") || (str == "$type")) || (str == "$value")))
                    {
                        string prefix = manager.LookupPrefix("http://james.newtonking.com/projects/json");
                        if (prefix == null)
                        {
                            int? nullable = null;
                            while (manager.LookupNamespace("json" + nullable) != null)
                            {
                                nullable = new int?(nullable.GetValueOrDefault() + 1);
                            }
                            prefix = "json" + nullable;
                            dictionary.Add("xmlns:" + prefix, "http://james.newtonking.com/projects/json");
                            manager.AddNamespace(prefix, "http://james.newtonking.com/projects/json");
                        }
                        if (str == "$values")
                        {
                            flag = true;
                        }
                        else
                        {
                            str = str.Substring(1);
                            reader.Read();
                            if (!JsonTokenUtils.IsPrimitiveToken(reader.TokenType))
                            {
                                throw JsonSerializationException.Create(reader, "Unexpected JsonToken: " + reader.TokenType);
                            }
                            str2 = reader.Value?.ToString();
                            dictionary.Add(prefix + ":" + str, str2);
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                    continue;
                Label_026B:
                    flag = true;
                    continue;
                Label_0272:
                    flag = true;
                    continue;
                Label_0279:
                    flag2 = true;
                    continue;
                Label_0280:
                    flag2 = true;
                }
            }
            return dictionary;
        }

        private void ReadElement(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string propertyName, XmlNamespaceManager manager)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw JsonSerializationException.Create(reader, "XmlNodeConverter cannot convert JSON with an empty property name to XML.");
            }
            Dictionary<string, string> attributeNameValues = this.ReadAttributeElements(reader, manager);
            string prefix = MiscellaneousUtils.GetPrefix(propertyName);
            if (propertyName.StartsWith('@'))
            {
                string qualifiedName = propertyName.Substring(1);
                string attributePrefix = MiscellaneousUtils.GetPrefix(qualifiedName);
                AddAttribute(reader, document, currentNode, qualifiedName, manager, attributePrefix);
            }
            else
            {
                if (propertyName.StartsWith('$'))
                {
                    if (propertyName == "$values")
                    {
                        propertyName = propertyName.Substring(1);
                        prefix = manager.LookupPrefix("http://james.newtonking.com/projects/json");
                        this.CreateElement(reader, document, currentNode, propertyName, manager, prefix, attributeNameValues);
                        return;
                    }
                    if (((propertyName == "$id") || (propertyName == "$ref")) || ((propertyName == "$type") || (propertyName == "$value")))
                    {
                        string attributeName = propertyName.Substring(1);
                        string attributePrefix = manager.LookupPrefix("http://james.newtonking.com/projects/json");
                        AddAttribute(reader, document, currentNode, attributeName, manager, attributePrefix);
                        return;
                    }
                }
                this.CreateElement(reader, document, currentNode, propertyName, manager, prefix, attributeNameValues);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            XmlNamespaceManager manager = new XmlNamespaceManager(new NameTable());
            IXmlDocument document = null;
            IXmlNode currentNode = null;
            if (typeof(XObject).IsAssignableFrom(objectType))
            {
                if ((objectType != typeof(XDocument)) && (objectType != typeof(XElement)))
                {
                    throw new JsonSerializationException("XmlNodeConverter only supports deserializing XDocument or XElement.");
                }
                document = new XDocumentWrapper(new XDocument());
                currentNode = document;
            }
            if (typeof(System.Xml.XmlNode).IsAssignableFrom(objectType))
            {
                if (objectType != typeof(XmlDocument))
                {
                    throw new JsonSerializationException("XmlNodeConverter only supports deserializing XmlDocuments");
                }
                XmlDocument document1 = new XmlDocument {
                    XmlResolver = null
                };
                document = new XmlDocumentWrapper(document1);
                currentNode = document;
            }
            if ((document == null) || (currentNode == null))
            {
                throw new JsonSerializationException("Unexpected type when converting XML: " + objectType);
            }
            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonSerializationException("XmlNodeConverter can only convert JSON that begins with an object.");
            }
            if (!string.IsNullOrEmpty(this.DeserializeRootElementName))
            {
                this.ReadElement(reader, document, currentNode, this.DeserializeRootElementName, manager);
            }
            else
            {
                reader.Read();
                this.DeserializeNode(reader, document, manager, currentNode);
            }
            if (objectType == typeof(XElement))
            {
                XElement wrappedNode = (XElement) document.DocumentElement.WrappedNode;
                wrappedNode.Remove();
                return wrappedNode;
            }
            return document.WrappedNode;
        }

        private string ResolveFullName(IXmlNode node, XmlNamespaceManager manager)
        {
            string str = ((node.NamespaceUri == null) || ((node.LocalName == "xmlns") && (node.NamespaceUri == "http://www.w3.org/2000/xmlns/"))) ? null : manager.LookupPrefix(node.NamespaceUri);
            if (!string.IsNullOrEmpty(str))
            {
                return (str + ":" + XmlConvert.DecodeName(node.LocalName));
            }
            return XmlConvert.DecodeName(node.LocalName);
        }

        private void SerializeGroupedNodes(JsonWriter writer, IXmlNode node, XmlNamespaceManager manager, bool writePropertyName)
        {
            Dictionary<string, List<IXmlNode>> dictionary = new Dictionary<string, List<IXmlNode>>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                IXmlNode node2 = node.ChildNodes[i];
                string propertyName = this.GetPropertyName(node2, manager);
                if (!dictionary.TryGetValue(propertyName, out List<IXmlNode> list))
                {
                    list = new List<IXmlNode>();
                    dictionary.Add(propertyName, list);
                }
                list.Add(node2);
            }
            foreach (KeyValuePair<string, List<IXmlNode>> pair in dictionary)
            {
                bool flag;
                List<IXmlNode> list2 = pair.Value;
                if (list2.Count == 1)
                {
                    flag = this.IsArray(list2[0]);
                }
                else
                {
                    flag = true;
                }
                if (!flag)
                {
                    this.SerializeNode(writer, list2[0], manager, writePropertyName);
                }
                else
                {
                    string key = pair.Key;
                    if (writePropertyName)
                    {
                        writer.WritePropertyName(key);
                    }
                    writer.WriteStartArray();
                    for (int j = 0; j < list2.Count; j++)
                    {
                        this.SerializeNode(writer, list2[j], manager, false);
                    }
                    writer.WriteEndArray();
                }
            }
        }

        private void SerializeNode(JsonWriter writer, IXmlNode node, XmlNamespaceManager manager, bool writePropertyName)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                    if ((!this.IsArray(node) || !AllSameName(node)) || (node.ChildNodes.Count <= 0))
                    {
                        manager.PushScope();
                        foreach (IXmlNode node2 in node.Attributes)
                        {
                            if (node2.NamespaceUri == "http://www.w3.org/2000/xmlns/")
                            {
                                string prefix = (node2.LocalName != "xmlns") ? XmlConvert.DecodeName(node2.LocalName) : string.Empty;
                                string uri = node2.Value;
                                manager.AddNamespace(prefix, uri);
                            }
                        }
                        if (writePropertyName)
                        {
                            writer.WritePropertyName(this.GetPropertyName(node, manager));
                        }
                        if ((!this.ValueAttributes(node.Attributes) && (node.ChildNodes.Count == 1)) && (node.ChildNodes[0].NodeType == XmlNodeType.Text))
                        {
                            writer.WriteValue(node.ChildNodes[0].Value);
                        }
                        else if ((node.ChildNodes.Count == 0) && CollectionUtils.IsNullOrEmpty<IXmlNode>(node.Attributes))
                        {
                            if (((IXmlElement) node).IsEmpty)
                            {
                                writer.WriteNull();
                            }
                            else
                            {
                                writer.WriteValue(string.Empty);
                            }
                        }
                        else
                        {
                            writer.WriteStartObject();
                            for (int i = 0; i < node.Attributes.Count; i++)
                            {
                                this.SerializeNode(writer, node.Attributes[i], manager, true);
                            }
                            this.SerializeGroupedNodes(writer, node, manager, true);
                            writer.WriteEndObject();
                        }
                        manager.PopScope();
                        return;
                    }
                    this.SerializeGroupedNodes(writer, node, manager, false);
                    return;

                case XmlNodeType.Attribute:
                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                case XmlNodeType.ProcessingInstruction:
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    if ((node.NamespaceUri != "http://www.w3.org/2000/xmlns/") || (node.Value != "http://james.newtonking.com/projects/json"))
                    {
                        if ((node.NamespaceUri != "http://james.newtonking.com/projects/json") || (node.LocalName != "Array"))
                        {
                            if (writePropertyName)
                            {
                                writer.WritePropertyName(this.GetPropertyName(node, manager));
                            }
                            writer.WriteValue(node.Value);
                        }
                        return;
                    }
                    return;

                case XmlNodeType.Comment:
                    if (writePropertyName)
                    {
                        writer.WriteComment(node.Value);
                    }
                    return;

                case XmlNodeType.Document:
                case XmlNodeType.DocumentFragment:
                    this.SerializeGroupedNodes(writer, node, manager, writePropertyName);
                    return;

                case XmlNodeType.DocumentType:
                {
                    IXmlDocumentType type2 = (IXmlDocumentType) node;
                    writer.WritePropertyName(this.GetPropertyName(node, manager));
                    writer.WriteStartObject();
                    if (!string.IsNullOrEmpty(type2.Name))
                    {
                        writer.WritePropertyName("@name");
                        writer.WriteValue(type2.Name);
                    }
                    if (!string.IsNullOrEmpty(type2.Public))
                    {
                        writer.WritePropertyName("@public");
                        writer.WriteValue(type2.Public);
                    }
                    if (!string.IsNullOrEmpty(type2.System))
                    {
                        writer.WritePropertyName("@system");
                        writer.WriteValue(type2.System);
                    }
                    if (!string.IsNullOrEmpty(type2.InternalSubset))
                    {
                        writer.WritePropertyName("@internalSubset");
                        writer.WriteValue(type2.InternalSubset);
                    }
                    writer.WriteEndObject();
                    return;
                }
                case XmlNodeType.XmlDeclaration:
                {
                    IXmlDeclaration declaration = (IXmlDeclaration) node;
                    writer.WritePropertyName(this.GetPropertyName(node, manager));
                    writer.WriteStartObject();
                    if (!string.IsNullOrEmpty(declaration.Version))
                    {
                        writer.WritePropertyName("@version");
                        writer.WriteValue(declaration.Version);
                    }
                    if (!string.IsNullOrEmpty(declaration.Encoding))
                    {
                        writer.WritePropertyName("@encoding");
                        writer.WriteValue(declaration.Encoding);
                    }
                    if (!string.IsNullOrEmpty(declaration.Standalone))
                    {
                        writer.WritePropertyName("@standalone");
                        writer.WriteValue(declaration.Standalone);
                    }
                    writer.WriteEndObject();
                    return;
                }
            }
            throw new JsonSerializationException("Unexpected XmlNodeType when serializing nodes: " + node.NodeType);
        }

        private bool ValueAttributes(List<IXmlNode> c)
        {
            using (List<IXmlNode>.Enumerator enumerator = c.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.NamespaceUri != "http://james.newtonking.com/projects/json")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private IXmlNode WrapXml(object value)
        {
            if (value is XObject)
            {
                return XContainerWrapper.WrapNode((XObject) value);
            }
            if (!(value is System.Xml.XmlNode))
            {
                throw new ArgumentException("Value must be an XML object.", "value");
            }
            return XmlNodeWrapper.WrapNode((System.Xml.XmlNode) value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IXmlNode node = this.WrapXml(value);
            XmlNamespaceManager manager = new XmlNamespaceManager(new NameTable());
            this.PushParentNamespaces(node, manager);
            if (!this.OmitRootObject)
            {
                writer.WriteStartObject();
            }
            this.SerializeNode(writer, node, manager, !this.OmitRootObject);
            if (!this.OmitRootObject)
            {
                writer.WriteEndObject();
            }
        }

        public string DeserializeRootElementName { get; set; }

        public bool WriteArrayAttribute { get; set; }

        public bool OmitRootObject { get; set; }
    }
}

