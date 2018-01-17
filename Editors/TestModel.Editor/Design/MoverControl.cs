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
    class MoverControlSuppressDesignTime { }
    class MoverControl : Control
    {
        Control _linkedControl;
        IEditorContainer editorContainer;
        public MoverControl(Control control, IEditorContainer container)
        {
            _linkedControl = control;
            editorContainer = container;

            this.Size = new Size(12, 12);
            this.ForeColor = Color.Yellow;

            Visible = control.Focused;
            control.GotFocus += (s, a) =>
            {
                this.Visible = true;
            };
            control.LostFocus += (s, a) =>
            {
                this.Visible = this.Focused;
            };
            this.LostFocus += (s, a) =>
            {
                this.Visible = _linkedControl.Focused;
            };
            control.Move += (s, a) => { SetPosition(); };

            SetPosition();
        }

        private void SetPosition()
        {
            Location = new Point(_linkedControl.Right, _linkedControl.Top);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            SetPosition();
            e.Graphics.FillRectangle(new SolidBrush(ForeColor), this.ClientRectangle);
        }

        bool moving = false;
        Point moveStartLocation;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (moving)
            {
                // Get the difference between the two points
                int xDiff = e.Location.X - moveStartLocation.X;
                int yDiff = e.Location.Y - moveStartLocation.Y;
                _linkedControl.Location = new Point(_linkedControl.Location.X + xDiff, _linkedControl.Location.Y + yDiff);
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            moving = false;
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            moving = true;
            moveStartLocation = e.Location;
        }
    }
}
