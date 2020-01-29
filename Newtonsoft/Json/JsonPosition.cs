namespace Newtonsoft.Json
{
    using Newtonsoft.Json.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;

    [StructLayout(LayoutKind.Sequential)]
    internal struct JsonPosition
    {
        private static readonly char[] SpecialCharacters;
        internal JsonContainerType Type;
        internal int Position;
        internal string PropertyName;
        internal bool HasIndex;
        public JsonPosition(JsonContainerType type)
        {
            this.Type = type;
            this.HasIndex = TypeHasIndex(type);
            this.Position = -1;
            this.PropertyName = null;
        }

        internal int CalculateLength()
        {
            switch (this.Type)
            {
                case JsonContainerType.Object:
                    return (this.PropertyName.Length + 5);

                case JsonContainerType.Array:
                case JsonContainerType.Constructor:
                    return (MathUtils.IntLength((ulong) this.Position) + 2);
            }
            throw new ArgumentOutOfRangeException("Type");
        }

        internal void WriteTo(StringBuilder sb)
        {
            switch (this.Type)
            {
                case JsonContainerType.Object:
                {
                    string propertyName = this.PropertyName;
                    if (propertyName.IndexOfAny(SpecialCharacters) == -1)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append('.');
                        }
                        sb.Append(propertyName);
                        return;
                    }
                    sb.Append("['");
                    sb.Append(propertyName);
                    sb.Append("']");
                    return;
                }
                case JsonContainerType.Array:
                case JsonContainerType.Constructor:
                    sb.Append('[');
                    sb.Append(this.Position);
                    sb.Append(']');
                    return;
            }
        }

        internal static bool TypeHasIndex(JsonContainerType type)
        {
            if (type != JsonContainerType.Array)
            {
                return (type == JsonContainerType.Constructor);
            }
            return true;
        }

        internal static string BuildPath(List<JsonPosition> positions, JsonPosition? currentPosition)
        {
            int capacity = 0;
            if (positions != null)
            {
                for (int i = 0; i < positions.Count; i++)
                {
                    JsonPosition position = positions[i];
                    capacity += position.CalculateLength();
                }
            }
            if (currentPosition.HasValue)
            {
                capacity += currentPosition.GetValueOrDefault().CalculateLength();
            }
            StringBuilder sb = new StringBuilder(capacity);
            if (positions != null)
            {
                foreach (JsonPosition position2 in positions)
                {
                    position2.WriteTo(sb);
                }
            }
            if (currentPosition.HasValue)
            {
                currentPosition.GetValueOrDefault().WriteTo(sb);
            }
            return sb.ToString();
        }

        internal static string FormatMessage(IJsonLineInfo lineInfo, string path, string message)
        {
            if (!message.EndsWith(Environment.NewLine, StringComparison.Ordinal))
            {
                message = message.Trim();
                if (!message.EndsWith('.'))
                {
                    message = message + ".";
                }
                message = message + " ";
            }
            message = message + "Path '{0}'".FormatWith(CultureInfo.InvariantCulture, path);
            if ((lineInfo != null) && lineInfo.HasLineInfo())
            {
                message = message + ", line {0}, position {1}".FormatWith(CultureInfo.InvariantCulture, lineInfo.LineNumber, lineInfo.LinePosition);
            }
            message = message + ".";
            return message;
        }

        static JsonPosition()
        {
            SpecialCharacters = new char[] { '.', ' ', '[', ']', '(', ')' };
        }
    }
}

