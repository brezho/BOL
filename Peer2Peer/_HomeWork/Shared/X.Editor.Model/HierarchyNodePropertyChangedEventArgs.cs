using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public class HierarchyNodePropertyChangedEventArgs : EventArgs
    {
        public HierarchyNode Item { get; private set; }
        public string PropertyName { get; private set; }
        public HierarchyNodePropertyChangedEventArgs(HierarchyNode item, string propertyName)
        {
            Item = item;
            PropertyName = propertyName;
        }
    }
}
