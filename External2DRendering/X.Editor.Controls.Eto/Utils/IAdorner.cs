using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace X.Editor.Controls.Utils
{
    public interface IAdorner
    {
        Rectangle GetRelativeBoundaries(Size ctrlSize);
        void PaintAt(Graphics graphics, Point offset);
    }
}
