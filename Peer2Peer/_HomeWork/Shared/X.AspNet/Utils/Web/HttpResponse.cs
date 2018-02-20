namespace System.Web
{
    public static class HttpResponseExtensions
    {
        public static void WriteLine(this HttpResponse obj, string line)
        {
            obj.Write(line.Replace(Environment.NewLine, "<br/>").AppendInCase("<br/>"));
        }

        //public static void Redirect(this HttpResponse obj, string url, UserInformation message)
        //{
        //    BasePage.Current.SetMessage(message, true);
        //    obj.Redirect(url, true);
        //}

        //public static void Transfer(this HttpServerUtility obj, string url, UserInformation message)
        //{
        //    BasePage.Current.SetMessage(message, true);
        //    obj.Transfer(X.AspNet.WebApp.Current.HomeControlUrl, false);
        //}

        public static void SetMaximumCaching(this HttpResponse obj)
        {
            TimeSpan freshness = 365.Days();
            obj.Cache.SetMaxAge(freshness);
            obj.Cache.SetExpires(DateTime.UtcNow.Add(freshness));
            obj.Cache.SetCacheability(HttpCacheability.Public);
            obj.Cache.SetLastModified(DateTime.UtcNow.Add(-30.Days()));
            obj.Cache.SetValidUntilExpires(true);
        }
    }
}
