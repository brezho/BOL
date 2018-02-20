using System;
using System.Collections.Generic;
using System.Helpers;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Packing
{
    public class Packer
    {
        static object locker = new object();
        static List<Type> knownPackables;
        static Packer()
        {
            lock (locker)
            {
                knownPackables = typeof(IXSerializable)
                                .Hype()
                                .GetMatchingTypes(x => x.IsConcrete())
                                .OrderBy(x => x.FullName)
                                .ToList();
            }
        }

        public static byte[] Pack(IXSerializable obj)
        {
            byte[] res;
            using (var ms = new MemoryStream())
            {
                var help = XStream.GetWriter(ms);
                var messageTypeIndex = (byte)knownPackables.IndexOf(obj.GetType());
                help.ReadWrite(ref messageTypeIndex);
                help.ReadWrite(ref obj);
                res = ms.ToArray();
            }
            return res;
        }

        public static IXSerializable Unpack(byte[] data)
        {
            IXSerializable res = null;
            using (var ms = new MemoryStream(data))
            {
                var help = XStream.GetReader(ms);
                byte messageTypeIndex = Byte.MaxValue;
                help.ReadWrite(ref messageTypeIndex);
                Type objType;
                lock (locker)
                {
                    objType = knownPackables[messageTypeIndex];
                }
                res = (IXSerializable)Activator.CreateInstance(objType);
                help.ReadWrite(ref res);
            }
            return res;
        }
    }
}
