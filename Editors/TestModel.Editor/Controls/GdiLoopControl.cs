using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Model;

namespace TestModel.Editor.Controls
{
    class GdiLoopControlX { }
    delegate void DoPaint(Graphics gr);
    public class GdiLoopControl : Control, IEditor
    {
        Thread renderThread;
        bool antiAliasing = true;

        event DoPaint DoPaint;
        public GdiLoopControl()
        {


            SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(System.Windows.Forms.ControlStyles.UserPaint, false);
            ClientSize = new System.Drawing.Size(800, 600);
            BackColor = Color.Black;

            DoPaint += GdiLoopControl_DoPaint;

            renderThread = new Thread(new ThreadStart(RenderLoop));
            //    ' MTA or bust. If not MTA then it isn't really Multithreaded.
            if (!renderThread.TrySetApartmentState(ApartmentState.MTA))
            {
                //    MsgBox("Some limitation on your PC prevents me from runing Multithreaded!")

            }
            taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            Task.Factory.StartNew(() =>
            {
                var rnd = new Random();
                while (true)
                {
                    Add(rnd.Next(0, 150));
                    Thread.Sleep(100);
                }
            });


        }
        public void Add(int value)
        {
            points.Add(value);
            //Task.Factory.StartNew(() => Invalidate(), CancellationToken.None, TaskCreationOptions.None, taskScheduler);

            //Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (!renderThread.IsAlive) renderThread.Start();

        }






        const int margin = 10;
        List<int> points = new List<int>();
        private TaskScheduler taskScheduler;

        int Map(int value, int maxValue, int rangeMax)
        {
            float ratio = (float)value / (float)maxValue;
            return (int)(ratio * (float)rangeMax);
        }

        private void GdiLoopControl_DoPaint(Graphics gr)
        {
            var graphics = gr;
            Pen axisPen = new Pen(Color.Green, 2);
            Pen gridPen = new Pen(Color.Green, 1);

            var origin = new Point(margin, Height - margin);
            var bottomRight = new Point(this.Width - margin, origin.Y);
            var topLeft = new Point(origin.X, margin);

            var graphHeight = origin.Y - topLeft.Y;
            var graphWidth = bottomRight.X - origin.X;

            graphics.DrawLine(axisPen, origin, topLeft);
            graphics.DrawLine(axisPen, origin, bottomRight);

            var allValues = points.ToArray();
            var nbOfPointsToRender = Math.Min(allValues.Length, graphWidth);


            var dataSource = allValues.Skip(allValues.Length - nbOfPointsToRender).ToArray();
            if (dataSource.Length == 0) return;
            var peekValue = dataSource.Max();
            var valuesToRender = dataSource.Select(x => Map(x, Math.Max(peekValue, 255), graphHeight)).ToArray();


            var allPoints = new List<PointF>();

            for (int i = 0; i < nbOfPointsToRender; i++)
            {
                var curval = valuesToRender[i];
                var pt = new PointF(bottomRight.X - nbOfPointsToRender + i, origin.Y - curval);
                allPoints.Add(pt);
            }

            var all = allPoints.ToArray();

            if (all.Length > 1)
            {
                graphics.DrawLines(gridPen, all);
            }
        }

        void RenderLoop()
        {
            // Alternatively for fullscreen ( Fake Fullscreen ) 
            // --> dont set client size , but instead set:
            // --> Borderstyle: None
            // --> Windowstate: Maximized
            // --> OnTop: True  [ Or use WIN32 API calls to force Always On Top  ] 


            // Create a backwash - er Backbuffer and some surfaces:
            var B_BUFFER = new Bitmap(this.ClientSize.Width, this.ClientSize.Height); // backbuffer
            var G_BUFFER = Graphics.FromImage(B_BUFFER); //drawing surface
            var G_TARGET = this.CreateGraphics(); // target surface

            // Clear the random gibberish that would have been behind (and now imprinted in) the form away. 
            G_TARGET.Clear(Color.Black);

            // Configure Surfaces for optimal rendering:

            G_TARGET.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            G_TARGET.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
            G_TARGET.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            G_TARGET.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            G_TARGET.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            G_TARGET.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;



            // Antialiased Polygons and Text?
            if (antiAliasing)
            {
                G_BUFFER.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                G_BUFFER.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }


            G_BUFFER.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            G_BUFFER.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            G_BUFFER.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            G_BUFFER.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;



            // The Loop, terminates automatically when the window is closing.
            while (!this.Disposing && !this.IsDisposed && this.Visible)
            {
                // we use an exception handler because sometimes the form may be
                // - beginning to unload after the above checks. 
                // - Most exceptions I get here are during the unloading stage.
                // - Or attempting to draw to a window that has probably begun unloading already .
                // - also any errors within OnPaint are ignored. 
                // - Use Exception handlers within OnPaint()
                try
                {

                    // Raise the Paint Event - were the drawing code will go
                    DoPaint(G_BUFFER);

                    // Update Window using the fastest available GDI function to do it with.
                    G_TARGET.DrawImageUnscaled(B_BUFFER, 0, 0);

                }
                catch (Exception E)
                {
                    // Show me what happened in the debugger
                    // Note: Too Many exception handlers can cause JIT to slow down your renderloop. 
                    // - One should be enough. Stack Trace (usually) tells all!
#if DEBUG 
                    System.Diagnostics.Debug.Print(E.ToString());
#endif
                }


            }

            // If we are here then the window is closing or has closed. 
            // - Causing the loop to end

            // Clean up:
            G_TARGET.Dispose();
            G_BUFFER.Dispose();
            B_BUFFER.Dispose();

            // Routine is done. K THX BYE    
        }

        public void ActivateIn(IEditorContainer newWindow)
        {
            //var surface = new Surface(newWindow);

            //var ts = new TimedSerie<int>();
            //var oscillo = new Oscilloscope();
            //var knob = new KnobControl();

            //oscillo.Location = new Point(200, 200);

            //surface.Controls.Add(oscillo);
            //surface.Controls.Add(knob);

            //ts.ValuesSource = knob;
            ////  oscillo.ValuesSource = ts;

            //surface.AdornWith<Resizer>(oscillo);
            //surface.AdornWith<Positioner>(oscillo);

            //var connectorAdorner = surface.AdornWith<Connector>(oscillo);
            //connectorAdorner.Add("Src", new Point(20, 20));

            //surface.AdornWith<Resizer>(knob);
            //surface.AdornWith<Positioner>(knob);

            //this.Controls.Add(surface);

        }
    }
}