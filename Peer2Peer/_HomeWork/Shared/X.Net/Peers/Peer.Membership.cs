using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer;
using System.Text;
using System.Threading.Tasks;
using X.Packing;

namespace X.Net.Peers
{
    public partial class Peer
    {
        PeerName GetPeerName()
        {
            return new PeerName(this.PeerNetworkName, PeerNameType.Unsecured);
        }


        PeerList _cached = null;

        public PeerList GetPeers(bool includeSelf = true)
        {
            var newList = new PeerList();
            _cached = new PeerList();

            var resolver = new PeerNameResolver();
            var results = resolver.Resolve(GetPeerName());

            foreach (PeerNameRecord record in results)
            {
                var peer = (Peer)Packer.Unpack(record.Data);
                newList.Add(peer);
                _cached.Add(peer);
            }

            if (includeSelf) newList.Add(this);

            return newList;
        }

       public IEnumerable<Peer> GetCachedRemotePeers()
        {
            if (_cached != null)
            {
                foreach (var it in _cached) yield return it;
            }
        }


        PeerNameRegistration registration;
        public void ComeOut()
        {
            registration = new PeerNameRegistration(GetPeerName(), this.Endpoint.Port);
            registration.Data = Encoding.ASCII.GetBytes(this.Endpoint.ToString());
            registration.UseAutoEndPointSelection = true;

            var NumberOfSlots = PeerList.numberOfPartitionsInKeySpace;

            var allPeers = GetPeers(false).ToArray();
            if (allPeers.Length == 0)
            {
                // simple case
                // this node covers for the entire keys universe => Ring.nbOfSlots - 1
                this.UpperBoundPartitionId = NumberOfSlots - 1;
            }
            else
            {
                // hard case... The keys universe is already partitioned
                // Here the rule is that a new node will always take a share of the 'busiest' node (busiest being the one who covers for the most slots) 
                // the 'share' (nb of slots) is calculated by spreading the keys space depending on available Capacity (arbitrary number, being the RAM at the moment)

                //let's do this....

                // 1. who's the node with the greatest number of slots (don't forget the ring) ?
                //      a. sort Peers by UpperBoundSlot
                //      b. keep reference on the last item to initialize the loop
                //      c. populate a new list of peers (coverageList) with their actual 'share' (nb of slots they actually cover)

                var coverageList = new List<Tuple<Peer, int>>();
                var sortedPeers = allPeers.OrderBy(x => x.UpperBoundPartitionId).ToArray();

                var previousMember = sortedPeers.Last();
                for (int i = 0; i < sortedPeers.Length; i++)
                {
                    var currentMember = sortedPeers[i];
                    if (currentMember == previousMember) coverageList.Add(Tuple.Create(currentMember, NumberOfSlots));
                    else
                    {
                        if (previousMember.UpperBoundPartitionId > currentMember.UpperBoundPartitionId)
                            coverageList.Add(Tuple.Create(currentMember, currentMember.UpperBoundPartitionId + (NumberOfSlots - previousMember.UpperBoundPartitionId)));
                        else
                            coverageList.Add(Tuple.Create(currentMember, currentMember.UpperBoundPartitionId - previousMember.UpperBoundPartitionId));
                    }
                    previousMember = currentMember;
                }

                var tupleWithMostAllocatedSlots = coverageList.OrderBy(x => x.Item2).Last();

                var busiestPeer = tupleWithMostAllocatedSlots.Item1;
                var nbOfSlotsCoveredByBusiestPeer = tupleWithMostAllocatedSlots.Item2;

                // alright, we know who's the busiest peer and how many slots he does cover

                #region that is the way of doing it based on a Capacity of some sort
                // let's see how many slots this node must take over based on this node and the busiest node respective Capacities
                //var thisNodeSharePercentage = (decimal)this.Capacity / (this.Capacity + busiestPeer.Capacity);
                //var thisNodeNumberOfSlotsToCover = (int)Math.Round(thisNodeSharePercentage * nbOfSlotsCoveredByBusiestPeer);
                #endregion

                #region other way : split the partition in half
                var thisNodeNumberOfSlotsToCover = (int)Math.Round((decimal)nbOfSlotsCoveredByBusiestPeer / 2);
                #endregion

                // finally let's now find out what is the UpperBound To allocate to this slot...
                var takeUpTo = busiestPeer.UpperBoundPartitionId - thisNodeNumberOfSlotsToCover;
                if (takeUpTo < 0) takeUpTo = NumberOfSlots - takeUpTo;

                // Boom... Job done!
                this.UpperBoundPartitionId = takeUpTo;
            }

            // ok, now the easy part is to publish ourselves on the peer to peer net


            // publishing ThisPeer infos


            registration.Data = Packer.Pack(this);// Encoding.ASCII.GetBytes(thisPeerInfo.ToString());
            registration.Start();

        }
    }
}
