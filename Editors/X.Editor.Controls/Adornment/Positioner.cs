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
        public AdornersControl Control { get; set; }

        const int SIZE = 10;

        public Positioner()
        {
          //  Control.Target.MouseMove += Target_MouseMove;
        }

        public Rectangle GetBounds(Rectangle ctrlBounds)
        {
            var myTopLeft = ctrlBounds.GetLocationOf(KnownPoint.TopRight).Translate(2,0);
            var res =new Rectangle(myTopLeft, new Size(6, 6));
            return res;

        }

        private void Target_MouseMove(object sender, MouseEventArgs e)
        {
            if (moving)
            {
                // Get the difference between the two points
                int xDiff = e.Location.X - moveStartLocation.X;
                int yDiff = e.Location.Y - moveStartLocation.Y;
                Control.Target.Location = Control.Target.Location.Translate(xDiff, yDiff);
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
