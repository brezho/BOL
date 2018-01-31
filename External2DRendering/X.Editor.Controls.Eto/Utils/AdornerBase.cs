using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls.Gdi;
using X.Editor.Model;

namespace X.Editor.Controls.Utils
{
    public partial class X { }
    public abstract class AdornerBase : BufferedControl, IAdorner
    {
        public AdornerBase() : base()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        internal IEditorContainer EditorShell { get; set; }
        internal Surface.SurfaceControlWrapper Wrapper { get; set; }

        public virtual Cursor GetHitTests(Point location)
        {
            return Cursors.Default;
        }

        public abstract Rectangle GetRelativeBoundaries(Size ctrlSize);

        public virtual void PaintAt(Graphics graphics, Point offset)
        {

        }
        internal void InternalMouseMove(MouseEventArgs e) { OnMouseMove(e); }
        internal void InternalMouseEnter(EventArgs e) { OnMouseEnter(e); }
        internal void InternalMouseLeave(EventArgs e) { OnMouseLeave(e); }
        internal void InternalMouseUp(MouseEventArgs e) { OnMouseUp(e); }
        internal void InternalMouseDown(MouseEventArgs e) { OnMouseDown(e); }
        internal void InternalMouseClick(MouseEventArgs e) { OnMouseClick(e); }
        internal void InternalMouseDoubleClick(MouseEventArgs e) { OnMouseDoubleClick(e); }
        internal void InternalMouseWheel(MouseEventArgs e) { OnMouseWheel(e); }
    }
}
