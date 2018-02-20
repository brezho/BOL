using Repositories.Databases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My = MySql;
namespace Repositories.MySql
{
    public class MySqlDatabase : IDatabase
    {
        public MySqlDatabase(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }

        public System.Data.Common.DbProviderFactory ProviderFactory
        {
            get
            {
                return new My.Data.MySqlClient.MySqlClientFactory();
            }
        }

        public IExecutor Executor
        {
            get { return new MySqlExecutor(this); }
        }

        public IRepository<T> GetRepository<T>()
        {
            return new Table<T>(this);
        }

        public bool IsConnectionUp()
        {
            var conn = ProviderFactory.CreateConnection();
            conn.ConnectionString = ConnectionString;
            bool result = false;
            try
            {
                conn.Open();
                result = true;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return result;
        }
    }

}
