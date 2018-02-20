using SchemaBuilder.Model;
using SchemaBuilder.Packages.DataSetViewBox;
using SchemaBuilder.UIModel.Designers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using WeifenLuo.WinFormsUI.Docking;

namespace SchemaBuilder.UIModel
{
    public class IdeEntity : XObject
    {
        public IShell Shell { get { return Get(() => Shell); } set { Set(() => Shell, value); } }
        public string Name { get { return Get(() => Name); } set { Set(() => Name, value); } }
        public XList<IToolWindow> Tools { get { return Get(() => Tools); } set { Set(() => Tools, value); } }
        public XList<IView> Views { get { return Get(() => Views); } set { Set(() => Views, value); } }
        public XNotificationSource<string> Trace { get { return Get(() => Trace); } set { Set(() => Trace, value); } }
        public XObject SelectedObject { get { return Get(() => SelectedObject); } set { Set(() => SelectedObject, value); } }

        public XSchema Schema { get { return Get(() => Schema); } set { Set(() => Schema, value); } }
        public void Initialize(XSchema schema)
        {
            Name = "IX";
            Schema = schema;

            LoadOrTrace<IToolWindow>(p =>
            {
                p.Initialize(this);
                Tools.Add(p);
            }, p => "Tool {0}: initialization failed " + p.ToolName);


            Views.ItemAdded += (s, a) =>
            {
                a.Item.Initialize(this);
            };

        }

        void LoadOrTrace<T>(Action<T> @do, Func<T, string> @trace)
        {
            typeof(T).Hype().GetMatchingTypes(x => x.IsConcrete())
                    .GetOne<T>()
                    .ToList()
                    .ForEach(p =>
                    {
                        try
                        {
                            @do(p);
                        }
                        catch
                        {
                            // Log somewhere that initialization failed for package...
                            Trace.Notify(@trace(p));
                        }
                    });
        }

        internal void OnViewClosing(IView View, System.ComponentModel.CancelEventArgs e)
        {
            if (View.Model.IsDirty())
            {
                var otherViewsOfSameDocument = Views.Where(x => x.Model == View.Model && x != View).Any();
                if (!otherViewsOfSameDocument)
                {
                    var res = Shell.Ask("Confirm", "Do you want to save before closing?", MessageBoxButtons.YesNoCancel);
                    if (res == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                    if (res == DialogResult.Yes)
                    {
                        //  View.Model.Save();
                    }
                }
            }
            Views.Remove(View);
        }

        internal void NewDataSet()
        {
            var ds = new XDataSet() { Name = "New " + Schema.Name };

            foreach (var type in Schema.Types)
            {
                var tbl = new XDataTable();
                ds.Tables.Add(tbl);
            }


            var vw = new DataSetView();
            vw.Model = ds;

            this.Views.Add(vw);
        }
    }
}
