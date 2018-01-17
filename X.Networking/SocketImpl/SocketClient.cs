using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using X.Networking;

namespace X.Networking.SocketImpl
{
    public class SocketClient : ClientBase
    {
        Stream stream;
        Socket socket;
        protected override bool MasksOutgoingMessages => true;

        public void Connect(IPEndPoint endpoint)
        {
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();
            connectArgs.RemoteEndPoint = endpoint;
            connectArgs.Completed += (s, a) => ProcessConnect(a);
            bool willRaiseEvent = socket.ConnectAsync(connectArgs);

            if (!willRaiseEvent)
                ProcessConnect(connectArgs);
        }

        void ProcessConnect(SocketAsyncEventArgs e)
        {
            var err = e.SocketError;

            stream = new NetworkStream(socket);
            base.Initialize(stream, stream);
        }
    }
}
