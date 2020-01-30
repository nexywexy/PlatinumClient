namespace PlatinumClient
{
    using System;
    using System.Collections.Generic;

    internal class Telemetry
    {
        public static TelemetryProvider[] providers = new TelemetryProvider[] { new CSGOProvider() };

        public static List<TelemetryReport> collect()
        {
            List<TelemetryReport> list = new List<TelemetryReport>();
            foreach (TelemetryProvider provider in providers)
            {
                foreach (byte[] buffer in provider.collect())
                {
                    TelemetryReport item = new TelemetryReport {
                        contents = buffer,
                        type = provider.GetType().Name,
                        contentType = provider.contentType()
                    };
                    list.Add(item);
                }
            }
            return list;
        }

        public static void prepare(bool status)
        {
            TelemetryProvider[] providers = Telemetry.providers;
            for (int i = 0; i < providers.Length; i++)
            {
                providers[i].prepare(status);
            }
        }

        public static bool status()
        {
            TelemetryProvider[] providers = Telemetry.providers;
            for (int i = 0; i < providers.Length; i++)
            {
                if (!providers[i].status())
                {
                    return false;
                }
            }
            return true;
        }
    }
}

