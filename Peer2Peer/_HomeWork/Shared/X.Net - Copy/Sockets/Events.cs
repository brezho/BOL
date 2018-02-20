using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Net.Packing;

namespace X.Net.Sockets
{
    public class ConnectedEventArgs : EventArgs
    {
        public IDualPort ConnectedTo { get; private set; }
        internal ConnectedEventArgs(IDualPort connectedTo)
        {
            ConnectedTo = connectedTo;
        }
    }
    public class DisconnectedEventArgs : EventArgs
    {
        public IDualPort DisconnectedFrom { get; private set; }
        internal DisconnectedEventArgs(IDualPort disconnectedFrom)
        {
            DisconnectedFrom = disconnectedFrom;
        }
    }
    public class PacketReceivedEventArgs : EventArgs
    {
        internal PacketReceivedEventArgs(IDualPort sender, IPacket packet)
        {
            ReceivedFrom = sender;
            Packet = packet;
        }
        public IDualPort ReceivedFrom { get; private set; }
        public IPacket Packet { get; private set; }
    }
}
