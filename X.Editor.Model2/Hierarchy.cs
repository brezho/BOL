using System;
using System.Collections.Generic;
using System.Text;

namespace X.Editor.Model2
{
    // a Hierarchy is a specific kind of HierarchyEntry which
    // acts as a root node
    // it is meant to be inherited

    public class Hierarchy : HierarchyEntry
    {
        public event EventHandler<ItemEventArgs> SelectedEntryChanged;
        public event EventHandler<ItemEventArgs> EntryActivated;
        public HierarchyEntry SelectedEntry { get; private set; }
        internal override Hierarchy InternalRoot { get { return this; } }

        protected Hierarchy()
        {
        }

        public void Select(HierarchyEntry entry)
        {
            if (SelectedEntry != entry)
            {
                SelectedEntry = entry;
                OnSelectedEntryChanged(new ItemEventArgs(entry));
            }
        }
        public void Activate(HierarchyEntry entry)
        {
            OnEntryActivated(new ItemEventArgs(entry));
        }

        protected virtual void OnSelectedEntryChanged(ItemEventArgs itemEventArgs)
        {
            if (SelectedEntryChanged != null) SelectedEntryChanged(this, itemEventArgs);
        }
        protected virtual void OnEntryActivated(ItemEventArgs itemEventArgs)
        {
            if (EntryActivated != null) EntryActivated(this, itemEventArgs);
        }
    }
}
