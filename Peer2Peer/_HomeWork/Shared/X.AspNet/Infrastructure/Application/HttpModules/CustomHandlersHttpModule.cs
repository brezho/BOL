using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Host.Infrastructure.Application;
using X.AspNet;
using System.IO;

namespace X.AspNet.Infrastructure.Application.HttpModules
{
    class CustomHandlersHttpModule : HttpModuleBase
    {
        static List<HttpHandlerBase> _wellKnownHandlers = null;

        protected override void OnInit(System.Web.HttpApplication application)
        {
            base.OnInit(application);
            if (_wellKnownHandlers == null)
            {
                _wellKnownHandlers = TypeCatalog.Instance.GetMatchingTypes(typeof(HttpHandlerBase), x => x.IsConcrete())
                    .Select(x => (HttpHandlerBase)System.Reflection.FastBuilder.Create(x))
                    .ToList();
            }
            application.Subscribe(h => application.BeginRequest += h, OnBeginRequest);
        }

        void OnBeginRequest(HttpContextBase context)
        {
      //      System.Diagnostics.Trace.WriteLine("WellKnownHandlerModule - Request received:" + context.Request.Url);
            var request = context.Request;
            var fileName = VirtualPathUtility.GetFileName(request.Url.AbsolutePath);
            var ext = Path.GetExtension(fileName).Safe();
            if (string.Equals(ext, ".ashx", StringComparison.InvariantCultureIgnoreCase))
            {
                var entry = fileName.ToLower().Replace(".ashx", "");
                var internalHandler = _wellKnownHandlers.Where(x => entry.EndsWith(x.GetType().Name.ToLower())).FirstOrDefault();
                if (internalHandler != null)
                {
      //              System.Diagnostics.Trace.WriteLine("Request is forwarded to " + internalHandler.GetType().FullName);
                    context.RemapHandler(internalHandler);
                }
            }
        }
    }
}
