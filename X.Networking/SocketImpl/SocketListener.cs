using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using X.Networking;

namespace X.Networking.SocketImpl
{
    public class SocketListener : ListenerBase
    {
        Socket listenSocket;
        ConcurrentBag<SocketAsyncEventArgs> acceptPool;
        public SocketListener(IPEndPoint endpoint)
        {
            acceptPool = new ConcurrentBag<SocketAsyncEventArgs>();
            listenSocket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(endpoint);
        }

        public SocketAsyncEventArgs Recycle()
        {
            SocketAsyncEventArgs item;
            if (acceptPool.TryTake(out item)) return item;
            item = new SocketAsyncEventArgs();
            item.Completed += (x, a) => ProcessAccept(a);
            return item;
        }
        public void Recycle(SocketAsyncEventArgs args)
        {
            acceptPool.Add(args);
        }

        public override void Start()
        {
            listenSocket.Listen(50);
            StartAccept();
        }
        void StartAccept()
        {
            SocketAsyncEventArgs acceptEventArg = Recycle();
            bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
        }
        void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            StartAccept();
            if (acceptEventArgs.SocketError != SocketError.Success)
            {
                //acceptEventArgs.AcceptSocket.Close();
                acceptEventArgs.AcceptSocket.Shutdown(SocketShutdown.Both);
                Recycle(acceptEventArgs);
                return;
            }

            var channel = new NetworkStream(acceptEventArgs.AcceptSocket);

            acceptEventArgs.AcceptSocket = null;
            Recycle(acceptEventArgs);

            if (HandShake(channel)) base.Accept(channel, channel);
            else channel.Dispose();
        }

        protected virtual bool HandShake(NetworkStream stream)
        {
            return true;
        }

        public override void Stop()
        {
            try
            {
                listenSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
            }
            listenSocket.Dispose();
           // listenSocket.Close();
        }
    }
}
