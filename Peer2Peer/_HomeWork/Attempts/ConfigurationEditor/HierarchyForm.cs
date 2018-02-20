using Configuration.Model;
using Configuration.Model.Design;
using Framework.Resources.Images.Icons;
using System;
using System.Collections.Generic;
using System.Helpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ConfigurationEditor
{
    public class HierarchyForm : DockContent
    {
        TreeView tv;
        IHierarchy _hierarchy;
        public IHierarchy Hierarchy
        {
            get { return _hierarchy; }
            set
            {
                if (value != null)
                {
                    _hierarchy = value;
                    var root = _hierarchy.Get(HierarchyId.Root);
                    foreach (var child in root.Children) BindItem(child);
                    //BindItem();
                }
            }
        }

        void BindItem(HierarchyItem item)
        {
            if (item != null)
            {
                TreeNode node = FindNodeWithId(item.Id);
                if (node == null && !item.Id.Equals(HierarchyId.Root))
                {
                    var parentNodeCollection = FindNodeCollection(item.Parent);
                    node = parentNodeCollection.Add(item.Id.ToString(), item.GetName());
                    node.Tag = item;

                    if (node.ContextMenuStrip == null)
                    {
                        node.ContextMenuStrip = new ContextMenuStrip();
                        node.ContextMenuStrip.ImageList = Framework.Resources.Images.Icons.IconHelper.GetList();
                    }

                    foreach (var comm in item.Commands)
                    {
                        var menuItem = new ToolStripMenuItem(comm.Text);
                        menuItem.Click += (s, a) =>
                        {
                            comm.Do();
                            _hierarchy.Command(comm.CommandName, item, comm.CommandArg);
                        };
                        menuItem.ImageKey = comm.Icon.ToString();
                        node.ContextMenuStrip.Items.Add(menuItem);
                    }

                    item.Commands.ItemAdded += (s, it) =>
                    {
                        var menuItem = new ToolStripMenuItem(it.Item.Text);
                        menuItem.Click += (m, a) =>
                        {
                            it.Item.Do();
                            _hierarchy.Command(it.Item.CommandName, item, it.Item.CommandArg);
                        };
                        menuItem.ImageKey = it.Item.Icon.ToString();
                        node.ContextMenuStrip.Items.Add(menuItem);
                    };
                    item.Children.ItemAdded += (s, it) =>
                    {
                        BindItem(it.Item);
                    };
                }
                foreach (var child in item.Children)
                {
                    BindItem(child);
                }
            }
        }

        TreeNode FindNodeWithId(HierarchyId id)
        {
            return tv.Nodes.Find(id.ToString(), true).FirstOrDefault();
        }

        TreeNodeCollection FindNodeCollection(HierarchyId id)
        {
            if (id.Equals(HierarchyId.Nil)) return tv.Nodes;
            if (id.Equals(HierarchyId.Root)) return tv.Nodes;
            return FindNodeWithId(id).Nodes;
        }

        public HierarchyForm(string text, IHierarchy hierarchy = null)
        {
            this.Text = text;
            tv = new TreeView();
            tv.Font = new System.Drawing.Font("Verdana", 9.75F);

            tv.Dock = DockStyle.Fill;
            this.tv.ImageList = IconHelper.GetList();
            this.Controls.Add(tv);

            this.tv.AfterSelect += (s, a) =>
            {
                var hieraechyItem = (HierarchyItem)a.Node.Tag;
                _hierarchy.Command(Enum<HierarchyCommands>.GetFullName(HierarchyCommands.Select), hieraechyItem);
            };

            this.tv.NodeMouseDoubleClick += (s, a) =>
            {
                var hieraechyItem = (HierarchyItem)a.Node.Tag;
                _hierarchy.Command(Enum<HierarchyCommands>.GetFullName(HierarchyCommands.Open), hieraechyItem);
            };


            this.Hierarchy = hierarchy;

        }
    }
}
