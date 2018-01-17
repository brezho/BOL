using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public partial class HierarchyNode
    {
        public HierarchyNode<T> Add<T>(T item) where T : class
        {
            if (item is HierarchyNode)
            {
                throw new ApplicationException("Use AddNode method");
            }
            var node = new HierarchyNode<T>(item);
           // Children.Add(node);
            return (HierarchyNode<T>)Children.Add(node);
        }

        public T AddNode<T>(T node) where T : HierarchyNode
        {
            Children.Add(node);
            return node;
        }

        public bool Remove(HierarchyNode item)
        {
            return Children.Remove(item);
        }

        public static HierarchyNode GetNode(long id)
        {
            return allItems.FirstOrDefault(x => x.Id == id);
        }

        public void Select()
        {
            Root.SetSelected(this);
        }
    }
}
