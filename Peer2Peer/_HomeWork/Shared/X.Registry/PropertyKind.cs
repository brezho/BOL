using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Registry
{
    public enum PropertyKind : ushort
    {
        String = 0,
        Int = 1,
        Binary = 2,
        DateTime = 3,
    }
}
