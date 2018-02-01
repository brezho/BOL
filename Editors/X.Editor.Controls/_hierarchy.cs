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

        public override void Log(params object[] args)
        {
            base.Log(args);
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (i % 2 == 0) container.Shell.Trace('[' + args[i]?.ToString() + ']');
                    else container.Shell.Trace(args[i]?.ToString());
                }
            }
            container.Shell.TraceLine();
        }

        public void ActivateIn(IEditorContainer newWindow)
        {
            container = newWindow;

            var oscillo = new Oscilloscope2();
            oscillo.Location = new Point(400, 400);
            this.Controls.Add(oscillo);
            Adorn(oscillo);

            var t = new TestGraph2();
            t.Location = new Point(200, 200);
            this.Controls.Add(t);
            Adorn(t);
        }
    }
}
