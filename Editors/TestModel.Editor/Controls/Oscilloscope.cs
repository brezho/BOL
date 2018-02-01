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
    class OscilloscopeSuppressDesignTime { }
    //public partial class Oscilloscope : Control
    //{
    //    List<int> points = new List<int>();
    //    protected override void OnMouseDown(MouseEventArgs e)
    //    {
    //        Focus();
    //    }

    //    private IPublisher<int> _valuesSubscription;
    //    public IPublisher<int> ValuesSource
    //    {
    //        set
    //        {
    //            if (_valuesSubscription != null) _valuesSubscription.OnNext -= _valuesSubscription_OnNext;
    //            _valuesSubscription = value;
    //            _valuesSubscription.OnNext += _valuesSubscription_OnNext;
    //        }
    //    }

    //    private void _valuesSubscription_OnNext(object sender, int e)
    //    {
    //        points.Add(e);
    //        var cnt = points.Count;
    //        if (cnt > this.Width - 2 * margin)
    //        {
    //            points.RemoveRange(0, (cnt - (this.Width - 2 * margin)));
    //        }
    //        this.Invalidate();
    //    }

    //    //protected override void OnClick(EventArgs e)
    //    //{
    //    //    MessageBox.Show("ds");
    //    //}

    //    public Oscilloscope() : base()
    //    {
    //        BackColor = Color.Black;
    //        Height = 150;
    //        Width = 150;
    //        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
    //               | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

    //        UpdateStyles();
    //        this.DoubleBuffered = true;
    //    }

    //    const int margin = 10;

    //    Pen axisPen = new Pen(Color.Green, 2);
    //    Pen gridPen = new Pen(Color.Green, 1);

    //    int Map(int value, int maxValue, int rangeMax)
    //    {
    //        float ratio = (float)value / (float)maxValue;
    //        return (int)(ratio * (float)rangeMax);
    //    }

    //    void DrawGrid(Graphics graphics)
    //    {
    //        var origin = new Point(margin, Height - margin);
    //        var bottomRight = new Point(this.Width - margin, origin.Y);
    //        var topLeft = new Point(origin.X, margin);

    //        var graphHeight = origin.Y - topLeft.Y;
    //        var graphWidth = bottomRight.X - origin.X;

    //        graphics.DrawLine(axisPen, origin, topLeft);
    //        graphics.DrawLine(axisPen, origin, bottomRight);

    //        var allValues = points.ToArray();
    //        var nbOfPointsToRender = Math.Min(allValues.Length, graphWidth);


    //        var dataSource = allValues.Skip(allValues.Length - nbOfPointsToRender).ToArray();
    //        if (dataSource.Length == 0) return;
    //        var peekValue = dataSource.Max();
    //        var valuesToRender = dataSource.Select(x => Map(x, Math.Max(peekValue, 255), graphHeight)).ToArray();


    //        var allPoints = new List<Point>();

    //        for (int i = 0; i < nbOfPointsToRender; i++)
    //        {
    //            var curval = valuesToRender[i];
    //            var pt = new Point(bottomRight.X - nbOfPointsToRender + i, origin.Y - curval);
    //            allPoints.Add(pt);
    //        }

    //        var all = allPoints.ToArray();

    //        if (all.Length > 1) graphics.DrawLines(gridPen, allPoints.ToArray());

    //    }

    //    protected override void OnPaint(PaintEventArgs e)
    //    {
    //        base.OnPaint(e);
    //        DrawGrid(e.Graphics);
    //        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
    //    }
    //}

    public class Oscilloscope : Control
    {
        TaskScheduler taskScheduler;
        const int margin = 10;
        List<int> points = new List<int>();
        Color penColor = Color.Green;
        public Oscilloscope()
        {
            BackColor = Color.Black;
            Size = new Size(200, 300);
            taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            Task.Factory.StartNew(() =>
            {
                var rnd = new Random();
                while (true)
                {
                    Add(rnd.Next(0, 150));
                    Thread.Sleep(20);
                }
            });
        }


        public void Add(int value)
        {
            points.Add(value);
            Task.Factory.StartNew(() => Invalidate(), CancellationToken.None, TaskCreationOptions.None, taskScheduler);

            //Invalidate();
        }

        int Map(int value, int maxValue, int rangeMax)
        {
            float ratio = (float)value / (float)maxValue;
            return (int)(ratio * (float)rangeMax);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var graphics = e.Graphics;
            Pen axisPen = new Pen(penColor, 2);
            Pen gridPen = new Pen(penColor, 1);

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
                graphics.DrawLines(gridPen, allPoints.ToArray());
            }
        }
    }


}
