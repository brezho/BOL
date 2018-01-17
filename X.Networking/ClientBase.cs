using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace X.Networking
{
    public abstract class ClientBase
    {
        Stream incomingChannel;
        Stream outgoingChannel;

        bool isClosed;
        public event EventHandler OnOpen;
        public event EventHandler OnClose;
        public event EventHandler<Message> OnMessage;

        protected abstract bool MasksOutgoingMessages { get; }

        public void Initialize(Stream inChannel, Stream outChannel)
        {
            incomingChannel = inChannel;
            outgoingChannel = outChannel;
            if (OnOpen != null) OnOpen(this, EventArgs.Empty);

            Listen();
        }
        public void Send(Message message)
        {
            WriteFrame(new NetworkFrame(Fin.Final, message.Type == MessageType.Text ? Opcode.Text : Opcode.Binary, message.Data, false, MasksOutgoingMessages));
        }
        public void Send(string message)
        {
            Send(Message.New(message));
        }
        //public Message Receive()
        //{
        //    var x = NetworkFrame.ReadFrame(incomingChannel, !MasksOutgoingMessages);
        //    if (DealWithControlFrame(x)) return Receive();
        //    return new Message(x.IsText ? MessageType.Text : MessageType.Binary, x.PayloadData.ApplicationData);
        //}
        public void Ping()
        {
            WriteFrame(NetworkFrame.CreatePingFrame(MasksOutgoingMessages));
        }
        public void Close()
        {
            if (!isClosed)
            {
                isClosed = true;
                if (incomingChannel != null)
                {
                    try
                    {
                        var frame = NetworkFrame.CreateCloseFrame(PayloadData.Empty, MasksOutgoingMessages).ToArray();
                        outgoingChannel.WriteBytes(frame, frame.Length);
                    }
                    catch
                    {
                        //Do nothing
                    }
                    try
                    {
                        // incomingChannel.Close();
                        incomingChannel.Dispose();
                    }
                    catch
                    {
                        //Do nothing
                    }
                }
                try
                {
                    if (outgoingChannel != null)
                    {
                      //  outgoingChannel.Close();
                        outgoingChannel.Dispose();
                    }
                }
                catch
                {
                    //Do nothing
                }

                if (OnClose != null) OnClose(this, EventArgs.Empty);
            }
        }
        void WriteFrame(NetworkFrame frame)
        {
            try
            {
                var data = frame.ToArray();
                outgoingChannel.WriteAsync(data, 0, data.Length);
            }
            catch
            {
                Close();
            }
        }
        void Listen()
        {
            NetworkFrame.ReadFrameAsync(incomingChannel, !MasksOutgoingMessages, x =>
            {
                if (!DealWithControlFrame(x))
                {
                    if (OnMessage != null) OnMessage(this, new Message(x.IsText ? MessageType.Text : MessageType.Binary, x.PayloadData.ApplicationData));
                }
                if (!isClosed) Listen();
            }, e =>
            {
                Close();
            });
        }
        bool DealWithControlFrame(NetworkFrame frame)
        {
            if (frame.IsControl)
            {
                if (frame.IsPing)
                {
                    WriteFrame(NetworkFrame.CreatePongFrame(PayloadData.Empty, MasksOutgoingMessages));
                }
                else if (frame.IsPong)
                {
                    Trace.WriteLine("Pong received");
                }
                else if (frame.IsClose)
                {
                    Close();
                }
                return true;
            }
            return false;
        }
    }
}
