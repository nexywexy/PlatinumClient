namespace Newtonsoft.Json.Converters
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml;
    using System.Xml.Linq;

    internal class XDeclarationWrapper : XObjectWrapper, IXmlDeclaration, IXmlNode
    {
        public XDeclarationWrapper(XDeclaration declaration) : base(null)
        {
            this.Declaration = declaration;
        }

        internal XDeclaration Declaration { get; private set; }

        public override XmlNodeType NodeType =>
            XmlNodeType.XmlDeclaration;

        public string Version =>
            this.Declaration.Version;

        public string Encoding
        {
            get => 
                this.Declaration.Encoding;
            set => 
                (this.Declaration.Encoding = value);
        }

        public string Standalone
        {
            get => 
                this.Declaration.Standalone;
            set => 
                (this.Declaration.Standalone = value);
        }
    }
}

