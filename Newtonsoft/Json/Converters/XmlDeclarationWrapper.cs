namespace Newtonsoft.Json.Converters
{
    using System;
    using System.Xml;

    internal class XmlDeclarationWrapper : XmlNodeWrapper, IXmlDeclaration, IXmlNode
    {
        private readonly XmlDeclaration _declaration;

        public XmlDeclarationWrapper(XmlDeclaration declaration) : base(declaration)
        {
            this._declaration = declaration;
        }

        public string Version =>
            this._declaration.Version;

        public string Encoding
        {
            get => 
                this._declaration.Encoding;
            set => 
                (this._declaration.Encoding = value);
        }

        public string Standalone
        {
            get => 
                this._declaration.Standalone;
            set => 
                (this._declaration.Standalone = value);
        }
    }
}

