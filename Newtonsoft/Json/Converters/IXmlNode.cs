namespace Newtonsoft.Json.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    internal interface IXmlNode
    {
        IXmlNode AppendChild(IXmlNode newChild);

        XmlNodeType NodeType { get; }

        string LocalName { get; }

        List<IXmlNode> ChildNodes { get; }

        List<IXmlNode> Attributes { get; }

        IXmlNode ParentNode { get; }

        string Value { get; set; }

        string NamespaceUri { get; }

        object WrappedNode { get; }
    }
}

