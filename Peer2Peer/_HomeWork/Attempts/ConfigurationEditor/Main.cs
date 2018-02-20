using Configuration.Model;
using Configuration.Model.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Helpers;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using WorkItems.Design;

namespace ConfigurationEditor
{
    public partial class Main : Form, IShell
    {
        IHierarchy hierarchy = new Hierarchy();
        public Main()
        {
            InitializeComponent();
            this.Font = new System.Drawing.Font("Verdana", 9.75F);

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Text = "Integrated Configuration Environment";
        }

        public TService GetService<TService>()
        {
            return (TService)GetService(typeof(TService));
        }

        protected override object GetService(Type service)
        {
            if (service == typeof(IShell))
                return this;

            return base.GetService(service);
        }


        WorkItems.WorkItemDefinition dein;

        private void ShowNewForm(object sender, EventArgs e)
        {
            dein = new WorkItems.WorkItemDefinition();
            dein.Fields.Add(new WorkItems.WorkItemField { Name = "TTT" });

            var root = hierarchy.Get(HierarchyId.Root);
            root.AddChild(hierarchy.GetItem(dein));

            hierarchy.Commanded += (s, a) =>
            {
                System.Diagnostics.Trace.WriteLine(a.CommandName);

                if (a.CommandName == Enum<HierarchyCommands>.GetFullName(HierarchyCommands.Open))
                {
                    MessageBox.Show("Open " + a.CommandTarget.GetName());
                }
            };

            var frm = new HierarchyForm("Explorer", hierarchy);
            var pg = new PropertiesForm("Properties", hierarchy);

            frm.Show(this.dockPanel1, DockState.DockRight);
            pg.Show(this.dockPanel1, DockState.DockLeft);
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.CheckFileExists = true;
            openDialog.Filter = "XPackage file|*.pkgx";
            openDialog.Title = "Open Package";

            if (openDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                var filePath = openDialog.FileName;
                // var package = new X.Packaging.PackageDescriptor { Path = filePath  };

                // open the package and find root node and build a ConfigurationNode out of it

            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {

            //var frm = (PackageEditorForm)this.dockPanel1.ActiveDocument;
            //string filePath = null;
            //if (frm.FilePath.IsNullOrEmpty())
            //{
            //    var saveDialog = new SaveFileDialog();
            //    saveDialog.Title = "Create New Package";
            //    saveDialog.Filter = "XPackage file|*.pkgx";
            //    saveDialog.DefaultExt = ".pkgx";
            //    saveDialog.OverwritePrompt = true;

            //    if (saveDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            //    {
            //        filePath = saveDialog.FileName;

            //        //var package = X.Packaging.Helper.GeneratePackage(filePath, "Hello");

            //    }

            //}
            //if (filePath.IsFilled())
            //{
            //    // var node = frm.RootNode.ToNode();

            //}

        }
    }
}
