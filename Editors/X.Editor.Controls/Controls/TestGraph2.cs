using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls.Gdi;

namespace X.Editor.Controls.Controls
{
    partial class X { }
    public class TestGraph2 : LoopControl
    {
        Random rnd;
        public TestGraph2()
        {
            Size = new System.Drawing.Size(200, 100);
            rnd = new Random();
        }

        protected override void OnLoop(Graphics gr)
        {
            using (var f = new Font("Arial", 14))
            {
                gr.DrawString(rnd.Next(2, 50).ToString(), f, Brushes.Red, 0, 20);
                gr.DrawString(FPS.ToString(), f, Brushes.Green, 0, 40);
            }
        }
    }
}
