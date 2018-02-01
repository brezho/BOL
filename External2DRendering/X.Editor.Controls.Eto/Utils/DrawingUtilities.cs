using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Controls.Utils
{
    public enum KnownPoint
    {
        None,
        Center,
        TopLeft,
        TopMiddle,
        TopRight,
        MiddleRight,
        BottomRight,
        BottomMiddle,
        BottomLeft,
        MiddleLeft,
    }

    public static class DrawingUtilities
    {
        public static Point GetLocationOf(this Size size, KnownPoint ofPoint = KnownPoint.TopLeft)
        {
            switch (ofPoint)
            {
                case KnownPoint.TopLeft: return Point.Empty;
                case KnownPoint.TopMiddle: return new Point(size.Width / 2, 0);
                case KnownPoint.TopRight: return new Point(size.Width, 0);
                case KnownPoint.MiddleRight: return new Point(size.Width, size.Height / 2);
                case KnownPoint.BottomRight: return new Point(size.Width, size.Height);
                case KnownPoint.BottomMiddle: return new Point(size.Width / 2, size.Height);
                case KnownPoint.BottomLeft: return new Point(0, size.Height);
                case KnownPoint.MiddleLeft: return new Point(0, size.Height / 2);
                case KnownPoint.Center: return new Point(size.Width / 2, size.Height / 2);
                default: throw new NotImplementedException();
            }
        }

        public static Point GetLocationOf(this Rectangle rectangle, KnownPoint ofPoint = KnownPoint.TopLeft)
        {
            return rectangle.Size.GetLocationOf(ofPoint).Translate(rectangle.X, rectangle.Y);
        }
        public static Point Translate(this Point source, int dx, int dy)
        {
            var newOne = new Point(source.X + dx, source.Y + dy);
            //newOne.Offset(dx, dy);
            return newOne;
        }
        public static Rectangle Translate(this Rectangle source, int dx, int dy)
        {
            return new Rectangle(source.Location.Translate(dx, dy), source.Size);
        }
        public static Size Grow(this Size source, int dw, int dh)
        {
            return new Size(source.Width + dw, source.Height + dh);
        }
        public static Rectangle Grow(this Rectangle source, int dw, int dh)
        {
            return Grow(source, KnownPoint.BottomRight, dw, dh);
        }
        public static Rectangle Grow(this Rectangle source, KnownPoint draggedPoint, int dw, int dh)
        {
            Size size = source.Size;
            Point loc = source.Location;

            switch (draggedPoint)
            {
                case KnownPoint.TopLeft:
                    loc = loc.Translate(dw, dh);
                    size = size.Grow(-dw, -dh);
                    break;
                case KnownPoint.TopMiddle:
                    loc = loc.Translate(0, dh);
                    size = size.Grow(0, -dh);
                    break;
                case KnownPoint.TopRight:
                    loc = loc.Translate(0, dh);
                    size = size.Grow(dw, -dh);
                    break;
                case KnownPoint.MiddleRight:
                    // no need to change location => loc = loc.Translate(0, 0);
                    size = size.Grow(dw, 0);
                    break;
                case KnownPoint.BottomRight:
                    // no need to change location => loc = loc.Translate(0, 0);
                    size = size.Grow(dw, dh);
                    break;
                case KnownPoint.BottomMiddle:
                    // no need to change location => loc = loc.Translate(0, 0);
                    size = size.Grow(0, dh);
                    break;
                case KnownPoint.BottomLeft:
                    loc = loc.Translate(dw, 0);
                    size = size.Grow(-dw, dh);
                    break;
                case KnownPoint.MiddleLeft:
                    loc = loc.Translate(dw, 0);
                    size = size.Grow(-dw, 0);
                    break;
                case KnownPoint.Center:
                    loc = loc.Translate(dw / 2, dh / 2);
                    size = size.Grow(dw, dh);
                    break;
            }
            return new Rectangle(loc, size);
        }
    }
}
