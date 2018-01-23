using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls.Utils;

namespace X.Editor.Controls.Adornment
{
    public class Connector : IAdorner
    {
        const int ConnectorSize = 8;
        List<ConnectionPoint> _points = new List<ConnectionPoint>();

        Rectangle[] _connectors;
        public Cursor GetHitTests(Point location)
        {
            return Cursors.Default;
        }

        public Rectangle GetRelativeBoundaries(Size ctrlSize)
        {
            List<Rectangle> _rectangles = new List<Rectangle>();
            foreach (var cp in _points)
            {
                var loc = Point.Empty.Translate(cp.Location.X, cp.Location.Y);
                var size = new Size(ConnectorSize, ConnectorSize);
                _rectangles.Add(new Rectangle(loc, size));
            }
            _connectors = _rectangles.ToArray();
            return new Rectangle(new Point(-ConnectorSize, -ConnectorSize), ctrlSize.Grow(2* ConnectorSize, 2* ConnectorSize));
        }


        public void PaintAt(Graphics graphics, Point offset)
        {
            var offsets = _connectors.Select(x => x.Translate(offset.X, offset.Y)).ToArray();
            foreach(var r in offsets) graphics.FillEllipse(Brushes.SkyBlue, r);

            //graphics.FillRectangles(Brushes.SkyBlue, offsets);
        }

        public void Add(string v, Point point)
        {
            _points.Add(new ConnectionPoint() { Name = v, Location = point });
        }

        class ConnectionPoint
        {
            public string Name;
            public Point Location;
        }
    }
}
