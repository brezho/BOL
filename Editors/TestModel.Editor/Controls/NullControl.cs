using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestModel.Editor.Design;
using X.Editor.Model;

namespace TestModel.Editor.Controls
{
    class NullControlSuppressDesignTime { }
    class NullControl : Control, IEditor
    {
       

        public void ActivateIn(IEditorContainer newWindow)
        {
            newWindow.Shell.TraceLine("NullControl loaded");
            newWindow.Shell.TraceLine("Size: " + this.Size.ToString());

            var oscillo = new Oscilloscope();
            var knob = new KnobControl();
            var ts = new TimedSerie<int>();

            this.Controls.Add(oscillo);
            this.Controls.Add(knob);

            Adorner.MakeMovable(knob, newWindow);
            Adorner.MakeMovable(oscillo, newWindow);

            //Adorner.MakeHandles(knob, newWindow);
            Adorner.MakeHandles(oscillo, newWindow);

            Adorner.MakeResizable(knob, newWindow);
            Adorner.MakeResizable(oscillo, newWindow);

            oscillo.ValuesSource = ts;
            ts.ValuesSource = knob;
        }
    }
}
