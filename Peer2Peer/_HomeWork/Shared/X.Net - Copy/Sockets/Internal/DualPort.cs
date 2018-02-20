using System;
using System.Collections.Generic;
using System.Helpers;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using X.Net.Sockets.Internal;
using X.Net.Packing;

namespace X.Net.Sockets.Internal
{
    class DualPort : IDualPort
    {
        public event EventHandler<PacketReceivedEventArgs> NewMessage;
        public event EventHandler<EventArgs> Disconnected;

        Socket targetSocket;
        NetworkStream networkStream;
        Packer messagePacker;

        PacketProtocol protocol;
        CancellationTokenSource keepReadingCancelEvent = new CancellationTokenSource();
        CancellationTokenSource keepAliveCancelEvent = new CancellationTokenSource();

        public DualPort(Socket socket)
        {
            targetSocket = socket;
            networkStream = new NetworkStream(targetSocket, true);
            messagePacker = new Packer();
            protocol = new PacketProtocol();

            protocol.MessageArrived += msg =>
            {
                IPacket result = messagePacker.Unpack(msg);
                if ((NewMessage != null) && !(result is KeepAliveMessage)) 
                    NewMessage(this, new PacketReceivedEventArgs(this, result));
            };

            Task.Factory.StartNew(PumpNetworkStream);
            Task.Factory.StartNew(KeepAlive);

        }

        void KeepAlive()
        {
            while (!keepAliveCancelEvent.Token.IsCancellationRequested)
            {
                Thread.Sleep(2000);
                Send(new KeepAliveMessage());
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
                    protocol.DataReceived(read);
                }
            }
            catch (Exception ex)
            {
                OnDisconnected(ex, EventArgs.Empty);
                return;
            }
        }

        public void Send(IPacket packet)
        {
            if (packet == null) throw new ArgumentNullException("packet");

            byte[] message = messagePacker.Pack(packet);

            var wrappedMessage = PacketProtocol.WrapMessage(message);

            try
            {
                networkStream.Write(wrappedMessage, 0, wrappedMessage.Length);
                networkStream.Flush();
            }
            catch (Exception ex)
            {
                OnDisconnected(ex, EventArgs.Empty);
                return;
            }
        }

        private void OnDisconnected(Exception ex, EventArgs eventArgs)
        {
            keepReadingCancelEvent.Cancel();
            keepAliveCancelEvent.Cancel();
            if (Disconnected != null) Disconnected(this, eventArgs);
        }

    }
}
