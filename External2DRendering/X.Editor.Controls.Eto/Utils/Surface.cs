
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls.Adornment;
using X.Editor.Controls.Controls;
using X.Editor.Controls.Utils;
using X.Editor.Model;

namespace X.Editor.Controls.Utils
{
    partial class X { }
    public class Surface : Control, IEditor
    {
        IEditorContainer container;
        public Surface() : base()
        {
            BackColor = Color.Black;
            Dock = DockStyle.Fill;
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            // TODO: Remove this
            if (e.Control is Button)
            {
                this.Controls.Add(e.Control);
                return;
            }
            if (!(e.Control is SurfaceControlWrapper))
            {
                this.Controls.Remove(e.Control);
                var surface = new SurfaceControlWrapper(container, e.Control);
                this.Controls.Add(surface);
                _surfaceForControl[e.Control] = surface;
            }
        }

        ConcurrentDictionary<Type, HashSet<Control>> _oneTypeOfAdornerPerControl = new ConcurrentDictionary<Type, HashSet<Control>>();
        ConcurrentDictionary<Control, SurfaceControlWrapper> _surfaceForControl = new ConcurrentDictionary<Control, SurfaceControlWrapper>();

        public T Adorn<T>(Control control) where T : AdornerBase, new()
        {
            var set = _oneTypeOfAdornerPerControl.GetOrAdd(typeof(T), new HashSet<Control>());
            if (!set.Add(control)) throw new Exception("Control is already adorned with " + typeof(T).FullName);

            var adorner = new T();
            var surface = _surfaceForControl[control];
            surface.AddAdorner(adorner);

            return adorner;
        }


        bool? first;
        public void ActivateIn(IEditorContainer newWindow)
        {
            container = newWindow;

            var t = new TestGraph();
            t.Location = new Point(200, 200);
            this.Controls.Add(t);

            var b = new Button { Text = "Click", BackColor = Color.Beige, };
            b.Click += (s, a) =>
            {
                t.Location = new Point(50, 50);
                if (!first.HasValue) first = true;
                else
                {
                    if (first == true)
                    {
                        //Adorn<Resizer2>(t);
                        //Adorn<Positioner>(t);
                        //Adorn<Resizer>(b);
                        //Adorn<Positioner>(b);
                        Adorn<Positioner2>(t);
                        first = false;
                    }
                }
            };
            this.Controls.Add(b);
        }

        internal class SurfaceControlWrapper : UserControl //, IMessageFilter
        {
            IEditorContainer container;

            Control guest;
            List<AdornerBase> adorners = new List<AdornerBase>();

            Point guestRequestedLocation = Point.Empty;

            Rectangle[] adornersBoundsRelativeToGuest;
            Rectangle[] adornersBoundsRelativeToThis;
            public SurfaceControlWrapper(IEditorContainer cntainer, Control control)
            {
                // Application.AddMessageFilter(this);
                guest = control;
                container = cntainer;

                control.SizeChanged += GuestSizeChanged;
                control.LocationChanged += GuestLocationChanged;
                control.Paint += GuestPaint;
                control.MouseMove += GuestMouseMove;
                control.MouseUp += GuestMouseUp;
                control.MouseDown += GuestMouseDown;
                //control.MouseEnter += GuestMouseEnter;
                //control.MouseLeave += GuestMouseLeave;
                control.MouseClick += GuestMouseClick;
                control.MouseDoubleClick += GuestMouseDoubleClick;
                control.MouseWheel += GuestMouseWheel;

                EmbedGuest();

                this.Controls.Add(control);
            }


            void EmbedGuest()
            {
                guestRequestedLocation = guest.Location;
                AdjustSizeAndPosition();
            }

