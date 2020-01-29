namespace Newtonsoft.Json.Converters
{
    using System;
    using System.Xml;

    internal class XmlElementWrapper : XmlNodeWrapper, IXmlElement, IXmlNode
    {
        private readonly XmlElement _element;

        public XmlElementWrapper(XmlElement element) : base(element)
        {
            this._element = element;
        }

        public string GetPrefixOfNamespace(string namespaceUri) => 
            this._element.GetPrefixOfNamespace(namespaceUri);

        public void SetAttributeNode(IXmlNode attribute)
        {
            XmlNodeWrapper wrapper = (XmlNodeWrapper) attribute;
            this._element.SetAttributeNode((System.Xml.XmlAttribute) wrapper.WrappedNode);
        }

        public bool IsEmpty =>
            this._element.IsEmpty;
    }
}

