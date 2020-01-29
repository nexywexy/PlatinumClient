namespace Newtonsoft.Json.Converters
{
    using System;

    internal interface IXmlDeclaration : IXmlNode
    {
        string Version { get; }

        string Encoding { get; set; }

        string Standalone { get; set; }
    }
}

