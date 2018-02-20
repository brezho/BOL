using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Net.Peers
{
    public class PeerList : IEnumerable<Peer>
    {
        SortedSet<Peer> _peers = new SortedSet<Peer>();

        internal const int numberOfPartitionsInKeySpace = 1024;
        public Peer GetPeerForKey(string key)
        {
            var partitionId = GetPartitionIdForKey(key);
            var peer = _peers.FirstOrDefault(x => x.UpperBoundPartitionId >= partitionId);
            return peer ?? _peers.FirstOrDefault();
        }

        public Peer GetNextOf(Peer peerInfos)
        {
            return _peers.FirstOrDefault(x => x.UpperBoundPartitionId > peerInfos.UpperBoundPartitionId)
                ?? _peers.FirstOrDefault()
                ?? peerInfos;
        }

        public Peer GetPreviousOf(Peer peerInfos)
        {
            return _peers.LastOrDefault(x => x.UpperBoundPartitionId < peerInfos.UpperBoundPartitionId)
                ?? _peers.LastOrDefault()
                ?? peerInfos;
        }

        public int GetPartitionIdForKey(string key)
        {
            var hash = key.GetHashCode();
            return Math.Abs(hash % numberOfPartitionsInKeySpace);
        }

        internal void Add(Peer peer)
        {
            _peers.Add(peer);
        }

        public IEnumerator<Peer> GetEnumerator()
        {
            return _peers.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
