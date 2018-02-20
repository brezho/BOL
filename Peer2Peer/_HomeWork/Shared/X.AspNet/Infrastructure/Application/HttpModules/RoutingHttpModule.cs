using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Host.Infrastructure;
using System.IO;
using X.AspNet.Infrastructure.Application;
using X.AspNet;
using System.Web.Hosting;

namespace Host.Infrastructure.Application.HttpModules
{
    public class RoutingHttpModule : HttpModuleBase
    {
        protected override void OnInit(HttpApplication application)
        {
            base.OnInit(application);
            application.Subscribe(h => application.BeginRequest += h, OnBeginRequest);
        }

        void OnBeginRequest(HttpContextBase context)
        {
            Log("RoutingModule - Request received:" + context.Request.Url);

            var request = context.Request;
            var fileName = VirtualPathUtility.GetFileName(request.Url.AbsolutePath);
            var ext = Path.GetExtension(fileName);

            // HANDLING OF EMBEDDED .ASCX CONTROLS
            if (String.IsNullOrEmpty(ext))
            {
                var virtualPath = VirtualPathUtility.ToAppRelative(context.Request.Url.AbsolutePath);
                Log("Looking for: " + virtualPath + ".ascx...");

                if (HostingEnvironment.VirtualPathProvider.FileExists(virtualPath + ".ascx"))
                {
                    context.Items["__ctrl"] = virtualPath + ".ascx";
                    Log(" Found!:" + virtualPath + ".ascx");
                }
                else
                {
                    Log(" not found");
                    Log("Looking for: " + virtualPath + "default.ascx...");
                    if (HostingEnvironment.VirtualPathProvider.FileExists(virtualPath + "default.ascx"))
                    {
                        context.Items["__ctrl"] = virtualPath + "default.ascx";
                        Log(" Found!:" + virtualPath + "default.ascx");
                    }
                    else
                    {
                        Log(" not found");
                        Log("Looking for: " + virtualPath + "/default.ascx...");
                        if (HostingEnvironment.VirtualPathProvider.FileExists(virtualPath + "/default.ascx"))
                        {
                            context.Items["__ctrl"] = virtualPath + "/default.ascx";
                            Log(" Found! :" + virtualPath + "/default.ascx");
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                context.RewritePath("~/GProd/Default.aspx", false);
            }
        }

        void Log(string s)
        {
            //System.Diagnostics.Trace.WriteLine(s);
        }
    }
}
