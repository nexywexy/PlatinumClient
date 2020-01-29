namespace Newtonsoft.Json.Linq
{
    using System;

    public class JsonMergeSettings
    {
        private Newtonsoft.Json.Linq.MergeArrayHandling _mergeArrayHandling;
        private Newtonsoft.Json.Linq.MergeNullValueHandling _mergeNullValueHandling;

        public Newtonsoft.Json.Linq.MergeArrayHandling MergeArrayHandling
        {
            get => 
                this._mergeArrayHandling;
            set
            {
                if ((value < Newtonsoft.Json.Linq.MergeArrayHandling.Concat) || (value > Newtonsoft.Json.Linq.MergeArrayHandling.Merge))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._mergeArrayHandling = value;
            }
        }

        public Newtonsoft.Json.Linq.MergeNullValueHandling MergeNullValueHandling
        {
            get => 
                this._mergeNullValueHandling;
            set
            {
                if ((value < Newtonsoft.Json.Linq.MergeNullValueHandling.Ignore) || (value > Newtonsoft.Json.Linq.MergeNullValueHandling.Merge))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._mergeNullValueHandling = value;
            }
        }
    }
}

