using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Helpers;
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
    partial class Coordinator
    {
        string _storageMode;
        string _storageConfig;

        List<IDispatcher> Dispatchers = new List<IDispatcher>();
        List<IRunner> Runners = new List<IRunner>();

        void OnRunnerConnected(IRunner runnerClient)
        {
            this.Runners.Add(runnerClient);
        }

        void OnDispatcherConnected(IDispatcher dispatcherClient)
        {
            this.Dispatchers.Add(dispatcherClient);
        }
    }
}
