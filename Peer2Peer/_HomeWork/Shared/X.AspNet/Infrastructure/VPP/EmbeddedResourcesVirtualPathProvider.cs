using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Caching;
using System.Collections;
using System.Reflection;
using System.Web;

namespace Host.Infrastructure.VPP
{
    public delegate string VirtualPathDehydrationDelegate(string virtualPath);
    public class EmbeddedResourcesVirtualPathProvider : VirtualPathProvider
    {
        public override bool FileExists(string virtualPath)
        {
            var result = ((GetResource(virtualPath) != null) || base.FileExists(virtualPath));
            //if (virtualPath.ToLowerInvariant().Contains("stream"))
            //{
            //    System.Diagnostics.Trace.WriteLine("EmbeddedResourcesVirtualPathProvider called...");
            //    System.Diagnostics.Trace.WriteLine("Embedded: " + (GetResource(virtualPath) != null).ToString());
            //    System.Diagnostics.Trace.WriteLine("Physical: " + base.FileExists(virtualPath));
            //    System.Diagnostics.Trace.WriteLine("filename:" + VirtualPathUtility.ToAbsolute(virtualPath));
            //    System.Diagnostics.Trace.WriteLine("Exists:" + System.IO.File.Exists(VirtualPathUtility.ToAbsolute(virtualPath)).ToString());
            //}

            //if (!result)
            //{
            //    System.Diagnostics.Trace.WriteLine("NO SUCESS VPP - " + virtualPath + " - " + AssemblyScanner.AllResources.Count + " registered items");
            //}
            //else System.Diagnostics.Trace.WriteLine("SUCESS VPP - " + virtualPath);

            return result;
        }

        public static EmbeddedResource GetResource(string virtualPath)
        {
            virtualPath = VirtualPathUtility.ToAppRelative(virtualPath);
            virtualPath = X.AspNet.WebApp.Current.VirtualPathShortener(virtualPath);
            return AssemblyScanner.GetResource(virtualPath);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            var entry = GetResource(virtualPath);
            return (entry != null) ? entry.GetFile() : base.GetFile(virtualPath);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            var entry = GetResource(virtualPath);
            return (entry != null) ? entry.GetCacheDependency() : base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        //internal static void ForceSelfRegistration()
        //{
        //    var internalVPP = new EmbeddedResourcesVirtualPathProvider();
        //    // we get the current instance of HostingEnvironment class. We can't create a new one
        //    // because it is not allowed to do so. An AppDomain can only have one HostingEnvironment
        //    // instance.
        //    HostingEnvironment hostingEnvironmentInstance = (HostingEnvironment)typeof(HostingEnvironment).InvokeMember("_theHostingEnvironment", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null);


        //    System.Diagnostics.Trace.Write("Attempt to Force VPP SelfRegistration... ");

        //    if (hostingEnvironmentInstance == null)
        //    {
        //        System.Diagnostics.Trace.WriteLine("Failed");

        //        return;
        //    }


        //    // we get the MethodInfo for RegisterVirtualPathProviderInternal method which is internal
        //    // and also static.
        //    MethodInfo mi = typeof(HostingEnvironment).GetMethod("RegisterVirtualPathProviderInternal", BindingFlags.NonPublic | BindingFlags.Static);
        //    if (mi == null) return;

        //    System.Diagnostics.Trace.WriteLine("Success");
        //    // finally we invoke RegisterVirtualPathProviderInternal method with one argument which
        //    // is the instance of our own VirtualPathProvider.
        //    mi.Invoke(hostingEnvironmentInstance, new object[] { (VirtualPathProvider)internalVPP });
        //}
    }
}
