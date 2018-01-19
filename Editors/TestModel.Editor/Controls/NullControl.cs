using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls;
using X.Editor.Controls.Adornment;
using X.Editor.Controls.Utils;
using X.Editor.Model;

namespace TestModel.Editor.Controls
{
    class NullControlSuppressDesignTime { }
    class NullControl : Control, IEditor
    {


        public void ActivateIn(IEditorContainer newWindow)
        {

            var surface = new Surface(newWindow);
            //var oscillo = new Oscilloscope();
            var knob = new KnobControl();
            //var ts = new TimedSerie<int>();

            //oscillo.ValuesSource = ts;
            //ts.ValuesSource = knob;

           // surface.Controls.Add(oscillo);
            surface.Controls.Add(knob);

            //surface.AdornWith<Positioner>(oscillo);
            //surface.AdornWith<Resizer>(oscillo);

            //var connectorAdorner = surface.AdornWith<Connector>(oscillo);
            //connectorAdorner.AddSourceAt("Src", KnownPoint.TopLeft, new Point(4, 4));
            //connectorAdorner.AddDestinationAt("Dst", KnownPoint.TopLeft, new Point(4, 20));
            //connectorAdorner.AddDestinationAt("Dst", KnownPoint.TopLeft, new Point(8, 40));
            //connectorAdorner.AddDestinationAt("Dst", KnownPoint.TopLeft, new Point(12, 60));
            //connectorAdorner.AddDestinationAt("Dst", KnownPoint.TopLeft, new Point(16, 80));
            //connectorAdorner.AddDestinationAt("Dst", KnownPoint.TopLeft, new Point(20, 100));
            //connectorAdorner.AddDestinationAt("Dst", KnownPoint.TopLeft, new Point(24, 120));
            //connectorAdorner.AddDestinationAt("Dst", KnownPoint.TopLeft, new Point(28, 140));

            surface.AdornWith<Positioner>(knob);
            surface.AdornWith<Resizer>(knob);

            this.Controls.Add(surface);
        }
    }
}
