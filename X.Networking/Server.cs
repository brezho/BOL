using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace X.Networking
{

    public delegate void OnServerEventDelegate(ClientContext context);
    public delegate void OnMessageDelegate(ClientContext context, Message message);
    public class Server
    {
        public IList<ListenerBase> Implementations { get; private set; }
        List<ClientContext> Contexts;

        public OnServerEventDelegate OnConnected = x => { };
        public OnServerEventDelegate OnDisconnected = x => { };
        public OnMessageDelegate OnMessage = (x, m) => { };
        public Server()
        {
            Implementations = new List<ListenerBase>();
            Contexts = new List<ClientContext>();
        }

        public void Start()
        {
            foreach (var impl in Implementations) impl.Start();
        }
        public void Stop()
        {
            var allConnections = Contexts.ToArray();
            foreach (var conn in allConnections) conn.Close();
            Contexts.Clear();
            foreach (var impl in Implementations) impl.Stop();
        }

        public void SendAll(Message message)
        {
            var allConnections = Contexts.ToArray();
            foreach (var conn in allConnections) conn.Send(message);
        }
        public void SendAll(string message)
        {
            SendAll(Message.New(message));
        }
        public void SendAllExcept(Message message, ClientContext ctx)
        {
            var allConnections = Contexts.ToArray();
            foreach (var conn in allConnections) if(conn != ctx) conn.Send(message);
        }
        public void AddSocketListener(IPEndPoint endpoint)
        {
            AddListener(new X.Networking.SocketImpl.SocketListener(endpoint));
        }
        public void AddWebSocketListener(IPEndPoint endpoint)
        {
            AddListener(new X.Networking.SocketImpl.WebSocketListener(endpoint));
        }
        public void AddListener(ListenerBase impl)
        {
            impl.OnAccept += (l, inStream, outStream) => 
            {
                Enroll(inStream, outStream);
            };
            Implementations.Add(impl);
        }

        void Enroll(Stream inChannel, Stream outChannel)
        {
            var context = new ClientContext();

            context.OnOpen += (s, a) =>
            {
                OnConnected(context);
            };

            context.OnClose += (s, a) =>
            {
                Contexts.Remove(context);
                OnDisconnected(context);
            };

            context.OnMessage += (s, a) =>
            {
                OnMessage(context, a);
            };
            

            context.Initialize(inChannel, outChannel);

            Contexts.Add(context);
        }
    }
}
