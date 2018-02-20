using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Net.Remoting
{
    internal enum MessageType
    {
        TerminateConnection = 0,
        MethodInvocation = 1,
        ReturnValues = 2,
        UnknownMethod = 3,
        ThrowException = 4,
        NegotiateInterface = 5
    };
}
