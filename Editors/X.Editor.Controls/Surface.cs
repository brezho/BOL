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

            if(_relations.Add(ctrl, adorner) == AddRelationResult.NewSource)
            {
                ctrl.LocationChanged += Ctrl_LocationChanged;
                ctrl.SizeChanged += Ctrl_SizeChanged;
                this.Controls.Add(ctrl);
            }

            return adorner;
        }

        private void Ctrl_SizeChanged(object sender, EventArgs e)
        {

        }

        private void Ctrl_LocationChanged(object sender, EventArgs e)
        {

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }
}
