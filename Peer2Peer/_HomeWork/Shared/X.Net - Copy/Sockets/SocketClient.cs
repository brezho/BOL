using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using X.Net.Sockets.Internal;

namespace X.Net.Sockets
{
    public class SocketClient
    {
        public event EventHandler<ConnectedEventArgs> Connected;
        public event EventHandler<ConnectedEventArgs> Disconnected;
        IDualPort wrapper;
        EndPoint remoteEndPoint;
        ManualResetEventSlim connectionEvent;

        public SocketClient(EndPoint ep)
        {
            remoteEndPoint = ep;
            connectionEvent = new ManualResetEventSlim();
        }

        void ConnectAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success) throw new SocketException((int)e.SocketError);
            wrapper = new DualPort(e.ConnectSocket);
            wrapper.Disconnected += (sd, sargs) =>
            {
                OnDisconnected(new ConnectedEventArgs(wrapper));
            };

            connectionEvent.Set();
            OnConnected(new ConnectedEventArgs(wrapper));

        }

        protected void OnConnected(ConnectedEventArgs clientConnectedEventArgs)
        {
            if (Connected != null) Connected(this, clientConnectedEventArgs);
        }
        protected void OnDisconnected(ConnectedEventArgs clientDisconnectedEventArgs)
        {
            if (Disconnected != null) Disconnected(this, clientDisconnectedEventArgs);
        }

        public IDualPort Connect()
        {
            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            var args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = remoteEndPoint;
            args.Completed += ConnectAsyncCompleted;
            socket.ConnectAsync(args);
            connectionEvent.Wait();
            return wrapper;
        }
    }
}
