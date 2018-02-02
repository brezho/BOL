using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
        GraphicsBuffer buffer;

        public MySurface()
        {
            buffer = new GraphicsBuffer(Size);
        }

        void Adorn(Control ctrl)
        {
            var positioner = Adorn<Positioner>(ctrl);
            positioner.IsVisibleOnFocusOf(ctrl);

            var resizer = Adorn<Resizer>(ctrl);
            resizer.IsVisibleOnFocusOf(ctrl);

            var debugger = Adorn<Debugger>(ctrl);
            debugger.IsVisibleOnFocusOf(ctrl);

        }

        public override void Log(params object[] args)
        {
            var str = new StringBuilder();
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (i % 2 == 0) str.Append('[' + args[i]?.ToString() + ']');
                    else str.Append(args[i]?.ToString());
                }
            }
            container.Shell.TraceLine(str.ToString());
            System.Diagnostics.Trace.WriteLine(str.ToString());
            base.Log(args);
        }

        public void ActivateIn(IEditorContainer newWindow)
        {
            container = newWindow;

            var knob = new KnobControl();
            knob.Location = new Point(0, 0);
            this.Controls.Add(knob);
            Adorn(knob);

            var knobOut = Adorn<Connector>(knob).Add("knobOut", new Point(12, 12), Color.SkyBlue);

            knobOut.ConnectedTo += (s, a) =>
            {
                Log(a.Source.Name + " connected to ", a.Destination.Name);
            };


            var oscillo = new Oscilloscope();
            oscillo.Location = new Point(400, 400);
            this.Controls.Add(oscillo);
            Adorn(oscillo);

            var oscilloIn = Adorn<Connector>(oscillo).Add("oscilloIn", new Point(12, 12), Color.Pink);

            oscilloIn.ConnectedFrom += (s, a) =>
            {
                var remoteKnob = (KnobControl)a.Source.Target;
                remoteKnob.OnNext += (rs, ra) =>
                {
                    oscillo.Add(ra);
                };

            };

            var t = new TestGraph();
            t.Location = new Point(200, 200);
            this.Controls.Add(t);
            Adorn(t);

        }


        protected override void OnResize(EventArgs e)
        {
            buffer.Resize(Size);
            base.OnResize(e);
        }
    }
}
