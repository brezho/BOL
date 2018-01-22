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
    public class Positioner : IAdorner
    {
        const int SIZE = 10;

        public Positioner()
        {
            //  Control.Target.MouseMove += Target_MouseMove;
        }

        public Rectangle GetRelativeBoundaries(Size ctrlSize)
        {
            var myTopLeft = ctrlSize.GetLocationOf(KnownPoint.TopRight).Translate(2, 0);
            return new Rectangle(myTopLeft, new Size(10, 10));
        }

        public void PaintAt(Graphics graphics, Point offset)
        {
            graphics.FillRectangle(Brushes.Yellow, new Rectangle(new Point(2, 0), new Size(10, 10)).Translate(offset.X, offset.Y));
        }

        private void Target_MouseMove(object sender, MouseEventArgs e)
        {
            if (moving)
            {
                // Get the difference between the two points
                int xDiff = e.Location.X - moveStartLocation.X;
                int yDiff = e.Location.Y - moveStartLocation.Y;
                //     Control.Target.Location = Control.Target.Location.Translate(xDiff, yDiff);
            }
        }

        //internal Positioner(Surface surface, Control target) 
        //{
        //    Surface = surface;
        //    Target = target;
        //    this.Size = new Size(SIZE, SIZE);
        //    BackColor = Color.Yellow;
        //    this.MakeLocationRelativeTo(target, 2, 0, KnownPoint.TopRight);
        //}

        bool moving = false;
        Point moveStartLocation;



        //protected override void OnMouseLeave(EventArgs e)
        //{
        //    this.Cursor = Cursors.Default;
        //}

        //protected override void OnMouseEnter(EventArgs e)
        //{
        //    this.Cursor = Cursors.Hand;
        //}

        //protected override void OnMouseUp(MouseEventArgs e)
        //{
        //    moving = false;
        //}
        //protected override void OnMouseDown(MouseEventArgs e)
        //{
        //    moving = true;
        //    moveStartLocation = e.Location;
        //}

    }
}
