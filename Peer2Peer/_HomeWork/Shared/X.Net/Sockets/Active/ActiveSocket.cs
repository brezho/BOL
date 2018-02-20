using System;
using System.Collections.Generic;
using System.Helpers;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Collections.Concurrent;
using System.Net;

namespace X.Net.Sockets.Active
{

    public class ActiveSocket : IActiveSocket
    {
        static byte[] EmptyPacket = new byte[0];

        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<EventArgs> Disconnected;

        Socket targetSocket;
        NetworkStream networkStream;

        BlockingCollection<byte[]> buffersQueue = new BlockingCollection<byte[]>();
        BlockingCollection<byte> readBytesQueue = new BlockingCollection<byte>();

        CancellationTokenSource keepReadingCancelEvent = new CancellationTokenSource();
        CancellationTokenSource keepAliveCancelEvent = new CancellationTokenSource();

        public EndPoint EndPoint { get { return targetSocket.RemoteEndPoint; } }

        public ActiveSocket(Socket socket)
        {
            targetSocket = socket;
            networkStream = new NetworkStream(targetSocket, true);

            Task.Run(() => BuffersConsumer());
            Task.Run(() => BytesConsumer());
            Task.Run(() => PumpNetworkStream());
            Task.Run(() => KeepAlive());
        }

        public void Send(byte[] packet)
        {
            if (packet == null) throw new ArgumentNullException("packet");

            byte[] lengthPrefix = BitConverter.GetBytes(packet.Length);

            // Concatenate the length prefix and the message
            byte[] data = new byte[lengthPrefix.Length + packet.Length];
            lengthPrefix.CopyTo(data, 0);
            packet.CopyTo(data, lengthPrefix.Length);

            try
            {
                networkStream.Write(data, 0, data.Length);
                networkStream.Flush();
            }
            catch (Exception ex)
            {
                OnDisconnected(ex, EventArgs.Empty);
                return;
            }
        }



        void BuffersConsumer()
        {
            byte[] buffer;
            while (true)
            {
                buffer = buffersQueue.Take();
                for (int i = 0; i < buffer.Length; i++)
                {
                    readBytesQueue.Add(buffer[i]);
                }
            }
        }

        void BytesConsumer()
        {
            var lengthBuffer = new byte[sizeof(int)];

            while (true)
            {
                for (int i = 0; i < sizeof(int); i++) lengthBuffer[i] = readBytesQueue.Take();

                var len = BitConverter.ToInt32(lengthBuffer, 0);

                if (len == 0)
                    continue; // no need to read content (nor raise DataReceived) when lenght is 0 (Keep alive)

                var dataBuffer = new byte[len];
                for (int i = 0; i < len; i++) dataBuffer[i] = readBytesQueue.Take();

                if ((DataReceived != null))
                    DataReceived(this, new DataReceivedEventArgs(this, dataBuffer));
            }
        }

        void KeepAlive()
        {
            while (!keepAliveCancelEvent.Token.IsCancellationRequested)
            {
                Thread.Sleep(2000);
                Send(EmptyPacket);
            }
        }

        void PumpNetworkStream()
        {
            try
            {
                while (!keepReadingCancelEvent.Token.IsCancellationRequested)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = networkStream.Read(buffer, 0, buffer.Length);

                    byte[] read = new byte[bytesRead];
                    Array.Copy(buffer, read, bytesRead);
                    buffersQueue.Add(read);
                }
            }
            catch (Exception ex)
            {
                OnDisconnected(ex, EventArgs.Empty);
                return;
            }
        }

        void OnDisconnected(Exception ex, EventArgs eventArgs)
        {
            keepReadingCancelEvent.Cancel();
            keepAliveCancelEvent.Cancel();
            if (Disconnected != null) Disconnected(this, eventArgs);
        }

        byte[] WrapMessage(byte[] message)
        {
            byte[] lengthPrefix = BitConverter.GetBytes(message.Length);

            // Concatenate the length prefix and the message
            byte[] ret = new byte[lengthPrefix.Length + message.Length];
            lengthPrefix.CopyTo(ret, 0);
            message.CopyTo(ret, lengthPrefix.Length);

            return ret;
        }

    }
}
