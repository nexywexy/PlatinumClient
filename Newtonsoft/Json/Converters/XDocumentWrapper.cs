namespace Newtonsoft.Json.Converters
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;

    internal class XDocumentWrapper : XContainerWrapper, IXmlDocument, IXmlNode
    {
        public XDocumentWrapper(XDocument document) : base(document)
        {
        }

        public override IXmlNode AppendChild(IXmlNode newChild)
        {
            XDeclarationWrapper wrapper = newChild as XDeclarationWrapper;
            if (wrapper != null)
            {
                this.Document.Declaration = wrapper.Declaration;
                return wrapper;
            }
            return base.AppendChild(newChild);
        }

        public IXmlNode CreateAttribute(string name, string value) => 
            new XAttributeWrapper(new XAttribute(name, value));

        public IXmlNode CreateAttribute(string qualifiedName, string namespaceUri, string value) => 
            new XAttributeWrapper(new XAttribute(XName.Get(MiscellaneousUtils.GetLocalName(qualifiedName), namespaceUri), value));

        public IXmlNode CreateCDataSection(string data) => 
            new XObjectWrapper(new XCData(data));

        public IXmlNode CreateComment(string text) => 
            new XObjectWrapper(new XComment(text));

        public IXmlElement CreateElement(string elementName) => 
            new XElementWrapper(new XElement(elementName));

        public IXmlElement CreateElement(string qualifiedName, string namespaceUri) => 
            new XElementWrapper(new XElement(XName.Get(MiscellaneousUtils.GetLocalName(qualifiedName), namespaceUri)));

        public IXmlNode CreateProcessingInstruction(string target, string data) => 
            new XProcessingInstructionWrapper(new XProcessingInstruction(target, data));

        public IXmlNode CreateSignificantWhitespace(string text) => 
            new XObjectWrapper(new XText(text));

        public IXmlNode CreateTextNode(string text) => 
            new XObjectWrapper(new XText(text));

        public IXmlNode CreateWhitespace(string text) => 
            new XObjectWrapper(new XText(text));

        public IXmlNode CreateXmlDeclaration(string version, string encoding, string standalone) => 
            new XDeclarationWrapper(new XDeclaration(version, encoding, standalone));

        public IXmlNode CreateXmlDocumentType(string name, string publicId, string systemId, string internalSubset) => 
            new XDocumentTypeWrapper(new XDocumentType(name, publicId, systemId, internalSubset));

        private XDocument Document =>
            ((XDocument) base.WrappedNode);

        public override List<IXmlNode> ChildNodes
        {
            get
            {
                List<IXmlNode> childNodes = base.ChildNodes;
                if ((this.Document.Declaration != null) && (childNodes[0].NodeType != XmlNodeType.XmlDeclaration))
                {
                    childNodes.Insert(0, new XDeclarationWrapper(this.Document.Declaration));
                }
                return childNodes;
            }
        }

        public IXmlElement DocumentElement
        {
            get
            {
                if (this.Document.Root == null)
                {
                    return null;
                }
                return new XElementWrapper(this.Document.Root);
            }
        }
    }
}

