using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls.Utils;

namespace X.Editor.Controls.Gdi
{
    partial class X { }
    public class BufferedControl : UserControl
    {
        SharpFPS fps;
        GraphicsBuffer buffer;
        TaskScheduler scheduler;

        protected Graphics Graph => buffer.Graphics;
        public int FPS => fps.FPS;

        public BufferedControl()
        {
            //    BorderStyle = BorderStyle.FixedSingle;
            fps = new SharpFPS();
            buffer = new GraphicsBuffer(Size);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            fps.Reset();
            this.Resize += (s, a) => buffer.Resize(Size);
            scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        protected void Repaint()
        {
            Task.Factory.StartNew(() => Refresh(), CancellationToken.None, TaskCreationOptions.None, scheduler);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            fps.Update();
            buffer.FlushTo(e.Graphics);
            base.OnPaint(e);
        }
    }
}
