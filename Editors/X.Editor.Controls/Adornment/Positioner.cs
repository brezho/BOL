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
        const int SIZE = 12;
        Rectangle handleArea;

        public Positioner()
        {
            //  Control.Target.MouseMove += Target_MouseMove;
            handleArea = new Rectangle(new Point(0, 0), new Size(SIZE, SIZE));
        }

        public Rectangle GetRelativeBoundaries(Size ctrlSize)
        {
            return handleArea.Translate(ctrlSize.Width + SIZE / 3, 0);
        }

        public void PaintAt(Graphics graphics, Point offset)
        {
            graphics.FillRectangle(Brushes.Yellow, handleArea.Translate(offset.X, offset.Y));
        }

        public Cursor GetHitTests(Point location)
        {
            if (handleArea.Contains(location)) return Cursors.Hand;
            return Cursors.Default;
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
