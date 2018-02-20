using Host.Infrastructure.VPP;
using X.AspNet.Services.Multilingual;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace X.AspNet.Infrastructure.Application.HttpHandlers
{
    public class Js : HttpHandlerBase
    {
        static string AllScripts = null;
        
        public override void ProcessRequest(HttpContext context)
        {
            if (AllScripts == null)
            {
                StringBuilder sb = new StringBuilder();

                var assembliesDefiningAnApplication = TypeCatalog.Instance.GetMatchingTypes(typeof(IWebApplication))
                        .Select(x => x.Assembly)
                        .Distinct().ToList();

                var res = AssemblyScanner
                    .AllResources
                    .Where(x => x.Name.EndsWith(".js") && (x.Name.StartsWith("Host") || assembliesDefiningAnApplication.Contains(x.Assembly)))
                    .OrderBy(x => x.Name);

                res
                    .Select(x => x.Assembly.GetResourceContent(x.Name))
                    .ForEach(x => sb.Append(Environment.NewLine + Translations.Translate(x) + Environment.NewLine));

                AllScripts = sb.ToString();
            }
            byte[] buffer;


            // COMPRESSION
            var encodingTypes = context.Request.Headers["Accept-Encoding"];
            string compressionType = "none";
            if (!string.IsNullOrEmpty(encodingTypes))
            {
                encodingTypes = encodingTypes.ToLower();
                if (context.Request.Browser.Browser == "IE")
                {
                    if (context.Request.Browser.MajorVersion < 6) compressionType = "none";
                    else if (
                        context.Request.Browser.MajorVersion == 6
                        && !string.IsNullOrEmpty(context.Request.ServerVariables["HTTP_USER_AGENT"])
                        && context.Request.ServerVariables["HTTP_USER_AGENT"].Contains("EV1"))
                        compressionType = "none";
                }
                if ((encodingTypes.Contains("gzip") || encodingTypes.Contains("x-gzip") || encodingTypes.Contains("*")))
                    compressionType = "gzip";
                else if (encodingTypes.Contains("deflate")) compressionType = "deflate";
            }

            if (compressionType == "gzip")
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(new GZipStream(stream, CompressionMode.Compress), Encoding.UTF8))
                    {
                        writer.Write(AllScripts);
                    }
                    buffer = stream.ToArray();
                    context.Response.AddHeader("Content-encoding", "gzip");
                }
            }
            else if (compressionType == "deflate")
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(new DeflateStream(stream, CompressionMode.Compress), Encoding.UTF8))
                    {
                        writer.Write(AllScripts);
                    }
                    buffer = stream.ToArray();
                    context.Response.AddHeader("Content-encoding", "deflate");
                }
            }
            else
            {
                context.Response.ContentType = "text/javascript";
                buffer = context.Request.ContentEncoding.GetBytes(AllScripts);
                context.Response.ContentEncoding = context.Request.ContentEncoding;
            }

            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.AppendHeader("Content-Length", buffer.Length.ToString());
            context.Response.SetMaximumCaching();

            context.Response.Flush();
        }

    }
}
