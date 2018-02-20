using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Repository;

namespace Repositories.InMemory
{
    public class InMemoryRepository<T> : List<T>, IRepository<T>
    {
        List<DynamicsCache.PropertyAccessor> IdPropertiesAccessors;
        Func<T, T, bool> findByIds;

        public InMemoryRepository()
        {
            var keyPropertyNames = X.Repository.Analysis.Analyser.GetKeyMembers(typeof(T));

            IdPropertiesAccessors = DynamicsCache
                .GetPropertiesAccessors<T>()
                .Where(x => keyPropertyNames.Select(p => p.Name).Contains(x.PropertyName))
                .ToList();

            var getters = IdPropertiesAccessors.Select(a => a.Getter);

            findByIds = (x, y) => getters.All(g => g.DynamicInvoke(x) == g.DynamicInvoke(y));
        }


        public void Update(T o)
        {
            var inLst = this.Where(x => findByIds(o, x)).FirstOrDefault();
            if (inLst != null)
            {
                this.Remove(inLst);
                this.Add(o);
            }
        }

        public void Save(T o)
        {
            var inLst = this.Where(x => findByIds(o, x)).FirstOrDefault();
            if (inLst != null)
            {
                this.Remove(inLst);
            }
            this.Add(o);
        }

        public void InsertMany(IEnumerable<T> list)
        {
            this.AddRange(list);
        }

        public void Delete(T o)
        {
            var inLst = this.Where(x => findByIds(o, x)).FirstOrDefault();
            if (inLst != null)
            {
                this.Remove(inLst);
            }
        }

        public void Delete(System.Linq.Expressions.Expression<Func<T, bool>> whereClause)
        {
            var compiled = whereClause.Compile();
            Predicate<T> pred = x => compiled(x);

            this.RemoveAll(pred);

        }

        public void DeleteAll()
        {
            this.Clear();
        }

        public void Update<U>(U o) where U : T
        {
            this.Update((T)o);
        }

        public void Add<U>(U o) where U : T
        {
            base.Add(o);
        }

        public void Save<U>(U o) where U : T
        {
            this.Save((T)o);
        }

        public void InsertMany<U>(IEnumerable<U> list) where U : T
        {
            this.InsertMany(list.Select(x => (T)x));
        }

        public void Delete<U>(U o) where U : T
        {
            this.Delete((T)o);
        }

        public void Delete<U>(System.Linq.Expressions.Expression<Func<U, bool>> whereClause) where U : T
        {
            var where = whereClause.Compile();
            this.RemoveAll(x => where((U)x));
        }

        public void DeleteAll<U>() where U : T
        {
            this.Clear();
        }

        public IEnumerable<T> Where(System.Linq.Expressions.Expression<Func<T, bool>> whereClause)
        {
            var compiled = whereClause.Compile();
            return this.Where(compiled);

        }

        public IEnumerable<T> All()
        {
            return this;
        }

        public IEnumerable<U> Where<U>(System.Linq.Expressions.Expression<Func<U, bool>> whereClause) where U : T
        {
            return this.OfType<U>().Where(whereClause.Compile());
        }

        public IEnumerable<U> All<U>() where U : T
        {
            return this.OfType<U>();
        }
    }

}
