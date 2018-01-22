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
                this.Controls.Add(ctrl);
            }

            return adorner;
        }

        private void Ctrl_LostFocus(object sender, EventArgs e)
        {
            latestTarget = null;
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

        Control latestTarget;
        Bitmap bitmap;
        Rectangle bitmapLocation;
        Rectangle boundsUnion;
        void InvalidateControl(Control target)
        {
            latestTarget = target;
            var affectedAdorners = _relations[target];
            List<Rectangle> _newBounds = new List<Rectangle>();
            foreach (var adoner in affectedAdorners)
            {
                _newBounds.Add(adoner.GetRelativeBoundaries(target.Size));
            }

            boundsUnion = Rectangle.Empty;
            foreach (var newBound in _newBounds) boundsUnion = Rectangle.Union(boundsUnion, newBound);

            bitmapLocation = boundsUnion.Translate(target.Location.X, target.Location.Y);
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
            if (latestTarget != null)
            {
                using (var ctrlGraphics = latestTarget.CreateGraphics())
                {
                    ctrlGraphics.DrawImage(bitmap, boundsUnion.Location);
                }
                e.Graphics.DrawImage(bitmap, bitmapLocation);
            }
        }
    }
}
