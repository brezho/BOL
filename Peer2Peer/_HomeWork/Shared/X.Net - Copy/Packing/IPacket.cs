using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Net.Packing
{
    public interface IPacket
    {
        void Pack(BinaryWriter sw);
        void Unpack(BinaryReader sr);
    }
}
