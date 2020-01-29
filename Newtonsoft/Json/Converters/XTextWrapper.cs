namespace Newtonsoft.Json.Converters
{
    using System;
    using System.Xml.Linq;

    internal class XTextWrapper : XObjectWrapper
    {
        public XTextWrapper(XText text) : base(text)
        {
        }

        private XText Text =>
            ((XText) base.WrappedNode);

        public override string Value
        {
            get => 
                this.Text.Value;
            set => 
                (this.Text.Value = value);
        }

        public override IXmlNode ParentNode
        {
            get
            {
                if (this.Text.Parent == null)
                {
                    return null;
                }
                return XContainerWrapper.WrapNode(this.Text.Parent);
            }
        }
    }
}

