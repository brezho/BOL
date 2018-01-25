using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using X.Application;
using X.Web;
using X.Web.Extensions;

namespace PixiServer
{
    class Program : XCommandApp, IHttpConfiguration
    {
        static void Main(string[] args)
        {
            Bootstrapper<Program>.Run(args);
        }

        protected override void OnStart(params string[] args)
        {
            this.Run();
            base.OnStart(args);
        }

        IPEndPoint IHttpConfiguration.ListenerEndPoint => new IPEndPoint(IPAddress.Any, 80);
        TimeSpan IHttpConfiguration.SessionsTimeout => 5.Minutes();
        X509Certificate2 IHttpConfiguration.OptionalSSLCertificate => null;

        void IHttpConfiguration.Configure(IHttpApplication app)
        {
            app.MapStaticFile("/pixi", new System.IO.FileInfo("./Resources/pixi.min.js"));
            app.MapFile("/", new System.IO.FileInfo("./Resources/main.html"));
            app.VirtualDirectory("/inc", "./Resources");
        }
    }
}
