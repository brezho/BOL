using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

using X.Editor.Model;

namespace X.Editor.Tools.Explorer
{
    partial class X { }
    public class ExplorerTool : ToolWindowBase
    {
        TreeView treeControl;
        SynchronizationContext context;
        public ExplorerTool(Main main)
            : base(main)
        {
            context = SynchronizationContext.Current;
            Text = "Explorer";
            treeControl = new TreeView();
            treeControl.HideSelection = false;
            treeControl.Dock = DockStyle.Fill;
            treeControl.Font = Main.Consolas975;
            Controls.Add(treeControl);

            main.HierarchyChanged += (s, a) =>
            {
                context.Post(st =>
                {
                    treeControl.Nodes.Clear();
                    foreach (var it in main.Hierarchy.Descendants()) BindEditorItemToNode(it);

                    main.Hierarchy.DescendantAdded += (ias, iaa) =>
                    {
                        context.Post(st2 => BindEditorItemToNode(iaa), null);
                    };
                    main.Hierarchy.DescendantRemoved += (ias, iaa) =>
                    {
                        context.Post(st2 => UnbindEditorItemToNode(iaa), null);
                    };

                    main.Hierarchy.SelectedNodeChanged += (sncs, snca) =>
                    {
                        context.Post(st2 =>
                            {
                                var tn = FindTreeNode(snca.Id);
                                if (tn != null) treeControl.SelectedNode = tn;
                            }, null);
                    };


                }, null);
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
            var node = FindTreeNode(it.Id);
            if (node != null)
            {
                node.Text = it.NodeDataAdapter.GetDisplayName();
            }
        }

        void BindEditorItemToNode(HierarchyNode it)
        {
            var existingNode = FindTreeNode(it.Id);
            if (existingNode == null)
            {
                var parentNode = FindTreeNode(it.Parent.Id);
                var addTo = (parentNode == null) ? treeControl.Nodes : parentNode.Nodes;

                TreeNode newNode = addTo.Add(it.Id.ToString(), it.NodeDataAdapter.GetDisplayName());
                newNode.Tag = it;
                it.Changed += it_Changed;

                if (it.Commands.Count > 0)
                {
                    newNode.ContextMenu = new ContextMenu();
                    foreach (var cmd in it.Commands)
                    {
                        AddCommandToMenu(cmd, newNode.ContextMenu);
                    }
                }
                it.Commands.ItemAdded += Commands_ItemAdded;
                it.Commands.ItemRemoved += Commands_ItemRemoved;

                foreach (var child in it.Nodes())
                {
                    BindEditorItemToNode(child);
                }
            }
        }

        void AddCommandToMenu(Command cmd, ContextMenu menu)
        {
            var menuItem = new MenuItem(cmd.Text);
            menuItem.Click += (s, a) => cmd.Invoke();
            menu.MenuItems.Add(menuItem);
        }

        void Commands_ItemRemoved(object sender, System.Helpers.ItemEventArgs<Command> e)
        {
            var treenode = FindTreeNode(e.Item.TargetNode.Id);
            if (treenode.ContextMenu != null)
            {
                var menu = treenode.ContextMenu.MenuItems.Find(e.Item.Text, false).FirstOrDefault();
                if (menu != null) treenode.ContextMenu.MenuItems.Remove(menu);
            }

        }

        void Commands_ItemAdded(object sender, System.Helpers.ItemEventArgs<Command> e)
        {
            var treenode = FindTreeNode(e.Item.TargetNode.Id);
            if (treenode.ContextMenu == null) treenode.ContextMenu = new ContextMenu();
            AddCommandToMenu(e.Item, treenode.ContextMenu);
        }

        void it_Changed(object sender, HierarchyNodePropertyChangedEventArgs e)
        {
            this.ResetNodeProperties(e.Item);
        }

        void UnbindEditorItemToNode(HierarchyNode it)
        {
            var nodeToRemove = FindTreeNode(it.Id);
            var parentNode = FindTreeNode(it.Parent.Id);
            it.Changed -= it_Changed;
            it.Commands.ItemAdded -= Commands_ItemAdded;
            it.Commands.ItemRemoved -= Commands_ItemRemoved;
            var removeFrom = (parentNode == null) ? treeControl.Nodes : parentNode.Nodes;
            removeFrom.Remove(nodeToRemove);
        }

        TreeNode FindTreeNode(long id)
        {
            return treeControl.Nodes.Find(id.ToString(), true).FirstOrDefault();
        }
    }
}
