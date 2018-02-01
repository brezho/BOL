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
    public class LoopControl : UserControl
    {
        SharpFPS paintFPS;

        GraphicsBuffer buffer;

        public int FPS => paintFPS.FPS;

        private ThreadStart _threadStart;
        private Thread _thread;
        TaskScheduler scheduler;

        public LoopControl()
        {

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            buffer = new GraphicsBuffer(Size);


            paintFPS = new SharpFPS();
            paintFPS.Reset();

            scheduler = TaskScheduler.FromCurrentSynchronizationContext();

            _threadStart = new ThreadStart(LongProcess);
            _thread = new Thread(_threadStart);
            _thread.Start();

        }
        protected override void OnResize(EventArgs e)
        {
            buffer.Resize(Size);
            base.OnResize(e);
        }

        void LongProcess()
        {
            while (!IsDisposed)
            {
                buffer.Draw(OnLoop);
                Task.Factory.StartNew(() =>
                   {
                       Refresh();
                       Application.DoEvents();
                   }, CancellationToken.None, TaskCreationOptions.None, scheduler)
                   .Wait();
            }
        }

        protected virtual void OnLoop(Graphics graphics)
        {

        }
        protected sealed override void OnPaint(PaintEventArgs e)
        {
            paintFPS.Update();
            buffer.FlushTo(e.Graphics);
            base.OnPaint(e);
        }
    }
}
