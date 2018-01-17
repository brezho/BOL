using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Helpers;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Storage;
using System.Text;
using System.Threading;
using X.Application;
using X.Node;
using X.Protocol;
using X.Web;

namespace X.Dispatcher
{
    class Dispatcher : NodeBase
    {
        DataStore Store;
        List<IRunner> RemoteRunners = new List<IRunner>();

        protected override IniReader ConfigurationReader => new IniReader("X.Dispatcher.ini");
        static void Main(params string[] args)
        {
            Thread.Sleep(1000); // allow Coordinator to start
            Bootstrapper<Dispatcher>.Run(args);
        }


        void BroadcastRemoteRunnersBut(IRunner runner, Action<IRunner> act)
        {
            var allRemote = RemoteRunners.ToArray();
            foreach (var rm in allRemote)
            {
                if (rm != runner) act(rm);
            }
        }

        protected override void OnCoordinatorConnected()
        {
            var connection = Coordinator.GetDBConnection();
            Console.WriteLine("DBConn: " + connection);
                 //var storageMode = args[0];
                 //var storageConfig = args[1];
                 //IRepositoryProvider provider = null;
                 //switch (storageMode)
                 //{
                 //    case "Archive": provider = new ArchiveRepository(new System.IO.FileInfo(storageConfig)); break;
                 //    case "FileSystem": provider = new FileSystemRepository(new System.IO.DirectoryInfo(storageConfig)); break;
                 //    case "InMemory": provider = new InMemoryRepository(); break;
                 //}
                 //Store = new DataStore(provider);
             
            Coordinator.SetDispatcherEndPointURIForRunners("ws://" + this.LocalEndpoint.ToString() + "/runner");
        }

        protected override void Configure(IHttpApplication application)
        {
            application.AddSocketHandler(x => x.Request.Path.Value == "/runner", sock =>
            {
                var runnerClient = new RunnerContext(sock);
                runnerClient.Connected += (s, a) => OnRunnerConnected(runnerClient);
            }, false);
        }

        private void OnRunnerConnected(RunnerContext runner)
        {
            RemoteRunners.Add(runner.Remote);
            runner.Disconnected += (s, a) => { RemoteRunners.Remove(runner.Remote); };
        }
    }
}
