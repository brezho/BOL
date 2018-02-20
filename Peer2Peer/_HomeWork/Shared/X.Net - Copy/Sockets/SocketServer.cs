using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Helpers;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using X.Net.Sockets.Internal;

namespace X.Net.Sockets
{
    public class SocketServer
    {
        public event EventHandler<ConnectedEventArgs> ClientConnected;
        public event EventHandler<ConnectedEventArgs> ClientDisconnected;
        List<IDualPort> connectedClients = new List<IDualPort>();

        IPEndPoint endPoint;

        public IEnumerable<IDualPort> Clients
        {
            get { return connectedClients; }
        }

        public SocketServer(IPEndPoint endpoint)
        {
            endPoint = endpoint;
        }

        public void Listen()
        {
            var s = new Socket(SocketType.Stream, ProtocolType.Tcp);
            s.Bind(endPoint);
            s.Listen(1000);
            s.BeginAccept(AcceptClient, s);
        }

        void AcceptClient(IAsyncResult ar)
        {
            var localServerSocket = ar.AsyncState as Socket;
            var remoteClientSocket = localServerSocket.EndAccept(ar);

            // Accepting new connection straight away
            localServerSocket.BeginAccept(AcceptClient, localServerSocket);

            var mgr = new DualPort(remoteClientSocket);
            mgr.Disconnected += (dSender, dArgs) =>
            {
               if(connectedClients.Contains(mgr)) connectedClients.Remove(mgr);
                OnClientDisconnected(new ConnectedEventArgs(mgr));
            };
            connectedClients.Add(mgr);
            OnClientConnected(new ConnectedEventArgs(mgr));
        }

        protected void OnClientConnected(ConnectedEventArgs connectedEventArgs)
        {
            if (ClientConnected != null)
            {
                ClientConnected(this, connectedEventArgs);
            }
        }
        protected void OnClientDisconnected(ConnectedEventArgs connectedEventArgs)
        {
            if (ClientConnected != null)
            {
                ClientConnected(this, connectedEventArgs);
            }
        }
    }
}
