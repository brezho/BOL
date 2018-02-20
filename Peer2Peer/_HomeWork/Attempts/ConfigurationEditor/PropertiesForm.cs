using Configuration.Model.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Helpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ConfigurationEditor
{
    public partial class PropertiesForm : DockContent
    {
        PropertyGrid pg;
        IHierarchy _hierarchy;
        public IHierarchy Hierarchy
        {
            get { return _hierarchy; }
            set
            {
                if (value != null)
                {
                    _hierarchy = value;
                    _hierarchy.Commanded += hierarchy_Commanded;
                }
            }
        }
        public PropertiesForm(string text, IHierarchy hierarchy = null)
        {
            this.Text = text;
           
            pg = new PropertyGrid();
            pg.Font = new System.Drawing.Font("Verdana", 9.75F);
            pg.Dock = DockStyle.Fill;

            this.Controls.Add(pg);
            this.Hierarchy = hierarchy;
        }

        void hierarchy_Commanded(object sender, HierarchyCommandArgs e)
        {
            if (Enum<HierarchyCommands>.GetFullName(HierarchyCommands.Select) == e.CommandName)
            {
                this.pg.SelectedObject = e.CommandTarget.Data;
            }
        }
    }
}
