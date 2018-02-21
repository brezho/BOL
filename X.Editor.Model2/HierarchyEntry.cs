using System;
using System.Collections.Generic;

namespace X.Editor.Model2
{
    public class ItemsEventArgs : EventArgs
    {
        public HierarchyEntry[] Entries { get; private set; }

        public ItemsEventArgs(HierarchyEntry[] entries)
        {
            Entries = entries;
        }
    }
    public class ItemEventArgs : EventArgs
    {
        public HierarchyEntry Entry { get; private set; }

        public ItemEventArgs(HierarchyEntry entry)
        {
            Entry = entry;
        }
    }
    public class HierarchyEntry
    {
        protected static Table AllEntries = new Table();
        public long EntryId { get; private set; }
        internal HierarchyEntry()
        {
            EntryId = AllEntries.Register(this);
        }

        public Hierarchy Root { get { return InternalRoot; } }
        public HierarchyEntry Parent { get { return AllEntries.GetParentOf(this); } }
        internal virtual Hierarchy InternalRoot { get { return Parent.Root; } }

        public event EventHandler<ItemsEventArgs> ChildrenAdded;
        public event EventHandler<ItemsEventArgs> ChildrenRemoved;
        public event EventHandler<ItemsEventArgs> DescendantsAdded;
        public event EventHandler<ItemsEventArgs> DescendantsRemoved;
        public void AddChildren(params HierarchyEntry[] entries)
        {
            AllEntries.AddChildren(this, entries);

            var eventArg = new ItemsEventArgs(entries);
            OnChildrenAdded(eventArg);

            foreach (var ancestor in AllEntries.GetAncestorsOf(this))
            {
                ancestor.OnDescendantsAdded(eventArg);
            }
        }

        public void RemoveChildren(params HierarchyEntry[] entries)
        {
            AllEntries.RemoveChildren(this, entries);

            var eventArg = new ItemsEventArgs(entries);
            OnChildrenRemoved(eventArg);
            foreach (var ancestor in AllEntries.GetAncestorsOf(this))
            {
                ancestor.OnDescendantsRemoved(eventArg);
            }
        }
        protected virtual void OnChildrenAdded(ItemsEventArgs eventArgs)
        {
            if (ChildrenAdded != null) ChildrenAdded(this, eventArgs);
        }
        protected virtual void OnDescendantsAdded(ItemsEventArgs eventArgs)
        {
            if (DescendantsAdded != null) DescendantsAdded(this, eventArgs);
        }
        protected virtual void OnChildrenRemoved(ItemsEventArgs eventArgs)
        {
            if (ChildrenRemoved != null) ChildrenRemoved(this, eventArgs);
        }
        protected virtual void OnDescendantsRemoved(ItemsEventArgs eventArgs)
        {
            if (DescendantsRemoved != null) DescendantsRemoved(this, eventArgs);
        }
    }
}
