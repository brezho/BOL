using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using X.Net.Sockets.Active;

namespace X.Net.Sockets
{
    class RemoteClient : IClient
    {
        ActiveSocket _monitor;
        SocketServer _server;

        public event EventHandler<ConnectedEventArgs> Connected;
        public event EventHandler<DisconnectedEventArgs> Disconnected;
        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public EndPoint EndPoint { get { return _monitor.EndPoint; } }

        public RemoteClient(SocketServer server, Socket remoteSocket)
        {
            _server = server;
            _monitor = new ActiveSocket(remoteSocket);
            _monitor.Disconnected += (dSender, dargs) =>
            {
                if (Disconnected != null) Disconnected(this, new DisconnectedEventArgs(this));
            };

            _monitor.DataReceived += (drsnd, drargs) =>
            {
                if (DataReceived != null) DataReceived(this, new DataReceivedEventArgs(this, drargs.Data));
            };
        }

        public void Send(byte[] p)
        {
            _monitor.Send(p);
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }
    }
}
