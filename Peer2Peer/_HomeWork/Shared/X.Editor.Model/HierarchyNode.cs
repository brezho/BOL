using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Helpers;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public partial class HierarchyNode 
    {
        static long Counter = 0;
        internal static List<HierarchyNode> allItems = new List<HierarchyNode>();
        public long Id { get; private set; }
        protected internal NotifyingList<HierarchyNode> Children { get; internal set; }
        public PropertiesList Properties { get; private set; }
        public CommandsList Commands { get; private set; }
        public virtual Hierarchy Root { get { return Parent.Root; } }
        public HierarchyNode Parent { get; private set; }

        object _tag;
        public object Tag
        {
            get { return _tag; }
            set
            {
                if (_tag != null) throw new ApplicationException("Tag already set, consider creating a new node");
                _tag = value;
                if (value is INotifyPropertyChanged)
                {
                    var inpc = value as INotifyPropertyChanged;
                    inpc.PropertyChanged += (s, a) =>
                    {
                        this.NotifyChanged(new HierarchyNodePropertyChangedEventArgs(this, a.PropertyName));
                    };
                }
            }
        }
        public HierarchyNode()
        {
            Id = System.Threading.Interlocked.Increment(ref Counter);
            allItems.Add(this);
            Children = new NotifyingList<HierarchyNode>();
            Properties = new PropertiesList();
            Commands = new CommandsList(this);

            Children.ItemAdded += (s, a) =>
            {
                a.Item.Parent = this;
                this.NotifyChildAdded(a);
            };
            Children.ItemRemoved += (s, a) =>
            {
                this.NotifyChildRemoved(a);
                a.Item.Parent = null;
            };
        }

        public override string ToString()
        {
            return string.Format("{0}, ({1})", this.GetType().Name, this.Id);
        }
    }
}
