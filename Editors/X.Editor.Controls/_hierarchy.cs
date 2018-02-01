using System.Drawing;
using System.Windows.Forms;
using X.Editor.Controls.Adornment;
using X.Editor.Controls.Controls;
using X.Editor.Controls.Utils;
using X.Editor.Model;

namespace X.Editor.Controls
{
    public class TestHierarchyProvider : IHierarchyProvider
    {
        public Hierarchy CreateHierarchy(IEditorShell shell)
        {
            return new TestHierarchy(shell);
        }
    }
    public class TestHierarchy : Hierarchy
    {
        IEditorShell shell;

        public TestHierarchy(IEditorShell shell)
        {
            this.shell = shell;
            BindRoot();
        }

        void BindRoot()
        {
            var terminalsFolder = this.AddFolder("Test");
            terminalsFolder.RegisterEditorBuilder(typeof(Control), () => new MySurface() { Dock = DockStyle.Fill });
        }
    }

    public class MySurface : Surface, IEditor
    {
        IEditorContainer container;

        void Adorn(Control ctrl)
        {
            Adorn<Positioner>(ctrl);
            Adorn<Resizer>(ctrl);
            var connector = Adorn<Connector>(ctrl);
            connector.Add("titi", new Point(12, 12));
        }

        public void ActivateIn(IEditorContainer newWindow)
        {
            container = newWindow;

            var oscillo = new Oscilloscope();
            oscillo.Location = new Point(400, 400);
            this.Controls.Add(oscillo);
            Adorn(oscillo);

            var t = new TestGraph();
            t.Location = new Point(200, 200);
            this.Controls.Add(t);
            Adorn(t);

            //var b = new Button { Text = "Click", BackColor = Color.Beige, };
            //b.Click += (s, a) =>
            //{
            //    t.Location = new Point(50, 50);
            //    if (!first.HasValue) first = true;
            //    else
            //    {
            //        if (first == true)
            //        {
            //            Adorn<Resizer2>(t);
            //          //  Adorn<Positioner2>(t);
            //            //var connector = Adorn<Connector>(t);
            //            //connector.Add("toto", new Point(20, 20));
            //            //first = false;
            //        }
            //    }
            //};
            //this.Controls.Add(b);
        }

    }
}
