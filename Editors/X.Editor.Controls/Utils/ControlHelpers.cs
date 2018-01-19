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
        public static void HasLocationRelativeTo(this Control ctrl, Control target, int dx, int dy, KnownPoint fromPoint = KnownPoint.TopLeft)
        {
            target.LocationChanged += (s, a) =>
            {
                ctrl.Location = target.Bounds.GetLocationOf(fromPoint).Translate(dx, dy);
            };
            target.SizeChanged += (s, a) =>
            {
                ctrl.Location = target.Bounds.GetLocationOf(fromPoint).Translate(dx, dy);
            };
            ctrl.Location = target.Bounds.GetLocationOf(fromPoint).Translate(dx, dy);
        }
        //public static void Wraps(this Control ctrl, Control target, int dx1, int dy1, int dx2, int dy2)
        //{
        //    target.LocationChanged += (s, a) =>
        //    {
        //        ctrl.Location = target.GetCoordinatesOf(MainPoint.TopLeft).Translate(-dx1, -dy1);
        //    };
        //    target.SizeChanged += (s, a) =>
        //    {
        //        // var loc = target.GetCoordinatesOf(MainPoint.TopLeft).Translate(-dx1, -dy1);
        //        var size = new Size(dx1 + target.Width + dx2, dy1 + target.Height + dy2);
        //        ctrl.SetBounds(0, 0, size.Width, size.Height, BoundsSpecified.Size);
        //    };
        //    var newLoc = target.GetCoordinatesOf(MainPoint.TopLeft).Translate(-dx1, -dy1);
        //    var newSize = new Size(dx1 + target.Width + dx2, dy1 + target.Height + dy2);
        //    ctrl.SetBounds(newLoc.X, newLoc.Y, newSize.Width, newSize.Height, BoundsSpecified.All);
        //    ctrl.SendToBack();
        //}

        public static void IsVisibleOnFocusOf(this Control ctrl, Control target)
        {
            target.GotFocus += (s, a) => ctrl.Visible = true;
            target.LostFocus += (s, a) => ctrl.Visible = ctrl.Focused;
            ctrl.Visible = target.Focused;
        }
    }


    public static class Helpers
    {
    }
}
