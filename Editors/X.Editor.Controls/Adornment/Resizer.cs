using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace X.Editor.Controls.Adornment
{
    class ResizerX { }
    public class Resizer : IAdorner
    {
        Rectangle targetBounds;
        internal Resizer(Surface surface, Control target) : base(surface, target)
        {

        }

        protected internal override void OnTargetMoved(Rectangle newBoundaries)
        {
            targetBounds = newBoundaries;
        }

        protected internal override void OnTargetResized(Rectangle newBoundaries)
        {
            targetBounds = newBoundaries;
        }

        enum HandlePositions
        {
            None,
            TopLeft,
            TopMiddle,
            TopRight,
            MiddleRight,
            BottomRight,
            BottomMiddle,
            BottomLeft,
            MiddleLeft,
        }
        const int handleSize = 4;

        Dictionary<HandlePositions, Rectangle> GetHandlesRectangles(Rectangle source)
        {
            var leftAlign = source.Left;
            var rightAlign = source.Right - handleSize;
            var topAlign = source.Top;
            var bottomAlign = source.Bottom - handleSize;

            var verticalMiddleAlign = leftAlign + source.Width / 2 - handleSize / 2;
            var horizontalMiddleAlign = topAlign + source.Height / 2 - handleSize / 2;

            var size = new Size(handleSize, handleSize);

            return new Dictionary<HandlePositions, Rectangle>()
                {
                    { HandlePositions.TopLeft , new Rectangle(new Point(leftAlign, topAlign), size)},
                    { HandlePositions.TopMiddle, new Rectangle(new Point(verticalMiddleAlign, topAlign), size) },
                    { HandlePositions.TopRight, new Rectangle(new Point(rightAlign, topAlign), size)},
                    { HandlePositions.MiddleRight, new Rectangle(new Point(rightAlign, horizontalMiddleAlign), size)},
                    { HandlePositions.BottomRight, new Rectangle(new Point(rightAlign, bottomAlign), size)},
                    { HandlePositions.BottomMiddle, new Rectangle(new Point(verticalMiddleAlign, bottomAlign), size)},
                    { HandlePositions.BottomLeft, new Rectangle(new Point(leftAlign, bottomAlign), size)},
                    { HandlePositions.MiddleLeft, new Rectangle(new Point(leftAlign, horizontalMiddleAlign), size)},
                };
        }
    }
}

