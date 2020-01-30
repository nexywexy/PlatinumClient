namespace PlatinumClient
{
    using Microsoft.CSharp.RuntimeBinder;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq.Expressions;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;

    internal class API
    {
        public static string API_PATH = "https://platinumcheats.net/client/";
        public static CookieWebClient client = new CookieWebClient();
        public static HttpClient httpClient = new HttpClient();

        [return: Dynamic]
        public static object authenticateClassic(string username, string password) => 
            JsonConvert.DeserializeObject(requestClassicAuthentication(username, password));

        public static PasswordlessResult authenticatePasswordless()
        {
            PasswordlessResult result = new PasswordlessResult();
            if (KeyManager.hasPrivateKey())
            {
                string str = requestPasswordlessAuthentication(KeyManager.getEncodedPrivateKey());
                if (str == null)
                {
                    result.authSuccess = false;
                    return result;
                }
                object obj2 = JsonConvert.DeserializeObject(str);
                if (<>o__5.<>p__2 == null)
                {
                    CSharpArgumentInfo[] argumentInfo = new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) };
                    <>o__5.<>p__2 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(API), argumentInfo));
                }
                if (<>o__5.<>p__1 == null)
                {
                    CSharpArgumentInfo[] argumentInfo = new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null) };
                    <>o__5.<>p__1 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof(API), argumentInfo));
                }
                if (<>o__5.<>p__0 == null)
                {
                    CSharpArgumentInfo[] argumentInfo = new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) };
                    <>o__5.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "success", typeof(API), argumentInfo));
                }
                if (<>o__5.<>p__2.Target(<>o__5.<>p__2, <>o__5.<>p__1.Target(<>o__5.<>p__1, <>o__5.<>p__0.Target(<>o__5.<>p__0, obj2), null)))
                {
                    result.authSuccess = true;
                    if (<>o__5.<>p__4 == null)
                    {
                        <>o__5.<>p__4 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(bool), typeof(API)));
                    }
                    if (<>o__5.<>p__3 == null)
                    {
                        CSharpArgumentInfo[] argumentInfo = new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) };
                        <>o__5.<>p__3 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "guard", typeof(API), argumentInfo));
                    }
                    result.guard = <>o__5.<>p__4.Target(<>o__5.<>p__4, <>o__5.<>p__3.Target(<>o__5.<>p__3, obj2));
                    return result;
                }
                result.authSuccess = false;
                return result;
            }
            result.authSuccess = false;
            return result;
        }

        public static bool checkClientHash()
        {
            bool flag;
            using (MD5 md = MD5.Create())
            {
                using (FileStream stream = System.IO.File.OpenRead(Assembly.GetEntryAssembly().Location))
                {
                    string str = BitConverter.ToString(md.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
                    string str2 = getLatestClientHash();
                    if (str2 == null)
                    {
                        return true;
                    }
                    flag = str.ToLower().Equals(str2.ToLower());
                }
            }
            return flag;
        }

        public static string getDashboard(long id) => 
            client.DownloadString(API_PATH + "dashboard/" + id);

        public static string getLatestClientHash()
        {
            try
            {
                object obj2 = JsonConvert.DeserializeObject(client.DownloadString(API_PATH + "clientHash"));
                if (<>o__9.<>p__1 == null)
                {
                    <>o__9.<>p__1 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof(string), typeof(API)));
                }
                if (<>o__9.<>p__0 == null)
                {
                    CSharpArgumentInfo[] argumentInfo = new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) };
                    <>o__9.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "md5", typeof(API), argumentInfo));
                }
                return <>o__9.<>p__1.Target(<>o__9.<>p__1, <>o__9.<>p__0.Target(<>o__9.<>p__0, obj2));
            }
            catch (Exception exception)
            {
                new MaterialMessageBox("We couldn't connect the Platinum Cheats servers.  Check your firewall and try again later. (" + exception.Message + ")", "Error", MessageBoxIcon.Hand).ShowDialog();
                return null;
            }
        }

        public static Subs getSubs()
        {
            try
            {
                return JsonConvert.DeserializeObject<Subs>(client.DownloadString(API_PATH + "subs"));
            }
            catch (Exception)
            {
                return new Subs();
            }
        }

        private static string requestClassicAuthentication(string username, string password)
        {
            try
            {
                NameValueCollection data = new NameValueCollection();
                data.Add("username", username);
                data.Add("password", password);
                byte[] bytes = client.UploadValues(API_PATH + "login", data);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception exception)
            {
                new MaterialMessageBox("We couldn't connect the Platinum Cheats servers.  Check your firewall and try again later. (" + exception.Message + ")", "Error", MessageBoxIcon.Hand).ShowDialog();
                Application.Exit();
            }
            return null;
        }

        private static string requestPasswordlessAuthentication(string privkey)
        {
            try
            {
                NameValueCollection data = new NameValueCollection();
                data.Add("privkey", privkey);
                byte[] bytes = client.UploadValues(API_PATH + "privkeyAuth", data);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool submitGuardCode(string code)
        {
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            object obj2 = JsonConvert.DeserializeObject(client.UploadString(API_PATH + "guardSubmitCode", "code=" + code));
            if (<>o__11.<>p__1 == null)
            {
                <>o__11.<>p__1 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(bool), typeof(API)));
            }
            if (<>o__11.<>p__0 == null)
            {
                CSharpArgumentInfo[] argumentInfo = new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) };
                <>o__11.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "result", typeof(API), argumentInfo));
            }
            return <>o__11.<>p__1.Target(<>o__11.<>p__1, <>o__11.<>p__0.Target(<>o__11.<>p__0, obj2));
        }

        public static void submitPlatformIDs()
        {
            try
            {
                string str = JsonConvert.SerializeObject(PlatformAccumulators.accumulate());
                NameValueCollection data = new NameValueCollection();
                data.Add("data", str);
                client.UploadValues(API_PATH + "submitPlatformIDs", data);
            }
            catch (Exception)
            {
            }
        }

        public static void submitTelemetryReport(TelemetryReport report, TelemetryDialog dialog)
        {
            HttpClientHandler handler1 = new HttpClientHandler {
                CookieContainer = API.client.CookieContainer
            };
            using (HttpClientHandler handler = handler1)
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    MultipartFormDataContent content = new MultipartFormDataContent();
                    content.Add(new ByteArrayContent(report.contents), "contents", "data.dat");
                    content.Add(new StringContent(report.contentType.ToString()), "contentType");
                    content.Add(new StringContent(report.type), "type");
                    client.PostAsync(API_PATH + "submitTelemetryReport", content).Wait();
                }
            }
        }

        [CompilerGenerated]
        private static class <>o__11
        {
            public static CallSite<Func<CallSite, object, object>> <>p__0;
            public static CallSite<Func<CallSite, object, bool>> <>p__1;
        }

        [CompilerGenerated]
        private static class <>o__5
        {
            public static CallSite<Func<CallSite, object, object>> <>p__0;
            public static CallSite<Func<CallSite, object, object, object>> <>p__1;
            public static CallSite<Func<CallSite, object, bool>> <>p__2;
            public static CallSite<Func<CallSite, object, object>> <>p__3;
            public static CallSite<Func<CallSite, object, bool>> <>p__4;
        }

        [CompilerGenerated]
        private static class <>o__9
        {
            public static CallSite<Func<CallSite, object, object>> <>p__0;
            public static CallSite<Func<CallSite, object, string>> <>p__1;
        }
    }
}

