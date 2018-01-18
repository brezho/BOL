using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace X.Editor.Controls.Adornment
{
    class AdornerX { }
    public abstract class AbstractAdorner : Control
    {
        protected Surface Surface { get; private set; }
        protected Control Target { get; private set; }
        internal AbstractAdorner(Surface surface, Control target)
        {
            Surface = surface;
            Target = target;
        }
        protected internal virtual void OnTargetResized() { }
        protected internal virtual void OnTargetMoved() { }
        protected internal virtual void OnTargetGotFocus() { }
    }
}
