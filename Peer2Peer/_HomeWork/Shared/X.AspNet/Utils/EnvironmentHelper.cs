using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace X.AspNet.Utils
{
    public sealed class EnvironmentHelper
    {
        public static string ApplicationName
        {
            get
            {
                return GetApplicationNameDelegate();
            }
        }
        public static Func<string> GetApplicationNameDelegate = GetName;


        private static string GetName()
        {
            return (System.Web.Hosting.HostingEnvironment.IsHosted)
                ? System.Web.Hosting.HostingEnvironment.SiteName
                : Assembly.GetEntryAssembly().GetName().Name;
        }

        public static bool InCassini
        {
            get
            {
                return System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName.StartsWith("WebDev.WebServer");
            }
        }

        public static string TryGetMachineName()
        {
            return TryGetMachineName(null);
        }

        public static string TryGetMachineName(HttpContext context)
        {
            return TryGetMachineName(context, null);
        }

        /// <remarks>
        /// If <paramref name="unknownName"/> is a null reference then this
        /// method will still return an empty string.
        /// </remarks>

        public static string TryGetMachineName(HttpContext context, string unknownName)
        {
            //
            // System.Web.HttpServerUtility.MachineName and 
            // System.Environment.MachineName require different permissions.
            // Try the former then the latter...chances are higher to have
            // permissions for the former.
            //

            if (context != null)
            {
                try
                {
                    return context.Server.MachineName;
                }
                catch (HttpException)
                {
                    // Yes, according to docs, HttpServerUtility.MachineName
                    // throws HttpException on failing to obtain computer name.
                }
                catch (SecurityException)
                {
                    // A SecurityException may occur in certain, possibly 
                    // user-modified, Medium trust environments.
                }
            }

            try
            {
                return System.Environment.MachineName;
            }
            catch (SecurityException)
            {
                // A SecurityException may occur in certain, possibly 
                // user-modified, Medium trust environments.
            }

            return unknownName.Safe();
        }

        private EnvironmentHelper() { }
    }
}
