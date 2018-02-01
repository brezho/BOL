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
    class Debugger : AdornerBase
    {
        const int SIZE = 12;
        protected override int ZIndex => 100;

        public Debugger(Surface surface, Control target) : base(surface, target)
        {
            BackColor = Color.Gainsboro;
            Size = new Size(SIZE, SIZE);
            this.MakeLocationRelativeTo(target, SIZE / 3, 2 * SIZE, KnownPoint.TopRight);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }
    }
}
