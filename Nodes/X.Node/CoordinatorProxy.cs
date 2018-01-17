using System;
using System.Collections.Generic;
using System.Text;
using X.Protocol;
using X.Web;
using X.Web.Rpc;

namespace X.Node
{
    public class CoordinatorProxy : RpcClient<ICoordinator>
    {
        public CoordinatorProxy(Uri coordinatorUri) : base(HttpSocketClient.GetOne(coordinatorUri))
        {
        }
    }
}
