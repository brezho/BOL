using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using X.Repository.Analysis;

namespace X.Repository
{
    public abstract class RepositoryBase<T>: IRepository<T>
    {
        public abstract IExecutor Executor
        {
            get; 
        }

        public IEnumerable<U> Where<U>(Expression<Func<U, bool>> whereClause) where U : T
        {
            var command = Analyser.BuildSelectCommand<U>(whereClause);
            return Executor.ExecuteSelect(command);
        }

        public void Update<U>(U o) where U : T
        {
            var command = Analyser.BuildUpdateCommand<U>(o);
            Executor.ExecuteUpdate(command);
        }

        public void Add<U>(U o) where U : T
        {
            var command = Analyser.BuildInsertCommand<U>(o);
            Executor.ExecuteInsert(command);
        }

        public void Save<U>(U o) where U : T
        {
            var command = Analyser.BuildSaveCommand<U>(o);
            Executor.ExecuteSave(command);
        }

        public void InsertMany<U>(IEnumerable<U> list) where U : T
        {
            var command = Analyser.BuildInsertManyCommand<U>(list);
            Executor.ExecuteInsertMany(command);
        }

        public void Delete<U>(U o) where U : T
        {
            var command = Analyser.BuildDeleteCommand<U>(o);
            Executor.ExecuteDelete(command);
        }

        public void DeleteAll<U>() where U : T
        {
            var command = Analyser.BuildDeleteAllCommand<U>();
            Executor.ExecuteDeleteAll(command);
        }

        public IEnumerable<U> All<U>() where U : T
        {
            var command = Analyser.BuildSelectCommand<U>(null);
            return Executor.ExecuteSelect(command);
        }

        public void Delete<U>(Expression<Func<U, bool>> whereClause) where U : T
        {
            var command = Analyser.BuildDeleteCommand<U>(whereClause);
            Executor.ExecuteDelete(command);
        }

        public IEnumerable<T> Where(Expression<Func<T, bool>> whereClause)
        {
            return Where<T>(whereClause);
        }

        public void Update(T o)
        {
            Update<T>(o);
        }

        public void Add(T o)
        {
            Add<T>(o);
        }

        public void Save(T o)
        {
            Save<T>(o);
        }

        public void InsertMany(IEnumerable<T> list)
        {
            InsertMany<T>(list);
        }

        public void Delete(T o)
        {
            Delete<T>(o);
        }

        public IEnumerable<T> All()
        {
            return All<T>();
        }

        public void Delete(Expression<Func<T, bool>> whereClause)
        {
            Delete<T>(whereClause);
        }

        public void DeleteAll()
        {
            DeleteAll<T>();
        }
    }
}
