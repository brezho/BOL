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
    class PositionerX { }
    public class Positioner : AbstractAdorner
    {
        const int SIZE = 10;

        internal Positioner(Surface surface, Control target) : base(surface, target)
        {
            this.Size = new Size(SIZE, SIZE);
            BackColor = Color.Yellow;
            this.HasLocationRelativeTo(target, 2, 0, KnownPoint.TopRight);
        }

        bool moving = false;
        Point moveStartLocation;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (moving)
            {
                // Get the difference between the two points
                int xDiff = e.Location.X - moveStartLocation.X;
                int yDiff = e.Location.Y - moveStartLocation.Y;
                Target.Location = Target.Location.Translate(xDiff, yDiff);
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            moving = false;
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            moving = true;
            moveStartLocation = e.Location;
        }

    }
}
