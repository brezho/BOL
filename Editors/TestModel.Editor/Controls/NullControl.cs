using System;
using System.Collections.Generic;
using System.Drawing;
using System.Helpers;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls;
using X.Editor.Controls.Adornment;
using X.Editor.Model;

namespace TestModel.Editor.Controls
{
    class NullControlSuppressDesignTime { }
    class NullControl : Control, IEditor, IPublisher<int>
    {
        public event EventHandler<int> OnNext = (s, a) => { };
        public NullControl()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    OnNext(this, DateTime.Now.Second);
                    Thread.Sleep(200);
                }

            }, CancellationToken.None);
        }
        public void ActivateIn(IEditorContainer newWindow)
        {
           var surface = new Surface(newWindow);

            var ts = new TimedSerie<int>();
            var oscillo = new Oscilloscope();
            var knob = new KnobControl();

            oscillo.Location = new Point(200, 200);

            surface.Controls.Add(oscillo);
            surface.Controls.Add(knob);

            ts.ValuesSource = knob;
          //  oscillo.ValuesSource = ts;

            surface.AdornWith<Resizer>(oscillo);
            surface.AdornWith<Positioner>(oscillo);

            var connectorAdorner = surface.AdornWith<Connector>(oscillo);
            connectorAdorner.Add("Src", new Point(20, 20));

            surface.AdornWith<Resizer>(knob);
            surface.AdornWith<Positioner>(knob);

            this.Controls.Add(surface);
        }
    }
}
