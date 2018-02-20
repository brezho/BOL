using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace X.AspNet.Infrastructure.Application
{
    public abstract class HttpHandlerBase : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    {
        public virtual bool IsReusable
        {
            get { return true; }
        }

        public abstract void ProcessRequest(HttpContext context);
    }
}