            bool adjusting;
            void AdjustSizeAndPosition()
            {
                if (!adjusting)
                {
                    adjusting = true;

                    // compute redefined bounds relative to guest size
                    adornersBoundsRelativeToGuest = new Rectangle[adorners.Count];
                    for (int i = 0; i < adorners.Count; i++)
                    {
                        adornersBoundsRelativeToGuest[i] = adorners[i].GetRelativeBoundaries(guest.Size);
                    }
                    Rectangle overallRelativeBoundaries = new Rectangle(Point.Empty, guest.Size);
                    foreach (var newBound in adornersBoundsRelativeToGuest) overallRelativeBoundaries = Rectangle.Union(overallRelativeBoundaries, newBound);


                    // reposition ourselve and guest locations based on requested clientLocation

                    // our location is client's original location offset by overallRelativeBoundaries if negative
                    var ourX = guestRequestedLocation.X;
                    var ourY = guestRequestedLocation.Y;
                    var guestX = 0;
                    var guestY = 0;

                    if (overallRelativeBoundaries.X < 0)
                    {
                        ourX += overallRelativeBoundaries.X;
                        guestX = -overallRelativeBoundaries.X;
                    }
                    if (overallRelativeBoundaries.Y < 0)
                    {
                        ourY += overallRelativeBoundaries.Y;
                        guestY = -overallRelativeBoundaries.Y;
                    }

                    this.Location = new Point(ourX, ourY);
                    guest.Location = new Point(guestX, guestY);

                    // our size is overallRelativeBoundaries size
                    this.Size = overallRelativeBoundaries.Size;

                    // recompute each adorner's offset
                    adornersBoundsRelativeToThis = new Rectangle[adorners.Count];
                    for (int i = 0; i < adorners.Count; i++)
                    {
                        adornersBoundsRelativeToThis[i] = adornersBoundsRelativeToGuest[i].Translate(-overallRelativeBoundaries.X, -overallRelativeBoundaries.Y);
                    }

                    adjusting = false;
                }
            }

            private void GuestLocationChanged(object sender, EventArgs e)
            {
                if (!adjusting)
                {

                    this.SuspendLayout();
                    guest.SuspendLayout();

                    guestRequestedLocation = guest.Location;
                    AdjustSizeAndPosition();

                    guest.ResumeLayout();
                    this.ResumeLayout();
                }
            }

            private void GuestSizeChanged(object sender, EventArgs e)
            {
                if (!adjusting)
                {
                    this.SuspendLayout();
                    guest.SuspendLayout();
                    AdjustSizeAndPosition();
                    guest.ResumeLayout();
                    this.ResumeLayout();
                }
            }

            //const int WM_MOUSEMOVE = 0x200;
            //const int WM_LBUTTONUP = 0x202;
            //public bool PreFilterMessage(ref Message m)
            //{
            //    //if (m.Msg >= WM_MOUSEMOVE && m.Msg <= WM_LBUTTONUP)
            //    //{
            //    //    var pos = this.PointToClient(Control.MousePosition);
            //    //    if (!this.IsDisposed && this.ClientRectangle.Contains(pos))
            //    //    {
            //    //        for (var i = 0; i < adorners.Count; i++)
            //    //        {
            //    //            var adornerBounds = adornersBoundsRelativeToThis[i];
            //    //            if (adornerBounds.Contains(pos))
            //    //            {
            //    //                var adorner = adorners[i];
            //    //                var adornerMousePos = pos.Translate(-adornerBounds.X, -adornerBounds.Y);
            //    //                var cursor = adorner.GetHitTests(adornerMousePos);
            //    //                if (cursor != Cursors.Default)
            //    //                {
            //    //                    // need to rethrow mouse events
            //    //                    container.Shell.TraceLine("Mouse captured");
            //    //                    //adorner.Invoke(adorner.InternalMouseMove, new MouseEventArgs(MouseButtons.None, 0, adornerMousePos.X, adornerMousePos.Y, 0)));
            //    //                     adorner.InternalMouseMove(new MouseEventArgs(MouseButtons.None, 0, adornerMousePos.X, adornerMousePos.Y, 0));
            //    //                    return true;
            //    //                }
            //    //            }
            //    //        }

            //    //        // return true; //if you have handled teh message your self
            //    //    }
            //    //}
            //    return false;
            //}
            public void AddAdorner<T>(T adorner) where T : AdornerBase
            {
                adorner.EditorShell = this.container;
                adorner.Wrapper = this;

                adorners.Insert(0, adorner);
                // adorners.Add(adorner);
                adorner.CursorChanged += AdornerCursorChanged;
                AdjustSizeAndPosition();
            }

            private void AdornerCursorChanged(object sender, EventArgs e)
            {
                Cursor = ((AdornerBase)sender).Cursor;
            }
            int FindAdornerAtPosition(Point location)
            {
                for (var i = 0; i < adorners.Count; i++)
                {
                    var adornerBounds = adornersBoundsRelativeToThis[i];
                    if (adornerBounds.Contains(location)) return i;
                }
                return -1;
            }

            void FowardMouseEventTo(Func<AdornerBase, GetGuestMouseEvent> getHandler, MouseEventArgs e)
            {
                var found = EnterLeaveCheck(e.Location);
                if (found != -1)
                {
                    var adornerBounds = adornersBoundsRelativeToThis[found];
                    var adornerMousePos = e.Location.Translate(-adornerBounds.X, -adornerBounds.Y);
                    getHandler(adorners[found])(new MouseEventArgs(e.Button, e.Clicks, adornerMousePos.X, adornerMousePos.Y, e.Delta));
                }
            }

