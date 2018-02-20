using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Net.Sockets.Internal
{
    class PacketProtocol
    {
        BlockingCollection<byte[]> buffersQueue = new BlockingCollection<byte[]>(100);
        BlockingCollection<byte> readBytesQueue = new BlockingCollection<byte>(4096);
        public Action<byte[]> MessageArrived { get; set; }

        public void DataReceived(byte[] data)
        {
            buffersQueue.Add(data);
        }

        public PacketProtocol()
        {
            Task.Run(() => BuffersConsumer());
            Task.Run(() => BytesConsumer());
        }
        void BuffersConsumer()
        {
            foreach (var currentBuffer in buffersQueue.GetConsumingEnumerable())
            {
                // reading all bytes from buffer
                for (int i = 0; i < currentBuffer.Length; i++)
                {
                    readBytesQueue.Add(currentBuffer[i]);
                }
            }
        }

        void BytesConsumer()
        {
            var incomingBytes = readBytesQueue.GetConsumingEnumerable();
            var enumerator = incomingBytes.GetEnumerator();

            var lengthBuffer = new byte[sizeof(int)];

            while (true)
            {
                for (int i = 0; i < sizeof(int); i++)
                {
                    enumerator.MoveNext();
                    lengthBuffer[i] = enumerator.Current;
                }

                var len = BitConverter.ToInt32(lengthBuffer, 0);

                var dataBuffer = new byte[len];
                for (int i = 0; i < len; i++)
                {
                    enumerator.MoveNext();
                    dataBuffer[i] = enumerator.Current;
                }

                if (this.MessageArrived != null) this.MessageArrived(dataBuffer);

            }
        }

        public static byte[] WrapMessage(byte[] message)
        {
            // Get the length prefix for the message
            byte[] lengthPrefix = BitConverter.GetBytes(message.Length);

            // Concatenate the length prefix and the message
            byte[] ret = new byte[lengthPrefix.Length + message.Length];
            lengthPrefix.CopyTo(ret, 0);
            message.CopyTo(ret, lengthPrefix.Length);

            return ret;
        }

        /// <summary>
        /// Wraps a keepalive (0-length) message. The wrapped message is ready to send to a stream.
        /// </summary>
        public static byte[] WrapKeepaliveMessage()
        {
            return BitConverter.GetBytes((int)0);
        }


    }
}
