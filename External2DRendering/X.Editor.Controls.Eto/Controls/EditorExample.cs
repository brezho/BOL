using SharpDX;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Model;

namespace X.Editor.Controls.Eto.Controls
{
    class EditorExampleX { }
    public class EditorExample : System.Windows.Forms.Control, IEditor
    {
        //SharpDevice device;
        public void ActivateIn(IEditorContainer newWindow)
        {
            ////background color
            //Color4 color = Color.CornflowerBlue;

            //RenderControl ctrl = new RenderControl();

            //ctrl.Dock = DockStyle.Fill;
            //this.Controls.Add(ctrl);
            //device = new SharpDevice(ctrl);
            //ctrl.Paint += Ctrl_Paint;

            //main loop
            //using (SharpDevice device = new SharpDevice(ctrl))
            //{
            //    RenderLoop.Run(ctrl, () =>
            //    {
            //        //resize if form was resized
            //        if (device.MustResize)
            //        {
            //            device.Resize();
            //        }

            //        //clear color
            //        device.Clear(color);

            //        //present
            //        device.Present();
            //    });
            //}


            var oscillo = new Oscilloscope();
            this.Controls.Add(oscillo);
        }

        //private void Ctrl_Paint(object sender, PaintEventArgs e)
        //{
        //    //resize if form was resized
        //    if (device.MustResize)
        //    {
        //        device.Resize();
        //    }

        //    //clear color
        //    device.Clear(Color.Black);

        //    //begin drawing text
        //    device.Font.Begin();

        //    //draw string
        //    device.Font.DrawString("Hello SharpDX", 0, 0);

        //    device.Font.DrawString("Current Time " + DateTime.Now.ToString(), 0, 32);

        //    //flush text to view
        //    device.Font.End();

        //    //present
        //    device.Present();
        //}
    }
}
