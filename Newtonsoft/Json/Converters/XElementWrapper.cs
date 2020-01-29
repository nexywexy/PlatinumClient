namespace Newtonsoft.Json.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    internal class XElementWrapper : XContainerWrapper, IXmlElement, IXmlNode
    {
        private List<IXmlNode> _attributes;

        public XElementWrapper(XElement element) : base(element)
        {
        }

        public override IXmlNode AppendChild(IXmlNode newChild)
        {
            this._attributes = null;
            return base.AppendChild(newChild);
        }

        public string GetPrefixOfNamespace(string namespaceUri) => 
            this.Element.GetPrefixOfNamespace(namespaceUri);

        public void SetAttributeNode(IXmlNode attribute)
        {
            XObjectWrapper wrapper = (XObjectWrapper) attribute;
            this.Element.Add(wrapper.WrappedNode);
            this._attributes = null;
        }

        private XElement Element =>
            ((XElement) base.WrappedNode);

        public override List<IXmlNode> Attributes
        {
            get
            {
                if (this._attributes == null)
                {
                    this._attributes = new List<IXmlNode>();
                    foreach (XAttribute attribute in this.Element.Attributes())
                    {
                        this._attributes.Add(new XAttributeWrapper(attribute));
                    }
                    string namespaceUri = this.NamespaceUri;
                    if (!string.IsNullOrEmpty(namespaceUri))
                    {
                        if (this.ParentNode == null)
                        {
                        }
                        if ((namespaceUri != null.NamespaceUri) && string.IsNullOrEmpty(this.GetPrefixOfNamespace(namespaceUri)))
                        {
                            bool flag = false;
                            foreach (IXmlNode node in this._attributes)
                            {
                                if (((node.LocalName == "xmlns") && string.IsNullOrEmpty(node.NamespaceUri)) && (node.Value == namespaceUri))
                                {
                                    flag = true;
                                }
                            }
                            if (!flag)
                            {
                                this._attributes.Insert(0, new XAttributeWrapper(new XAttribute("xmlns", namespaceUri)));
                            }
                        }
                    }
                }
                return this._attributes;
            }
        }

        public override string Value
        {
            get => 
                this.Element.Value;
            set => 
                (this.Element.Value = value);
        }

        public override string LocalName =>
            this.Element.Name.LocalName;

        public override string NamespaceUri =>
            this.Element.Name.NamespaceName;

        public bool IsEmpty =>
            this.Element.IsEmpty;
    }
}

