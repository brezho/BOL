using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Model;

namespace X.Editor.Tools.PropertyGrid
{
    public class PropertyGridTool : ToolWindowBase
    {
        System.Windows.Forms.PropertyGrid Grid;
        public PropertyGridTool(Main main)
            : base(main)
        {
            Text = "Properties";
            Grid = new System.Windows.Forms.PropertyGrid();
            Grid.Dock = DockStyle.Fill;
            Controls.Add(Grid);

            main.HierarchyChanged += (snd, args) =>
            {

                main.Hierarchy.SelectedNodeChanged += (s, a) =>
                {
                    var current = Grid.SelectedObject as HierarchyNode;
                    if (current != null) current.PropertyChanged -= Item_PropertyChanged;
                    Grid.SelectedObject = a.Item;
                    a.Item.PropertyChanged += Item_PropertyChanged;
                };
            };
        }

        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Grid.Refresh();
        }
    }
}
