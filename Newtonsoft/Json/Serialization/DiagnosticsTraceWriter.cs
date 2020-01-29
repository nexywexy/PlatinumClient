namespace Newtonsoft.Json.Serialization
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class DiagnosticsTraceWriter : ITraceWriter
    {
        private TraceEventType GetTraceEventType(TraceLevel level)
        {
            switch (level)
            {
                case TraceLevel.Error:
                    return TraceEventType.Error;

                case TraceLevel.Warning:
                    return TraceEventType.Warning;

                case TraceLevel.Info:
                    return TraceEventType.Information;

                case TraceLevel.Verbose:
                    return TraceEventType.Verbose;
            }
            throw new ArgumentOutOfRangeException("level");
        }

        public void Trace(TraceLevel level, string message, Exception ex)
        {
            if (level != TraceLevel.Off)
            {
                TraceEventCache eventCache = new TraceEventCache();
                TraceEventType traceEventType = this.GetTraceEventType(level);
                foreach (TraceListener listener in System.Diagnostics.Trace.Listeners)
                {
                    if (!listener.IsThreadSafe)
                    {
                        TraceListener listener2 = listener;
                        lock (listener2)
                        {
                            listener.TraceEvent(eventCache, "Newtonsoft.Json", traceEventType, 0, message);
                            goto Label_007D;
                        }
                    }
                    listener.TraceEvent(eventCache, "Newtonsoft.Json", traceEventType, 0, message);
                Label_007D:
                    if (System.Diagnostics.Trace.AutoFlush)
                    {
                        listener.Flush();
                    }
                }
            }
        }

        public TraceLevel LevelFilter { get; set; }
    }
}

