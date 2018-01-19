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
    class ConnectorX { }
    public class Connector : AbstractAdorner
    {
        const int MARGIN = 40;
        const int GRIPS_SIZE = 6;
        List<ConnectorPoint> _points = new List<ConnectorPoint>();
        internal Connector(Surface surface, Control target) : base(surface, target)
        {
            this.IsVisibleOnFocusOf(target);
            this.MakeLocationRelativeTo(target, -MARGIN, -MARGIN);
            this.MakeSizeRelativeTo(target, -MARGIN, -MARGIN, MARGIN, MARGIN);
            BringToFront();
            BackColor = Color.Green;

            PrecomputeDimensions();
            target.SizeChanged += (s, a) => PrecomputeDimensions();
            target.LocationChanged += (s, a) => PrecomputeDimensions();

        }

        private void PrecomputeDimensions()
        {

            var bound = Rectangle.Empty;
            foreach (var s in _points)
            {
                bound = Rectangle.Union(bound, new Rectangle(ClientRectangle.GetLocationOf(s.PositionRelativeTo).Translate(s.Offset.X, s.Offset.Y), new Size(4, 4)));
            }

            this.SetBounds(bound.X, bound.Y, bound.Width, bound.Height);
            Surface.Log("This bound", bound);
        }

        ConnectorPoint Add(ConnectorPoint x)
        {
            _points.Add(x);
            PrecomputeDimensions();
            return x;
        }

        public ConnectorPoint AddSourceAt(string name, KnownPoint point, Point offset)
        {
            return Add(new ConnectorPoint { Name = name, PositionRelativeTo = point, Offset = offset, Type = ConnectorPointType.Source });
        }

        public ConnectorPoint AddDestinationAt(string name, KnownPoint point, Point offset)
        {
            return Add(new ConnectorPoint { Name = name, PositionRelativeTo = point, Offset = offset, Type = ConnectorPointType.Source });
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var aPointRectangle = new Rectangle(0, 0, 4, 4);
            var allRectangles = new List<Rectangle>();

            var y = 0;
            foreach (var c in _points)
            {
                //var loc = Target.ClientRectangle.GetLocationOf(c.PositionRelativeTo).Translate(c.Offset.X, c.Offset.Y);
                var loc = ClientRectangle.GetLocationOf(KnownPoint.BottomRight).Translate(12, y);
                var rect = aPointRectangle.Translate(loc.X, loc.Y);
                Surface.Log(y, rect);
                allRectangles.Add(rect);
                y += 4;
            }
            e.Graphics.FillRectangles(Brushes.SkyBlue, allRectangles.ToArray());
            //base.OnPaint(e);
        }
    }
    public enum ConnectorPointType
    {
        Source,
        Destination
    }
    public class ConnectorPoint
    {
        public ConnectorPointType Type;
        public string Name;
        public KnownPoint PositionRelativeTo;
        public Point Offset;
    }
}
