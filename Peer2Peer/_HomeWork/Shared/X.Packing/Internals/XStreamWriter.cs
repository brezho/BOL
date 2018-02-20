using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace X.Packing.Internals
{
    class XStreamWriter : BinaryWriter, IXStream
    {
        public bool IsWriting
        {
            get { return true; }
        }
        public XStreamWriter(Stream inner) : base(inner) { }


        public void ReadWrite<T>(ref T data) where T : IXSerializable
        {
            var isNull = data == null;
            base.Write((bool)isNull);
            if (!isNull) data.ReadWrite(this);
        }

        public void ReadWrite(ref string data)
        {
            var len = (data != null) ? Encoding.UTF8.GetByteCount(data) : -1;
            base.Write(len);
            if (len > 0)
            {
                base.Write(Encoding.UTF8.GetBytes(data));
            }
        }

        public void ReadWrite(ref byte data)
        {
            base.Write(data);
        }
        public void ReadWrite(ref int data)
        {
            base.Write(data);
        }

        public void ReadWrite(ref Guid data)
        {
            XStream.debug(data.ToString());
            base.Write(data.ToByteArray());
        }

        public void ReadWrite<T>(ref T[] data)
        {
            //    XStream.debug("array of " + typeof(T).Hype().FriendlyName);
            var arr = new ArrayOfStuff<T>(data);
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
            base.Write(data);
        }


        public void ReadWrite(ref IPEndPoint data)
        {
            var isNull = (data == null);
            base.Write(isNull);
            if (!isNull)
            {
                var adressBytes = data.Address.GetAddressBytes();
                base.Write(adressBytes.Length);
                base.Write(adressBytes);
                base.Write(data.Port);
            }
        }

        public void ReadWrite(ref bool data)
        {
            base.Write(data);
        }


        public void ReadWrite(ref DateTimeOffset data)
        {
            var seconds = DateTimeHelper.SecondsSinceEpoch(data);
            base.Write(seconds);
        }

        public void ReadWrite(ref byte[] data)
        {
            var len = (data == null) ? -1 : data.Length;
            base.Write(len);
            if (len > 0) base.Write(data);
        }
    }
}
