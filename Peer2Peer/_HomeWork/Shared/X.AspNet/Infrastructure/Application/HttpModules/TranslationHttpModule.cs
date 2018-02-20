using Host.Default.Helpers.HttpFilters;
using Host.Infrastructure.Application.StreamFilters;
using X.AspNet;
using X.AspNet.Infrastructure.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Host.Infrastructure.Application.HttpModules
{
    class TranslationHttpModule : HttpModuleBase
    {
        protected override void OnInit(HttpApplication application)
        {
            application.Subscribe(h => application.PostRequestHandlerExecute += h, OnPostRequestHandlerExecute);
        }
        void OnPostRequestHandlerExecute(HttpContextBase obj)
        {
            if (obj.Response.ContentType == "text/html")
            {
                // Combine Javascript calls and Defer their execution after other content loading
                obj.Response.Filter = new JavascriptDeferringFilter( obj);
                // Translate the stream for HTML response
                obj.Response.Filter = new HtmlHttpFilter(obj.Response.Filter);
            }

        }

    }
}
