namespace Newtonsoft.Json.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    internal class XmlNodeWrapper : IXmlNode
    {
        private readonly System.Xml.XmlNode _node;
        private List<IXmlNode> _childNodes;
        private List<IXmlNode> _attributes;

        public XmlNodeWrapper(System.Xml.XmlNode node)
        {
            this._node = node;
        }

        public IXmlNode AppendChild(IXmlNode newChild)
        {
            XmlNodeWrapper wrapper = (XmlNodeWrapper) newChild;
            this._node.AppendChild(wrapper._node);
            this._childNodes = null;
            this._attributes = null;
            return newChild;
        }

        internal static IXmlNode WrapNode(System.Xml.XmlNode node)
        {
            XmlNodeType nodeType = node.NodeType;
            if (nodeType != XmlNodeType.Element)
            {
                if (nodeType == XmlNodeType.DocumentType)
                {
                    return new XmlDocumentTypeWrapper((XmlDocumentType) node);
                }
                if (nodeType == XmlNodeType.XmlDeclaration)
                {
                    return new XmlDeclarationWrapper((XmlDeclaration) node);
                }
                return new XmlNodeWrapper(node);
            }
            return new XmlElementWrapper((XmlElement) node);
        }

        public object WrappedNode =>
            this._node;

        public XmlNodeType NodeType =>
            this._node.NodeType;

        public virtual string LocalName =>
            this._node.LocalName;

        public List<IXmlNode> ChildNodes
        {
            get
            {
                if (this._childNodes == null)
                {
                    this._childNodes = new List<IXmlNode>(this._node.ChildNodes.Count);
                    foreach (System.Xml.XmlNode node in this._node.ChildNodes)
                    {
                        this._childNodes.Add(WrapNode(node));
                    }
                }
                return this._childNodes;
            }
        }

        public List<IXmlNode> Attributes
        {
            get
            {
                if (this._node.Attributes == null)
                {
                    return null;
                }
                if (this._attributes == null)
                {
                    this._attributes = new List<IXmlNode>(this._node.Attributes.Count);
                    foreach (System.Xml.XmlAttribute attribute in this._node.Attributes)
                    {
                        this._attributes.Add(WrapNode(attribute));
                    }
                }
                return this._attributes;
            }
        }

        public IXmlNode ParentNode
        {
            get
            {
                System.Xml.XmlNode node = (this._node is System.Xml.XmlAttribute) ? ((System.Xml.XmlAttribute) this._node).OwnerElement : this._node.ParentNode;
                if (node == null)
                {
                    return null;
                }
                return WrapNode(node);
            }
        }

        public string Value
        {
            get => 
                this._node.Value;
            set => 
                (this._node.Value = value);
        }

        public string NamespaceUri =>
            this._node.NamespaceURI;
    }
}

