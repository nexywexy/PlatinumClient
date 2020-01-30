namespace PlatinumClient
{
    using System;
    using System.Collections.Generic;

    internal interface TelemetryProvider
    {
        List<byte[]> collect();
        TelemetryContentType contentType();
        void prepare(bool status);
        bool status();
    }
}

