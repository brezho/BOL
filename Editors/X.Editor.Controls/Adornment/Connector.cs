using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void Add(string connectorName, Point point)
        {
            var connector = new ConnectionPoint(this, Surface, Target, connectorName, point);
            _points.Add(connector);
            Surface.Adorn(Target, connector);
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

        class Arrow : AdornerBase
        {
            ConnectionPoint Source;

            public Arrow(ConnectionPoint point, Surface surface, Control target) : base(surface, target)
            {
                Source = point;
                Size = new Size(1, 1);
                this.MakeLocationRelativeTo(point, 0, 0);
            }
            protected override void OnPaint(PaintEventArgs e)
            {

                //using (var pen = new Pen(Color.LightSeaGreen, 3))
                //{
                //    pen.EndCap = LineCap.ArrowAnchor;
                //    pen.StartCap = LineCap.RoundAnchor;

                LogData("Paint");
                using (var shape = new GraphicsPath())
                {
                    shape.AddLine(0, 0, 50, 50);
                   // this.Region = new System.Drawing.Region(shape);
                    using (var gr = Surface.CreateGraphics())
                    {
                        gr.FillPath(Brushes.Aquamarine, shape);
                    }
                }
                // }

                base.OnPaint(e);
            }

            internal void SetEndpointDelta(Point delta)
            {
                Size = new Size(Math.Abs(delta.X), Math.Abs(delta.Y));
            }
            void LogData(string stuff)
            {
                Surface.Log("Event", stuff, "Location", Location, "Endpoint", Size);
            }
        }

        class ConnectionPoint : AdornerBase
        {
            Connector ConnectionManager { get; set; }
            List<Arrow> _arrows = new List<Arrow>();
            protected override int ZIndex => 100;

            public ConnectionPoint(Connector parent, Surface surface, Control target, string name, Point location) : base(surface, target)
            {
                ConnectionManager = parent;
                Name = name;
                Size = ConnectorGraphics.Size;
                this.MakeLocationRelativeTo(target, location.X, location.Y);
            }

            public Arrow Add()
            {
                var arrow = new Arrow(this, Surface, Target);
                _arrows.Add(arrow);
                Surface.Controls.Add(arrow);
                arrow.BringToFront();
                return arrow;
            }

            Point moveStartLocation;
            bool isDragging = false;
            Arrow current = null;
            protected override void OnMouseDown(MouseEventArgs e)
            {
                isDragging = true;
                moveStartLocation = e.Location;
                base.OnMouseDown(e);
            }
            protected override void OnMouseUp(MouseEventArgs e)
            {
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
                ConnectorGraphics.Buffer.FlushTo(e.Graphics);
                base.OnPaint(e);
            }
        }
    }
}
