using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public partial class HierarchyNode
    {
        public IEnumerable<HierarchyNode> Ancestors()
        {
            return Ascend(Parent);
        }

        public IEnumerable<HierarchyNode> AncestorsAndSelf()
        {
            return Ascend(this);
        }

        public IEnumerable<HierarchyNode> Nodes()
        {
            return Children;
        }
        public IEnumerable<HierarchyNode> Descendants()
        {
            return Descend(Children);
        }

        public IEnumerable<HierarchyNode> DescendantsAndSelf()
        {
            return Descend(new[] { this });
        }

        static IEnumerable<HierarchyNode> Descend(IEnumerable<HierarchyNode> startNodes)
        {
            IEnumerable<HierarchyNode> current = startNodes;
            foreach (var startingNode in startNodes)
            {
                yield return startingNode;
                foreach (var kid in Descend(startingNode.Children)) yield return kid;
            }
        }

        static IEnumerable<HierarchyNode> Ascend(HierarchyNode startNode)
        {
            var current = startNode;
            do
            {
                yield return current;
                current = current.Parent;
            }
            while (current != null);
        }
    }
}
