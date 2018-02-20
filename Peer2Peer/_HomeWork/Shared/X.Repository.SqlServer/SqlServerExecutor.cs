using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Repository.Analysis;
using System.Diagnostics;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq.Expressions;
using X.Repository.Databases;
using X.Repository.Databases.Helpers;

namespace X.Repository.SqlServer
{
    public class SqlServerExecutor : AdoNetQueryExecutorBase
    {
      //  public int VisitParamCounter = 0;

        public SqlServerExecutor(IDatabase database)
            : base(database)
        { }

        public override IEnumerable<T> ExecuteSelect<T>(string databaseObjectName, SelectCommand<T> command)
        {
            var columns = command.Projection.ToString(",", "[", "]");

            var whereClause = string.Empty;
            object[] args = null;
            if (command.Where != null)
            {
                var visitor = new WhereClauseVisitor<T>();

                var partialyEvaluated = (Expression<Func<T,bool>>) Evaluator.PartialEval(command.Where);
                var result = visitor.Convert(partialyEvaluated);

                whereClause = " where " + result.Item1;
                args = result.Item2;
            }
            var sql = string.Format("select {0} from {1}{2}; ", columns, databaseObjectName, whereClause);
            return base.Query<T>(sql, args);
        }

        public override void ExecuteInsertMany<T>(string databaseObjectName, InsertManyCommand<T> command)
        {
            var sql = GetInsertStatement<T>(databaseObjectName, command.InsertComands.First());
            var values = command.InsertComands.First().Values.Count().ToList(x => x.ToString()).ToString(",", "@", "");
            List<DbCommand> commands = new List<DbCommand>();
            foreach (var item in command.InsertComands)
            {
                commands.Add(CreateCommand(sql, item.Values.Select(x => x.Value).ToArray()));
            }
            base.Execute(commands);
        }

        string GetInsertStatement<T>(string databaseObjectName, Analysis.InsertCommand<T> command)
        {
            var columns = command.Values.Select(x => x.Key).ToString(",", "[", "]");
            var values = command.Values.Count().ToList(x => x.ToString()).ToString(",", "@", "");
            var sql = string.Format("insert into [{0}] ({1}) values ({2}); ", databaseObjectName, columns, values);
            return sql;
        }

        public override void ExecuteInsert<T>(string databaseObjectName, Analysis.InsertCommand<T> command)
        {
            var sql = GetInsertStatement<T>(databaseObjectName, command);
            var args = command.Values.Select(x => x.Value).ToArray();

            base.NonQuery(sql, args);
        }

        public override void ExecuteSave<T>(string databaseObjectName, Analysis.SaveCommand<T> command)
        {
            var keyColumnsList = command.KeyValues.ToList();
            var keyColumnsCount = keyColumnsList.Count();
            var whereClause = string.Join(" AND ", keyColumnsCount.ToList(x => "[" + keyColumnsList[x].Key + "] = @" + x.ToString()));

            var updateClauseColumnsList = command.ColumnValues.ToList();
            var updateColumnsCount = updateClauseColumnsList.Count();
            var updateSetClause = string.Join(", ", updateColumnsCount.ToList(x => "[" + updateClauseColumnsList[x].Key + "] = @" + (x + keyColumnsCount).ToString()));

            keyColumnsList.AddRange(updateClauseColumnsList);
            var insertedColumnsStatement = keyColumnsList.Select(x => x.Key).ToString(", ", "[", "]");
            var insertedValuesStatement = string.Join(", ", keyColumnsList.Count().ToList(x => "@" + x.ToString()));



            var sql = string.Format("if exists(select 1 from [{0}] where {1}) ", databaseObjectName, whereClause);
            sql += string.Format("update [{0}] set {1} where {2} ", databaseObjectName, updateSetClause, whereClause);
            sql += "else ";
            sql += string.Format("insert into [{0}] ({1}) values({2});", databaseObjectName, insertedColumnsStatement, insertedValuesStatement);


            var lstArgs = command.KeyValues.Select(x => x.Value).ToList();
            lstArgs.AddRange(command.ColumnValues.Select(x => x.Value));

            base.NonQuery(sql, lstArgs.ToArray());
        }

        public override void ExecuteUpdate<T>(string databaseObjectName, Analysis.UpdateCommand<T> command)
        {
            var args = new List<object>();

            var whereClause = string.Empty;
            if (command.Where != null)
            {
                var visitor = new WhereClauseVisitor<T>();
                // var result = visitor.Convert(command.Where);
                var partialyEvaluated = (Expression<Func<T, bool>>)Evaluator.PartialEval(command.Where);
                var result = visitor.Convert(partialyEvaluated);
                whereClause = " where " + result.Item1;
                args = result.Item2.ToList();
            }


            var columnsToUpdate = command.Set.Select(x =>
                {
                    var ret = "[" + x.Key + "] = @" + args.Count;
                    args.Add(x.Value);
                    return ret;
                }).ToString(",");

            var sql = string.Format("update [{0}] set {1}{2}", databaseObjectName, columnsToUpdate, whereClause);

            base.NonQuery(sql, args.ToArray());
        }


        public override void ExecuteDelete<T>(string databaseObjectName, DeleteCommand<T> command)
        {
            object[] args = null;
            var whereClause = string.Empty;
            if (command.Where != null)
            {
                var visitor = new WhereClauseVisitor<T>();
                // var result = visitor.Convert(command.Where);
                var partialyEvaluated = (Expression<Func<T, bool>>)Evaluator.PartialEval(command.Where);
                var result = visitor.Convert(partialyEvaluated);

                whereClause = " where " + result.Item1;
                args = result.Item2;
            }

            var sql = string.Format("delete from [{0}]{1}", databaseObjectName, whereClause);

            base.NonQuery(sql, args.ToArray());
        }

        public override void ExecuteDeleteAll<T>(string databaseObjectName, DeleteAllCommand<T> command)
        {
            var sql = string.Format("delete from [{0}]", databaseObjectName);
            base.NonQuery(sql);
        }

        public override bool ObjectExists(string objectName)
        {
            var result = base.Scalar<int>("select isnull(object_id(@0),-1)", objectName);
            return (result != -1);
        }
    }
}
