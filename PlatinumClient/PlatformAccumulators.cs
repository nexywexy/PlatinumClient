namespace PlatinumClient
{
    using System;
    using System.Collections.Generic;

    internal class PlatformAccumulators
    {
        public static PlatformAccumulator[] accumulators = new PlatformAccumulator[] { new SWAccumulator() };

        public static Dictionary<string, string[]> accumulate()
        {
            Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
            foreach (PlatformAccumulator accumulator in accumulators)
            {
                dictionary.Add(accumulator.type(), accumulator.accumulate());
            }
            return dictionary;
        }
    }
}

