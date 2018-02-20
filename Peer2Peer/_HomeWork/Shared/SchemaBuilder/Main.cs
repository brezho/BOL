using SchemaBuilder.Model;
using SchemaBuilder.UIModel;
using SchemaBuilder.UIModel.Designers;
using SchemaBuilder.UIModel.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Reflection;

namespace SchemaBuilder
{
    public partial class Main : Form, IShell
    {
        //private List<ToolWindow> _tools = new List<ToolWindow>();
        UIModel.IdeEntity _model = new IdeEntity();
        UIModel.IdeEntity IDEModel { get { return _model; } }
        public Main()
        {
            InitializeComponent();

            IDEModel.Shell = this;

            IDEModel.Views.ItemAdded += (s, a) =>
            {
                var toolWindow = new DesignerWindow();
                toolWindow.Initialize(IDEModel);
                toolWindow.View = a.Item;
                toolWindow.Show(dockPanel1, DockState.Document);
            };

            //IDEModel.NewCommandList.ItemAdded += (s, a) =>
            //{
            //    newToolStripButton.DropDownItems.Add(a.Item.Text, null, (x, z) =>
            //    {
            //        a.Item.OnClick(); // IDEModel.NewDocument(a.Item);
            //    });
            //};

            IDEModel.Tools.ItemAdded += (s, a) =>
            {
                var toolWindow = new ToolWindow();
                toolWindow.Initialize(IDEModel);
                toolWindow.TabText = a.Item.ToolName;
                toolWindow.Controls.Add(a.Item.Control);
                a.Item.Control.Dock = DockStyle.Fill;
                toolWindow.Show(dockPanel1, a.Item.DockState);
               // _tools.Add(toolWindow);

            };

           this.DataBind(() => Text, IDEModel, () => IDEModel.Name);


            Load += Main_Load;
        }

        void Main_Load(object sender, EventArgs e)
        {          
            var schematxt = File.ReadAllText(".\\schema.xml");


            var schema = schematxt.FromXML<XSchema>(typeof(XPropertySpec).Hype().GetMatchingTypes().ToArray());
            IDEModel.Initialize(schema);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

            IDEModel.Trace.Notify("safdsadfds");

            //if (IDEModel.Documents.Count > 0)
            //{
            //    IDEModel.Documents.RemoveAt(0);
            //}
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            //var dlg = new OpenFileDialog();
            //dlg.CheckFileExists = true;
            //dlg.Filter = IDEModel.Handlers.Select(x => x.Name.Append("|*").Append(x.DefaultExtension.PrependInCase("."))).ToString("|");
            //dlg.FilterIndex = 1;
            //if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    //AppData.OpenFile(dlg.FileName);
            //    //var f = dlg.FileName;
            //    //var xml = File.ReadAllText(f);
            //    //var matching = typeof(IGenerated).GetMatchingTypes(x => x.IsConcrete()).ToArray();
            //    //var stuff = xml.FromXML<DataContainer>(matching);
            //}
        }

        public DialogResult Ask(string caption, string message, MessageBoxButtons buttons )
        {
            return MessageBox.Show(message, caption, buttons);
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            IDEModel.NewDataSet();
        }
    }

    public static class ContrExt
    {
        public static void DataBind(this Control ctrl, Expression<Func<string>> ctrlProperty, XObject dataSource, Expression<Func<string>> dataMember)
        {
            var dataMemberName = dataMember.ToPropertyName();
            ctrl.DataBindings.Add(ctrlProperty.ToPropertyName(), dataSource, dataMemberName);
            //dataSource.PropertyChanged += (s, a) =>
            //{
            //    if (a.PropertyName == dataMemberName)
            //    {

            //    }
            //};
        }
    }
}
