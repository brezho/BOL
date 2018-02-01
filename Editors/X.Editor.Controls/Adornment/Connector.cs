using System;
using System.Collections.Generic;
using System.Drawing;
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

        public Connector(Surface surface, Control target) : base(surface, target)
        {
            this.Size = new Size(1, 1);
            this.MakeLocationRelativeTo(target, 0, 0);
        }

        public void Add(string connectorName, Point point)
        {
            var connector = new ConnectionPoint(Surface, Target, connectorName, point);
            _points.Add(connector);
            Surface.Controls.Add(connector);
            connector.BringToFront();
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
                        _buffer.Graphics.FillEllipse(Brushes.SkyBlue, new Rectangle(Point.Empty, Size));
                    }
                    return _buffer;
                }
            }
        }

        class ConnectionPoint : AdornerBase
        {

            public ConnectionPoint(Surface surface, Control target, string name, Point location) : base(surface, target)
            {
                Name = name;
                Size = ConnectorGraphics.Size;
                this.MakeLocationRelativeTo(target, location.X, location.Y);
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
