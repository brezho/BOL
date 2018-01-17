using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Helpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public partial class HierarchyNode : NotifyingObject// : INotifyPropertyChanged
    {
        public event EventHandler<HierarchyNodePropertyChangedEventArgs> Changed;

        public event EventHandler<HierarchyNodePropertyChangedEventArgs> ChildChanged;
        public event EventHandler<HierarchyNode> ChildAdded;
        public event EventHandler<HierarchyNode> ChildRemoved;

        public event EventHandler<HierarchyNodePropertyChangedEventArgs> DescendantChanged;
        public event EventHandler<HierarchyNode> DescendantAdded;
        public event EventHandler<HierarchyNode> DescendantRemoved;


        protected void NotifyChanged(HierarchyNodePropertyChangedEventArgs e)
        {
            //PropertyChanged(this, new PropertyChangedEventArgs(e.PropertyName));
            base.RaisePropertyChanged(e.PropertyName);
            Changed(this, e);
            if (Parent != null) Parent.NotifyChildChanged(e);
        }

        void NotifyChildChanged(HierarchyNodePropertyChangedEventArgs e)
        {
            if (ChildChanged != null) ChildChanged(this, e);
            NotifyDescendantChanged(e);
        }

        void NotifyChildAdded(HierarchyNode e)
        {
            if (ChildAdded != null) ChildAdded(this, e);
            NotifyDescendantAdded(e);
        }

        void NotifyChildRemoved(HierarchyNode e)
        {
            if (ChildRemoved != null) ChildRemoved(this, e);
            NotifyDescendantRemoved(e);
        }

        protected void NotifyDescendantChanged(HierarchyNodePropertyChangedEventArgs e)
        {
            if (DescendantChanged != null) DescendantChanged(this, e);
            if (Parent != null) Parent.NotifyDescendantChanged(e);
        }

        protected void NotifyDescendantAdded(HierarchyNode e)
        {
            if (DescendantAdded != null) DescendantAdded(this, e);
            if (Parent != null) Parent.NotifyDescendantAdded(e);
        }

        protected void NotifyDescendantRemoved(HierarchyNode e)
        {
            if (DescendantRemoved != null) DescendantRemoved(this, e);
            if (Parent != null) Parent.NotifyDescendantRemoved(e);
        }

        public virtual bool HandleUserInput(UserInput input)
        {
            return false;
        }
    }
}
