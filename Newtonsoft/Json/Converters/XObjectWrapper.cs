namespace Newtonsoft.Json.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;

    internal class XObjectWrapper : IXmlNode
    {
        private static readonly List<IXmlNode> EmptyChildNodes = new List<IXmlNode>();
        private readonly XObject _xmlObject;

        public XObjectWrapper(XObject xmlObject)
        {
            this._xmlObject = xmlObject;
        }

        public virtual IXmlNode AppendChild(IXmlNode newChild)
        {
            throw new InvalidOperationException();
        }

        public object WrappedNode =>
            this._xmlObject;

        public virtual XmlNodeType NodeType =>
            this._xmlObject.NodeType;

        public virtual string LocalName =>
            null;

        public virtual List<IXmlNode> ChildNodes =>
            EmptyChildNodes;

        public virtual List<IXmlNode> Attributes =>
            null;

        public virtual IXmlNode ParentNode =>
            null;

        public virtual string Value
        {
            get => 
                null;
            set
            {
                throw new InvalidOperationException();
            }
        }

        public virtual string NamespaceUri =>
            null;
    }
}

