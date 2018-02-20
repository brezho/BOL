using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Repositories.Databases;

namespace Repositories.SqlLite
{
    public class SqlLiteDatabase : IDatabase
    {
        public SqlLiteDatabase(string FilePath)
        {
            if (!File.Exists(FilePath))
                this.GetType().Assembly.SaveResourceAs(this.GetType().Namespace + ".empty.db", FilePath);

            ConnectionString = "Data Source=" + FilePath;
        }

        public string ConnectionString { get; private set; }

        public System.Data.Common.DbProviderFactory ProviderFactory
        {
            get { return new System.Data.SQLite.SQLiteFactory();  }
        }

        public IExecutor Executor
        {
            get { return new SqlLiteExecutor(this); }
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
