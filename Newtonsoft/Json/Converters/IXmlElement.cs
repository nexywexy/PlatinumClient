namespace Newtonsoft.Json.Converters
{
    using System;

    internal interface IXmlElement : IXmlNode
    {
        string GetPrefixOfNamespace(string namespaceUri);
        void SetAttributeNode(IXmlNode attribute);

        bool IsEmpty { get; }
    }
}

