using System;
using System.Collections.Generic;

namespace X.Editor.Model2
{
    public class HierarchyEntryEventArgs : EventArgs
    {
        public HierarchyEntry[] Entries { get; private set; }
        public HierarchyEntryEventArgs(HierarchyEntry hierarchyEntry)
        {
            Entries = new[] { hierarchyEntry };
        }
        public HierarchyEntryEventArgs(HierarchyEntry[] entries)
        {
            Entries = entries;
        }
    }
    public class HierarchyEntry
    {
        protected static Table AllEntries = new Table();
        public long EntryId { get; private set; }
        protected HierarchyEntry()
        {
            EntryId = AllEntries.Register(this);
        }

        public event EventHandler<HierarchyEntryEventArgs> ChildrenAdded;
        public event EventHandler<HierarchyEntryEventArgs> ChildrenRemoved;
        public event EventHandler<HierarchyEntryEventArgs> DescendantsAdded;
        public event EventHandler<HierarchyEntryEventArgs> DescendantsRemoved;
        public void AddChildren(params HierarchyEntry[] entries)
        {
            AllEntries.AddChildren(this, entries);

            var eventArg = new HierarchyEntryEventArgs(entries);
            OnChildrenAdded(eventArg);

            foreach (var ancestor in AllEntries.GetAncestorsOf(this))
            {
                ancestor.OnDescendantsAdded(eventArg);
            }
        }

        public void RemoveChildren(params HierarchyEntry[] entries)
        {
            AllEntries.RemoveChildren(this, entries);

            var eventArg = new HierarchyEntryEventArgs(entries);
            OnChildrenRemoved(eventArg);
            foreach (var ancestor in AllEntries.GetAncestorsOf(this))
            {
                ancestor.OnDescendantsRemoved(eventArg);
            }
        }
        protected virtual void OnChildrenAdded(HierarchyEntryEventArgs eventArgs)
        {
            if (ChildrenAdded != null) ChildrenAdded(this, eventArgs);
        }
        protected virtual void OnDescendantsAdded(HierarchyEntryEventArgs eventArgs)
        {
            if (DescendantsAdded != null) DescendantsAdded(this, eventArgs);
        }
        protected virtual void OnChildrenRemoved(HierarchyEntryEventArgs eventArgs)
        {
            if (ChildrenRemoved != null) ChildrenRemoved(this, eventArgs);
        }
        protected virtual void OnDescendantsRemoved(HierarchyEntryEventArgs eventArgs)
        {
            if (DescendantsRemoved != null) DescendantsRemoved(this, eventArgs);
        }
    }
}
