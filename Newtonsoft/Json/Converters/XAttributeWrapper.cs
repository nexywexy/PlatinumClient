namespace Newtonsoft.Json.Converters
{
    using System;
    using System.Xml.Linq;

    internal class XAttributeWrapper : XObjectWrapper
    {
        public XAttributeWrapper(XAttribute attribute) : base(attribute)
        {
        }

        private XAttribute Attribute =>
            ((XAttribute) base.WrappedNode);

        public override string Value
        {
            get => 
                this.Attribute.Value;
            set => 
                (this.Attribute.Value = value);
        }

        public override string LocalName =>
            this.Attribute.Name.LocalName;

        public override string NamespaceUri =>
            this.Attribute.Name.NamespaceName;

        public override IXmlNode ParentNode
        {
            get
            {
                if (this.Attribute.Parent == null)
                {
                    return null;
                }
                return XContainerWrapper.WrapNode(this.Attribute.Parent);
            }
        }
    }
}

