using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace X.Networking.SocketImpl
{
    public class WebSocketListener : SocketListener
    {
        static readonly SHA1 hasher = SHA1.Create();
        static readonly Regex getRegex = new Regex("^GET");
        static readonly Regex webSocketKeyRegex = new Regex("Sec-WebSocket-Key: (.*)");
        public WebSocketListener(IPEndPoint endpoint) : base(endpoint)
        {
        }
        protected override bool HandShake(NetworkStream stream)
        {
            var buffer = new byte[4096];
            int read = 0;
            while (read == 0)
            {
                read = stream.Read(buffer, 0, buffer.Length);
            }

            // Negotiation

            var request = UTF8Encoding.UTF8.GetString(buffer, 0, read);

            if (getRegex.IsMatch(request))
            {
                Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + Environment.NewLine
                    + "Connection: Upgrade" + Environment.NewLine
                    + "Upgrade: websocket" + Environment.NewLine
                    + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                        hasher.ComputeHash(
                            Encoding.UTF8.GetBytes(
                                webSocketKeyRegex.Match(request).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                            )
                        )
                    ) + Environment.NewLine
                    + Environment.NewLine);

                stream.Write(response, 0, response.Length);

                return true;
            }

            return false;
        }

    }
}
