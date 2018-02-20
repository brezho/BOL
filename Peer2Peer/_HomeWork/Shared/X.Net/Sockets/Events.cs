using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Net.Sockets
{
    public class DataReceivedEventArgs : EventArgs
    {
        public IClient Client { get; private set; }
        public byte[] Data { get; private set; }
        internal DataReceivedEventArgs(IClient client, byte[] data)
        {
            Client = client;
            Data = data;
        }
    }
    public class ConnectedEventArgs : EventArgs
    {
        public IClient Client { get; private set; }
        internal ConnectedEventArgs(IClient client)
        {
            Client = client;
        }
    }

    public class DisconnectedEventArgs : EventArgs
    {
        public IClient Client { get; private set; }
        internal DisconnectedEventArgs(IClient connectedTo)
        {
            Client = connectedTo;
        }
    }
}
