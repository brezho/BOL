using Host.Infrastructure.Application;
using Host.Infrastructure.Listener;
using System;
using System.Linq;
using System.ComponentModel.Design;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Collections.Generic;
using X.Application;

namespace Host.Infrastructure.AspNet
{
    public class AspNet : MarshalByRefObject, IRegisteredObject
    {
        public HttpServer Server;
        public static string[] Prefixes;
        public AspNet()
        {
            var tf = HttpRuntime.TargetFramework;
            var dir = HttpRuntime.BinDirectory;
            Bootstrapper.OnBoot();
        }

        public void Configure(string[] prefixes, AuthenticationSchemes schemes)
        {
            Prefixes = prefixes;
            Server = new HttpServer(prefixes, schemes, ProcessRequest);
        }

        void IRegisteredObject.Stop(bool immediate)
        {
            Server.Stop();
        }
        public override object InitializeLifetimeService()
        {
            return null;
        }

        void ProcessRequest(HttpListenerContext x)
        {
            var toto = new AuthenticatedWorkerRequest(x);
            HttpRuntime.ProcessRequest(toto);
        }

        internal void Start()
        {

            typeof(IOnStartRoutine).Hype()
                .GetMatchingTypes(x => x.IsConcrete())
                .OrderBy(x => x.FullName)
                .GetOne<IOnStartRoutine>()
                .ForEach(x => x.OnStart());


            Server.Start();
        }

        internal void Stop()
        {
            Server.Stop();
            typeof(IOnStopRoutine).Hype()
              .GetMatchingTypes(x => x.IsConcrete())
              .OrderBy(x => x.FullName)
              .GetOne<IOnStopRoutine>()
              .ForEach(x => x.OnStop());

        }
    }
}
