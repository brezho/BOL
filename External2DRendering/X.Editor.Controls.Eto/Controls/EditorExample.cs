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
        public void ActivateIn(IEditorContainer newWindow)
        {
            //background color
            Color4 color = Color.CornflowerBlue;

            RenderForm form = new RenderForm();


            form.Text = "Tutorial 2: Init Device (press key from 1 to 8)";
            //main loop
            using (SharpDevice device = new SharpDevice(form))
            {
                RenderLoop.Run(form, () =>
                {
                    //resize if form was resized
                    if (device.MustResize)
                    {
                        device.Resize();
                    }

                    //clear color
                    device.Clear(color);

                    //present
                    device.Present();
                });
            }


            //var oscillo = new Oscilloscope().ToNative();
            //oscillo.Dock = DockStyle.Fill;
            //this.Controls.Add(oscillo);
        }
    }
}
