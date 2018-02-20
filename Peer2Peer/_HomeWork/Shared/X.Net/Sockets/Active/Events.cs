using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Net.Sockets.Active
{
    public class ConnectedEventArgs : EventArgs
    {
        public IActiveSocket ConnectedTo { get; private set; }
        internal ConnectedEventArgs(IActiveSocket connectedTo)
        {
            ConnectedTo = connectedTo;
        }
    }
    public class DisconnectedEventArgs : EventArgs
    {
        public IActiveSocket DisconnectedFrom { get; private set; }
        internal DisconnectedEventArgs(IActiveSocket disconnectedFrom)
        {
            DisconnectedFrom = disconnectedFrom;
        }
    }
    public class DataReceivedEventArgs : EventArgs
    {
        internal DataReceivedEventArgs(IActiveSocket sender, byte[] data)
        {
            ReceivedFrom = sender;
            Data = data;
        }
        public IActiveSocket ReceivedFrom { get; private set; }
        public byte[] Data { get; private set; }
    }
}
