using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace X.Packing.Internals
{
    class XStreamReader : BinaryReader, IXStream
    {
        public bool IsWriting
        {
            get { return false; }
        }
        public XStreamReader(byte[] data) : this(new MemoryStream(data)) { }
        public XStreamReader(Stream inner) : base(inner, Encoding.UTF8) { }

        public void ReadWrite<T>(ref T data) where T : IXSerializable
        {
            //    XStream.debug("object of " + typeof(T).Hype().FriendlyName);
            var wasWrittenObjectNull = base.ReadBoolean();
            if (wasWrittenObjectNull) data = default(T);
            else
            {
                if (data == null) data = Activator.CreateInstance<T>();
                data.ReadWrite(this);
            }
        }

        public void ReadWrite(ref string data)
        {
            var len = base.ReadInt32();
            switch (len)
            {
                case -1:
                    data = null;
                    break;
                case 0:
                    data = string.Empty;
                    break;
                default:
                    data = Encoding.UTF8.GetString(base.ReadBytes(len));
                    break;
            }

        }

        public void ReadWrite(ref byte data)
        {
            data = base.ReadByte();
        }
        public void ReadWrite(ref int data)
        {
            data = base.ReadInt32();
        }


        public void ReadWrite(ref Guid data)
        {
            var b = base.ReadBytes(16);
            data = new Guid(b);
            XStream.debug(data.ToString());
        }

        public void ReadWrite<T>(ref T[] data)
        {
            //   XStream.debug("array of " + typeof(T).Hype().FriendlyName);
            var arr = new ArrayOfStuff<T>();
            ReadWrite(ref arr);
            data = arr.GetStuff();
        }
        public void ReadWrite<TKey, TValue>(ref Dictionary<TKey, TValue> data)
        {
            var arr = new DictionaryOfStuff<TKey, TValue>(data);
            ReadWrite(ref arr);
            data = arr.GetStuff();
        }

        public void ReadWrite(ref long data)
        {
            data = base.ReadInt64();
        }

        public void ReadWrite(ref IPEndPoint data)
        {
            var isNull = base.ReadBoolean();
            if (isNull)
            {
                data = null;
                return;
            }
            var address = new IPAddress(base.ReadBytes(base.ReadInt32()));
            var port = base.ReadInt32();
            data = new IPEndPoint(address, port);
        }


        public void ReadWrite(ref bool data)
        {
            data = base.ReadBoolean();
        }

        public void ReadWrite(ref DateTimeOffset data)
        {
            var seconds = base.ReadInt64();
            data = DateTimeHelper.OffsetFromEpoch(seconds);
        }

        public void ReadWrite(ref byte[] data)
        {
            int len = base.ReadInt32();
            switch (len)
            {
                case -1: data = null; break;
                case 0: data = new byte[0]; break;
                default: data = base.ReadBytes(len); break;
            }
        }
    }
}
