
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls.Utils;

namespace X.Editor.Controls
{
    class AdornersX { }
    public class AdornersControl : Control
    {
        List<IAdorner> _adorners = new List<IAdorner>();
        public Surface Surface { get; private set; }
        public Control Target { get; private set; }
        internal AdornersControl(Surface surface, Control target)
        {
            Target = target;
            Surface = surface;

            //SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            //UpdateStyles();
            //DoubleBuffered = true;

            RecomputeBounds();
            this.IsVisibleOnFocusOf(Target);
            Target.SizeChanged += (s, a) => { RecomputeBounds(); };
            Target.LocationChanged += (s, a) => { RecomputeBounds(); };
            SendToBack();
          //  BackColor = Color.Transparent;
        }


        Rectangle[] _rectangles;
        private void RecomputeBounds()
        {
            Surface.Log("TargetControl Bounds", Target.Bounds);
            _rectangles = _adorners.Select(x => x.GetBounds(Target.Bounds)).ToArray();

            Rectangle computedBounds = Rectangle.Empty;
            foreach (var r in _rectangles)
            {
                computedBounds = Rectangle.Union(r, computedBounds);
            }

            if (computedBounds != Bounds) SetBounds(computedBounds.X, computedBounds.Y, computedBounds.Width +1 , computedBounds.Height+1);
            Surface.Log("Bounds", computedBounds);
        }

        internal void AddAdorner(IAdorner adorner)
        {
            _adorners.Add(adorner);
            RecomputeBounds();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            e.Graphics.DrawRectangles(new Pen(Brushes.Red), _rectangles);
        }
    }
}
