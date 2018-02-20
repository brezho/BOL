using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace X.Editor.Model2
{
    public class Table
    {
        long counter = 0;
        static readonly HierarchyEntry[] Empty = new HierarchyEntry[0];

        Dictionary<long, HierarchyEntry> _allEntries = new Dictionary<long, HierarchyEntry>();
        ConcurrentDictionary<HierarchyEntry, HashSet<HierarchyEntry>> _children = new ConcurrentDictionary<HierarchyEntry, HashSet<HierarchyEntry>>();
        Dictionary<HierarchyEntry, HierarchyEntry> _parents = new Dictionary<HierarchyEntry, HierarchyEntry>();
        
        internal Table()
        {

        }

        internal long Register(HierarchyEntry hierarchyEntry)
        {
            var newId = Interlocked.Increment(ref counter);
            _allEntries.Add(newId, hierarchyEntry);
            return newId;
        }

        public HierarchyEntry[] GetChildrenOf(HierarchyEntry entry)
        {
            HashSet<HierarchyEntry> res = null;
            if (_children.TryGetValue(entry, out res)) return res.ToArray();
            return Empty;
        }

        public IEnumerable<HierarchyEntry> GetDescendantsOf(HierarchyEntry entry, bool includeSelf = false)
        {
            var children = GetChildrenOf(entry);
            while (children != Empty)
            {
                foreach (var child in children)
                {
                    foreach (var grand in GetDescendantsOf(child, true)) yield return grand;
                }
            }
            if (includeSelf) yield return entry;
        }
        public HierarchyEntry GetParentOf(HierarchyEntry entry)
        {
            HierarchyEntry res = null;
            _parents.TryGetValue(entry, out res);
            return res;
        }
        public IEnumerable<HierarchyEntry> GetAncestorsOf(HierarchyEntry entry, bool includeSelf = false)
        {
            if (includeSelf) yield return entry;
            var parent = GetParentOf(entry);
            while (parent != null)
            {
                yield return parent;
                parent = GetParentOf(parent);
            }
        }

        internal void AddChildren(HierarchyEntry parent, HierarchyEntry[] entries)
        {
            HashSet<HierarchyEntry> lst;
            if (!_children.TryGetValue(parent, out lst))
            {
                lst = _children.GetOrAdd(parent, new HashSet<HierarchyEntry>());
            }

            foreach (var child in entries) lst.Add(child);
        }
        internal void RemoveChildren(HierarchyEntry parent, HierarchyEntry[] entries)
        {
            HashSet<HierarchyEntry> lst;
            if (_children.TryGetValue(parent, out lst))
            {
                foreach (var child in entries) lst.Remove(child);
            }
        }
    }
}
