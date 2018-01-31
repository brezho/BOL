using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls.Eto.Gdi;

namespace X.Editor.Controls.Eto.Controls
{
    partial class X { }
    public class TestGraph : BufferedControl
    {
        public TestGraph()
        {
            Size = new System.Drawing.Size(200, 100);

            Task.Factory.StartNew(() =>
            {
                var rnd = new Random();
                using (var f = new Font("Arial", 14))
                {
                    while (true)
                    {
                        var sleep = rnd.Next(2, 50);

                        Graph.DrawString(sleep.ToString(), f, Brushes.Red, 0, 20);
                        Graph.DrawString(FPS.ToString(), f, Brushes.Green, 0, 40);
                        Repaint();
                        Thread.Sleep(1);
                    }
                }
            });
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    using (var f = new Font("Arial", 14))
        //    {
        //            e.Graphics.DrawString("toto", f, Brushes.Red, 0, 20);
        //    }
        //    base.OnPaint(e);
        //}

    }
}
