using Book.Services.Client;
using Book.Services.Crypto;
using System;
using System.Diagnostics;
using System.Helpers;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using X.Application;
using X.Net.Peers;
using X.Web;

namespace Book
{
    partial class Program : XCommandApp
    {
        internal Config config { get; private set; }
        internal Http http { get; private set; }
        internal DataSource data { get; private set; }
        internal Reader reader { get; private set; }
        internal KeyPair thisPair { get; private set; }

        static void Main(string[] args)
        {
            Bootstrapper<Program>.Run(args);
        }

        protected override void OnInitialize(params string[] args)
        {
            base.OnInitialize(args);
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            config = Config.Read("Book.ini");

            if (args.Length != 0) // this is to start in 'peer' mode
            {
                config._listenerEndPoint.Port = 0;
            }

            http = Http.Setup(config);
            data = Storage.Setup(config);
            reader = new Reader(this);
        }

        protected override void OnStart(params string[] args)
        {
            http.Run();

            var version = data.Preferences.Get("version").FirstOrDefault();

            if (version == null) PrepareForFirstTimeUse();
            else PrepareForSubsequentUse();

            base.OnStart(args);
        }


        private void PrepareForSubsequentUse()
        {
            var keys = data.Preferences.Get("PKS", "PK").ToArray();
            thisPair = new KeyPair(keys[0].Value, keys[1].Value);
            reader.Handshake();
        }

        public void Handshake()
        {
        }
        private void PrepareForFirstTimeUse()
        {
            // create a pair of pub/pri keys
            thisPair = KeyPair.Create();

            data.Preferences.Add(new Preference { Key = "PK", Value = thisPair.PublicKey }, new Preference { Key = "PKS", Value = thisPair.PrivateKey });
            data.Preferences.Add(new Preference { Key = "version", Value = DateTimeOffset.UtcNow.Year.ToString() });

            // 
            reader.ExchangeKeys();

            // create a peerId
            // prepare a random port for communication
            // get in touch with GB (Gutenberg Bible)
        }
    }
}
