using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Controls.Utils
{
    public enum Corner
    {
        TopRight,
        TopLeft,
        BottomRight,
        BottomLeft,
    }
    public static class Helpers
    {
        public static Rectangle MoveAt(this Rectangle source, int x = 0, int y = 0)
        {
            return  new Rectangle(new Point(x, y), source.Size);
        }

        public static Point Translate(this Point source, int dx = 0, int dy = 0)
        {
            var newOne = new Point(source.X, source.Y);
            newOne.Offset(dx, dy);
            return newOne;
        }
        public static Rectangle Translate(this Rectangle source, int dx = 0, int dy = 0)
        {
            var newOne = new Rectangle(source.Location, source.Size);
            newOne.Offset(dx, dy);
            return newOne;
        }
        public static Rectangle Expand(this Rectangle source, Corner fromCorner, int width = 0, int height = 0)
        {
            var newOne = new Rectangle(source.Location, source.Size);
            newOne.Inflate(width, height);
            switch (fromCorner)
            {
                case Corner.BottomRight:
                    newOne.Offset(width, height);
                    break;
                case Corner.TopLeft:
                    newOne.Offset(- width, - height);
                    break;
                case Corner.BottomLeft:
                    newOne.Offset(-width, height);
                    break;
                case Corner.TopRight:
                    newOne.Offset(width, - height);
                    break;
            }
            return newOne;
        }
    }
}
