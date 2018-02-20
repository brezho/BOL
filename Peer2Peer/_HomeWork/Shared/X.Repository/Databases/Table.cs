using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace X.Repository.Databases
{
    public class Table<T> : RepositoryBase<T>
    {
        public IDatabase Database { get; private set; }

        internal Table(IDatabase database)
        {
            Database = database; 
        }

        public override IExecutor Executor
        {
            get { return Database.Executor; }
        }
    }
}
