using System;
using System.Collections.Generic;
using System.Helpers;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Net.Packing;

namespace X.Net.Packing
{
    public class Packer
    {
        static object locker = new object();
        static List<Type> knownPackables;
        static Packer()
        {
            lock (locker)
            {
                knownPackables = typeof(IPacket)
                                .Hype()
                                .GetMatchingTypes(x => x.IsConcrete())
                                .OrderBy(x => x.FullName)
                                .ToList();
            }
        }

        public byte[] Pack(IPacket obj)
        {
            byte[] res;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write((byte)knownPackables.IndexOf(obj.GetType()));
                obj.Pack(writer);
                res = ms.ToArray();
            }
            return res;
        }

        public IPacket Unpack(byte[] data)
        {
            IPacket res = null;
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                var messageTypeIndex = br.ReadByte();
                Type objType;
                lock (locker)
                {
                    objType = knownPackables[messageTypeIndex];
                }
                res = (IPacket)Activator.CreateInstance(objType);
                res.Unpack(br);
            }
            return res;
        }
    }
}
