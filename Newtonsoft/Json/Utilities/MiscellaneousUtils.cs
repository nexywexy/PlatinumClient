namespace Newtonsoft.Json.Utilities
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal static class MiscellaneousUtils
    {
        public static int ByteArrayCompare(byte[] a1, byte[] a2)
        {
            int num2 = a1.Length.CompareTo(a2.Length);
            if (num2 != 0)
            {
                return num2;
            }
            for (int i = 0; i < a1.Length; i++)
            {
                int num4 = a1[i].CompareTo(a2[i]);
                if (num4 != 0)
                {
                    return num4;
                }
            }
            return 0;
        }

        public static ArgumentOutOfRangeException CreateArgumentOutOfRangeException(string paramName, object actualValue, string message) => 
            new ArgumentOutOfRangeException(paramName, message + Environment.NewLine + "Actual value was {0}.".FormatWith(CultureInfo.InvariantCulture, actualValue));

        internal static string FormatValueForPrint(object value)
        {
            if (value == null)
            {
                return "{null}";
            }
            if (value is string)
            {
                return ("\"" + value + "\"");
            }
            return value.ToString();
        }

        public static string GetLocalName(string qualifiedName)
        {
            GetQualifiedNameParts(qualifiedName, out _, out string str2);
            return str2;
        }

        public static string GetPrefix(string qualifiedName)
        {
            GetQualifiedNameParts(qualifiedName, out string str, out _);
            return str;
        }

        public static void GetQualifiedNameParts(string qualifiedName, out string prefix, out string localName)
        {
            int index = qualifiedName.IndexOf(':');
            if (((index == -1) || (index == 0)) || ((qualifiedName.Length - 1) == index))
            {
                prefix = null;
                localName = qualifiedName;
            }
            else
            {
                prefix = qualifiedName.Substring(0, index);
                localName = qualifiedName.Substring(index + 1);
            }
        }

        public static string ToString(object value)
        {
            if (value == null)
            {
                return "{null}";
            }
            if (value is string)
            {
                return ("\"" + value.ToString() + "\"");
            }
            return value.ToString();
        }

        public static bool ValueEquals(object objA, object objB)
        {
            if ((objA == null) && (objB == null))
            {
                return true;
            }
            if ((objA != null) && (objB == null))
            {
                return false;
            }
            if ((objA == null) && (objB != null))
            {
                return false;
            }
            if (!(objA.GetType() != objB.GetType()))
            {
                return objA.Equals(objB);
            }
            if (ConvertUtils.IsInteger(objA) && ConvertUtils.IsInteger(objB))
            {
                return Convert.ToDecimal(objA, CultureInfo.CurrentCulture).Equals(Convert.ToDecimal(objB, CultureInfo.CurrentCulture));
            }
            return (((((objA is double) || (objA is float)) || (objA is decimal)) && (((objB is double) || (objB is float)) || (objB is decimal))) && MathUtils.ApproxEquals(Convert.ToDouble(objA, CultureInfo.CurrentCulture), Convert.ToDouble(objB, CultureInfo.CurrentCulture)));
        }
    }
}

