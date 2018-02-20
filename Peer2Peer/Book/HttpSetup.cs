using Book.Services.Crypto;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using X.Web;
using X.Web.Extensions;

namespace Book
{
    class Http : IHttpConfiguration
    {
        Config _config;

        Http(Config config)
        {
            this._config = config;
        }

        public static Http Setup(Config config)
        {
            return new Http(config);
        }

        public IPEndPoint ListenerEndPoint => _config._listenerEndPoint;

        public TimeSpan SessionsTimeout => _config._webServerTimeout;

        public X509Certificate2 OptionalSSLCertificate => _config._certificate;

        public void Configure(IHttpApplication app)
        {
            app.Get("/", x => x.Response.Write("Hello"));

            app.Post("/register", x =>
            {
                var remotePK = x.Request.Body.GetBytes();

                var privateK = DHKeyExchange.GetPrivateKey();
                var publicK = DHKeyExchange.GetPublicKey(privateK);

                var computed = DHKeyExchange.CalculateSharedKey(remotePK, privateK);

                Console.WriteLine("SERVER VIEW:");
                Console.Write(computed.Select(X => X.ToString("x2")).ToString(""));
                Console.WriteLine();


                // store somewhere info about this peer....

                // and return
                x.Response.Body.Write(publicK, 0, publicK.Length);
            });
        }
    }
}
