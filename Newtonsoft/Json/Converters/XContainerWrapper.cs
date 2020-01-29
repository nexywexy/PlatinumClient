namespace Newtonsoft.Json.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    internal class XContainerWrapper : XObjectWrapper
    {
        private List<IXmlNode> _childNodes;

        public XContainerWrapper(XContainer container) : base(container)
        {
        }

        public override IXmlNode AppendChild(IXmlNode newChild)
        {
            this.Container.Add(newChild.WrappedNode);
            this._childNodes = null;
            return newChild;
        }

        internal static IXmlNode WrapNode(XObject node)
        {
            switch (node)
            {
                case (XDocument _):
                    return new XDocumentWrapper((XDocument) node);
                    break;
            }
            if (node is XElement)
            {
                return new XElementWrapper((XElement) node);
            }
            if (node is XContainer)
            {
                return new XContainerWrapper((XContainer) node);
            }
            if (node is XProcessingInstruction)
            {
                return new XProcessingInstructionWrapper((XProcessingInstruction) node);
            }
            if (node is XText)
            {
                return new XTextWrapper((XText) node);
            }
            if (node is XComment)
            {
                return new XCommentWrapper((XComment) node);
            }
            if (node is XAttribute)
            {
                return new XAttributeWrapper((XAttribute) node);
            }
            if (node is XDocumentType)
            {
                return new XDocumentTypeWrapper((XDocumentType) node);
            }
            return new XObjectWrapper(node);
        }

        private XContainer Container =>
            ((XContainer) base.WrappedNode);

        public override List<IXmlNode> ChildNodes
        {
            get
            {
                if (this._childNodes == null)
                {
                    this._childNodes = new List<IXmlNode>();
                    foreach (XNode node in this.Container.Nodes())
                    {
                        this._childNodes.Add(WrapNode(node));
                    }
                }
                return this._childNodes;
            }
        }

        public override IXmlNode ParentNode
        {
            get
            {
                if (this.Container.Parent == null)
                {
                    return null;
                }
                return WrapNode(this.Container.Parent);
            }
        }
    }
}

