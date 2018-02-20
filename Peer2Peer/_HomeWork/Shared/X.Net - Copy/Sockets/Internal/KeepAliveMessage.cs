using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Net.Packing;

namespace X.Net.Sockets.Internal
{
    struct KeepAliveMessage : IPacket
    {
        void IPacket.Pack(BinaryWriter sw)
        {
            sw.Write(0);
        }

        void IPacket.Unpack(BinaryReader sr)
        {
            sr.ReadInt32();
        }
    }
}
