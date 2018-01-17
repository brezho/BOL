using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace X.Networking
{
    public class ClientContext : ClientBase
    {
        protected override bool MasksOutgoingMessages => false;
    }

}
