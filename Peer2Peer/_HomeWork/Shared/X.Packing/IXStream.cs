using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace X.Packing
{
    public interface IXStream
    {
        bool IsWriting { get; }
        void ReadWrite<TKey, TValue>(ref Dictionary<TKey, TValue> data);
        void ReadWrite<T>(ref T data) where T : IXSerializable;
        void ReadWrite(ref string data);
        void ReadWrite(ref bool data);
        void ReadWrite(ref byte data);
        void ReadWrite(ref byte[] data);
        void ReadWrite(ref int data);
        void ReadWrite(ref long data);
        void ReadWrite(ref Guid data);
        void ReadWrite<T>(ref T[] data);
        void ReadWrite(ref IPEndPoint data);
        void ReadWrite(ref DateTimeOffset data);
    }
}
