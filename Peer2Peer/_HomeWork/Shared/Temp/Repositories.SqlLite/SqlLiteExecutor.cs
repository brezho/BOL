using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Repositories.Databases.Helpers;
using Repositories.Analysis;
using System.Data.Common;
using Repositories.Databases;

namespace Repositories.SqlLite
{
    class SqlLiteExecutor : AdoNetQueryExecutorBase
    {
        public SqlLiteExecutor(IDatabase database)
            : base(database)
        { }

        public override IEnumerable<T> ExecuteSelect<T>(string databaseObjectName, SelectCommand<T> command)
        {
            var columns = command.Projection.ToString(",");

            var whereClause = string.Empty;
            object[] args = null;
            if (command.Where != null)
            {
                var visitor = new WhereClauseVisitor<T>();

                var partialyEvaluated = (Expression<Func<T, bool>>)Evaluator.PartialEval(command.Where);
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
            var columns = command.Values.Select(x => x.Key).ToString(",");
            var values = command.Values.Count().ToList(x => x.ToString()).ToString(",", "@", "");
            var sql = string.Format("insert into {0} ({1}) values ({2}); ", databaseObjectName, columns, values);
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
            var merged = command.KeyValues.Union(command.ColumnValues).Distinct().ToList();

            var insertColumns = string.Join(", ", merged.Select(x => x.Key));
            var values = string.Join(", ", merged.Count().ToList(x => "@" + x.ToString()));

            var insertStatement = string.Format("insert or ignore into {0}({1}) values({2});", databaseObjectName, insertColumns, values);

            var keysStatement = string.Join(" and ", merged.Count().ToList(x => command.KeyValues.Keys.Contains(merged[x].Key) ? merged[x].Key + " = @" + x.ToString() : null).Where(x => x != null));
            var columnsStatement = string.Join(", ", merged.Count().ToList(x => command.ColumnValues.Keys.Contains(merged[x].Key) ? merged[x].Key + " = @" + x.ToString() : null).Where(x => x != null));

            var updateStatement = string.Format("update {0} set {1} where {2};", databaseObjectName, columnsStatement, keysStatement);

            var sql = insertStatement + updateStatement;
            var args = merged.Select(x => x.Value).ToArray();

            base.NonQuery(sql, args);

        }

        public override void ExecuteUpdate<T>(string databaseObjectName, Analysis.UpdateCommand<T> command)
        {
            var args = new List<object>();

            var whereClause = string.Empty;
            if (command.Where != null)
            {
                var visitor = new WhereClauseVisitor<T>();
                var partialyEvaluated = (Expression<Func<T, bool>>)Evaluator.PartialEval(command.Where);
                var result = visitor.Convert(partialyEvaluated);
                whereClause = " where " + result.Item1;
                args = result.Item2.ToList();
            }


            var columnsToUpdate = command.Set.Select(x =>
            {
                var ret = x.Key + " = @" + args.Count;
                args.Add(x.Value);
                return ret;
            }).ToString(",");

            var sql = string.Format("update {0} set {1}{2}", databaseObjectName, columnsToUpdate, whereClause);

            base.NonQuery(sql, args.ToArray());
        }


        public override void ExecuteDelete<T>(string databaseObjectName, DeleteCommand<T> command)
        {
            object[] args = null;
            var whereClause = string.Empty;
            if (command.Where != null)
            {
                var visitor = new WhereClauseVisitor<T>();
                var partialyEvaluated = (Expression<Func<T, bool>>)Evaluator.PartialEval(command.Where);
                var result = visitor.Convert(partialyEvaluated);

                whereClause = " where " + result.Item1;
                args = result.Item2;
            }

            var sql = string.Format("delete from {0}{1}", databaseObjectName, whereClause);

            base.NonQuery(sql, args.ToArray());
        }

        public override void ExecuteDeleteAll<T>(string databaseObjectName, DeleteAllCommand<T> command)
        {
            var sql = string.Format("delete from {0}", databaseObjectName);
            base.NonQuery(sql);
        }

        public override bool ObjectExists(string objectName)
        {
            bool result = false;
            var command = CreateCommand(string.Format("select 1 from {0} limit 1", objectName));
            try
            {
                var commandRresult = base.Execute(command);
                result = true;
            }
            finally { }

            return result;
        }
    }
}

