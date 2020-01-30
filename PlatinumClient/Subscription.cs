namespace PlatinumClient
{
    using System;

    public class Subscription
    {
        public int id;
        public string product;
        public string plan;
        public string expiration;
        public string productShortName;

        public override string ToString()
        {
            DateTime now = DateTime.Now;
            TimeSpan span = (TimeSpan) (DateTime.Parse(this.expiration) - now);
            object[] objArray1 = new object[] { this.product, " (Expires in ", Math.Ceiling(span.TotalDays), " days)" };
            return string.Concat(objArray1);
        }
    }
}

