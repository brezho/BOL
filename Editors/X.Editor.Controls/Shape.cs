using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Editor.Controls.Utils;
using System.Helpers;
using System.Collections.Specialized;

namespace X.Editor.Controls
{
    public class ShapeCollection : NotifyingList<Shape>
    {
        Rectangle _bounds;

        public Rectangle Bounds { get { return _bounds; } }
        public ShapeCollection()
        {
            _bounds = Rectangle.Empty;
            this.ItemAdded += ShapeCollection_ItemAdded;
            this.ItemRemoved += ShapeCollection_ItemRemoved;

        }
        void RecomputeBounds()
        {
            if (this.Count == 0)
            {
                _bounds = Rectangle.Empty;
            }
            else
            {
                _bounds = this[0].Bounds;
                for (int i = 1; i < this.Count; i++)
                {
                    _bounds = Rectangle.Union(_bounds, this[i].Bounds);
                }
            }
        }

        private void Item_BoundsChanged(object sender, EventArgs e)
        {
            RecomputeBounds();
        }

        private void ShapeCollection_ItemAdded(object sender, ItemEventArgs<Shape> e)
        {
            e.Item.BoundsChanged += Item_BoundsChanged;
            RecomputeBounds();
        }

        private void ShapeCollection_ItemRemoved(object sender, ItemEventArgs<Shape> e)
        {
            RecomputeBounds();
            e.Item.BoundsChanged -= Item_BoundsChanged;
        }
    }


    public class Shape
    {
        Rectangle _bounds;
        public event EventHandler BoundsChanged;
        public Rectangle Bounds
        {
            get { return _bounds; }
            set { _bounds = value; EventsHelper.Fire(BoundsChanged, this, EventArgs.Empty); }
        }
        public Shape()
        {
            Bounds = Rectangle.Empty;
        }
        public virtual void Draw(Graphics graphics) { }
    }
    public class Figure : Shape
    {
        public Figure()
        {
            Bounds = new Rectangle(Point.Empty, new Size(25, 25));
        }
        public override void Draw(Graphics graphics)
        {
            graphics.FillRectangle(Brushes.AliceBlue, Bounds);
        }
    }
    public class Quadrilatere : Shape
    {
        public Quadrilatere()
        {
            Bounds = new Rectangle(Point.Empty.Translate(50, 50), new Size(25, 25));
        }

        public override void Draw(Graphics graphics)
        {
            graphics.FillRectangle(Brushes.BlanchedAlmond, Bounds);
        }
    }
}
