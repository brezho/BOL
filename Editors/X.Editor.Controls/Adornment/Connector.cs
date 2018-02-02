using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using X.Editor.Controls.Gdi;
using X.Editor.Controls.Utils;

namespace X.Editor.Controls.Adornment
{
    partial class X { }
    public class Connector : AdornerBase
    {
        const int ConnectorSize = 8;
        List<ConnectionPoint> _points = new List<ConnectionPoint>();

        protected override int ZIndex => 100;
        public Connector(Surface surface, Control target) : base(surface, target)
        {
            this.Size = new Size(1, 1);
            this.MakeLocationRelativeTo(target, 0, 0);
        }

        public void Add(string connectorName, Point point, Color color)
        {
            var connector = new ConnectionPoint(Surface, Target, connectorName, point, color);
            _points.Add(connector);
            Surface.Adorn(Target, connector);
        }
    }

    static class ConnectorGraphics
    {
        public static readonly Size Size = new Size(12, 12);
        static GraphicsBuffer _buffer;
        public static GraphicsBuffer Buffer
        {
            get
            {
                if (_buffer == null)
                {
                    _buffer = new GraphicsBuffer(Size);
                    _buffer.Draw(x =>
                    {
                        x.FillEllipse(Brushes.SkyBlue, new Rectangle(Point.Empty, Size));
                    });
                }
                return _buffer;
            }
        }
    }
    public class ConnectionPoint : AdornerBase
    {
        List<Connection> _arrows = new List<Connection>();
        protected override int ZIndex => 100;
        Color _color = Color.White;
        GraphicsBuffer _buffer;
        public ConnectionPoint(Surface surface, Control target, string name, Point location, Color color) : base(surface, target)
        {
            Name = name;
            Size = ConnectorGraphics.Size;
            this.MakeLocationRelativeTo(target, location.X, location.Y);

            _buffer = new GraphicsBuffer(Size);
            _buffer.Draw(x =>
            {
                using (var b = new SolidBrush(color))
                {
                    x.FillEllipse(b, new Rectangle(Point.Empty, Size));
                }
            });
        }

        public Connection Add()
        {
            var arrow = new Connection(this, Surface, Target);
            _arrows.Add(arrow);
            Surface.Controls.Add(arrow);
            arrow.BringToFront();
            return arrow;
        }

        Point moveStartLocation;
        bool isDragging = false;
        Connection current = null;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            isDragging = true;
            moveStartLocation = new Point(Size.Width / 2, Size.Height / 2);
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (current != null)
            {
                var surfaceLocation = Parent.PointToClient(PointToScreen(e.Location));
                var destination = Surface.GetAdornersAt(surfaceLocation).OfType<ConnectionPoint>().FirstOrDefault();

                if (destination == null)
                {
                    _arrows.Remove(current);
                    Surface.Controls.Remove(current);
                }
                else
                {
                    current.ConnectEnd(destination);
                }
            }
            isDragging = false;
            current = null;
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isDragging)
            {
                if (current == null)
                {
                    current = Add();
                }
                var delta = e.Location.Delta(moveStartLocation);
                current.SetEndpointDelta(delta);
                current.Refresh();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Cursor = Cursors.Cross;
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            Cursor = Cursors.Default;
            base.OnMouseLeave(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            _buffer.FlushTo(e.Graphics);
            base.OnPaint(e);
        }
    }

    public class Connection : AdornerBase
    {
        protected override int ZIndex => 1;

        ConnectionPoint Source;
        ConnectionPoint Destination;

        public Connection(ConnectionPoint source, Surface surface, Control target) : base(surface, target)
        {
            this.BorderStyle = BorderStyle.FixedSingle;
            Source = source;
            Size = new Size(1, 1);

            AdjustSizeAndPosition();
            target.LocationChanged += (s, a) =>
            {
                AdjustSizeAndPosition();
            };
        }


        Point startPoint;
        Point endPoint;
        void AdjustSizeAndPosition()
        {
            startPoint = Source.Bounds.GetLocationOf(KnownPoint.Center);
            endPoint = Destination?.Bounds.GetLocationOf(KnownPoint.Center) ?? Surface.PointToClient(MousePosition);

            var delta = endPoint.Delta(startPoint);

            int newX = (delta.X > 0) ? startPoint.X : endPoint.X;
            int newY = (delta.Y > 0) ? startPoint.Y : endPoint.Y;

            Location = new Point(newX, newY);
            Size = new Size(Math.Abs(delta.X), Math.Abs(delta.Y));
        }

        internal void ConnectEnd(ConnectionPoint destination)
        {
            Destination = destination;
            AdjustSizeAndPosition();
            destination.LocationChanged += (s, a) =>
            {
                AdjustSizeAndPosition();
            };
        }

        internal void SetEndpointDelta(Point delta)
        {
            AdjustSizeAndPosition();
            //Size = (Size)delta;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            GraphicsPath path = new GraphicsPath();



            path.AddLine(startPoint.Delta(Location), endPoint.Delta(Location));
            path.Widen(new Pen(Color.Transparent, 2));

            // Construct a region based on the path.
            Region region = new Region(path);
            this.Region = region;
            e.Graphics.FillPath(Brushes.Pink, path);

            //// Set the clipping region of the Graphics object.
            //e.Graphics.SetClip(region, CombineMode.Replace);

            base.OnPaint(e);
        }

    }
}
