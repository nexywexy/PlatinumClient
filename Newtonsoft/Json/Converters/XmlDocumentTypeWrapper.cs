﻿namespace Newtonsoft.Json.Converters
{
    using System;
    using System.Xml;

    internal class XmlDocumentTypeWrapper : XmlNodeWrapper, IXmlDocumentType, IXmlNode
    {
        private readonly XmlDocumentType _documentType;

        public XmlDocumentTypeWrapper(XmlDocumentType documentType) : base(documentType)
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

