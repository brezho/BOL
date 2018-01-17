using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace X.Networking
{
    public delegate void OnAcceptDelegate(ListenerBase listener, Stream incomingDataStream, Stream outgoingDataStream);
    public abstract class ListenerBase
    {
        public event OnAcceptDelegate OnAccept;
        public abstract void Start();
        public abstract void Stop();

        protected void Accept(Stream incomingDataStream, Stream outgoingDataStream)
        {
            if (OnAccept != null) OnAccept(this, incomingDataStream, outgoingDataStream);
        }
    }
}
