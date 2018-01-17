using System;
using X.Protocol;
using X.Web;
using X.Web.Rpc;

namespace X.Coordinator
{
    public class CoordinatorContext<T> : RpcClientContext<T>, ICoordinator
        where T : class
    {
        public CoordinatorContext(IHttpSocket httpSocket) : base(httpSocket)
        {
        }

        public string GetADispactcherUri()
        {
            return "Let me find you something";
        }

        public string GetDBConnection()
        {
            return "Hello world";
        }

        string dispatcherURi;
        public void SetDispatcherEndPointURIForRunners(string uri)
        {
            dispatcherURi = uri;
            Console.WriteLine("Dispatcher @: " + dispatcherURi);
        }
    }
}