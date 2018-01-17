using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public partial class HierarchyNode<T> : HierarchyNode where T : class
    {
        public new T Tag { get { return base.Tag as T; } set { base.Tag = value; } }
        public HierarchyNode(T item)
            : base()
        {
            Tag = item;
        }
    }
}
