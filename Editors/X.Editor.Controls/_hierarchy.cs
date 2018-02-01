using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using X.Editor.Controls.Adornment;
using X.Editor.Controls.Controls;
using X.Editor.Controls.Gdi;
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
        ShapeCollection Shapes = new ShapeCollection();
        GraphicsBuffer buffer;

        public MySurface()
        {
            buffer = new GraphicsBuffer(Size);
        }

        void Adorn(Control ctrl)
        {
            Adorn<Resizer>(ctrl);
            Adorn<Positioner>(ctrl);
            Adorn<Debugger>(ctrl);
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

            var knob = new KnobControl();
            knob.Location = new Point(0, 0);
            this.Controls.Add(knob);
            Adorn(knob);

            var oscillo = new Oscilloscope();
            oscillo.Location = new Point(400, 400);
            this.Controls.Add(oscillo);
            Adorn(oscillo);

            var t = new TestGraph();
            t.Location = new Point(200, 200);
            this.Controls.Add(t);
            Adorn(t);

            Shapes.Add(new Quadrilatere());
            Shapes.Add(new Figure());
        }
        protected override void OnResize(EventArgs e)
        {
            buffer.Resize(Size);
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            buffer.Draw((gr) =>
            {
                gr.Clear(Color.Transparent);
                foreach (var sh in Shapes)
                {
                    sh.Draw(gr);
                }
            });

            //var copy = buffer.Copy();
            //e.Graphics.DrawImageUnscaled(copy,0,0);

            //e.Graphics.DrawImageUnscaled(copy, e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height);

            // buffer.FlushTo(e.Graphics);
            base.OnPaint(e);

            e.Graphics.SetClip(Shapes.Bounds);
            buffer.FlushTo(e.Graphics);
        }
    }
}
