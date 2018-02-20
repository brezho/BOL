using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using X.Packing.Internals;

namespace X.Packing
{
    public static class XStream
    {
        const bool DEBUG = true;
        internal static void debug(string stuff)
        {
            if (DEBUG) Console.WriteLine(stuff);
        }
        public static IXStream GetWriter(Stream inner)
        {
            return new XStreamWriter(inner);
        }
        public static IXStream GetReader(Stream inner)
        {
            return new XStreamReader(inner);
        }
        public static IXStream GetReader(byte[] data)
        {
            return new XStreamReader(data);
        }

    }
}