            AdornerBase lastEntered = null;
            int EnterLeaveCheck(Point location)
            {
                var found = FindAdornerAtPosition(location);
                if (found == -1)
                {
                    if (lastEntered != null)
                    {
                        lastEntered.InternalMouseLeave(EventArgs.Empty);
                        lastEntered = null;
                    }
                }
                else
                {
                    var adorner = adorners[found];
                    if (lastEntered != adorner)
                    {
                        if (lastEntered != null)
                        {
                            lastEntered.InternalMouseLeave(EventArgs.Empty);
                            lastEntered = null;
                        }

                        lastEntered = adorner;
                        adorner.InternalMouseEnter(EventArgs.Empty);
                    }
                }
                return found;
            }

            delegate void GetGuestMouseEvent(MouseEventArgs args);

            protected override void OnMouseMove(MouseEventArgs e)
            {
                FowardMouseEventTo(x => x.InternalMouseMove, e);
                base.OnMouseMove(e);
            }

            void GuestMouseMove(object sender, MouseEventArgs e)
            {
                OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, e.X + guest.Location.X, e.Y + guest.Location.Y, e.Delta));
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                FowardMouseEventTo(x => x.InternalMouseUp, e);
                base.OnMouseUp(e);
            }
            void GuestMouseUp(object sender, MouseEventArgs e)
            {
                OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, e.X + guest.Location.X, e.Y + guest.Location.Y, e.Delta));
            }
            protected override void OnMouseDown(MouseEventArgs e)
            {
                FowardMouseEventTo(x => x.InternalMouseDown, e);
                base.OnMouseDown(e);
            }
            void GuestMouseDown(object sender, MouseEventArgs e)
            {
                OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, e.X + guest.Location.X, e.Y + guest.Location.Y, e.Delta));
            }
            protected override void OnMouseWheel(MouseEventArgs e)
            {
                FowardMouseEventTo(x => x.InternalMouseWheel, e);
                base.OnMouseWheel(e);
            }
            private void GuestMouseWheel(object sender, MouseEventArgs e)
            {
                OnMouseWheel(new MouseEventArgs(e.Button, e.Clicks, e.X + guest.Location.X, e.Y + guest.Location.Y, e.Delta));
            }
            protected override void OnMouseDoubleClick(MouseEventArgs e)
            {
                FowardMouseEventTo(x => x.InternalMouseDoubleClick, e);
                base.OnMouseDoubleClick(e);
            }
            private void GuestMouseDoubleClick(object sender, MouseEventArgs e)
            {
                OnMouseDoubleClick(new MouseEventArgs(e.Button, e.Clicks, e.X + guest.Location.X, e.Y + guest.Location.Y, e.Delta));
            }
            protected override void OnMouseClick(MouseEventArgs e)
            {
                FowardMouseEventTo(x => x.InternalMouseClick, e);
                base.OnMouseClick(e);
            }
            private void GuestMouseClick(object sender, MouseEventArgs e)
            {
                OnMouseClick(new MouseEventArgs(e.Button, e.Clicks, e.X + guest.Location.X, e.Y + guest.Location.Y, e.Delta));
            }
            protected override void OnMouseEnter(EventArgs e)
            {
                //   EnterLeaveCheck(this.PointToClient(Control.MousePosition));
                base.OnMouseEnter(e);
            }
            void GuestMouseEnter(object sender, EventArgs e)
            {
                OnMouseEnter(e);
            }
            protected override void OnMouseLeave(EventArgs e)
            {
                //  EnterLeaveCheck(this.PointToClient(Control.MousePosition));
                base.OnMouseLeave(e);
            }
            void GuestMouseLeave(object sender, EventArgs e)
            {
                OnMouseLeave(e);
            }


            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                for (int i = 0; i < adorners.Count; i++)
                {
                    adorners[i].PaintAt(e.Graphics, adornersBoundsRelativeToThis[i].Location);
                }
            }
            void GuestPaint(object sender, PaintEventArgs e)
            {
                //var newPaint = new PaintEventArgs(e.Graphics, e.ClipRectangle.Translate(-guest.Location.X, -guest.Location.Y));
                //OnPaint(newPaint);
                for (int i = 0; i < adorners.Count; i++)
                {
                    adorners[i].PaintAt(e.Graphics, adornersBoundsRelativeToGuest[i].Location);
                }
            }

        }
    }
}
