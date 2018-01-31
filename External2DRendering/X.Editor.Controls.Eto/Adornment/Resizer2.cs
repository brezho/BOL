using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls.Utils;

namespace X.Editor.Controls.Adornment
{
    partial class X { }
    public class Resizer2 : AdornerBase
    {
        const int GRIPS_SIZE = 6;

        Dictionary<KnownPoint, Rectangle> _handles = null;
        Rectangle _borderLineArea;

        //Point mouseMoveStartLocation;
        //KnownPoint currentlyHoveredGrip;
        //bool isResizing = false;
        public override Rectangle GetRelativeBoundaries(Size ctrlSize)
        {
            var marginAround = GRIPS_SIZE + GRIPS_SIZE / 2;
            var relativeArea = new Rectangle(new Point(-marginAround, -marginAround), ctrlSize.Grow(2 * marginAround, 2 * marginAround));
            _borderLineArea = Rectangle.Empty.Translate(GRIPS_SIZE / 2, GRIPS_SIZE / 2).Grow(ctrlSize.Width + 2 * GRIPS_SIZE, ctrlSize.Height + 2 * GRIPS_SIZE);

            var gripsSize = new Size(GRIPS_SIZE, GRIPS_SIZE);
            var gripsAlignedTo = _borderLineArea.Translate(-GRIPS_SIZE / 2, -GRIPS_SIZE / 2);
            _handles = new Dictionary<KnownPoint, Rectangle>()
                    {
                        { KnownPoint.TopLeft , new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.TopLeft), gripsSize)},
                        { KnownPoint.TopMiddle, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.TopMiddle), gripsSize) },
                        { KnownPoint.TopRight, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.TopRight), gripsSize)},
                        { KnownPoint.MiddleRight, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.MiddleRight), gripsSize)},
                        { KnownPoint.BottomRight, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.BottomRight), gripsSize)},
                        { KnownPoint.BottomMiddle, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.BottomMiddle), gripsSize)},
                        { KnownPoint.BottomLeft, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.BottomLeft), gripsSize)},
                        { KnownPoint.MiddleLeft, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.MiddleLeft), gripsSize)},
                        { KnownPoint.Center, new Rectangle(gripsAlignedTo.GetLocationOf(KnownPoint.Center), gripsSize)},
                    };

            // border line area (leave 1 Grip size all around control)

            return relativeArea;

        }
        public override void PaintAt(Graphics graphics, Point offset)
        {
            using (var p = new Pen(Color.Red, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
            {
                graphics.DrawRectangle(p, _borderLineArea.Translate(offset.X, offset.Y));
            }

            graphics.FillRectangles(Brushes.Red, _handles.Values.Select(x => x.Translate(offset.X, offset.Y)).ToArray());
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            EditorShell.Shell.TraceLine("Leave  resizer");
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            EditorShell.Shell.TraceLine("Enter resizer");
        }

        public Cursor GetHitTests(Point location)
        {
            foreach (var handle in _handles)
            {
                if (handle.Value.Contains(location))
                {
                    switch (handle.Key)
                    {
                        case KnownPoint.BottomRight:
                            return Cursors.SizeNWSE;
                        case KnownPoint.MiddleRight:
                            return Cursors.SizeWE;
                        case KnownPoint.TopRight:
                            return Cursors.SizeNESW;
                        case KnownPoint.BottomLeft:
                            return Cursors.SizeNESW;
                        case KnownPoint.MiddleLeft:
                            return Cursors.SizeWE;
                        case KnownPoint.TopLeft:
                            return Cursors.SizeNWSE;
                        case KnownPoint.BottomMiddle:
                            return Cursors.SizeNS;
                        case KnownPoint.TopMiddle:
                            return Cursors.SizeNS;
                        case KnownPoint.Center:
                            return Cursors.Cross;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
            return Cursors.Default;
        }

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    if (!isResizing)
        //    {
        //        currentlyHoveredGrip = _gripsBounds.FirstOrDefault(x => x.Value.Contains(e.Location)).Key;

        //        switch (currentlyHoveredGrip)
        //        {
        //            case KnownPoint.BottomRight:
        //                Cursor = Cursors.SizeNWSE;
        //                break;
        //            case KnownPoint.MiddleRight:
        //                Cursor = Cursors.SizeWE;
        //                break;
        //            case KnownPoint.TopRight:
        //                Cursor = Cursors.SizeNESW;
        //                break;
        //            case KnownPoint.BottomLeft:
        //                Cursor = Cursors.SizeNESW;
        //                break;
        //            case KnownPoint.MiddleLeft:
        //                Cursor = Cursors.SizeWE;
        //                break;
        //            case KnownPoint.TopLeft:
        //                Cursor = Cursors.SizeNWSE;
        //                break;
        //            case KnownPoint.BottomMiddle:
        //                Cursor = Cursors.SizeNS;
        //                break;
        //            case KnownPoint.TopMiddle:
        //                Cursor = Cursors.SizeNS;
        //                break;
        //            case KnownPoint.Center:
        //                Cursor = Cursors.Cross;
        //                break;
        //            default:
        //                Cursor = Cursors.Default;
        //                break;

        //        }
        //    }
        //    else
        //    {
        //        var currentMouseLocation = this.PointToScreen(e.Location);
        //        var deltaX = currentMouseLocation.X - mouseMoveStartLocation.X;
        //        var deltaY = currentMouseLocation.Y - mouseMoveStartLocation.Y;
        //        Rectangle newBoundaries = Target.Bounds.Grow(currentlyHoveredGrip, deltaX, deltaY);
        //        mouseMoveStartLocation = currentMouseLocation;
        //        Target.SetBounds(newBoundaries.X, newBoundaries.Y, newBoundaries.Width, newBoundaries.Height, BoundsSpecified.All);
        //    }
        //}
        //protected override void OnMouseDown(MouseEventArgs e)
        //{
        //    if (currentlyHoveredGrip != default(KnownPoint))
        //    {
        //        isResizing = true;
        //        mouseMoveStartLocation = this.PointToScreen(e.Location);
        //    }
        //}
        //protected override void OnMouseUp(MouseEventArgs e)
        //{
        //    isResizing = false;
        //}

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    base.OnPaint(e);
        //    e.Graphics.DrawRectangle(_borderPen, _borderBounds);
        //    e.Graphics.FillRectangles(_borderPen.Brush, _gripsBounds.Values.ToArray());
        //}
    }
}

