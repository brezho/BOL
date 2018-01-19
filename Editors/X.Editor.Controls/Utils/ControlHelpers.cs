using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace X.Editor.Controls.Utils
{

    public static class ControlHelpers
    {
        public static void MakeLocationRelativeTo(this Control ctrl, Control target, int dx, int dy, KnownPoint fromPoint = KnownPoint.TopLeft)
        {
            ctrl.Location = target.Bounds.GetLocationOf(fromPoint).Translate(dx, dy);
            target.LocationChanged += (s, a) =>
             {
                 ctrl.Location = target.Bounds.GetLocationOf(fromPoint).Translate(dx, dy);
             };
            target.SizeChanged += (s, a) =>
            {
                ctrl.Location = target.Bounds.GetLocationOf(fromPoint).Translate(dx, dy);
            };
        }

        public static void MakeSizeRelativeTo(this Control ctrl, Control target, int dx1, int dy1, int dx2, int dy2)
        {
            ctrl.Size = target.Bounds.Wrapper(dx1, dy1, dx2, dy2).Size;
            target.LocationChanged += (s, a) =>
            {
                ctrl.Size = target.Bounds.Wrapper(dx1, dy1, dx2, dy2).Size;
            };
            target.SizeChanged += (s, a) =>
            {
                ctrl.Size = target.Bounds.Wrapper(dx1, dy1, dx2, dy2).Size;
            };
        }

        public static void IsVisibleOnFocusOf(this Control ctrl, Control target)
        {
            ctrl.Visible = target.Focused;
            target.GotFocus += (s, a) => ctrl.Visible = true;
            target.LostFocus += (s, a) => ctrl.Visible = ctrl.Focused;
            ctrl.LostFocus += (s, a) => ctrl.Visible = target.Focused;
        }
    }

}
