using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Registry.Storage
{
    public class BinaryStore<K> : KVStore<K, byte[]>
          where K : IComparable<K>
    {
        public BinaryStore(string location)
            : base(location, X => X, X => X)
        { }
    }
}
