using System;
using System.Collections.Generic;
using System.Drawing;
using System.Helpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

using X.Editor.Model;

namespace X.Editor.Tools.Commander
{
    partial class X { }
    class CommanderTool : ToolWindowBase
    {
        TextBox InputBox;
        TableLayoutPanel Panel;

        NotifyingList<Command> InternalList;
        public CommanderTool(Main main)
            : base(main)
        {
            Text = "Act";

            Panel = new TableLayoutPanel();
            Panel.Dock = DockStyle.Fill;
            Panel.BackColor = Color.White;
            Panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

            Controls.Add(Panel);

            InputBox = new TextBox();
            InputBox.Dock = DockStyle.Top;
            InputBox.Font = Main.Consolas975;

            InputBox.TextChanged += (tcs, tca) => { ApplyFilter(); };
            Panel.Controls.Add(InputBox);

            InternalList = new NotifyingList<Command>();
            InternalList.ItemAdded += (ias, iaa) => BindCommandToListView(iaa.Item);
            InternalList.ItemRemoved += (ias, iaa) => UnbindCommandToListView(iaa.Item);

            main.Commands.ItemAdded += (ias, iaa) => InternalList.Add(iaa.Item);
            main.Commands.ItemRemoved += (ias, iaa) => InternalList.Remove(iaa.Item);

            foreach (var it in main.Commands) InternalList.Add(it);

            main.HierarchyChanged += (s, a) =>
            {
                main.Hierarchy.SelectedNodeChanged += Hierarchy_SelectedItemChanged;
            };
        }

        void Hierarchy_SelectedItemChanged(object sender, HierarchyNode e)
        {
            InternalList.Clear();

            foreach (var it in Shell.Commands) InternalList.Add(it);
            foreach (var it in e.Commands) InternalList.Add(it);

            ApplyFilter();

            e.Commands.ItemAdded += (ias, iaa) => InternalList.Add(iaa.Item);
            e.Commands.ItemRemoved += (ias, iaa) => InternalList.Remove(iaa.Item);
        }

        void BindCommandToListView(Command it)
        {
            var panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Top;

            panel.Tag = it;
            panel.AutoSize = true;
            panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;

            var lbl = new Label();
            lbl.Text = it.Text;
            lbl.Dock = DockStyle.Top;
            panel.Controls.Add(lbl);

            var desc = new Label();
            desc.Text = it.Description;
            desc.Dock = DockStyle.Top;
            panel.Controls.Add(desc);

            lbl.MouseClick += (mcs, mca) => { it.Invoke(); };
            desc.MouseClick += (mcs, mca) => { it.Invoke(); };

            Panel.Controls.Add(panel);
        }

        void UnbindCommandToListView(Command it)
        {
            var panel = Panel.Controls.OfType<Panel>().FirstOrDefault(x => x.Tag == it);
            if (panel != null)
            {
                Panel.Controls.Remove(panel);
            }
        }

        void ApplyFilter()
        {
            var searchedText = InputBox.Text.ToLowerInvariant();
            var commands = Shell.Commands.Where<Command>(X => X.Text.Safe().ToLowerInvariant().Contains(searchedText) || X.Description.Safe().ToLowerInvariant().Contains(searchedText)).ToList();
            foreach (var p in Panel.Controls.OfType<Panel>())
            {
                p.Visible = (searchedText.IsNullOrEmpty()) || commands.Contains((Command)p.Tag);
            }
        }
    }
}
