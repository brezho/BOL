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
        SharpFPS loopFPS;
        GraphicsBuffer buffer;


        protected Graphics Graph => buffer.Graphics;
        public int FPS => paintFPS.FPS;

        private ThreadStart _threadStart;
        private Thread _thread;
        private SynchronizationContext _synchronizationContext;
        TaskScheduler scheduler;
        public LoopControl()
        {
            paintFPS = new SharpFPS();
            loopFPS = new SharpFPS();

            buffer = new GraphicsBuffer(Size);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, false);
            this.DoubleBuffered = true;
            paintFPS.Reset();

            _synchronizationContext = WindowsFormsSynchronizationContext.Current;
            _threadStart = new ThreadStart(LongProcess);
            _thread = new Thread(_threadStart);
            _thread.Start();


               scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            //Task.Factory.StartNew(() =>
            //{
            //    while (!IsDisposed)
            //    {
            //        Task.Factory.StartNew(() => Refresh(), CancellationToken.None, TaskCreationOptions.None, scheduler);
            //        Thread.Sleep(3);
            //    }
            //});
        }
        void LongProcess()
        {
            while(!IsDisposed)
            {
                loopFPS.Update();
                Task.Factory.StartNew(() => Refresh(), CancellationToken.None, TaskCreationOptions.None, scheduler);

               // _synchronizationContext.Post((o) => Refresh(), null);
                Thread.Sleep(3);
            }
        }
        protected sealed override void OnPaint(PaintEventArgs e)
        {
            paintFPS.Update();
            buffer.FlushTo(e.Graphics);
            base.OnPaint(e);
        }
    }
}
