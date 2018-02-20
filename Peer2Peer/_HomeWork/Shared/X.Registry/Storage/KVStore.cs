using Microsoft.Database.Isam.Config;
using Microsoft.Isam.Esent.Collections.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace X.Registry.Storage
{
    public class KVStore<K, V> :
        IDisposable
        , IEnlistmentNotification
        , IDictionary<K, V>
        where K : IComparable<K>
    {
        PersistentDictionary<K, PersistentBlob> _persistentDictionary;
        Dictionary<K, PersistentBlob> _uncommitedChanges;
        HashSet<K> _uncommitedDelete;
        Func<V, byte[]> _toBytesConverter;
        Func<byte[], V> _fromBytesConverter;

        public KVStore(string location, Func<V, byte[]> toBytesConverter, Func<byte[], V> fromBytesConverter)
        {
            _persistentDictionary = new PersistentDictionary<K, PersistentBlob>(location);
            _uncommitedChanges = new Dictionary<K, PersistentBlob>();
            _uncommitedDelete = new HashSet<K>();
            _toBytesConverter = toBytesConverter;
            _fromBytesConverter = fromBytesConverter;
        }

        IEnumerable<K> EnumerateKeys()
        {
            bool inTransac = IsInTransaction();

            foreach (var it in _persistentDictionary.Keys)
            {
                if (!inTransac || (!_uncommitedDelete.Contains(it) && !_uncommitedChanges.ContainsKey(it)))
                {
                    yield return it;
                }
            }
            if (inTransac)
            {
                foreach (var it in _uncommitedChanges.Keys)
                {
                    yield return it;
                }
            }
        }


        void Set(K key, V value)
        {
            var blob = new PersistentBlob(_toBytesConverter(value));
            if (IsInTransaction())
            {
                _uncommitedDelete.Remove(key);
                _uncommitedChanges[key] = blob;
            }
            else
            {
                _persistentDictionary[key] = blob;
            }
        }
        V Get(K ptr)
        {
            V value = default(V);
            TryGetValue(ptr, out value);
            return value;
        }
        public void Add(K key, V value)
        {
            Set(key, value);
        }
        public void Add(KeyValuePair<K, V> item)
        {
            Add(item.Key, item.Value);
        }

        public bool TryGetValue(K ptr, out V value)
        {
            PersistentBlob res;
            if (IsInTransaction())
            {
                if (_uncommitedDelete.Contains(ptr))
                {
                    value = default(V);
                    return false;
                }
                if (_uncommitedChanges.TryGetValue(ptr, out res))
                {
                    value = _fromBytesConverter(res.GetBytes());
                    return true;
                }
            }

            if (_persistentDictionary.TryGetValue(ptr, out res))
            {
                value = _fromBytesConverter(res.GetBytes());
                return true;
            }
            value = default(V);
            return false;
        }
        public bool ContainsKey(K ptr)
        {
            if (IsInTransaction())
            {
                if (_uncommitedDelete.Contains(ptr)) return false;
                if (_uncommitedChanges.ContainsKey(ptr)) return true;
            }
            return _persistentDictionary.ContainsKey(ptr);
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return ContainsKey(item.Key);
        }

        public bool Remove(K ptr)
        {
            if (IsInTransaction())
            {
                _uncommitedChanges.Remove(ptr);
                return _uncommitedDelete.Add(ptr);
            }
            else return _persistentDictionary.Remove(ptr);
        }
        public bool Remove(KeyValuePair<K, V> item)
        {
            return Remove(item.Key);
        }

        public ICollection<K> Keys
        {
            get
            {
                return EnumerateKeys().ToList();
            }
        }

        public int Count
        {
            get
            {
                return this.Keys.Count();
            }
        }

        #region Transactional management

        bool isTransacting = false;
        bool IsInTransaction()
        {
            var ambientTransaction = Transaction.Current;
            if (ambientTransaction != null)
            {
                if (!isTransacting)
                {
                    ambientTransaction.EnlistVolatile(this, EnlistmentOptions.None);
                    isTransacting = true;
                }
            }
            return ambientTransaction != null;
        }
        void ApplyUncommited()
        {
            foreach (var it in _uncommitedDelete) _persistentDictionary.Remove(it);
            foreach (var it in _uncommitedChanges) _persistentDictionary[it.Key] = it.Value;
            _uncommitedDelete.Clear();
            _uncommitedChanges.Clear();
            isTransacting = false;
        }
        void RollbackUncommited()
        {
            _uncommitedDelete.Clear();
            _uncommitedChanges.Clear();
            isTransacting = false;
        }
        void IEnlistmentNotification.Commit(Enlistment enlistment)
        {
            ApplyUncommited();
            enlistment.Done();
        }

        void IEnlistmentNotification.InDoubt(Enlistment enlistment)
        {
            RollbackUncommited();
            enlistment.Done();
        }

        void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        void IEnlistmentNotification.Rollback(Enlistment enlistment)
        {
            RollbackUncommited();
            enlistment.Done();
        }


        #endregion

        #region Disposable and finalizer
        ~KVStore()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool _disposed;
        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _persistentDictionary.Flush();
                _persistentDictionary.Dispose();
                _persistentDictionary = null;
            }
            _disposed = true;
        }
        #endregion

        public ICollection<V> Values
        {
            get
            {
                return EnumerateKeys().Select(x => Get(x)).ToList();
            }
        }

        public V this[K key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value);
            }
        }


        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly
        {
            get { return _persistentDictionary.IsReadOnly; }
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            foreach (var k in EnumerateKeys())
            {
                yield return new KeyValuePair<K, V>(k, Get(k));
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
