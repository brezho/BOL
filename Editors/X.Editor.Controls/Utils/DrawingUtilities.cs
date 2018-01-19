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
        public static Point GetLocationOf(this Rectangle rectangle, KnownPoint ofPoint = KnownPoint.TopLeft)
        {
            switch (ofPoint)
            {
                case KnownPoint.TopLeft: return rectangle.Location;
                case KnownPoint.TopMiddle: return rectangle.Location.Translate(rectangle.Width / 2, 0);
                case KnownPoint.TopRight: return rectangle.Location.Translate(rectangle.Width, 0);
                case KnownPoint.MiddleRight: return rectangle.Location.Translate(rectangle.Width, rectangle.Height / 2);
                case KnownPoint.BottomRight: return rectangle.Location.Translate(rectangle.Width, rectangle.Height);
                case KnownPoint.BottomMiddle: return rectangle.Location.Translate(rectangle.Width / 2, rectangle.Height);
                case KnownPoint.BottomLeft: return rectangle.Location.Translate(0, rectangle.Height);
                case KnownPoint.MiddleLeft: return rectangle.Location.Translate(0, rectangle.Height / 2);
                case KnownPoint.Center: return rectangle.Location.Translate(rectangle.Width / 2, rectangle.Height / 2);
                default: throw new NotImplementedException();
            }
        }

        public static Rectangle Wrapper(this Rectangle ctrl, int dx1, int dy1, int dx2, int dy2)
        {
            var newLoc = ctrl.Location.Translate(-dx1, -dy1);
            var newSize = new Size(dx1 + ctrl.Width + dx2, dy1 + ctrl.Height + dy2);
            return new Rectangle(newLoc, newSize);
        }

        public static Point Translate(this Point source, int dx = 0, int dy = 0)
        {
            var newOne = new Point(source.X, source.Y);
            newOne.Offset(dx, dy);
            return newOne;
        }
        public static Size Translate(this Size source, int dx = 0, int dy = 0)
        {
            return new Size(source.Width + dx, source.Height + dy);
        }
        public static Rectangle Translate(this Rectangle source, int dx = 0, int dy = 0)
        {
            var newOne = new Rectangle(source.Location, source.Size);
            newOne.Offset(dx, dy);
            return newOne;
        }
        public static Rectangle Expand(this Rectangle source, KnownPoint fromPoint, int deltaX = 0, int deltaY = 0)
        {
            Size size = source.Size;
            Point loc = source.Location;

            switch (fromPoint)
            {
                case KnownPoint.TopLeft:
                    loc = loc.Translate(deltaX, deltaY);
                    size = size.Translate(-deltaX, -deltaY);
                    break;
                case KnownPoint.TopMiddle:
                    loc = loc.Translate(0, deltaY);
                    size = size.Translate(0, -deltaY);
                    break;
                case KnownPoint.TopRight:
                    loc = loc.Translate(0, deltaY);
                    size = size.Translate(deltaX, -deltaY);
                    break;
                case KnownPoint.MiddleRight:
                    // no need to change location => loc = loc.Translate(0, 0);
                    size = size.Translate(deltaX, 0);
                    break;
                case KnownPoint.BottomRight:
                    // no need to change location => loc = loc.Translate(0, 0);
                    size = size.Translate(deltaX, deltaY);
                    break;
                case KnownPoint.BottomMiddle:
                    // no need to change location => loc = loc.Translate(0, 0);
                    size = size.Translate(0, deltaY);
                    break;
                case KnownPoint.BottomLeft:
                    loc = loc.Translate(deltaX, 0);
                    size = size.Translate(-deltaX, deltaY);
                    break;
                case KnownPoint.MiddleLeft:
                    loc = loc.Translate(deltaX, 0);
                    size = size.Translate(-deltaX, 0);
                    break;
            }
            return new Rectangle(loc, size);
        }
    }
}
