using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Helpers;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using X.Application;
using X.Protocol;
using X.Web;
using X.Web.Extensions;

namespace X.Coordinator
{
    partial class Coordinator : XCommandApp, IHttpConfiguration
    {
        IPEndPoint _listenerEndPoint;
        bool _httpsEnabled;
        X509Certificate2 _certificate;
        TimeSpan _webServerTimeout;

        public IPEndPoint ListenerEndPoint => _listenerEndPoint;
        public TimeSpan SessionsTimeout => _webServerTimeout;
        public X509Certificate2 OptionalSSLCertificate => _certificate;


        static void Main(string[] args)
        {
            Bootstrapper<Coordinator>.Run(args);
        }
        protected override void OnStart(params string[] args)
        {
            this.Run();
            base.OnStart(args);
        }

        protected override void OnInitialize(params string[] args)
        {
            base.OnInitialize(args);
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            ReadConfig();
        }

        private void ReadConfig()
        {
            var config = new IniReader("X.Coordinator.ini");

            _webServerTimeout = TimeSpan.FromMinutes(int.Parse(config.GetValue("WebSocketInternal", "SessionTimeOut", "5")));

            var listenOn = int.Parse(config.GetValue("WebSocketInternal", "ListensOnPort", "0"));
            _listenerEndPoint = new IPEndPoint(IPAddress.Loopback, listenOn);

            _httpsEnabled = bool.Parse(config.GetValue("WebSocketInternal", "EnableHttps", "false"));
            if (_httpsEnabled)
            {
                var certLocation = config.GetValue("WebSocketInternal", "HttpsCertLocation");
                _certificate = new X509Certificate2(certLocation, "0000");
            }

            _storageMode = config.GetValue("Storage", "Mode");
            _storageConfig = config.GetValue("Storage", "Config");
        }

        public void Configure(IHttpApplication app)
        {
            app.AddSocketHandler(x => x.Request.Path.Value == "/dispatcher", sock =>
            {
                OnDispatcherConnected(new DispatcherContext(sock).Remote);
            }, false);

            app.AddSocketHandler(x => x.Request.Path.Value == "/runner", sock =>
            {
                OnRunnerConnected(new RunnerContext(sock).Remote);
            }, false);
        }

    }
}
