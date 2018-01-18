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

        internal Positioner(Surface surface, Control target) : base(surface, target)
        {
            this.Size = new Size(SIZE, SIZE);
            ResetLocation();
        }

        private void ResetLocation()
        {
            Location = new Point(Target.Right + 2, Target.Top);
        }

        protected internal override void OnTargetMoved(Rectangle newBoundaries)
        {
            ResetLocation();
        }

        protected internal override void OnTargetResized(Rectangle newBoundaries)
        {
            ResetLocation();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Red, ClientRectangle);
        }
    }
}
