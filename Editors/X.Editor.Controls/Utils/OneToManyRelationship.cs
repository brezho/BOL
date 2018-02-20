using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Controls.Utils
{
    public enum AddRelationResult
    {
        AlreadyExists,
        NewSource,
        NewTarget
    }
    public class OneToManyRelationship<TSource, TTarget>
    {

        ConcurrentDictionary<TSource, HashSet<TTarget>> _relationship = new ConcurrentDictionary<TSource, HashSet<TTarget>>();
        Dictionary<TTarget, TSource> _reversedRelationship = new Dictionary<TTarget, TSource>();

        public AddRelationResult Add(TSource source, TTarget target)
        {
            var result = AddRelationResult.AlreadyExists;

            HashSet<TTarget> lst;
            if (!_relationship.TryGetValue(source, out lst))
            {
                result = AddRelationResult.NewSource;
                lst = _relationship.GetOrAdd(source, new HashSet<TTarget>());
            }
            if (lst.Add(target))
            {
                if (result == AddRelationResult.AlreadyExists) result = AddRelationResult.NewTarget;
                _reversedRelationship.Add(target, source);
            }

            return result;
        }
        public TSource[] Sources { get { return _relationship.Keys.ToArray() ?? Array.Empty<TSource>(); } }
        public TTarget[] Targets { get { return _reversedRelationship.Keys.ToArray() ?? Array.Empty<TTarget>(); } }

        public TTarget[] this[TSource source]
        {
            get
            {
                HashSet<TTarget> res = null;
                _relationship.TryGetValue(source, out res);
                if (res != null) return res.ToArray();
                return Array.Empty<TTarget>();
            }
        }
        public TSource this[TTarget target]
        {
            get
            {
                TSource res = default(TSource);
                _reversedRelationship.TryGetValue(target, out res);
                return res;
            }
        }

        public bool Contains(TSource source)
        {
            return _relationship.ContainsKey(source);
        }
        public bool Contains(TTarget source)
        {
            return _reversedRelationship.ContainsKey(source);
        }
    }
}
