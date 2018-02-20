using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using System.Text;
using System.Net;
using System.Diagnostics;
using X.AspNet.Utils;
namespace Host.Default.Helpers.HttpFilters
{

    public static class ScriptsUtils
    {

        #region Fields

        private static Regex _FindScriptTags = new Regex(@"<script\s*src\s*=\s*""(?<url>.[^""]+)"".[^>]*>\s*</script>", RegexOptions.Compiled);

        #endregion Fields

        #region Methods

        /// <summary>
        /// Combine script references.
        /// It will replace multiple script references using one 
        /// </summary>
        /// 
        public static string AddResourceMark(string path)
        {
            return path.Contains("?") ? path.Append("&ver=" + ResourceVersion) : path.Append("?ver=" + ResourceVersion);
        }

        //public static string RemoveResourceMark(string path)
        //{
        //    return path.Contains("&ver = " + ResourceVersion) ? path.Replace("&ver=" + ResourceVersion, "") : path.Replace("?ver=" + ResourceVersion, "");
        //}

        static string ver;

        public static string ResourceVersion
        {
            get
            {
                if (ver == null) ver = File.GetLastWriteTime(typeof(ScriptsUtils).Assembly.Location).Ticks.ToString();
                return ver;
            }
        }



        public static string CombineScriptBlocks(string scripts)
        {
            if (HttpContext.Current.Application["WELLKNOWNSCRIPTS"] == null)
            {
                HttpContext.Current.Application["WELLKNOWNSCRIPTS"] = new OrderedDictionary<string, string>();
            }

            var WellKnownScripts = HttpContext.Current.Application["WELLKNOWNSCRIPTS"] as OrderedDictionary<string, string>;

           // List<UrlMapSet> sets = LoadSets(baseUrl);
            string output = scripts;

            int FirstMatchPosition = -1;
            List<string> requestedUrls = new List<string>();
            string setName = String.Empty;

            var hostName = Dns.GetHostName();
            var ofl = HttpContext.Current.Request.Url.Authority;
            var serverUrl = HttpContext.Current.Request.Url.Scheme + "://" + hostName + ":" + HttpContext.Current.Request.Url.Port; // HttpContext.Current.Request.Url.Authority;

            output = _FindScriptTags.Replace(output,new MatchEvaluator(x =>
            {
                var requestedUrl = x.Groups["url"].Value;
                if (!requestedUrl.ToLowerInvariant().Contains("js.ashx?") && !WellKnownScripts.Keys.Contains(requestedUrl))
                {
                    StringBuilder sb = new StringBuilder();
                    if (GetResource(sb, serverUrl + requestedUrl))
                    {
                        WellKnownScripts.Add(requestedUrl, sb.ToString());
                    }
                }

                if (WellKnownScripts.Keys.Contains(requestedUrl))
                {
                    if (FirstMatchPosition < 0) FirstMatchPosition = x.Index;
                    setName += "s" + WellKnownScripts.IndexOfKey(requestedUrl);
                    return string.Empty;
                }
                else 
                {
                    // not found or not able to download....
                    return x.Value;
                }
            }));

            if (FirstMatchPosition >= 0)
            {
                string newScriptTag = "<script type=\"text/javascript\" src=\"" 
                    + "/DeferredScripts.ashx?set=" 
                    + HttpUtility.UrlEncode(setName)
                    + "&ver=" + HttpUtility.UrlEncode(ResourceVersion) 
                    + "\"></script>";

                output = output.Insert(FirstMatchPosition, newScriptTag);
            }


            return output;
        }

        static HttpWebRequest CreateHttpWebRequest(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add("Accept-Encoding", "gzip");
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.MaximumAutomaticRedirections = 2;
            request.MaximumResponseHeadersLength = 4 * 1024;
            request.ReadWriteTimeout = 1 * 1000;
            request.Timeout = 5 * 1000;

            return request;
        }


        public  static bool GetResource(StringBuilder buffer, string fullUrl)
        {
            Trace.WriteLine("Making WebCall : " + fullUrl);
            string mapUrlForJS = HttpUtility.HtmlDecode(fullUrl).Replace("'", "\'");

            //buffer.Append(Environment.NewLine);
            //buffer.AppendLine("/************************");
            //buffer.AppendLine("* " + fullUrl);
            //buffer.AppendLine("************************/");
            buffer.Append(Environment.NewLine);

            try
            {
                HttpWebRequest request = CreateHttpWebRequest(fullUrl);
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            string responseContent = reader.ReadToEnd();
                            buffer.Append(responseContent);
                            buffer.Append(Environment.NewLine);
                        }
                        return true;
                    }
                    else
                    {
                        //Trace.WriteLine("");
                        //Trace.WriteLine("HTTP (" + response.StatusCode.ToString() + "): " + response.StatusDescription.ToString());
                        //Trace.WriteLine("  indexMappedFile.Url " + fullUrl);

                        buffer.Append(Environment.NewLine);
                        buffer.Append("alert(\"Cannot load script from:" + mapUrlForJS + ". Please correct mapping.\");");
                        buffer.Append(Environment.NewLine);
                    }
                }
            }
            catch (Exception x)
            {
                //Trace.WriteLine("");
                //Trace.WriteLine("XXXXXXXXXXXXXXXXXXXXX");
                //Trace.WriteLine("  indexMappedFile.Url " + fullUrl);
                x.Log();

                //buffer.Append(Environment.NewLine);
                //buffer.Append("alert(\"Cannot load script from:" + fullUrl + ".\");");
                //buffer.Append(Environment.NewLine);
            }
            return false;
        }

        #endregion Methods
    }
}