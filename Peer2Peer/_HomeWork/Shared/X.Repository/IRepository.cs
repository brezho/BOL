using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace X.Repository
{
    public interface IReadOnlyRepository<T>
    {
        // Parents methods
        IEnumerable<T> Where(Expression<Func<T, bool>> whereClause);
        IEnumerable<T> All();

        // Same for inherited objects
        IEnumerable<U> Where<U>(Expression<Func<U, bool>> whereClause) where U : T;
        IEnumerable<U> All<U>() where U : T;
    }

    public interface IRepository<T> : IReadOnlyRepository<T>
    {
        //void Update(T o);
        //void Add(T o);
        //void Save(T o);
        //void InsertMany(IEnumerable<T> list);
        //void Delete(T o);
        //void Delete(Expression<Func<T, bool>> whereClause);
        //void DeleteAll();

        void Update<U>(U o) where U : T;
        void Add<U>(U o) where U : T;
        void Save<U>(U o) where U : T;
        void InsertMany<U>(IEnumerable<U> list) where U : T;
        void Delete<U>(U o) where U : T;
        void Delete<U>(Expression<Func<U, bool>> whereClause) where U : T;
        void DeleteAll<U>() where U : T;
    }

    public static class RepoExtensions
    {
        public static IReadOnlyRepository<T> AsReadOnly<T>(this IRepository<T> item)
        {
            return (IReadOnlyRepository<T>)item;
        }
    }
}
