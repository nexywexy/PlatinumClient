namespace Newtonsoft.Json.Converters
{
    using System;
    using System.Xml.Linq;

    internal class XProcessingInstructionWrapper : XObjectWrapper
    {
        public XProcessingInstructionWrapper(XProcessingInstruction processingInstruction) : base(processingInstruction)
        {
        }

        private XProcessingInstruction ProcessingInstruction =>
            ((XProcessingInstruction) base.WrappedNode);

        public override string LocalName =>
            this.ProcessingInstruction.Target;

        public override string Value
        {
            get => 
                this.ProcessingInstruction.Data;
            set => 
                (this.ProcessingInstruction.Data = value);
        }
    }
}

