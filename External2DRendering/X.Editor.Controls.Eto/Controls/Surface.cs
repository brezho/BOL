
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls.Adornment;
using X.Editor.Controls.Utils;
using X.Editor.Model;

namespace X.Editor.Controls.Eto.Controls
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

        public T Adorn<T>(Control control) where T : IAdorner, new()
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
                        Adorn<Resizer>(t);
                        Adorn<Positioner>(t);
                        Adorn<Resizer>(b);
                        Adorn<Positioner>(b);
                        first = false;
                    }
                }
            };
            this.Controls.Add(b);
        }

        class SurfaceControlWrapper : UserControl, IMessageFilter
        {
            IEditorContainer container;

            Control guest;
            List<IAdorner> adorners = new List<IAdorner>();

            Point guestRequestedLocation = Point.Empty;

            Rectangle[] adornersBoundsRelativeToGuest;
            Rectangle[] adornersBoundsRelativeToThis;
            public SurfaceControlWrapper(IEditorContainer cntainer, Control control)
            {
                Application.AddMessageFilter(this);
                guest = control;
                container = cntainer;

                control.SizeChanged += GuestSizeChanged;
                control.LocationChanged += GuestLocationChanged;
                control.Paint += GuestPaint;

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

            const int WM_MOUSEMOVE = 0x200;
            const int WM_LBUTTONUP = 0x202;
            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg >= WM_MOUSEMOVE && m.Msg <= WM_LBUTTONUP)
                {
                    var pos = this.PointToClient(Control.MousePosition);
                    if (!this.IsDisposed && this.ClientRectangle.Contains(pos))
                    {
                        var idx = -1;
                        for (var i = 0; i < adorners.Count; i++)
                        {
                            if (adornersBoundsRelativeToThis[i].Contains(pos))
                            {
                                container.Shell.TraceLine();
                                container.Shell.TraceLine("Caugth in: " + adornersBoundsRelativeToThis[i].ToString());
                                container.Shell.TraceLine("with: " + adorners[i].GetHitTests(pos).ToString());

                                break;
                            }
                        }

                        // return true; //if you have handled teh message your self
                    }
                }
                return false;
            }
            public void AddAdorner<T>(T adorner) where T : IAdorner, new()
            {
                adorners.Insert(0, adorner);
                AdjustSizeAndPosition();
            }
            void GuestPaint(object sender, PaintEventArgs e)
            {
                for (int i = 0; i < adorners.Count; i++)
                {
                    adorners[i].PaintAt(e.Graphics, adornersBoundsRelativeToGuest[i].Location);
                }
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                for (int i = 0; i < adorners.Count; i++)
                {
                    adorners[i].PaintAt(e.Graphics, adornersBoundsRelativeToThis[i].Location);
                }
            }
        }
    }
}
