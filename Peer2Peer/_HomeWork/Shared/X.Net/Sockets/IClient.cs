using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace X.Net.Sockets
{
    public interface IClient
    {
        event EventHandler<ConnectedEventArgs> Connected;
        event EventHandler<DisconnectedEventArgs> Disconnected;
        event EventHandler<DataReceivedEventArgs> DataReceived;
        void Send(byte[] data);
        void Connect();
        EndPoint EndPoint { get; }
    }
}
