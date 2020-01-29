namespace System.Net.Http
{
    using System;
    using System.Runtime.CompilerServices;

    public static class HttpClientHandlerExtensions
    {
        public static bool SupportsAllowAutoRedirect(this HttpClientHandler handler) => 
            true;

        public static bool SupportsPreAuthenticate(this HttpClientHandler handler) => 
            true;

        public static bool SupportsProtocolVersion(this HttpClientHandler handler) => 
            true;

        public static bool SupportsTransferEncodingChunked(this HttpClientHandler handler) => 
            true;

        public static bool SupportsUseProxy(this HttpClientHandler handler) => 
            true;
    }
}

