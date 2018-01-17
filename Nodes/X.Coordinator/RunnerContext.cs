using System;
using System.Collections.Generic;
using System.Text;
using X.Protocol;
using X.Web;
using X.Web.Rpc;

namespace X.Coordinator
{
    public class RunnerContext : CoordinatorContext<IRunner>
    {
        public RunnerContext(IHttpRemoteSocketClientContext httpSocket) : base(httpSocket)
        {
        }
    }
}
