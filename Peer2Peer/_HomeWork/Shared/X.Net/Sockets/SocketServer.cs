using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Helpers;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace X.Net.Sockets
{
    public class SocketServer
    {
        public event EventHandler<ConnectedEventArgs> ClientConnected;
        public event EventHandler<DisconnectedEventArgs> ClientDisconnected;
        public event EventHandler<DataReceivedEventArgs> DataReceived;
        List<RemoteClient> connectedClients = new List<RemoteClient>();

        IPEndPoint endPoint;

        public SocketServer(IPEndPoint endpoint)
        {
            endPoint = endpoint;
        }

        public void Broadcast(byte[] data)
        {
            foreach (var cl in connectedClients) cl.Send(data);
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

            var remoteClient = new RemoteClient(this, remoteClientSocket);
            remoteClient.DataReceived += remoteClient_DataReceived;
            remoteClient.Disconnected += remoteClient_Disconnected;
            connectedClients.Add(remoteClient);
            if (ClientConnected != null) ClientConnected(this, new ConnectedEventArgs(remoteClient));
        }

        void remoteClient_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (DataReceived != null)
                DataReceived(this, new DataReceivedEventArgs(e.Client, e.Data));
        }

        void remoteClient_Disconnected(object sender, DisconnectedEventArgs e)
        {
            if (ClientDisconnected != null)
                ClientDisconnected(this, new DisconnectedEventArgs(e.Client));
        }

    }
}
