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
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.IsVisibleOnFocusOf(target);
            this.MakeLocationRelativeTo(target, -MARGIN, -MARGIN);
            this.MakeSizeRelativeTo(target, -MARGIN, -MARGIN, MARGIN, MARGIN);
            BringToFront();
            BackColor = Color.Green;

            //PrecomputeDimensions();
            //target.SizeChanged += (s, a) => PrecomputeDimensions();
            //target.LocationChanged += (s, a) => PrecomputeDimensions();

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
            //PrecomputeDimensions();
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
            #region region example
            Point point = new Point(60, 10);

            // Assume that the variable "point" contains the location of the
            // most recent mouse click.
            // To simulate a hit, assign (60, 10) to point.
            // To simulate a miss, assign (0, 0) to point.

            SolidBrush solidBrush = new SolidBrush(Color.Black);
            Region region1 = new Region(new Rectangle(50, 0, 50, 150));
            Region region2 = new Region(new Rectangle(0, 50, 150, 50));

            // Create a plus-shaped region by forming the union of region1 and 
            // region2.
            // The union replaces region1.
            region1.Union(region2);

            if (region1.IsVisible(point, e.Graphics))
            {
                // The point is in the region. Use an opaque brush.
                solidBrush.Color = Color.FromArgb(255, 255, 0, 0);
            }
            else
            {
                // The point is not in the region. Use a semitransparent brush.
                solidBrush.Color = Color.FromArgb(64, 255, 0, 0);
            }
            Graphics g = this.Target.CreateGraphics();
            g.FillRegion(solidBrush, region1);

            //e.Graphics.FillRegion(solidBrush, region1);
            #endregion


            var aPointRectangle = new Rectangle(0, 0, 12, 12);
            var allRectangles = new List<Rectangle>();

            foreach (var c in _points)
            {
                var loc = Target.ClientRectangle.GetLocationOf(c.PositionRelativeTo).Translate(c.Offset.X, c.Offset.Y);

                var rect = aPointRectangle.Translate(loc.X, loc.Y);
                Surface.Log("Rect @", rect);
                allRectangles.Add(rect);
            }
            
            e.Graphics.FillRectangles(Brushes.SkyBlue, allRectangles.ToArray());


            //Graphics g = this.Target.CreateGraphics();
            //g.FillRectangles(Brushes.SkyBlue, allRectangles.ToArray());

            ////base.OnPaint(e);
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
