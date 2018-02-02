using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls.Gdi;
using X.Editor.Controls.Utils;

namespace X.Editor.Controls.Adornment
{
    partial class X { }

    public class Resizer : AdornerBase
    {
        const int GRIPS_SIZE = 6;
        const int MARGIN = 9; // 1.5 * GRIPS_SIZE
        protected override int ZIndex => -100;

        public Resizer(Surface surface, Control target) : base(surface, target)
        {
            this.MakeLocationRelativeTo(target, -MARGIN, -MARGIN, KnownPoint.TopLeft);
            this.MakeSizeRelativeTo(target, new Point(2 * MARGIN, 2 * MARGIN));
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            RecomputeLocations();
            base.OnSizeChanged(e);
        }

        List<KeyValuePair<KnownPoint, Rectangle>> handles = null;
        Rectangle border;

        GraphicsPath borderPath;
        GraphicsPath[] handlesPath;
        void RecomputeLocations()
        {
            border = Rectangle.Empty.Translate(GRIPS_SIZE / 2, GRIPS_SIZE / 2).Grow(Target.Width + 2 * GRIPS_SIZE, Target.Height + 2 * GRIPS_SIZE);
            var gripsSize = new Size(GRIPS_SIZE, GRIPS_SIZE);
            var gripsAlignedTo = border.Translate(-GRIPS_SIZE / 2, -GRIPS_SIZE / 2);
            handles = new List<KeyValuePair<KnownPoint, Rectangle>>(
                new[] {
                       new KeyValuePair<KnownPoint, Rectangle>(KnownPoint.TopLeft, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.TopLeft), gripsSize)),
                        new KeyValuePair<KnownPoint, Rectangle>(KnownPoint.TopMiddle, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.TopMiddle), gripsSize)),
                        new KeyValuePair<KnownPoint, Rectangle>(KnownPoint.TopRight, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.TopRight), gripsSize)),
                        new KeyValuePair<KnownPoint, Rectangle>(KnownPoint.MiddleRight, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.MiddleRight), gripsSize)),
                        new KeyValuePair<KnownPoint, Rectangle>(KnownPoint.BottomRight, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.BottomRight), gripsSize)),
                        new KeyValuePair<KnownPoint, Rectangle>(KnownPoint.BottomMiddle, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.BottomMiddle), gripsSize)),
                        new KeyValuePair<KnownPoint, Rectangle>(KnownPoint.BottomLeft, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.BottomLeft), gripsSize)),
                        new KeyValuePair<KnownPoint, Rectangle>(KnownPoint.MiddleLeft, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.MiddleLeft), gripsSize))
                    //  { KnownPoint.Center, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.Center), gripsSize)},
                    });

            borderPath = new GraphicsPath();
            borderPath.AddRectangle(border);

            handlesPath = handles.Select(y => { var r = new GraphicsPath(); r.AddRectangle(y.Value); return r; }).ToArray();

            

        }

        Point mouseMoveStartLocation;
        KnownPoint currentlyHoveredGrip;
        bool isResizing = false;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!isResizing)
            {
                currentlyHoveredGrip = handles.FirstOrDefault(x => x.Value.Contains(e.Location)).Key;

                switch (currentlyHoveredGrip)
                {
                    case KnownPoint.BottomRight:
                        Cursor = Cursors.SizeNWSE;
                        break;
                    case KnownPoint.MiddleRight:
                        Cursor = Cursors.SizeWE;
                        break;
                    case KnownPoint.TopRight:
                        Cursor = Cursors.SizeNESW;
                        break;
                    case KnownPoint.BottomLeft:
                        Cursor = Cursors.SizeNESW;
                        break;
                    case KnownPoint.MiddleLeft:
                        Cursor = Cursors.SizeWE;
                        break;
                    case KnownPoint.TopLeft:
                        Cursor = Cursors.SizeNWSE;
                        break;
                    case KnownPoint.BottomMiddle:
                        Cursor = Cursors.SizeNS;
                        break;
                    case KnownPoint.TopMiddle:
                        Cursor = Cursors.SizeNS;
                        break;
                    case KnownPoint.Center:
                        Cursor = Cursors.Cross;
                        break;
                    default:
                        Cursor = Cursors.Default;
                        break;

                }
            }
            else
            {
                var currentMouseLocation = this.PointToScreen(e.Location);
                var deltaX = currentMouseLocation.X - mouseMoveStartLocation.X;
                var deltaY = currentMouseLocation.Y - mouseMoveStartLocation.Y;
                Rectangle newBoundaries = Target.Bounds.Grow(currentlyHoveredGrip, deltaX, deltaY);
                mouseMoveStartLocation = currentMouseLocation;
                Target.SetBounds(newBoundaries.X, newBoundaries.Y, newBoundaries.Width, newBoundaries.Height, BoundsSpecified.All);
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (currentlyHoveredGrip != default(KnownPoint))
            {
                isResizing = true;
                mouseMoveStartLocation = this.PointToScreen(e.Location);
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            isResizing = false;
        }


        static Pen redPen = new Pen(Brushes.Red, 1);
        protected override void OnPaint(PaintEventArgs e)
        {
            if (e.ClipRectangle.IntersectsWith(border))
            {
                e.Graphics.DrawPath(redPen, borderPath);
            }
            for (int i = 0; i < handles.Count; i++)
            {
                if (handles[i].Value.IntersectsWith(e.ClipRectangle))
                {
                    e.Graphics.FillPath(redPen.Brush, handlesPath[i]);
                }
            }
            base.OnPaint(e);
        }
    }
}

