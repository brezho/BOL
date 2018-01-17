using System;
using System.Collections.Generic;
using System.Helpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public class FolderNode : HierarchyNode
    {
        public string Name { get { return Get(() => Name); } set { Set(value); } }
        public FolderNode(string name)
        {
            Name = name;
            this.CreateBoundedPropertyFor(this, x => x.Name, isReadonly: true);
        }
        public override string ToString()
        {
            return Name;
        }

    }

    public static class FolderExt
    {
        public static FolderNode AddFolder(this HierarchyNode node, string name)
        {
            var child = new FolderNode(name);
            node.Children.Add(child);
            return child;
        }
    }

}
