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
using X.Editor.Model;

namespace TestModel.Editor.Design
{
    class SurfaceSuppress { }
    public partial class Surface : Control
    {
        private Point mouseStartLocation;
        IEditorContainer editorContainer;

        HashSet<Control> movableControls = new HashSet<Control>();
        List<IAdorner> adorners = new List<IAdorner>();

        public Surface(IEditorContainer container)
        {
            editorContainer = container;
            this.Dock = DockStyle.Fill;
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            mouseStartLocation = e.Location;
            editorContainer.Shell.TraceLine();
            editorContainer.Shell.TraceLine("Mouse down at " + mouseStartLocation);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            int deltaX = e.Location.X - mouseStartLocation.X;
            int deltaY = e.Location.Y - mouseStartLocation.Y;
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            editorContainer.Shell.TraceLine("Mouse up at " + e.Location);
            int deltaX = e.Location.X - mouseStartLocation.X;
            int deltaY = e.Location.Y - mouseStartLocation.Y;
            editorContainer.Shell.TraceLine("Delta " + new Point(deltaX, deltaY));
        }


        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public void MakeMovable(Control control)
        {
            Adorn<MoverControl>(control);
        }

        ConcurrentDictionary<Type, HashSet<Control>> _registered = new ConcurrentDictionary<Type, HashSet<Control>>();

        void Adorn<T>(Control control) where T : IAdorner, new()
        {
            var hashset = _registered.GetOrAdd(typeof(T), new HashSet<Control>());
            if (!hashset.Add(control)) return; // already adorned 
            var adorner = new T();
            adorners.Add(adorner);
        }

    }
}
