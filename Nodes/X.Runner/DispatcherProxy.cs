using System;
using System.Collections.Generic;
using System.Text;
using X.Protocol;
using X.Web;
using X.Web.Rpc;

namespace X.Runner
{
    class DispatcherProxy : RpcClient<IDispatcher>
    {
        public DispatcherProxy(Uri coordinatorUri) : base(HttpSocketClient.GetOne(coordinatorUri))
        {
        }
    }
}
