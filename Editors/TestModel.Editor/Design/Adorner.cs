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
    public interface IAdorner
    {
        void Reposition();
    }

    public static class Adorner
    {
        private static HashSet<Control> movableControls = new HashSet<Control>();
        private static HashSet<Control> resizableControls = new HashSet<Control>();
        private static HashSet<Control> handlesControls = new HashSet<Control>();


        static void MakeAdorner(IAdorner adorner, Control adorned, IEditorContainer container)
        {
            var ctrl = adorner as Control;

            ctrl.Visible = adorned.Focused;

            adorned.GotFocus += (s, a) =>
            {
                ctrl.Visible = true;
            };

            adorned.LostFocus += (s, a) =>
            {
                ctrl.Visible = ctrl.Focused;
            };

            ctrl.LostFocus += (s, a) =>
            {
                ctrl.Visible = adorned.Focused;
            };

            adorned.Move += (s, a) => {

                adorner.Reposition();
            };
            adorned.Resize += (s, a) =>
            {
                adorner.Reposition();
            };
        }


        public static void MakeHandles(Control control, IEditorContainer container)
        {
            if (!handlesControls.Add(control)) return;
            var ctrl = new Handles(control, container);
            MakeAdorner(ctrl, control, container);
            control.Parent.Controls.Add(ctrl);
        }
        public static void MakeMovable(Control control, IEditorContainer container)
        {
            if (!movableControls.Add(control)) return;
            control.Parent.Controls.Add(new MoverControl(control, container));
        }
        public static void MakeResizable(Control control, IEditorContainer container)
        {
            if (!resizableControls.Add(control)) return;
            //control.Parent.Controls.Add(new ResizerControl(control, container));
        }
        //class ResizerControl : Control
        //{
        //    Control _linkedControl;
        //    IEditorContainer editorContainer;
        //    public ResizerControl(Control control, IEditorContainer container)
        //    {
        //        _linkedControl = control;
        //        editorContainer = container;

        //        this.Size = new Size(12, 12);
        //        this.ForeColor = Color.Green;
        //        Visible = false;
        //        SetPosition();
        //        control.GotFocus += (s, a) =>
        //        {
        //            this.Visible = true;
        //        };
        //        control.LostFocus += (s, a) =>
        //        {
        //            this.Visible = this.Focused;
        //        };
        //        this.LostFocus += (s, a) =>
        //        {
        //            this.Visible = _linkedControl.Focused;
        //        };
        //        control.Move += (s, a) => { SetPosition(); };
        //    }

        //    private void SetPosition()
        //    {
        //        Location = new Point(_linkedControl.Right - this.Width, _linkedControl.Top);
        //        this.BringToFront();
        //    }
        //    protected override void OnPaint(PaintEventArgs e)
        //    {
        //        if (Visible) e.Graphics.FillRectangle(new SolidBrush(ForeColor), this.ClientRectangle);
        //    }

        //    bool moving = false;
        //    Point moveStartLocation;
        //    protected override void OnMouseMove(MouseEventArgs e)
        //    {
        //        if (moving)
        //        {
        //            // Get the difference between the two points
        //            int xDiff = e.Location.X - moveStartLocation.X;
        //            int yDiff = e.Location.Y - moveStartLocation.Y;
        //            _linkedControl.Location = new Point(_linkedControl.Location.X + xDiff, _linkedControl.Location.Y + yDiff);
        //        }
        //    }
        //    protected override void OnMouseLeave(EventArgs e)
        //    {
        //        this.Cursor = Cursors.Default;
        //    }

        //    protected override void OnMouseEnter(EventArgs e)
        //    {
        //        this.Cursor = Cursors.Hand;
        //    }

        //    protected override void OnMouseUp(MouseEventArgs e)
        //    {
        //        moving = false;
        //    }
        //    protected override void OnMouseDown(MouseEventArgs e)
        //    {
        //        moving = true;
        //        moveStartLocation = e.Location;
        //    }
        //}

    }
}
