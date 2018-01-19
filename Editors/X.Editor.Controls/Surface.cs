using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        ConcurrentDictionary<Type, HashSet<Control>> _ = new ConcurrentDictionary<Type, HashSet<Control>>();
        IEditorContainer _editor;
        OneToManyRelationship<Control, AbstractAdorner> _relations = new OneToManyRelationship<Control, AbstractAdorner>();

        public Surface(IEditorContainer container)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            UpdateStyles();
            DoubleBuffered = true;

            _editor = container;
            this.Dock = DockStyle.Fill;
        }

        Control[] AllControls { get { return _relations.Sources; } }
        AbstractAdorner[] AllAdorners { get { return _relations.Targets; } }


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

        public T AdornWith<T>(Control ctrl) where T : AbstractAdorner
        {
            var set = _.GetOrAdd(typeof(T), new HashSet<Control>());
            if (!set.Add(ctrl)) throw new Exception("Control is already adorned with " + typeof(T).FullName);
            var adorner = (T)typeof(T).Hype().GetOne(this, ctrl);

            if (_relations.Add(ctrl, adorner) == AddRelationResult.NewSource)
            {
                SubscribeToControlEvents(ctrl);
            }

            this.Controls.Add(adorner);

            return adorner;
        }

        void SubscribeToControlEvents(Control ctrl)
        {
            ctrl.Paint += Ctrl_Paint;
        }

        private void Ctrl_Paint(object sender, PaintEventArgs e)
        {
            Control ctrl = sender as Control;
            var adorners = _relations[ctrl];
            Log();
            Log("PAAAAIIIIINNNTTT");
            foreach (var adorner in adorners)
            {
                Log(adorner);
            }
            //var gr = ctrl.CreateGraphics();

          //  e.Graphics.DrawString("tutu", new Font("Arial", 24, FontStyle.Bold), Brushes.Red, new Point(20, 20));
        }

        Point? mouseDownLocation = null;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            mouseDownLocation = e.Location;
            Log("Mouse down", mouseDownLocation.Value);

            if (e.Button == MouseButtons.Right)
            {
                var bnds = AllControls[0].Bounds;
                var inc = bnds.Translate(100, 100);
                AllControls[0].SetBounds(inc.Location.X, inc.Location.Y, inc.Width, inc.Height, BoundsSpecified.All);

                AllControls[0].Width += 20;
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            var delta = new Point(e.Location.X - mouseDownLocation.Value.X, e.Location.Y - mouseDownLocation.Value.Y);
            Log("Mouse up", e.Location, "Delta", delta);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            var rect = new Rectangle(0, 0, 50, 50);
            SolidBrush solidBrush = new SolidBrush(Color.Yellow);
            Region region1 = new Region(rect);
            Graphics g = AllControls[0].CreateGraphics();
            g.FillRegion(solidBrush, region1);
        }
    }
}
