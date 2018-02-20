using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Net.Packing;

namespace X.Net.Sockets
{
    public interface IDualPort
    {
        void Send(IPacket packet);
        event EventHandler<PacketReceivedEventArgs> NewMessage;
        event EventHandler<EventArgs> Disconnected;
    }
}
