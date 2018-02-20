using System;
using System.Collections.Generic;
using System.Helpers;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Book
{
    class Config
    {
        public TimeSpan _webServerTimeout { get; private set; }
        public IPEndPoint _listenerEndPoint { get; private set; }
        public X509Certificate2 _certificate { get; private set; }
        public bool _httpsEnabled { get; private set; }
        public string _storageMode { get; private set; }
        public string _storageConfig { get; private set; }

        public static Config Read(string filePath)
        {
            var result = new Config();
            var reader = new IniReader(filePath);

            result._webServerTimeout = TimeSpan.FromMinutes(int.Parse(reader.GetValue("GB0", "SessionTimeOut", "5")));

            var listenOn = int.Parse(reader.GetValue("GB0", "ListensOnPort", "99"));
            result._listenerEndPoint = new IPEndPoint(IPAddress.Any, listenOn);

            result._httpsEnabled = bool.Parse(reader.GetValue("GB0", "EnableHttps", "false"));
            if (result._httpsEnabled)
            {
                var certLocation = reader.GetValue("GB0", "HttpsCertLocation");
                result._certificate = new X509Certificate2(certLocation, "k72kpdp");
            }
            result._storageMode = reader.GetValue("Storage", "Mode");
            result._storageConfig = reader.GetValue("Storage", "Config");
            return result;
        }
    }
}
