using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using X.Net.Sockets.Active;

namespace X.Net.Sockets
{
    public class SocketClient : IClient
    {
        public event EventHandler<ConnectedEventArgs> Connected;
        public event EventHandler<DisconnectedEventArgs> Disconnected;
        public event EventHandler<DataReceivedEventArgs> DataReceived;
        EndPoint remoteEndPoint;
        EndPoint localEndPoint;
        IActiveSocket serverInterface;
        public EndPoint EndPoint
        {
            get
            {
                return localEndPoint;

            }
        }

        public SocketClient(EndPoint destinationEndpoint)
        {
            remoteEndPoint = destinationEndpoint;
        }

        public void Connect()
        {
            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            localEndPoint = socket.LocalEndPoint;
            var args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = remoteEndPoint;
            args.Completed += ConnectAsyncCompleted;
            socket.ConnectAsync(args);
        }

        public void Send(byte[] data)
        {
            serverInterface.Send(data);
        }

        void ConnectAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success) throw new SocketException((int)e.SocketError);
            serverInterface = new ActiveSocket(e.ConnectSocket);
            serverInterface.Disconnected += (dsnd, dargs) =>
            {
                if (Disconnected != null) Disconnected(this, new DisconnectedEventArgs(this));
            };
            serverInterface.DataReceived += (prsnd, prargs) =>
            {
                if (DataReceived != null) DataReceived(this, new DataReceivedEventArgs(this, prargs.Data));
            };

            if (Connected != null) Connected(this, new ConnectedEventArgs(this));
        }
    }
}
