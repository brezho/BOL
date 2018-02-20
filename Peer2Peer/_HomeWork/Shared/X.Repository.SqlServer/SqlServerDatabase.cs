using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Configuration;
using System.Data.SqlClient;
using X.Repository.Databases;

namespace X.Repository.SqlServer
{
    public class SqlServerDatabase : IDatabase
    {
        public SqlServerDatabase(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public static SqlServerDatabase Get(string connectionString)
        {
            return new SqlServerDatabase(connectionString);
        }

        public static SqlServerDatabase GetFromConfigFile(string connectionStringName)
        {
            return (ConfigurationManager.ConnectionStrings[connectionStringName] != null)
                ? Get(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString)
                : null;
        }

        public static SqlServerDatabase Get(string ServerName, string DatabaseName)
        {
            return Get(ServerName, DatabaseName, null, null);
        }

        public static SqlServerDatabase Get(string ServerName, string DatabaseName, string UserName, string Password)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = ServerName;
            builder.InitialCatalog = DatabaseName;

            if (!string.IsNullOrWhiteSpace(UserName))
            {
                builder.UserID = UserName;
                builder.Password = Password;
            }
            else builder.IntegratedSecurity = true;

            return Get(builder.ConnectionString);
        }

        public string ConnectionString { get; private set; }

        public System.Data.Common.DbProviderFactory ProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.SqlClient"); }
        }

        public IExecutor Executor
        {
            get { return new SqlServerExecutor(this); }
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
