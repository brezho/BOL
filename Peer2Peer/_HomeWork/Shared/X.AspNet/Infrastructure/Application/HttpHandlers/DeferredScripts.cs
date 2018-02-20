using X.AspNet.Utils;
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
    public class DeferredScripts : HttpHandlerBase
    {
        static string SetCacheKey = "Scripts_Set_";
        public override void ProcessRequest(HttpContext context)
        {
            var setName = HttpContext.Current.Request["set"];
            var output = string.Empty;

            StringBuilder buffer = new StringBuilder();
            if (context.Cache[SetCacheKey + setName] == null)
            {
                var indicesRequesteds = setName.Split("s").Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToInt());
                var WellKnownScripts = HttpContext.Current.Application["WELLKNOWNSCRIPTS"] as OrderedDictionary<string, string>;

                var loadedScripts = new List<string>();

                indicesRequesteds.ForEach(x =>
                    {
                        loadedScripts.Add(WellKnownScripts.Keys.ElementAt(x));
                        buffer.Append(Environment.NewLine);
                        buffer.Append(WellKnownScripts[x]);
                        buffer.Append(Environment.NewLine);
                    });

                foreach (var script in loadedScripts)
                {
                    buffer.Append(Environment.NewLine);
                    buffer.Append(@"
                            if(typeof(Sys)!=='undefined') Array.add(Sys._ScriptLoader._getLoadedScripts(), '" + script + @"'); 
                            if( !window._combinedScripts ) { window._combinedScripts = []; } 
                            window._combinedScripts.push('" + script + @"');");
                    buffer.Append(Environment.NewLine);
                }

                output = buffer.ToString();
                context.Cache.Insert(SetCacheKey + setName, output);
            }
            else
            {
                output = context.Cache[SetCacheKey + setName] as string;
            }

            byte[] encodedBytes;

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
                        writer.Write(output);
                    }
                    encodedBytes = stream.ToArray();
                    context.Response.AddHeader("Content-encoding", "gzip");
                }
            }
            else if (compressionType == "deflate")
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(new DeflateStream(stream, CompressionMode.Compress), Encoding.UTF8))
                    {
                        writer.Write(output);
                    }
                    encodedBytes = stream.ToArray();
                    context.Response.AddHeader("Content-encoding", "deflate");
                }
            }
            else
            {
                context.Response.ContentType = "text/javascript";
                encodedBytes = context.Request.ContentEncoding.GetBytes(output);
                context.Response.ContentEncoding = context.Request.ContentEncoding;
            }

            context.Response.AppendHeader("Content-Length", encodedBytes.Length.ToString());
            context.Response.OutputStream.Write(encodedBytes, 0, encodedBytes.Length);
            context.Response.SetMaximumCaching();

            context.Response.Flush();
        }

        public override bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
