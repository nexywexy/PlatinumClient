namespace Newtonsoft.Json.Converters
{
    using System;

    internal interface IXmlDocument : IXmlNode
    {
        IXmlNode CreateAttribute(string name, string value);
        IXmlNode CreateAttribute(string qualifiedName, string namespaceUri, string value);
        IXmlNode CreateCDataSection(string data);
        IXmlNode CreateComment(string text);
        IXmlElement CreateElement(string elementName);
        IXmlElement CreateElement(string qualifiedName, string namespaceUri);
        IXmlNode CreateProcessingInstruction(string target, string data);
        IXmlNode CreateSignificantWhitespace(string text);
        IXmlNode CreateTextNode(string text);
        IXmlNode CreateWhitespace(string text);
        IXmlNode CreateXmlDeclaration(string version, string encoding, string standalone);
        IXmlNode CreateXmlDocumentType(string name, string publicId, string systemId, string internalSubset);

        IXmlElement DocumentElement { get; }
    }
}

