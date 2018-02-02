using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls.Gdi;

namespace X.Editor.Controls.Controls
{
    partial class X { }
    public class Oscilloscope : LoopControl
    {
        const int margin = 10;
        List<int> points = new List<int>();

        object locker = new object();

        Color brushColor = Color.Green;
        public Oscilloscope() : base()
        {
            Size = new System.Drawing.Size(300, 200);
            Compute();
            //var rnd = new Random();
            //for (int i = 0; i < 500; i++)
            //{
            //    points.Add(rnd.Next(0, 200));
            //}

            //Task.Factory.StartNew(() =>
            //{
            //    while (true)
            //    {
            //        Add(rnd.Next(0, 150));
            //        Thread.Sleep(2);
            //    }
            //});

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
                Compute();
            }
        }

        int Map(int value, int maxValue, int rangeMax)
        {
            float ratio = (float)value / (float)maxValue;
            return (int)(ratio * (float)rangeMax);
        }



        Point origin;
        Point topLeft;
        Point bottomRight;
        Point[] pointsToDraw;

        void Compute()
        {
            origin = new Point(margin, Height - margin);
            bottomRight = new Point(Width - margin, origin.Y);
            topLeft = new Point(origin.X, margin);

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

            pointsToDraw = allPoints.ToArray();
        }


        protected override void OnLoop(Graphics gr)
        {
            
            using (var brush = new SolidBrush(brushColor))
            using (var pen = new Pen(brush))
            {
                gr.Clear(Color.Black);
                gr.DrawLine(pen, origin, topLeft);
                gr.DrawLine(pen, origin, bottomRight);
                if (pointsToDraw.Length > 1)
                {
                    gr.DrawLines(pen, pointsToDraw);
                }
                gr.DrawString("FPS:" + FPS, new Font("Arial", 14), brush, 0, 12);
            }

        }
    }
}