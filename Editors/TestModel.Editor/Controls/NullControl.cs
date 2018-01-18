using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestModel.Editor.Design;
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
            var knob = new KnobControl();
            var ts = new TimedSerie<int>();


            //oscillo.ValuesSource = ts;
            //ts.ValuesSource = knob;

            surface.Controls.Add(oscillo);
            //surface.Controls.Add(knob);

            surface.AdornWith<Positioner>(oscillo);
            surface.AdornWith<Resizer>(oscillo);

            this.Controls.Add(surface);



        }
    }
}
