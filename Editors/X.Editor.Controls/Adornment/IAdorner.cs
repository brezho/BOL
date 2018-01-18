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
    public abstract class IAdorner : Control
    {
        protected Surface Surface { get; private set; }
        protected Control Target { get; private set; }
        internal IAdorner(Surface surface, Control target)
        {
            Surface = surface;
            Target = target;
        }
        protected internal virtual void OnTargetResized(Rectangle newBoundaries) { }
        protected internal virtual void OnTargetMoved(Rectangle newBoundaries) { }
    }
}
