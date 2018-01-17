using System;
using System.Diagnostics;
using System.Helpers;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using X.Application;
using X.Protocol;
using X.Web;

namespace X.Node
{
    public abstract class NodeBase : XCommandApp, IHttpConfiguration
    {
        IPEndPoint _localEndpoint;
        protected IPEndPoint LocalEndpoint { get { return _localEndpoint; } }
        bool _httpsEnabled;
        X509Certificate2 _certificate;
        TimeSpan _webServerTimeout;
        Uri _coordinatorUri;

        protected ICoordinator Coordinator { get; private set; }
        protected abstract IniReader ConfigurationReader { get; }
        protected override void OnInitialize(params string[] args)
        {
            base.OnInitialize(args);
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            ReadConfig();
            var proxy = new CoordinatorProxy(_coordinatorUri);
            proxy.Connected += (s,a)=> {
                Coordinator = proxy.Remote;
                OnCoordinatorConnected();
            };
            proxy.Connect();
        }

        protected override void OnStart(params string[] args)
        {
            this.Run();
            base.OnStart(args);
        }

        private void ReadConfig()
        {
            var config = ConfigurationReader;

            _webServerTimeout = TimeSpan.FromMinutes(int.Parse(config.GetValue("WebSocketInternal", "SessionTimeOut", "5")));

            var listenOn = int.Parse(config.GetValue("WebSocketInternal", "ListensOnPort", "0"));
            if (listenOn == 0)
            {
                listenOn = (1200 + (Guid.NewGuid().GetHashCode() % 100)) * 10;
            }
            _localEndpoint = new IPEndPoint(IPAddress.Loopback, listenOn);

            _httpsEnabled = bool.Parse(config.GetValue("WebSocketInternal", "EnableHttps", "false"));
            if (_httpsEnabled)
            {
                var certLocation = config.GetValue("WebSocketInternal", "HttpsCertLocation");
                _certificate = new X509Certificate2(certLocation, "0000");
            }
            var coordEndpoint = config.GetValue("Coordinator", "Uri", "ws://localhost:8000");
            _coordinatorUri = new Uri(coordEndpoint);
        }

        protected virtual void OnCoordinatorConnected() { }
        protected abstract void Configure(IHttpApplication application);

        IPEndPoint IHttpConfiguration.ListenerEndPoint => _localEndpoint;
        TimeSpan IHttpConfiguration.SessionsTimeout => _webServerTimeout;
        X509Certificate2 IHttpConfiguration.OptionalSSLCertificate => _certificate;

        void IHttpConfiguration.Configure(IHttpApplication app)
        {
            Configure(app);
        }

    }

}
