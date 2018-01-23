using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Controls.Adornment;
using X.Editor.Controls.Utils;
using X.Editor.Model;


namespace X.Editor.Controls
{
    class SurfaceX { }
    public partial class Surface : Control
    {
        ConcurrentDictionary<Type, HashSet<Control>> _oneTypeOfAdornerPerControl = new ConcurrentDictionary<Type, HashSet<Control>>();
        IEditorContainer _editor;
        OneToManyRelationship<Control, IAdorner> _relations = new OneToManyRelationship<Control, IAdorner>();

        public Surface(IEditorContainer container)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            UpdateStyles();
            DoubleBuffered = true;

            _editor = container;
            this.Dock = DockStyle.Fill;
        }

        bool mouseCaptured = false;
        Point mouseCaptureStartLocation;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var bnds = AllControls[0].Bounds;
                var newBnds = bnds.Translate(75, 50).Grow(20, 10);
                AllControls[0].SetBounds(newBnds.X, newBnds.Y, newBnds.Width, newBnds.Height);
            }
            else
            {
                var hitTest = GetHitTest(e.Location);
                if (hitTest != null)
                {
                    mouseCaptured = true;
                    mouseCaptureStartLocation = e.Location;
                }
            }
            //else base.OnMouseDown(e);
        }

        Tuple<IAdorner, Cursor> GetHitTest(Point location)
        {
            if (focusedControl != null)
            {
                var affectedAdorners = _relations[focusedControl];

                foreach (var adoner in affectedAdorners)
                {
                    var bnds = adoner.GetRelativeBoundaries(focusedControl.Size);
                    var adornerBounds = bnds.Translate(focusedControl.Location.X, focusedControl.Location.Y);
                    if (adornerBounds.Contains(location))
                    {
                        var testResult = adoner.GetHitTests(location.Translate(-adornerBounds.X, -adornerBounds.Y));
                        if (testResult != Cursors.Default)
                        {
                            return Tuple.Create(adoner, testResult);
                        }
                    }
                }
            }
            return null;
        }
       

        protected override void OnMouseLeave(EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            mouseCaptured = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mouseCaptured)
            {
                // Get the difference between the two points
                int xDiff = e.Location.X - mouseCaptureStartLocation.X;
                int yDiff = e.Location.Y - mouseCaptureStartLocation.Y;
                focusedControl.Location = focusedControl.Location.Translate(xDiff, yDiff);
                mouseCaptureStartLocation = e.Location;
            }
            if (focusedControl != null)
            {
                var test = GetHitTest(e.Location);
                if (test != null) Cursor = test.Item2;
            }
        }

        Control[] AllControls { get { return _relations.Sources; } }
        IAdorner[] AllAdorners { get { return _relations.Targets; } }

        public void Log(params object[] stuff)
        {
            if (stuff != null)
            {
                var i = 0;
                foreach (var it in stuff)
                {
                    var val = it.ToString();
                    if (i % 2 == 0)
                    {
                        if (val.Length < 20) _editor.Shell.Trace("[" + val + new string(' ', 20 - val.Length) + "] ");
                        else _editor.Shell.Trace("[" + val.Substring(0, 20) + "] ");
                    }
                    else _editor.Shell.Trace(it.ToString());
                    i++;
                }
            }
            _editor.Shell.TraceLine();
        }
        public T AdornWith<T>(Control ctrl) where T : IAdorner, new()
        {
            var set = _oneTypeOfAdornerPerControl.GetOrAdd(typeof(T), new HashSet<Control>());
            if (!set.Add(ctrl)) throw new Exception("Control is already adorned with " + typeof(T).FullName);

            var adorner = new T();

            if (_relations.Add(ctrl, adorner) == AddRelationResult.NewSource)
            {
                ctrl.GotFocus += Ctrl_GotFocus;
                ctrl.LostFocus += Ctrl_LostFocus;
                ctrl.LocationChanged += Ctrl_LocationChanged;
                ctrl.SizeChanged += Ctrl_SizeChanged;
                ctrl.Paint += Ctrl_Paint;
                this.Controls.Add(ctrl);
            }

            return adorner;
        }
        private void Ctrl_Paint(object sender, PaintEventArgs e)
        {
            InvalidateControl((Control)sender);
            Invalidate();
        }
        private void Ctrl_LostFocus(object sender, EventArgs e)
        {
            focusedControl = null;
            Invalidate();
        }
        private void Ctrl_GotFocus(object sender, EventArgs e)
        {
            InvalidateControl((Control)sender);
            Invalidate();
        }
        private void Ctrl_SizeChanged(object sender, EventArgs e)
        {
            InvalidateControl((Control)sender);
        }
        private void Ctrl_LocationChanged(object sender, EventArgs e)
        {
            InvalidateControl((Control)sender);
        }

        Control focusedControl;
        Bitmap bitmap;
        Rectangle bitmapLocation;
        Rectangle boundsUnion;
        void InvalidateControl(Control target)
        {
            focusedControl = target;
            var affectedAdorners = _relations[focusedControl];
            List<Rectangle> _newBounds = new List<Rectangle>();
            foreach (var adoner in affectedAdorners)
            {
                _newBounds.Add(adoner.GetRelativeBoundaries(focusedControl.Size));
            }

            boundsUnion = Rectangle.Empty;
            foreach (var newBound in _newBounds) boundsUnion = Rectangle.Union(boundsUnion, newBound);

            bitmapLocation = boundsUnion.Translate(focusedControl.Location.X, focusedControl.Location.Y);
            bitmap = new Bitmap(boundsUnion.Width, boundsUnion.Height, PixelFormat.Format32bppArgb);


            using (var gr = Graphics.FromImage(bitmap))
            {
                for (int i = 0; i < affectedAdorners.Length; i++)
                {
                    var offset = new Point(_newBounds[i].X - boundsUnion.X, _newBounds[i].Y - boundsUnion.Y);
                    affectedAdorners[i].PaintAt(gr, offset);
                }
            }

            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (focusedControl != null)
            {
                using (var ctrlGraphics = focusedControl.CreateGraphics())
                {
                    ctrlGraphics.DrawImage(bitmap, boundsUnion.Location);
                }
                e.Graphics.DrawImage(bitmap, bitmapLocation);
            }
        }
    }
}
