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
    public abstract class AdornerBase : UserControl
    {
        protected Surface Surface { get; private set; }
        protected Control Target { get; private set; }
        protected virtual int ZIndex { get { return -1; } }
        internal int RelativeZIndex { get { return ZIndex; } }

        public AdornerBase(Surface surface, Control target) : base()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            Surface = surface;
            Target = target;
        }

        protected internal virtual void OnAttached()
        {

        }
        
        internal void InternalMouseMove(MouseEventArgs e)
        {
            OnMouseMove(e);
        }
        internal void InternalMouseEnter(EventArgs e)
        {
            OnMouseEnter(e);
        }
        internal void InternalMouseLeave(EventArgs e)
        {
            OnMouseLeave(e);
        }
        internal void InternalMouseUp(MouseEventArgs e) { OnMouseUp(e); }
        internal void InternalMouseDown(MouseEventArgs e) { OnMouseDown(e); }
        internal void InternalMouseClick(MouseEventArgs e) { OnMouseClick(e); }
        internal void InternalMouseDoubleClick(MouseEventArgs e) { OnMouseDoubleClick(e); }
        internal void InternalMouseWheel(MouseEventArgs e) { OnMouseWheel(e); }
        internal void InternalPaint(PaintEventArgs e) { OnPaint(e); }
    }
}
