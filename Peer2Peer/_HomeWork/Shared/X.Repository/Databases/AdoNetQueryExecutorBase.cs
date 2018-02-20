using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Concurrent;
using X.Repository.Databases.Attributes;
using System.Text.RegularExpressions;
using System.Reflection;

namespace X.Repository.Databases
{
    /// <summary>
    /// Opens ADO.Net connection
    /// Create commands
    /// and Execute given SQL 
    /// using connectionstring and provider factory (coming from IDatabase)
    /// </summary>
    public abstract class AdoNetQueryExecutorBase : IExecutor
    {
        public IDatabase Database { get; private set; }
        static ConcurrentDictionary<string, string> DbObjectsNameMapping = new ConcurrentDictionary<string, string>();

        public AdoNetQueryExecutorBase(IDatabase database)
        {
            Database = database;
        }

        public void ExecuteSave<T>(Analysis.SaveCommand<T> command)
        {
            var dbObject = GetDBObjectName(typeof(T));
            ExecuteSave<T>(dbObject, command);
        }

        public IEnumerable<T> ExecuteSelect<T>(Analysis.SelectCommand<T> command)
        {
            var dbObject = GetDBObjectName(typeof(T));
            return ExecuteSelect<T>(dbObject, command);
        }

        public void ExecuteInsertMany<T>(Analysis.InsertManyCommand<T> command)
        {
            var dbObject = GetDBObjectName(typeof(T));
            ExecuteInsertMany<T>(dbObject, command);
        }

        public void ExecuteInsert<T>(Analysis.InsertCommand<T> command)
        {
            var dbObject = GetDBObjectName(typeof(T));
            ExecuteInsert<T>(dbObject, command);
        }

        public void ExecuteUpdate<T>(Analysis.UpdateCommand<T> command)
        {
            var dbObject = GetDBObjectName(typeof(T));
            ExecuteUpdate<T>(dbObject, command);
        }

        public void ExecuteDelete<T>(Analysis.DeleteCommand<T> command)
        {
            var dbObject = GetDBObjectName(typeof(T));
            ExecuteDelete<T>(dbObject, command);
        }

        public void ExecuteDeleteAll<T>(Analysis.DeleteAllCommand<T> command)
        {
            var dbObject = GetDBObjectName(typeof(T));
            ExecuteDeleteAll<T>(dbObject, command);
        }

        string GetDBObjectName(Type t)
        {
            return DbObjectsNameMapping.GetOrAdd(t.FullName, x =>
            {
                var name = t
                    .GetCustomAttributes(typeof(DBObjectNameAttribute), true)
                    .Cast<DBObjectNameAttribute>()
                    .Where(a => a!=null)
                    .Select(p => p.Name)
                    .FirstOrDefault();

                if (name == null) name = t.Name;
                return name;
            });
        }

        public abstract void ExecuteInsert<T>(string databaseObjectName, Analysis.InsertCommand<T> command);
        public abstract void ExecuteSave<T>(string databaseObjectName, Analysis.SaveCommand<T> command);
        public abstract void ExecuteInsertMany<T>(string databaseObjectName, Analysis.InsertManyCommand<T> command);
        public abstract void ExecuteUpdate<T>(string databaseObjectName, Analysis.UpdateCommand<T> command);
        public abstract void ExecuteDelete<T>(string databaseObjectName, Analysis.DeleteCommand<T> command);
        public abstract void ExecuteDeleteAll<T>(string databaseObjectName, Analysis.DeleteAllCommand<T> command);
        public abstract IEnumerable<T> ExecuteSelect<T>(string databaseObjectName, Analysis.SelectCommand<T> command);
        public abstract bool ObjectExists(string objectName);

        public T Scalar<T>(string sql, params object[] args)
        {
            return (T)Scalar(sql, args);
        }

        public object Scalar(string sql, params object[] args)
        {
            object result = null;
            using (var conn = OpenConnection())
            {
                var comm = CreateCommand(sql, args);
                comm.Connection = conn;
                result = comm.ExecuteScalar();
            }
            return result;
        }

        public void NonQuery(string sql, params object[] args)
        {
            using (var conn = OpenConnection())
            {
                var comm = CreateCommand(sql, args);
                comm.Connection = conn;
                comm.ExecuteNonQuery();
            }
        }

        public IEnumerable<T> Query<T>(string sql, params object[] args)
        {
            return DynamicQ(sql, args).Select(FastObjectAccessor.DynamicTo<T>);
        }

        public IEnumerable<dynamic> DynamicQ(string sql, params object[] args)
        {
            using (var conn = OpenConnection())
            {
                var comm = CreateCommand(sql, args);
                comm.Connection = conn;
                var rdr = comm.ExecuteReader(CommandBehavior.CloseConnection);
                while (rdr.Read())
                {
                    yield return rdr.RecordToExpando();
                }
            }
        }

        DbConnection OpenConnection()
        {
            var conn = Database.ProviderFactory.CreateConnection();
            conn.ConnectionString = Database.ConnectionString;
            conn.Open();
            return conn;
        }

        protected DbCommand CreateCommand(string sql, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(sql)) return null;
            DbCommand result = null;
            result = Database.ProviderFactory.CreateCommand();
            result.CommandText = sql;
            if (args != null && args.Length > 0) result.AddParams(args);
            return result;
        }


        protected int Execute(DbCommand command)
        {
            return Execute(new DbCommand[] { command });
        }
        /// <summary>
        /// Executes a series of DBCommands in a transaction
        /// </summary>
        protected int Execute(IEnumerable<DbCommand> commands)
        {
            var result = 0;
            using (var conn = OpenConnection())
            {
                using (var tx = conn.BeginTransaction())
                {
                    foreach (var cmd in commands)
                    {
                        cmd.Connection = conn;
                        cmd.Transaction = tx;
                        result += cmd.ExecuteNonQuery();
                    }
                    tx.Commit();
                }
            }
            return result;
        }

        public void RunNativeScript(string script)
        {
            if (string.IsNullOrEmpty(script)) return;

            var scripts = SplitSqlScript(script);

            if (scripts == null || scripts.Count() == 0) return;

            var all = scripts.Select(x => CreateCommand(x));

            Execute(all);

        }

        IEnumerable<string> SplitSqlScript(string script)
        {
            Regex splitter = new Regex(@"\n\s*(GO|go|Go|gO)\s*\n?");
            string[] scripts = splitter.Split(script);

            if (scripts == null || scripts.Length == 0 || (scripts.Length == 1 && string.IsNullOrWhiteSpace(scripts[0])))
                return null;

            var res = scripts.Where(s => !string.IsNullOrWhiteSpace(s) && s.Trim().ToLowerInvariant() != "go");

            return res;
        }
    }
}
