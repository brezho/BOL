using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace X.Net.Peers
{
    public partial class Peer : IComparable<Peer>, XSerializable

    {
        public int UpperBoundPartitionId;
        public IPEndPoint Endpoint;
        public string PeerNetworkName;
        public Peer()
        {

        }

        public static Peer OnRandomPort(string networkName)
        {
            var ip = GetIP();
            var port = GetRandomUnusedPort();

            Console.WriteLine(port);
            var endpoint = new IPEndPoint(ip, port);
            return new Peer(networkName, endpoint);
        }
        static IPAddress GetIP()
        {
            return Dns
                .GetHostEntry(Dns.GetHostName())
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
        public static string GetMac()
        {
            return NetworkInterface
                            .GetAllNetworkInterfaces()
                            .Select(x => x.GetPhysicalAddress().GetAddressBytes())
                            .FirstOrDefault()
                            .Aggregate(string.Empty, (res, b) => res += b.ToString("X2"));
        }


        static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }


        public Peer(string networkName, IPEndPoint ep)
            : this()
        {
            PeerNetworkName = networkName;
            Endpoint = ep;
        }

        public override string ToString()
        {
            return string.Format("{0}", Endpoint.ToString());
        }

        public static bool operator ==(Peer item1, Peer item2)
        {
            if (object.ReferenceEquals(item1, item2)) { return true; }
            if ((object)item1 == null || (object)item2 == null) { return false; }
            return item1.Endpoint.ToString() == item2.Endpoint.ToString();
        }

        public static bool operator !=(Peer i1, Peer i2)
        {
            return !(i1.Endpoint == i2.Endpoint);
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return (obj is Peer) && (this == (obj as Peer));
        }
        public override int GetHashCode()
        {
            return Endpoint.ToString().GetHashCode();
        }


        public void ReadWrite(XStream stream)
        {
            stream.ReadWrite(ref Endpoint);
            stream.ReadWrite(ref UpperBoundPartitionId);
            stream.ReadWrite(ref PeerNetworkName);
        }
        public int CompareTo(Peer other)
        {
            return this.UpperBoundPartitionId.CompareTo(other.UpperBoundPartitionId);
        }
    }
}
