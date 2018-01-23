using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls;
using X.Editor.Controls.Adornment;
using X.Editor.Model;

namespace TestModel.Editor.Controls
{
    class NullControlSuppressDesignTime { }
    class NullControl : Control, IEditor
    {


        public void ActivateIn(IEditorContainer newWindow)
        {

            var surface = new Surface(newWindow);
            var oscillo = new Oscilloscope();
            //var knob = new KnobControl();
            //var ts = new TimedSerie<int>();

            //oscillo.ValuesSource = ts;
            //ts.ValuesSource = knob;

            surface.Controls.Add(oscillo);
            //surface.Controls.Add(knob);

            surface.AdornWith<Resizer>(oscillo);
            surface.AdornWith<Positioner>(oscillo);
            var connectorAdorner = surface.AdornWith<Connector>(oscillo);
            connectorAdorner.Add("Src", new Point(20, 20));

            //surface.AdornWith<Resizer>(knob);
            //surface.AdornWith<Positioner>(knob);

            this.Controls.Add(surface);
        }
    }
}
