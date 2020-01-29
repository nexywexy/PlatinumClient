namespace Newtonsoft.Json.Utilities
{
    using System;

    internal static class MathUtils
    {
        public static bool ApproxEquals(double d1, double d2)
        {
            if (d1 == d2)
            {
                return true;
            }
            double num = ((Math.Abs(d1) + Math.Abs(d2)) + 10.0) * 2.2204460492503131E-16;
            double num2 = d1 - d2;
            return ((-num < num2) && (num > num2));
        }

        public static int IntLength(ulong i)
        {
            if (i < 0x2540be400L)
            {
                if (i < 10L)
                {
                    return 1;
                }
                if (i < 100L)
                {
                    return 2;
                }
                if (i < 0x3e8L)
                {
                    return 3;
                }
                if (i < 0x2710L)
                {
                    return 4;
                }
                if (i < 0x186a0L)
                {
                    return 5;
                }
                if (i < 0xf4240L)
                {
                    return 6;
                }
                if (i < 0x989680L)
                {
                    return 7;
                }
                if (i < 0x5f5e100L)
                {
                    return 8;
                }
                if (i < 0x3b9aca00L)
                {
                    return 9;
                }
                return 10;
            }
            if (i < 0x174876e800L)
            {
                return 11;
            }
            if (i < 0xe8d4a51000L)
            {
                return 12;
            }
            if (i < 0x9184e72a000L)
            {
                return 13;
            }
            if (i < 0x5af3107a4000L)
            {
                return 14;
            }
            if (i < 0x38d7ea4c68000L)
            {
                return 15;
            }
            if (i < 0x2386f26fc10000L)
            {
                return 0x10;
            }
            if (i < 0x16345785d8a0000L)
            {
                return 0x11;
            }
            if (i < 0xde0b6b3a7640000L)
            {
                return 0x12;
            }
            if (i < 10_000_000_000_000_000_000L)
            {
                return 0x13;
            }
            return 20;
        }

        public static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char) (n + 0x30);
            }
            return (char) ((n - 10) + 0x61);
        }

        public static double? Max(double? val1, double? val2)
        {
            if (!val1.HasValue)
            {
                return val2;
            }
            if (!val2.HasValue)
            {
                return val1;
            }
            return new double?(Math.Max(val1.GetValueOrDefault(), val2.GetValueOrDefault()));
        }

        public static int? Max(int? val1, int? val2)
        {
            if (!val1.HasValue)
            {
                return val2;
            }
            if (!val2.HasValue)
            {
                return val1;
            }
            return new int?(Math.Max(val1.GetValueOrDefault(), val2.GetValueOrDefault()));
        }

        public static int? Min(int? val1, int? val2)
        {
            if (!val1.HasValue)
            {
                return val2;
            }
            if (!val2.HasValue)
            {
                return val1;
            }
            return new int?(Math.Min(val1.GetValueOrDefault(), val2.GetValueOrDefault()));
        }
    }
}

