using System;
using System.Collections.Generic;
using System.Text;
using X.Protocol;
using X.Web;
using X.Web.Rpc;

namespace X.Dispatcher
{
    public class RunnerContext : RpcClientContext<IRunner>, IDispatcher
    {
        public RunnerContext(IHttpRemoteSocketClientContext httpSocket) : base(httpSocket)
        {

        }
    }
}
