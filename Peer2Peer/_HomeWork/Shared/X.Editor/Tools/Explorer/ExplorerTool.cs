using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

using X.Editor.Model;

namespace X.Editor.Tools.Explorer
{
    public class ExplorerTool : ToolWindowBase
    {
        TreeView treeControl;

        public ExplorerTool(Main main)
            : base(main)
        {
            Text = "Explorer";
            treeControl = new TreeView();
            treeControl.Dock = DockStyle.Fill;
            treeControl.Font = Main.Consolas975;
            Controls.Add(treeControl);

            main.HierarchyChanged += (sender, hierachyArgs) =>
            {
                treeControl.Nodes.Clear();
                foreach (var it in main.Hierarchy.Descendants()) BindEditorItemToNode(it);
                main.Hierarchy.DescendantAdded += (ias, iaa) => BindEditorItemToNode(iaa.Item);
                main.Hierarchy.DescendantRemoved += (ias, iaa) => UnbindEditorItemToNode(iaa.Item);

                main.Hierarchy.SelectedNodeChanged += (sncs, snca) =>
                {
                    var tn = FindNode(snca.Item.Id);
                    if (tn != null) treeControl.SelectedNode = tn;
                };
            };

            treeControl.AfterSelect += (s, a) =>
            {
                var item = (HierarchyNode)a.Node.Tag;
                item.Root.SetSelected(item);
            };

            treeControl.NodeMouseDoubleClick += (s, a) =>
            {
                var item = (HierarchyNode)a.Node.Tag;
                item.Root.Activate(item);
            };
        }

        void ResetNodeProperties(HierarchyNode it)
        {
            var node = FindNode(it.Id);
            if (node != null)
            {
                node.Text = it.NodeDataAdapter.GetDisplayName();
            }
        }

        void BindEditorItemToNode(HierarchyNode it)
        {
            var existingNode = FindNode(it.Id);
            if (existingNode == null)
            {
                var parentNode = FindNode(it.Parent.Id);
                var addTo = (parentNode == null) ? treeControl.Nodes : parentNode.Nodes;
                var newNode = addTo.Add(it.Id.ToString(), it.NodeDataAdapter.GetDisplayName());
                it.Changed += it_Changed;
                newNode.Tag = it;

                foreach (var child in it.Nodes())
                {
                    BindEditorItemToNode(child);
                }
            }
        }

        void it_Changed(object sender, HierarchyNodePropertyChangedEventArgs e)
        {
            this.ResetNodeProperties(e.Item);
        }

        void UnbindEditorItemToNode(HierarchyNode it)
        {
            var nodeToRemove = FindNode(it.Id);
            var parentNode = FindNode(it.Parent.Id);
            it.Changed -= it_Changed;

            var removeFrom = (parentNode == null) ? treeControl.Nodes : parentNode.Nodes;
            removeFrom.Remove(nodeToRemove);
        }

        TreeNode FindNode(long id)
        {
            return treeControl.Nodes.Find(id.ToString(), true).FirstOrDefault();
        }
    }
}
