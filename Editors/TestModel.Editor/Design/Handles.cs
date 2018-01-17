using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Model;

namespace TestModel.Editor.Design
{
    class HandlesControlSuppressDesignTime { }

    public class Handles : Control, IAdorner
    {
        Control _linkedControl;
        IEditorContainer editorContainer;

        const int margin = 8;
        const int handleSize = 4;
        Pen _border = new Pen(Color.Red, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };



        public Handles(Control control, IEditorContainer container) : base()
        {
            _linkedControl = control;
            editorContainer = container;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            UpdateStyles();
            DoubleBuffered = true;
            Reposition();
        }

        public void Reposition()
        {
            Location = new Point(_linkedControl.Location.X - margin, _linkedControl.Location.Y - margin);

            var w = _linkedControl.Width + 2 * margin;
            var h = _linkedControl.Height + 2 * margin;
            if (Width != w) Width = w;
            if (Height != h) Height = h;

            HandlesRectangles = null;
        }

        HandlePositions LastKnownHandle = HandlePositions.None;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!isResizing)
            {
                var rectangles = GetRectangles();
                LastKnownHandle = rectangles.FirstOrDefault(x => x.Value.Contains(e.Location)).Key;

                switch (LastKnownHandle)
                {
                    case HandlePositions.None:
                        Cursor = Cursors.Default;
                        break;
                    case HandlePositions.BottomRight:
                        Cursor = Cursors.SizeNWSE;
                        break;
                    case HandlePositions.MiddleRight:
                        Cursor = Cursors.SizeWE;
                        break;
                    case HandlePositions.TopRight:
                        Cursor = Cursors.SizeNESW;
                        break;
                    case HandlePositions.BottomLeft:
                        Cursor = Cursors.SizeNESW;
                        break;
                    case HandlePositions.MiddleLeft:
                        Cursor = Cursors.SizeWE;
                        break;
                    case HandlePositions.TopLeft:
                        Cursor = Cursors.SizeNWSE;
                        break;
                    case HandlePositions.BottomMiddle:
                        Cursor = Cursors.SizeNS;
                        break;
                    case HandlePositions.TopMiddle:
                        Cursor = Cursors.SizeNS;
                        break;
                }
            }
            else
            {
                //      editorContainer.Shell.TraceLine("Resizing from " + LastKnownHandle.ToString());

                var newHeight = originalSize.Height + e.Location.Y - originalLocation.Y;
                var newWidth = originalSize.Width + e.Location.X - originalLocation.X;

                switch (LastKnownHandle)
                {
                    case HandlePositions.BottomRight:
                        _linkedControl.Height = newHeight;
                        _linkedControl.Width = newWidth;
                        break;
                    case HandlePositions.MiddleRight:
                        Cursor = Cursors.SizeWE;
                        break;
                    case HandlePositions.TopRight:
                        Cursor = Cursors.SizeNESW;
                        break;
                    case HandlePositions.BottomLeft:
                        Cursor = Cursors.SizeNESW;
                        break;
                    case HandlePositions.MiddleLeft:
                        Cursor = Cursors.SizeWE;
                        break;
                    case HandlePositions.TopLeft:
                        Cursor = Cursors.SizeNWSE;
                        break;
                    case HandlePositions.BottomMiddle:
                        Cursor = Cursors.SizeNS;
                        break;
                    case HandlePositions.TopMiddle:
                        Cursor = Cursors.SizeNS;
                        break;
                }
            }
        }

        bool isResizing = false;
        Point originalLocation;
        Size originalSize;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (LastKnownHandle != HandlePositions.None)
            {
                isResizing = true;
                originalLocation = e.Location;
                originalSize = _linkedControl.Size;
            }

        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            isResizing = false;
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            DrawBorders(e.Graphics);
            DrawHandles(e.Graphics);
        }

        enum HandlePositions
        {
            None,
            TopLeft,
            TopMiddle,
            TopRight,
            MiddleRight,
            BottomRight,
            BottomMiddle,
            BottomLeft,
            MiddleLeft,
        }

        Dictionary<HandlePositions, Rectangle> HandlesRectangles = null;
        Dictionary<HandlePositions, Rectangle> GetRectangles()
        {
            if (HandlesRectangles == null)
            {
                var leftAlign = ClientRectangle.Left;
                var rightAlign = ClientRectangle.Right - handleSize;
                var topAlign = ClientRectangle.Top;
                var bottomAlign = ClientRectangle.Bottom - handleSize;

                var verticalMiddleAlign = leftAlign + ClientRectangle.Width / 2 - handleSize / 2;
                var horizontalMiddleAlign = topAlign + ClientRectangle.Height / 2 - handleSize / 2;

                var size = new Size(handleSize, handleSize);

                HandlesRectangles = new Dictionary<HandlePositions, Rectangle>()
                {
                    { HandlePositions.TopLeft , new Rectangle(new Point(leftAlign, topAlign), size)},
                    { HandlePositions.TopMiddle, new Rectangle(new Point(verticalMiddleAlign, topAlign), size) },
                    { HandlePositions.TopRight, new Rectangle(new Point(rightAlign, topAlign), size)},
                    { HandlePositions.MiddleRight, new Rectangle(new Point(rightAlign, horizontalMiddleAlign), size)},
                    { HandlePositions.BottomRight, new Rectangle(new Point(rightAlign, bottomAlign), size)},
                    { HandlePositions.BottomMiddle, new Rectangle(new Point(verticalMiddleAlign, bottomAlign), size)},
                    { HandlePositions.BottomLeft, new Rectangle(new Point(leftAlign, bottomAlign), size)},
                    { HandlePositions.MiddleLeft, new Rectangle(new Point(leftAlign, horizontalMiddleAlign), size)},
                };
            }
            return HandlesRectangles;
        }

        private void DrawHandles(Graphics graphics)
        {
            graphics.FillRectangles(_border.Brush, GetRectangles().Values.ToArray());
        }
        private void DrawBorders(Graphics graphics)
        {
            var borderRectangle = new Rectangle(ClientRectangle.Location + new Size(handleSize / 2, handleSize / 2), ClientRectangle.Size - new Size(handleSize, handleSize));
            graphics.DrawRectangle(_border, borderRectangle);
        }
    }
}
