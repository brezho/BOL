using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls.Utils;

namespace X.Editor.Controls.Gdi
{
    partial class X { }
    public class BufferedControl : UserControl
    {
        GraphicsBuffer buffer;

        public BufferedControl()
        {
            buffer = new GraphicsBuffer(Size);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
        }

        void Draw(Action<Graphics> drawingMethod)
        {
            buffer.Draw(drawingMethod);
            Refresh();
        }
        protected override void OnResize(EventArgs e)
        {
            buffer.Resize(Size);
            base.OnResize(e);
        }

        protected sealed override void OnPaint(PaintEventArgs e)
        {
            buffer.FlushTo(e.Graphics);
            base.OnPaint(e);
        }
    }
}
