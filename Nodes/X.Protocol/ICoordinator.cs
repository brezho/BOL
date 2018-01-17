using System;
using System.Collections.Generic;
using System.Text;

namespace X.Protocol
{
    public interface ICoordinator
    {
        string GetDBConnection();
        void SetDispatcherEndPointURIForRunners(string uri);
        string GetADispactcherUri();
    }
    
}
