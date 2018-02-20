using Book.Services.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Book.Services.Client
{
    class Reader
    {
        Program host;
        HttpClientHandler handler;
        CookieContainer cookies;
        HttpClient client;
        public Reader(Program hst, string baseAddress = "http://localhost:80")
        {
            host = hst;
            handler = new HttpClientHandler();
            cookies = new CookieContainer();

            handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            handler.CookieContainer = cookies;

            // TODO: Remove this if not in development
            // Certificate should be a proper valid certificate
            handler.ServerCertificateCustomValidationCallback += (HttpRequestMessage rm, X509Certificate2 cert, X509Chain chain, SslPolicyErrors err) =>
            {
                return true;
            };

            //specify to use TLS 1.2 as default connection
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            client = new HttpClient(handler);
            client.BaseAddress = new Uri(baseAddress);
        }

        public void Handshake()
        {
            var sessionId = client.GetAsync("/").ContinueWith(x =>
            {
                x.Wait();
                var cooks = cookies.GetCookies(new Uri(client.BaseAddress.ToString() + "/"));
                return cooks["__HttpHostSessId"].Value;
            }).Result;


            // sign the SessionId with our private key
            var signedSessionId = KeyPair.Sign(host.thisPair.PrivateKey, sessionId);

            // if server has been able to verify that with his version of our public key
            // then it is our turn
            // the server will have replied with the sessionId signed with its own private key
            var authReply = client.PostAsync("/auth", new StringContent(signedSessionId)).ContinueWith(x => x.Result.Content.ReadAsStringAsync().Result).Result;

            //TODO: verify the signature is valid with what we believe is the server public key
            // NOT with "host.thisPair.PublicKey"
            if (KeyPair.Verify(host.thisPair.PublicKey, authReply, sessionId))
            {
                // if we can ve
            }
        }

        internal void ExchangeKeys()
        {
            var privateK = DHKeyExchange.GetPrivateKey();
            var publicK = DHKeyExchange.GetPublicKey(privateK);

            var content = new ByteArrayContent(publicK);
            var post = client.PostAsync("/register", content);
            var res = post.Result;

            var remotePK = res.Content.ReadAsByteArrayAsync().Result;

            var computed = DHKeyExchange.CalculateSharedKey(remotePK, privateK);

            Console.WriteLine("CLIENT VIEW:");
            Console.Write(computed.Select(X=>X.ToString("x2")).ToString(""));
            Console.WriteLine();


        }
    }
}
