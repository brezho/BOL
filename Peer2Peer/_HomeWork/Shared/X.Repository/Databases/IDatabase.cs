using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace X.Repository.Databases
{
    public interface IDatabase : IRepositoryProvider
    {
        string ConnectionString { get; }
        DbProviderFactory ProviderFactory { get; }
        IExecutor Executor { get; }

        bool IsConnectionUp();
    }
}
