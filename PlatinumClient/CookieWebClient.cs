namespace PlatinumClient
{
    using System;
    using System.Net;
    using System.Runtime.CompilerServices;

    public class CookieWebClient : WebClient
    {
        public CookieWebClient()
        {
            this.CookieContainer = new System.Net.CookieContainer();
        }

        public CookieWebClient(System.Net.CookieContainer cookieContainer)
        {
            this.CookieContainer = cookieContainer;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest webRequest = base.GetWebRequest(address) as HttpWebRequest;
            if (webRequest == null)
            {
                return base.GetWebRequest(address);
            }
            webRequest.CookieContainer = this.CookieContainer;
            return webRequest;
        }

        public System.Net.CookieContainer CookieContainer { get; private set; }
    }
}

