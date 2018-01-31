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


    class Positioner2 : AdornerBase
    {
        const int SIZE = 12;
        Rectangle handleArea;

        public Positioner2()
        {
            BorderStyle = BorderStyle.Fixed3D;
            handleArea = new Rectangle(new Point(0, 0), new Size(SIZE, SIZE));
        }

        public override Rectangle GetRelativeBoundaries(Size ctrlSize)
        {
            return new Rectangle(new Point(ctrlSize.Width + SIZE / 3, 0), handleArea.Size);
        }

        public override void PaintAt(Graphics graphics, Point offset)
        {
            graphics.FillRectangle(Brushes.Yellow, handleArea.Translate(offset.X, offset.Y));
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            EditorShell.Shell.TraceLine("Leave  positioner");
            this.Cursor = Cursors.Default;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            EditorShell.Shell.TraceLine("Enter positioner");
            this.Cursor = Cursors.Hand;
        }
 
    }
}
