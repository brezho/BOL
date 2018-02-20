using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X.Repository
{
    public interface IRepositoryProvider
    {
        IRepository<T> GetRepository<T>();
    }
}
