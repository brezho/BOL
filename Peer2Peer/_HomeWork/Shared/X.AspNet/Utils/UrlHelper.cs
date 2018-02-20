using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace X.AspNet.Utils
{
    public class UrlHelper
    {
        private static readonly char[] ToRemove = "/".ToCharArray();

        /// <summary>
        /// Gets the application root.
        /// </summary>
        /// <value>The application root.</value>
        public static string ApplicationRoot
        {
            get { return (Application.Length > 0) ? "/" + Application : ""; }
        }

        /// <summary>
        /// Gets the scheme name.
        /// </summary>
        /// <value>A <c>System.String</c> that contains the scheme for this URL, converted to lowercase.</value>
        public static string Scheme
        {
            get { return HttpContext.Current.Request.Url.Scheme; }
        }

        /// <summary>
        /// Gets the host component.
        /// </summary>
        /// <value>
        /// A <c>System.String</c> that contains the host name. This is usually the DNS host
        /// name or IP address of the server.
        /// </value>
        public static string Host
        {
            get { return HttpContext.Current.Request.Url.Host; }
        }

        /// <summary>
        /// Gets the port number of the current URL.
        /// </summary>
        /// <value>An <c>System.Int32</c> value that contains the port number for the current URL.</value>
        public static string Port
        {
            get
            {
                return HttpContext.Current.Request.Url.Port.ToString();
            }
        }

        /// <summary>
        /// Gets the ASP.NET application's virtual application root path on the server.
        /// The / character is removed (left and right trim).
        /// </summary>
        /// <value>The application.</value>
        public static string Application
        {
            get { return HttpRuntime.AppDomainAppVirtualPath.TrimStart(ToRemove).TrimEnd(ToRemove); }
        }

        public static string Directories
        {
            get
            {
                var result = HttpContext.Current.Request.Path.Remove(0, Application.Length);
                if (File.Length > 0) result = result.Replace(File, "");
                return result.TrimStart(ToRemove).TrimEnd(ToRemove);
            }
        }


        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <value>The file.</value>
        public static string File
        {
            get
            {
                var result =
                    HttpContext.Current.Request.Path.Substring(HttpContext.Current.Request.Path.LastIndexOf("/") + 1);
                return result.Contains(".") ? result : "";
            }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public static string Parameters
        {
            get { return HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString.ToString()); }
        }

        /// <summary>
        /// Determines whether the specified url is dangerous (CrossSiteScriptingValidation)
        /// </summary>
        /// <param name="url">The url</param>
        /// <returns>
        /// 	<c>true</c> if [is dangerous URL]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDangerousUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            url = url.Trim();

            var length = url.Length;
            if (((((length > 4) && ((url[0] == 'h') || (url[0] == 'H'))) && ((url[1] == 't') || (url[1] == 'T'))) &&
                 (((url[2] == 't') || (url[2] == 'T')) && ((url[3] == 'p') || (url[3] == 'P')))) &&
                ((url[4] == ':') || (((length > 5) && ((url[4] == 's') || (url[4] == 'S'))) && (url[5] == ':'))))
                return false;

            return url.IndexOf(':') != -1;
        }
    }
}
