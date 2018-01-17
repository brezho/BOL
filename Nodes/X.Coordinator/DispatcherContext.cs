using System;
using System.Collections.Generic;
using System.Text;
using X.Protocol;
using X.Web;
using X.Web.Rpc;

namespace X.Coordinator
{
    class DispatcherContext : CoordinatorContext<IDispatcher>
    {
        public DispatcherContext(IHttpRemoteSocketClientContext httpSocket) : base(httpSocket)
        {
        }
    }
}
