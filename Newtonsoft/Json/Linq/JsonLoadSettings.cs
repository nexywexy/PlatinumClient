namespace Newtonsoft.Json.Linq
{
    using System;

    public class JsonLoadSettings
    {
        private Newtonsoft.Json.Linq.CommentHandling _commentHandling;
        private Newtonsoft.Json.Linq.LineInfoHandling _lineInfoHandling;

        public Newtonsoft.Json.Linq.CommentHandling CommentHandling
        {
            get => 
                this._commentHandling;
            set
            {
                if ((value < Newtonsoft.Json.Linq.CommentHandling.Ignore) || (value > Newtonsoft.Json.Linq.CommentHandling.Load))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._commentHandling = value;
            }
        }

        public Newtonsoft.Json.Linq.LineInfoHandling LineInfoHandling
        {
            get => 
                this._lineInfoHandling;
            set
            {
                if ((value < Newtonsoft.Json.Linq.LineInfoHandling.Ignore) || (value > Newtonsoft.Json.Linq.LineInfoHandling.Load))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._lineInfoHandling = value;
            }
        }
    }
}

