using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestModel.Editor.Design;
using X.Editor.Model;

namespace TestModel.Editor
{
    class GrabHandlesControlSuppressDesignTime { }

    public class GrabHandles : Control, IAdorner
    {
        public const int BOX_SIZE = 3;
        Control _linkedControl;
        IEditorContainer editorContainer;

        public void Reposition()
        {
            ResetBounds();
            Location = new Point(TopLeft.X, TopLeft.Y);
            this.Width = TopRight.X - TopLeft.X + TopRight.Width - 1;
            this.Height = BottomLeft.Y - TopLeft.Y + BottomLeft.Height - 1;

            editorContainer.Shell.TraceLine();
            editorContainer.Shell.TraceLine("Ctrl point:" + _linkedControl.Location);
            editorContainer.Shell.TraceLine("Ctrl size:" + _linkedControl.Size);
            editorContainer.Shell.TraceLine("Adorner point:" + this.Location);
            editorContainer.Shell.TraceLine("Adorner size:" + this.Size);
        }

        public GrabHandles(Control control, IEditorContainer container)
        {

            _linkedControl = control;
            editorContainer = container;


            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            UpdateStyles();
            this.DoubleBuffered = true;

            this.BorderWidth = 4;
            Reposition();
        }


        public Rectangle BorderBounds { get; private set; }
        public int BorderWidth { get; set; }
        public bool Locked { get; set; }

        public Rectangle TotalBounds
        {
            get { return Rectangle.Union(this.TopLeft, this.BottomRight); }
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Draw(e.Graphics, true);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        }

        internal void Draw(Graphics g, bool firstSelection)
        {
            //ControlPaint.ContrastControlDark


            //ControlPaint.DrawBorder(g, this.BorderBounds, Color.Red, ButtonBorderStyle.Dotted);

            if (this.Locked)
            {
                this.DrawLock(g);
            }
            else
            {
                this.DrawGrabHandle(g, this.TopLeft, firstSelection);
                this.DrawGrabHandle(g, this.TopMiddle, firstSelection);
                this.DrawGrabHandle(g, this.TopRight, firstSelection);
                this.DrawGrabHandle(g, this.MiddleLeft, firstSelection);
                this.DrawGrabHandle(g, this.MiddleRight, firstSelection);
                this.DrawGrabHandle(g, this.BottomLeft, firstSelection);
                this.DrawGrabHandle(g, this.BottomMiddle, firstSelection);
                this.DrawGrabHandle(g, this.BottomRight, firstSelection);
            }

        }

        internal void ResetBounds()
        {
            BorderBounds = new Rectangle(_linkedControl.Location.X - this.BorderWidth,
                                              _linkedControl.Location.Y - this.BorderWidth,
                                              _linkedControl.Width + 2 * this.BorderWidth,
                                              _linkedControl.Height + 2 * this.BorderWidth);

        }

        private void DrawGrabHandle(Graphics g, Rectangle rect, bool firstSelection)
        {
            if (firstSelection)
            {
                var rect1 = rect;
                var rect2 = rect;
                var innerRect = rect;
                innerRect.Inflate(-1, -1);
                rect1.X += 1;
                rect1.Width -= 2;
                rect2.Y += 1;
                rect2.Height -= 2;

                g.FillRectangle(Brushes.Blue, rect1);
                g.FillRectangle(Brushes.Blue, rect2);
                g.FillRectangle(Brushes.Blue, innerRect);
            }
            else
            {
                g.FillRectangle(Brushes.Red, rect);
            }
        }

        private void DrawLock(Graphics g)
        {
            var rect = this.TopLeft;
            rect.X -= 1;
            rect.Width -= 1;
            rect.Height -= 2;

            var innerRect = rect;
            innerRect.Inflate(-1, -1);

            g.FillRectangle(Brushes.White, innerRect);
            g.DrawRectangle(Pens.Black, rect);

            var outerHandleRect1 = rect;
            outerHandleRect1.Y -= 2;
            outerHandleRect1.Height = 2;
            outerHandleRect1.Width = 5;
            outerHandleRect1.X += 1;

            var outerHandleRect2 = outerHandleRect1;
            outerHandleRect2.Y -= 1;
            outerHandleRect2.X += 1;
            outerHandleRect2.Width = 3;
            outerHandleRect2.Height = 1;

            var innerHandleRect = outerHandleRect1;
            innerHandleRect.X += 1;
            innerHandleRect.Width = 3;

            g.FillRectangle(Brushes.Black, outerHandleRect1);
            g.FillRectangle(Brushes.Black, outerHandleRect2);
            g.FillRectangle(Brushes.White, innerHandleRect);
        }
        internal Rectangle TopLeft
        {
            get
            {
                return new Rectangle(this.BorderBounds.X - BOX_SIZE,
                                     this.BorderBounds.Y - BOX_SIZE,
                                     2 * BOX_SIZE + 1, 2 * BOX_SIZE + 1);
            }
        }

        internal Rectangle TopRight
        {
            get
            {
                return new Rectangle(this.BorderBounds.Right - BOX_SIZE,
                                     this.BorderBounds.Y - BOX_SIZE,
                                     2 * BOX_SIZE + 1, 2 * BOX_SIZE + 1);
            }
        }

        internal Rectangle TopMiddle
        {
            get
            {
                return new Rectangle(this.BorderBounds.X + this.BorderBounds.Width / 2 - BOX_SIZE,
                                     this.BorderBounds.Y - BOX_SIZE,
                                     2 * BOX_SIZE + 1, 2 * BOX_SIZE + 1);
            }
        }

        internal Rectangle MiddleLeft
        {
            get
            {
                return new Rectangle(this.BorderBounds.X - BOX_SIZE,
                                     this.BorderBounds.Y + this.BorderBounds.Height / 2 - BOX_SIZE,
                                     2 * BOX_SIZE + 1, 2 * BOX_SIZE + 1);
            }
        }

        internal Rectangle MiddleRight
        {
            get
            {
                return new Rectangle(this.BorderBounds.Right - BOX_SIZE,
                                     this.BorderBounds.Y + this.BorderBounds.Height / 2 - BOX_SIZE,
                                     2 * BOX_SIZE + 1, 2 * BOX_SIZE + 1);
            }
        }

        internal Rectangle MiddleMiddle
        {
            get
            {
                return new Rectangle(this.BorderBounds.X + this.BorderBounds.Width / 2 - BOX_SIZE,
                                     this.BorderBounds.Y + this.BorderBounds.Height / 2 - BOX_SIZE,
                                     2 * BOX_SIZE + 1, 2 * BOX_SIZE + 1);
            }
        }

        internal Rectangle BottomLeft
        {
            get
            {
                return new Rectangle(this.BorderBounds.X - BOX_SIZE,
                                     this.BorderBounds.Bottom - BOX_SIZE,
                                     2 * BOX_SIZE + 1, 2 * BOX_SIZE + 1);
            }
        }

        internal Rectangle BottomRight
        {
            get
            {
                return new Rectangle(this.BorderBounds.Right - BOX_SIZE,
                                     this.BorderBounds.Bottom - BOX_SIZE,
                                     2 * BOX_SIZE + 1, 2 * BOX_SIZE + 1);
            }
        }

        internal Rectangle BottomMiddle
        {
            get
            {
                return new Rectangle(this.BorderBounds.X + this.BorderBounds.Width / 2 - BOX_SIZE,
                                     this.BorderBounds.Bottom - BOX_SIZE,
                                     2 * BOX_SIZE + 1, 2 * BOX_SIZE + 1);
            }
        }

    }
}
