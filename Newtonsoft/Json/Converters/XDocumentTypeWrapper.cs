namespace Newtonsoft.Json.Converters
{
    using System;
    using System.Xml.Linq;

    internal class XDocumentTypeWrapper : XObjectWrapper, IXmlDocumentType, IXmlNode
    {
        private readonly XDocumentType _documentType;

        public XDocumentTypeWrapper(XDocumentType documentType) : base(documentType)
        {
            this._documentType = documentType;
        }

        public string Name =>
            this._documentType.Name;

        public string System =>
            this._documentType.SystemId;

        public string Public =>
            this._documentType.PublicId;

        public string InternalSubset =>
            this._documentType.InternalSubset;

        public override string LocalName =>
            "DOCTYPE";
    }
}

