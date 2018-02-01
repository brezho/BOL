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
    partial class X { }
    class Positioner : AdornerBase
    {
        const int SIZE = 12;
        protected override int ZIndex => 100;

        public Positioner(Surface surface, Control target) : base(surface, target)
        {
            BackColor = Color.Yellow;
            Size = new Size(SIZE, SIZE);
            this.MakeLocationRelativeTo(target, SIZE / 3, 0, KnownPoint.TopRight);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.Cursor = Cursors.Default;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            base.OnMouseEnter(e);
        }

        bool moving = false;
        Point moveStartLocation;

        protected override void OnMouseUp(MouseEventArgs e)
        {
            moving = false;
            base.OnMouseUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Surface.BringToFront(Target);

            moving = true;
            moveStartLocation = e.Location;
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (moving)
            {
                // Get the difference between the two points
                var delta = e.Location.Delta(moveStartLocation);
                Target.Location = Target.Location.Translate(delta);
            }

            base.OnMouseMove(e);
        }
    }
}
