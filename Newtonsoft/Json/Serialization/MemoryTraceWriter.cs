namespace Newtonsoft.Json.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class MemoryTraceWriter : ITraceWriter
    {
        private readonly Queue<string> _traceMessages;

        public MemoryTraceWriter()
        {
            this.LevelFilter = TraceLevel.Verbose;
            this._traceMessages = new Queue<string>();
        }

        public IEnumerable<string> GetTraceMessages() => 
            this._traceMessages;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (string str in this._traceMessages)
            {
                if (builder.Length > 0)
                {
                    builder.AppendLine();
                }
                builder.Append(str);
            }
            return builder.ToString();
        }

        public void Trace(TraceLevel level, string message, Exception ex)
        {
            if (this._traceMessages.Count >= 0x3e8)
            {
                this._traceMessages.Dequeue();
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff", CultureInfo.InvariantCulture));
            builder.Append(" ");
            builder.Append(level.ToString("g"));
            builder.Append(" ");
            builder.Append(message);
            this._traceMessages.Enqueue(builder.ToString());
        }

        public TraceLevel LevelFilter { get; set; }
    }
}

