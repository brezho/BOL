//using System;
//using System.Collections.Generic;
//using System.Net.NetworkInformation;
//using System.Security.Cryptography;
//using System.Text;

//namespace Book.Services.Peers
//{
//    public class PeerId
//    {
//        public PeerId(byte[] data)
//        {
//            //Debug.Assert(data.Length == Size);
//            //_internal = data;
//        }
//        public static PeerId GetRandom()
//        {
//            var buffer = new byte[32];
//            RNGCryptoServiceProvider.Create().GetBytes(buffer);
//            var bytes = Encoding.ASCII.GetBytes(info);
//            var md5 = MD5.Create();
//            var hash = md5.ComputeHash(bytes);
//            return new PeerId(hash);
//        }

//        public static bool operator ==(PeerId item1, PeerId item2)
//        {
//            if (object.ReferenceEquals(item1, item2)) { return true; }
//            if ((object)item1 == null || (object)item2 == null) { return false; }
//            return item1.Endpoint.ToString() == item2.Endpoint.ToString();
//        }

//        public static bool operator !=(PeerId i1, PeerId i2)
//        {
//            return !(i1.Endpoint == i2.Endpoint);
//        }
//        public override bool Equals(object obj)
//        {
//            if (obj == null) return false;
//            return (obj is PeerId) && (this == (obj as Peer));
//        }
//        public override int GetHashCode()
//        {
//            return Endpoint.ToString().GetHashCode();
//        }
//    }
//}
