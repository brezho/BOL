
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
    public class Surface : Control
    {
        ConcurrentDictionary<Type, HashSet<Control>> _oneTypeOfAdornerPerControl = new ConcurrentDictionary<Type, HashSet<Control>>();
        OneToManyRelationship<Control, AdornerBase> _adornersByControl = new OneToManyRelationship<Control, AdornerBase>();

        public Surface() : base()
        {
            BackColor = Color.Black;
            Dock = DockStyle.Fill;
            this.ControlAdded += GuestAdded;
        }

        Control[] AllGuests { get { return _adornersByControl.Sources; } }
        AdornerBase[] AllAdorners { get { return _adornersByControl.Targets; } }

        private void GuestAdded(object sender, ControlEventArgs e)
        {
            if (!(e.Control is AdornerBase))
            {
                var guest = e.Control;

                guest.SizeChanged += GuestSizeChanged;
                guest.LocationChanged += GuestLocationChanged;
                guest.Paint += GuestPaint;
                guest.MouseMove += GuestMouseMove;
                guest.MouseUp += GuestMouseUp;
                guest.MouseDown += GuestMouseDown;
                guest.MouseEnter += GuestMouseEnter;
                guest.MouseLeave += GuestMouseLeave;
                guest.MouseClick += GuestMouseClick;
                guest.MouseDoubleClick += GuestMouseDoubleClick;
                guest.MouseWheel += GuestMouseWheel;
            }
        }
        public T Adorn<T>(Control control) where T : AdornerBase
        {
            var set = _oneTypeOfAdornerPerControl.GetOrAdd(typeof(T), new HashSet<Control>());
            if (!set.Add(control)) throw new Exception("Control is already adorned with " + typeof(T).FullName);

            var adorner = typeof(T).Hype().GetOne<T>(this, control);
            _adornersByControl.Add(control, adorner);

            this.Controls.Add(adorner);

            return adorner;
        }


        void GuestLocationChanged(object sender, EventArgs e)
        {
            //var guest = (Control)sender;
            //guest.Refresh();
            //  Refresh();
        }

        void GuestSizeChanged(object sender, EventArgs e)
        {
            //var guest = (Control)sender;
            //guest.Refresh();
            //  Refresh();
        }
        void GuestMouseMove(object sender, MouseEventArgs e)
        {
            var guest = (Control)sender;
            OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, e.X + guest.Location.X, e.Y + guest.Location.Y, e.Delta));
        }
        void GuestMouseUp(object sender, MouseEventArgs e)
        {
            var guest = (Control)sender;
            OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, e.X + guest.Location.X, e.Y + guest.Location.Y, e.Delta));
        }
        void GuestMouseDown(object sender, MouseEventArgs e)
        {
            var guest = (Control)sender;
            OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, e.X + guest.Location.X, e.Y + guest.Location.Y, e.Delta));
        }
        private void GuestMouseWheel(object sender, MouseEventArgs e)
        {
            var guest = (Control)sender;
            OnMouseWheel(new MouseEventArgs(e.Button, e.Clicks, e.X + guest.Location.X, e.Y + guest.Location.Y, e.Delta));
        }
        private void GuestMouseDoubleClick(object sender, MouseEventArgs e)
        {
            var guest = (Control)sender;
            OnMouseDoubleClick(new MouseEventArgs(e.Button, e.Clicks, e.X + guest.Location.X, e.Y + guest.Location.Y, e.Delta));
        }
        private void GuestMouseClick(object sender, MouseEventArgs e)
        {
            var guest = (Control)sender;
            OnMouseClick(new MouseEventArgs(e.Button, e.Clicks, e.X + guest.Location.X, e.Y + guest.Location.Y, e.Delta));
        }
        void GuestMouseLeave(object sender, EventArgs e)
        {
            var guest = (Control)sender;
            OnMouseLeave(e);
        }
        void GuestMouseEnter(object sender, EventArgs e)
        {
            var guest = (Control)sender;
            OnMouseEnter(e);
        }
        void GuestPaint(object sender, PaintEventArgs e)
        {

        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            FowardMouseEventTo(x => x.InternalMouseMove, e);
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            FowardMouseEventTo(x => x.InternalMouseUp, e);
            base.OnMouseUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            FowardMouseEventTo(x => x.InternalMouseDown, e);
            base.OnMouseDown(e);
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            FowardMouseEventTo(x => x.InternalMouseWheel, e);
            base.OnMouseWheel(e);
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            FowardMouseEventTo(x => x.InternalMouseDoubleClick, e);
            base.OnMouseDoubleClick(e);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            FowardMouseEventTo(x => x.InternalMouseClick, e);
            base.OnMouseClick(e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            EnterLeaveCheck(this.PointToClient(Control.MousePosition));
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            EnterLeaveCheck(this.PointToClient(Control.MousePosition));
            base.OnMouseLeave(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        int[] FindAdornersAtPosition(Point locationInThis)
        {
            List<int> res = new List<int>();
            var adorners = AllAdorners;
            for (var i = 0; i < adorners.Length; i++)
            {
                if (adorners[i].Bounds.Contains(locationInThis)) res.Add(i);
            }
            return res.ToArray();
        }

        delegate void GetGuestMouseEvent(MouseEventArgs args);
        void FowardMouseEventTo(Func<AdornerBase, GetGuestMouseEvent> getHandler, MouseEventArgs e)
        {
            var found = EnterLeaveCheck(e.Location);
            if (found.Length > 0)
            {
                var adorners = AllAdorners;
                foreach (var i in found)
                {
                    var adornerBounds = AllAdorners[i].Bounds;
                    var adornerMousePos = e.Location.Translate(-adornerBounds.X, -adornerBounds.Y);
                    getHandler(adorners[i])(new MouseEventArgs(e.Button, e.Clicks, adornerMousePos.X, adornerMousePos.Y, e.Delta));
                }
            }
        }

        int[] alreadyEntered = null;
        int[] EnterLeaveCheck(Point locationInThis)
        {
            int[] newEntrants;
            var adorners = AllAdorners;

            var foundAtLocation = FindAdornersAtPosition(locationInThis);
            if (alreadyEntered != null)
            {
                var leavers = alreadyEntered.Except(foundAtLocation).ToArray();
                foreach (var ad in leavers) adorners[ad].InternalMouseLeave(EventArgs.Empty);
                newEntrants = foundAtLocation.Except(alreadyEntered).ToArray();
            }
            else newEntrants = foundAtLocation;

            foreach (var ad in newEntrants) adorners[ad].InternalMouseEnter(EventArgs.Empty);

            alreadyEntered = foundAtLocation;

            return foundAtLocation;
        }

    }
}
