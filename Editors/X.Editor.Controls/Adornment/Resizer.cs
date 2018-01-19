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
    class ResizerX { }
    public class Resizer : IAdorner
    {
        public AdornersControl Control { get; set; }

        const int MARGIN = 10;
        const int GRIPS_SIZE = 6;

        Pen _borderPen = new Pen(Color.Red, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };
        Dictionary<KnownPoint, Rectangle> _gripsBounds = null;
        Rectangle _borderBounds = Rectangle.Empty;
        Point mouseMoveStartLocation;
        KnownPoint currentlyHoveredGrip;
        bool isResizing = false;

        public Rectangle GetBounds(Rectangle ctrlBounds)
        {
            var res = ctrlBounds.Translate(-MARGIN, -MARGIN).Grow(2 * MARGIN, 2 * MARGIN);
            return res;
        }

        public Resizer()
        {
        }
        //internal Resizer(Surface surface, Control target) 
        //{
        //    Surface = surface;
        //    Target = target;
        //    this.MakeLocationRelativeTo(target, -MARGIN, -MARGIN);
        //    this.MakeSizeRelativeTo(target, -MARGIN, -MARGIN, MARGIN, MARGIN);

        //    PrecomputeDimensions();
        //    target.SizeChanged += (s, a) => PrecomputeDimensions();
        //    target.LocationChanged += (s, a) => PrecomputeDimensions();
           
        //    //RECENT
        //    //BringToFront();
        //}

        //void PrecomputeDimensions()
        //{
        //    int lineDistanceToBorder = MARGIN / 2;
        //    _borderBounds = ClientRectangle.Translate(lineDistanceToBorder, lineDistanceToBorder).Grow(-MARGIN, -MARGIN);

        //    var gripsSize = new Size(GRIPS_SIZE, GRIPS_SIZE);
        //    var gripsLocationsAlignedOn = _borderBounds.Translate(-GRIPS_SIZE / 2, -GRIPS_SIZE / 2);

        //    _gripsBounds = new Dictionary<KnownPoint, Rectangle>()
        //        {
        //            { KnownPoint.TopLeft , new Rectangle(gripsLocationsAlignedOn.GetLocationOf(KnownPoint.TopLeft), gripsSize)},
        //            { KnownPoint.TopMiddle, new Rectangle(gripsLocationsAlignedOn.GetLocationOf(KnownPoint.TopMiddle), gripsSize) },
        //            { KnownPoint.TopRight, new Rectangle(gripsLocationsAlignedOn.GetLocationOf(KnownPoint.TopRight), gripsSize)},
        //            { KnownPoint.MiddleRight, new Rectangle(gripsLocationsAlignedOn.GetLocationOf(KnownPoint.MiddleRight), gripsSize)},
        //            { KnownPoint.BottomRight, new Rectangle(gripsLocationsAlignedOn.GetLocationOf(KnownPoint.BottomRight), gripsSize)},
        //            { KnownPoint.BottomMiddle, new Rectangle(gripsLocationsAlignedOn.GetLocationOf(KnownPoint.BottomMiddle), gripsSize)},
        //            { KnownPoint.BottomLeft, new Rectangle(gripsLocationsAlignedOn.GetLocationOf(KnownPoint.BottomLeft), gripsSize)},
        //            { KnownPoint.MiddleLeft, new Rectangle(gripsLocationsAlignedOn.GetLocationOf(KnownPoint.MiddleLeft), gripsSize)},
        //            { KnownPoint.Center, new Rectangle(gripsLocationsAlignedOn.GetLocationOf(KnownPoint.Center), gripsSize)},
        //        };
        //}


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

