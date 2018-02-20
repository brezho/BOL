using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace X.AspNet.Infrastructure.Application
{
    public abstract class HttpModuleBase : IHttpModule
    {
        void IHttpModule.Init(HttpApplication context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            OnInit(context);
        }

        void IHttpModule.Dispose()
        {
            OnDispose();
        }

        protected virtual void OnInit(HttpApplication application) { }

        protected virtual void OnDispose() { }
    }
}
