using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Repository;

namespace Repositories.InMemory
{
    public class InMemoryRepositoryProvider : IRepositoryProvider
    {
        public IRepository<T> GetRepository<T>()
        {
            return new InMemoryRepository<T>();
        }

    }
}
