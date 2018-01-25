using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls.Eto;

class OscilloscopeX { }
public class Oscilloscope : Control
{
    SharpFPS fps;
    public SharpDX.Direct2D1.Factory Factory2D { get; private set; }
    public WindowRenderTarget RenderTarget2D { get; private set; }
    public SolidColorBrush SceneColorBrush { get; private set; }

    RenderControl renderControl;


    TaskScheduler taskScheduler;
    const int margin = 10;
    List<int> points = new List<int>();

    object locker = new object();


    public Oscilloscope()
    {
        renderControl = new RenderControl();
        renderControl.Dock = DockStyle.Fill;
        this.Controls.Add(renderControl);


        Size = new System.Drawing.Size(300, 200);
        fps = new SharpFPS();


        InitDirect2DAndDirectWrite();

        renderControl.Resize += RenderControl_Resize;
        renderControl.Paint += RenderControl_Paint;
       // this.Resize += RenderControl_Resize;
       // this.Paint += RenderControl_Paint;



        ////SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        //SetStyle(ControlStyles.ResizeRedraw, true);

        var rnd = new Random();
        for (int i = 0; i < 500; i++)
        {
            Add(rnd.Next(0, 200));
        }


        taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        //Task.Factory.StartNew(() =>
        //{
        //    var rnd = new Random();
        //    while (true)
        //    {
        //        Add(rnd.Next(0, 150));
        //        Thread.Sleep(20);
        //    }
        //});
    }

    private void RenderControl_Paint(object sender, PaintEventArgs e)
    {
        Draw();
    }

    private void RenderControl_Resize(object sender, EventArgs e)
    {
        try
        {
            RenderTarget2D.Resize(new Size2(renderControl.Size.Width, renderControl.Size.Height));
            //CurrentTextLayout.MaxWidth = renderControl.Size.Width;
            //CurrentTextLayout.MaxHeight = renderControl.Size.Height;
        }
        catch (Exception ex)
        {
            //LogException(ex);
        }
    }

    private void InitDirect2DAndDirectWrite()
    {
        Factory2D = new SharpDX.Direct2D1.Factory();
     //   FactoryDWrite = new SharpDX.DirectWrite.Factory();

        var properties = new HwndRenderTargetProperties();
        properties.Hwnd = renderControl.Handle;
        properties.PixelSize = new Size2(renderControl.ClientSize.Width, renderControl.ClientSize.Height);
        properties.PresentOptions = PresentOptions.None;

        RenderTarget2D = new WindowRenderTarget(Factory2D, new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)), properties);
        RenderTarget2D.AntialiasMode = AntialiasMode.PerPrimitive;
        RenderTarget2D.TextAntialiasMode = TextAntialiasMode.Cleartype;

        SceneColorBrush = new SolidColorBrush(RenderTarget2D, Color.Black);
    }

    public void Add(int value)
    {
        lock (locker)
        {
            points.Add(value);
        }
      //  Task.Factory.StartNew(() => Refresh(), CancellationToken.None, TaskCreationOptions.None, taskScheduler);
    }

    int Map(int value, int maxValue, int rangeMax)
    {
        float ratio = (float)value / (float)maxValue;
        return (int)(ratio * (float)rangeMax);
    }

    void DoPaint(RenderTarget graphics)
    {




        var origin = new Vector2(margin, Height - margin);
        var bottomRight = new Vector2(this.Width - margin, origin.Y);
        var topLeft = new Vector2(origin.X, margin);

        var graphHeight = origin.Y - topLeft.Y;
        var graphWidth = bottomRight.X - origin.X;


        int[] allValues;
        lock (locker)
        {
            allValues = points.ToArray();
        }

        var nbOfPointsToRender = (int)Math.Min(allValues.Length, graphWidth);


        var dataSource = allValues.Skip(allValues.Length - nbOfPointsToRender).ToArray();

        var allPoints = new List<Vector2>();
        if (dataSource.Length > 0)
        {
            var peekValue = dataSource.Max();
            var valuesToRender = dataSource.Select(dsx => Map(dsx, Math.Max(peekValue, 255), (int)graphHeight)).ToArray();



            for (int i = 0; i < nbOfPointsToRender; i++)
            {
                var curval = valuesToRender[i];
                var pt = new Vector2(bottomRight.X - nbOfPointsToRender + i, origin.Y - curval);
                allPoints.Add(pt);
            }
        }

        var all = allPoints.ToArray();


        using (var brush = new SolidColorBrush(graphics, SharpDX.Color.Green))
        {
            graphics.DrawLine(origin, topLeft, brush);
            graphics.DrawLine(origin, bottomRight, brush);
            if (all.Length > 1)
            {
                for (int i = 0; i < all.Length - 1; i++)
                    graphics.DrawLine(all[i], all[i + 1], brush);

            }
        }




    }

    void Draw()
    {
        try
        {
            RenderTarget2D.BeginDraw();

            RenderTarget2D.Clear(Color.Black);

            DoPaint(RenderTarget2D);
            //RenderTarget2D.DrawTextLayout(new Vector2(0, 0), CurrentTextLayout, SceneColorBrush);

            RenderTarget2D.EndDraw();
        }
        catch (Exception ex)
        {

        }
        
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Draw();
    }
}
