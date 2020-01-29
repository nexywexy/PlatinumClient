namespace Newtonsoft.Json.Converters
{
    using System;
    using System.Xml.Linq;

    internal class XCommentWrapper : XObjectWrapper
    {
        public XCommentWrapper(XComment text) : base(text)
        {
        }

        private XComment Text =>
            ((XComment) base.WrappedNode);

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

