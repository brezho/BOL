using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Repository.Analysis;

namespace X.Repository
{
    public interface IExecutor
    {
        void ExecuteInsert<T>(InsertCommand<T> command);
        void ExecuteUpdate<T>(UpdateCommand<T> command);
        void ExecuteSave<T>(SaveCommand<T> command);
        void ExecuteDelete<T>(DeleteCommand<T> command);
        void ExecuteDeleteAll<T>(DeleteAllCommand<T> command);
        void ExecuteInsertMany<T>(InsertManyCommand<T> command);
        IEnumerable<T> ExecuteSelect<T>(SelectCommand<T> command);


        void RunNativeScript(string script);
        object Scalar(string sql, params object[] args);
        void NonQuery(string sql, params object[] args);
        IEnumerable<T> Query<T>(string sql, params object[] args);
        bool ObjectExists(string objectName);
    }
}
