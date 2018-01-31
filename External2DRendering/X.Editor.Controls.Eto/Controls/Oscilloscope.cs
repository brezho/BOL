using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls.Eto;
using X.Editor.Controls.Gdi;

namespace X.Editor.Controls.Controls
{
    partial class X { }
    public class Oscilloscope : BufferedControl
    {

        TaskScheduler taskScheduler;
        const int margin = 10;
        List<int> points = new List<int>();

        object locker = new object();

        Color brushColor = Color.Green;
        public Oscilloscope()
        {
            Size = new System.Drawing.Size(300, 200);

            var rnd = new Random();
            for (int i = 0; i < 500; i++)
            {
                points.Add(rnd.Next(0, 200));
            }


            taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Add(rnd.Next(0, 150));
                    Thread.Sleep(1);
                }
            });

        }

        protected override void OnMouseEnter(EventArgs e)
        {
            brushColor = Color.Red;
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            brushColor = Color.Green;
            base.OnMouseLeave(e);
        }

        public void Add(int value)
        {
            lock (locker)
            {
                var toDrop = points.Count - 500;
                if (toDrop > 100)
                {
                    points.RemoveRange(0, 100);
                }
                points.Add(value);
                DoPaint();
            }
        }

        int Map(int value, int maxValue, int rangeMax)
        {
            float ratio = (float)value / (float)maxValue;
            return (int)(ratio * (float)rangeMax);
        }

        void DoPaint()
        {
            var origin = new Point(margin, Height - margin);
            var bottomRight = new Point(Width - margin, origin.Y);
            var topLeft = new Point(origin.X, margin);

            var graphHeight = origin.Y - topLeft.Y;
            var graphWidth = bottomRight.X - origin.X;


            int[] allValues;
            lock (locker)
            {
                allValues = points.ToArray();
            }

            var nbOfPointsToRender = (int)Math.Min(allValues.Length, graphWidth);


            var dataSource = allValues.Skip(allValues.Length - nbOfPointsToRender).ToArray();

            var allPoints = new List<Point>();
            if (dataSource.Length > 0)
            {
                var peekValue = dataSource.Max();
                var valuesToRender = dataSource.Select(dsx => Map(dsx, Math.Max(peekValue, 255), (int)graphHeight)).ToArray();



                for (int i = 0; i < nbOfPointsToRender; i++)
                {
                    var curval = valuesToRender[i];
                    var pt = new Point(bottomRight.X - nbOfPointsToRender + i, origin.Y - curval);
                    allPoints.Add(pt);
                }
            }

            var all = allPoints.ToArray();


            using (var brush = new SolidBrush(brushColor))
            using (var pen = new Pen(brush))
            {
                Graph.Clear(Color.Black);
                Graph.DrawLine(pen, origin, topLeft);
                Graph.DrawLine(pen, origin, bottomRight);
                if (all.Length > 1)
                {
                    Graph.DrawLines(pen, all);
                }
                Graph.DrawString("FPS:" + FPS, new Font("Arial", 14), brush, 0, 12);
            }

         //   Repaint();
         
        }
    }
}