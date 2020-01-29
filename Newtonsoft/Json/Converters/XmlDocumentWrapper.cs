namespace Newtonsoft.Json.Converters
{
    using System;
    using System.Xml;

    internal class XmlDocumentWrapper : XmlNodeWrapper, IXmlDocument, IXmlNode
    {
        private readonly XmlDocument _document;

        public XmlDocumentWrapper(XmlDocument document) : base(document)
        {
            this._document = document;
        }

        public IXmlNode CreateAttribute(string name, string value) => 
            new XmlNodeWrapper(this._document.CreateAttribute(name)) { Value = value };

        public IXmlNode CreateAttribute(string qualifiedName, string namespaceUri, string value) => 
            new XmlNodeWrapper(this._document.CreateAttribute(qualifiedName, namespaceUri)) { Value = value };

        public IXmlNode CreateCDataSection(string data) => 
            new XmlNodeWrapper(this._document.CreateCDataSection(data));

        public IXmlNode CreateComment(string data) => 
            new XmlNodeWrapper(this._document.CreateComment(data));

        public IXmlElement CreateElement(string elementName) => 
            new XmlElementWrapper(this._document.CreateElement(elementName));

        public IXmlElement CreateElement(string qualifiedName, string namespaceUri) => 
            new XmlElementWrapper(this._document.CreateElement(qualifiedName, namespaceUri));

        public IXmlNode CreateProcessingInstruction(string target, string data) => 
            new XmlNodeWrapper(this._document.CreateProcessingInstruction(target, data));

        public IXmlNode CreateSignificantWhitespace(string text) => 
            new XmlNodeWrapper(this._document.CreateSignificantWhitespace(text));

        public IXmlNode CreateTextNode(string text) => 
            new XmlNodeWrapper(this._document.CreateTextNode(text));

        public IXmlNode CreateWhitespace(string text) => 
            new XmlNodeWrapper(this._document.CreateWhitespace(text));

        public IXmlNode CreateXmlDeclaration(string version, string encoding, string standalone) => 
            new XmlDeclarationWrapper(this._document.CreateXmlDeclaration(version, encoding, standalone));

        public IXmlNode CreateXmlDocumentType(string name, string publicId, string systemId, string internalSubset) => 
            new XmlDocumentTypeWrapper(this._document.CreateDocumentType(name, publicId, systemId, null));

        public IXmlElement DocumentElement
        {
            get
            {
                if (this._document.DocumentElement == null)
                {
                    return null;
                }
                return new XmlElementWrapper(this._document.DocumentElement);
            }
        }
    }
}

