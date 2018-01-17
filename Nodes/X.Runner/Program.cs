using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Helpers;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using X.Application;
using X.Node;
using X.Protocol;
using X.Web;
using X.Web.Extensions;
using System.Linq;
namespace X.Runner
{
    partial class Runner : NodeBase
    {
        WebAppConfig webApp;

        IDispatcher Dispatcher { get; set; }
        static void Main(params string[] args)
        {
            Thread.Sleep(2000); // allow Coordinator and Dispatcher to start
            Bootstrapper<Runner>.Run(args);
        }
        protected override void OnInitialize(params string[] args)
        {
            base.OnInitialize(args);
            webApp = new WebAppConfig(this, ConfigurationReader);
        }

        protected override void OnStart(params string[] args)
        {
            webApp.Run();
            base.OnStart(args);
        }

        protected override IniReader ConfigurationReader => new IniReader("X.Runner.ini");
        protected override void OnCoordinatorConnected()
        {
            var dbConn = Coordinator.GetDBConnection();
            var dispatcherUri = Coordinator.GetADispactcherUri();
            var proxy = new DispatcherProxy(new Uri(dispatcherUri));

            proxy.Connected += (s,a)=> OnDispatcherConnected(proxy.Remote);
            proxy.Connect();

        }

        private void OnDispatcherConnected(IDispatcher remote)
        {
            Dispatcher = remote;
        }

        protected override void Configure(IHttpApplication application)
        {
        }
    }

    class WebAppConfig : IHttpConfiguration
    {
        Runner _runner;
        public WebAppConfig(Runner runner, IniReader reader)
        {
            _runner = runner;
            SessionsTimeout = TimeSpan.FromMinutes(int.Parse(reader.GetValue("WebApp", "SessionTimeOut", "5")));

            var listenOn = int.Parse(reader.GetValue("WebApp", "ListensOnPort", "0"));
            if (listenOn == 0)
            {
                listenOn = (1200 + (Guid.NewGuid().GetHashCode() % 100)) * 10;
            }
            ListenerEndPoint = new IPEndPoint(IPAddress.Loopback, listenOn);

            bool _httpsEnabled = bool.Parse(reader.GetValue("WebApp", "EnableHttps", "false"));
            if (_httpsEnabled)
            {
                var certLocation = reader.GetValue("WebApp", "HttpsCertLocation");
                OptionalSSLCertificate = new X509Certificate2(certLocation, "0000");
            }
        }
        public IPEndPoint ListenerEndPoint { get; private set; }
        public TimeSpan SessionsTimeout { get; private set; }
        public X509Certificate2 OptionalSSLCertificate { get; private set; }

        public void Configure(IHttpApplication app)
        {
            app.Bundle("/js", MimeTypeHelper.GetMimeType("js"),
                () =>
                {
                    var di = new DirectoryInfo(@"C:\Workspaces\_HomeWork\Lamallette\X.Runner\content\js\");
                    return di.GetFiles("*.js", SearchOption.AllDirectories).Select(X => X.FullName);
                });

            app.MapFile("/", new System.IO.FileInfo("App.html"));

            app.AddSocketHandler(x => x.Request.Path.Value == "/mainSocket", sock =>
            {
                //var webClient = new RemoteClientFor<IRunnerService, IWebAppClient>(sock);
                //webClient.OnConnected += (s, a) => _runner.OnWebClientConnected(webClient);
            });
        }
    }
}
