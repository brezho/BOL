using Host.Infrastructure.VPP;
using X.AspNet.Services.Multilingual;
using X.AspNet.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace X.AspNet.Infrastructure.Application.HttpHandlers
{
    public class Css : HttpHandlerBase
    {
        static string AllScripts = null;

        public override void ProcessRequest(HttpContext context)
        {
            var browserBasedUrl = UrlHelper.Application.AppendInCase("/").PrependInCase("/");
            if (AllScripts == null)
            {
                StringBuilder sb = new StringBuilder();

                var assembliesDefiningAnApplication = TypeCatalog.Instance.GetMatchingTypes(typeof(IWebApplication))
                        .Select(x => x.Assembly)
                        .Distinct().ToList();

                AssemblyScanner
                    .AllResources.Where(x => x.Name.EndsWith(".css") && (x.Name.StartsWith("Host") || assembliesDefiningAnApplication.Contains(x.Assembly)) )
                    .OrderBy(x => Path.GetFileName(x.VirtualPath))
                    .Select(x => x.Assembly.GetResourceContent(x.Name))
                    .ForEach(x => sb.Append(Environment.NewLine + Translations.Translate(x).Replace("~/", browserBasedUrl) + Environment.NewLine));

                AllScripts = sb.ToString();
            }

            context.Response.ContentType = "text/css";
            var encodedBytes = context.Request.ContentEncoding.GetBytes(AllScripts);
            context.Response.ContentEncoding = context.Request.ContentEncoding;
            context.Response.AppendHeader("Content-Length", encodedBytes.Length.ToString());
            context.Response.OutputStream.Write(encodedBytes, 0, encodedBytes.Length);
            context.Response.SetMaximumCaching();

            context.Response.Flush();
        }
    }
}
