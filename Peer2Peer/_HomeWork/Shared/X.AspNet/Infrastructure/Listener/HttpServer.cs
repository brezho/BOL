using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Host.Infrastructure.Listener
{
    public class HttpServer : IDisposable
    {
        HttpListener _listener;
        BlockingCollection<HttpListenerContext> _collection;
        Action<HttpListenerContext> _job;
        CancellationTokenSource _src;

        public HttpServer(string[] prefixes, AuthenticationSchemes schemes, Action<HttpListenerContext> onRequest)
        {
            _listener = new HttpListener();
            foreach (var prefix in prefixes) _listener.Prefixes.Add(prefix);

            _listener.AuthenticationSchemes = schemes;
            _collection = new BlockingCollection<HttpListenerContext>(100);
            _job = onRequest;
            _src = new System.Threading.CancellationTokenSource();
        }
        public void Start()
        {
            _listener.Start();
            for (int i = 0; i < 10; i++)
            {
                Task.Run(() =>
               {
                   foreach (var context in _collection.GetConsumingEnumerable())
                   {
                       _job(context);
                   }
               }, _src.Token);
            }

            var callBack = new AsyncCallback((r) =>
            {
                if (_listener.IsListening)
                {
                    var ctx = _listener.EndGetContext(r);
                    _collection.Add(ctx);
                }
            });

            Task.Run(() =>
            {
                while (!_src.IsCancellationRequested)
                {
                    try
                    { 
                    IAsyncResult result = _listener.BeginGetContext(callBack, null);
                    result.AsyncWaitHandle.WaitOne();
                        }
                    catch{
                    // exception occur on shutdown
                    };
                }
            }, _src.Token);
        }



        public void Stop()
        {
            _listener.Stop();
            _src.Cancel();
        }

        public void Dispose()
        {
            _src.Dispose();
        }
    }
}
