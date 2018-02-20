using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace X.Net.Sockets.Active
{
    public interface IActiveSocket
    {
        void Send(byte[] data);
        event EventHandler<DataReceivedEventArgs> DataReceived;
        event EventHandler<EventArgs> Disconnected;
    }
}
