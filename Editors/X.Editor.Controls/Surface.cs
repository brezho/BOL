﻿using System;
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
        OneToManyRelationship<Control, IAdorner> _relations = new OneToManyRelationship<Control, IAdorner>();

        public Surface(IEditorContainer container)
        {
            _editor = container;
            this.Dock = DockStyle.Fill;
        }

        Control[] AllControls { get { return _relations.Sources; } }
        IAdorner[] AllAdorners { get { return _relations.Targets; } }


        void Log(params object[] stuff)
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

        public T AdornWith<T>(Control ctrl) where T : IAdorner
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
            ctrl.GotFocus += (s, a) =>
            {
                Log("Ctrl focused", ctrl);
            };
            ctrl.LostFocus += (s, a) =>
            {
                Log("Ctrl lost focus", ctrl);
            };
            ctrl.Resize += (s, a) =>
            {
                var bnds = ctrl.Bounds;
                Log("Ctrl resized", ctrl, "New size", bnds.Size);
                var adorners = _relations[ctrl];
                foreach (var ad in adorners) ad.OnTargetResized(bnds);
            };
            ctrl.LocationChanged += (s, a) =>
            {
                var bnds = ctrl.Bounds;
                Log("Ctrl moved", ctrl, "New position", bnds.Location);
                var adorners = _relations[ctrl];
                foreach (var ad in adorners) ad.OnTargetMoved(bnds);
            };
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
        //protected override void OnPaint(PaintEventArgs pe)
        //{
        //    Log("Painting");
        //    var adorners = AllAdorners;
        //    foreach (var ad in adorners) ad.Paint(pe.Graphics);
        //}
    }
}
