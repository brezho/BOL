//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using X.Editor.Controls.Utils;
//using System.Helpers;
//using System.Collections.Specialized;
//using System.Windows.Forms;
//using System.Drawing.Drawing2D;

//namespace X.Editor.Controls.Shapes
//{
//    public partial class X { }
//    public class Shape : Control
//    {
//        public Shape()
//        {
//            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
//           // SetStyle(ControlStyles.ResizeRedraw, true);
//            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
//            this.DoubleBuffered = true;
//        }
//    }
//    public class LineShape : Shape
//    {
//        Pen pen;

//        public LineShape(Point vector, int thickness, Color color) : base()
//        {
//            pen = new Pen(color, thickness) { Alignment = PenAlignment.Center };
//            Size = (Size)vector;
//        }
//        protected override void OnSizeChanged(EventArgs e)
//        {
//            ComputePath();
//            base.OnSizeChanged(e);
//        }

//        GraphicsPath path;
//        void ComputePath()
//        {
//            path = new GraphicsPath();
//            path.AddLine(Point.Empty, (Point)Size);
//            path.Widen(pen);
//            Region = new Region(path);
//        }

//        protected override void OnPaint(PaintEventArgs e)
//        {
//            //e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
//            e.Graphics.DrawPath(pen, path);
//            base.OnPaint(e);
//        }
//    }
//    public class RectangleShape : Shape
//    {
//        public RectangleShape(Size size, Color color)
//        {
//            Size = size;
//            BackColor = color;
//        }
//    }
//    public class SquareShape : RectangleShape
//    {
//        public SquareShape(int x, Color color) : base(new Size(x, x), color)
//        {
//        }
//    }
//    //public class ShapeCollection : NotifyingList<Shape>
//    //{
//    //    Rectangle _bounds;

//    //    public Rectangle Bounds { get { return _bounds; } }
//    //    public ShapeCollection()
//    //    {
//    //        _bounds = Rectangle.Empty;
//    //        this.ItemAdded += ShapeCollection_ItemAdded;
//    //        this.ItemRemoved += ShapeCollection_ItemRemoved;

//    //    }
//    //    void RecomputeBounds()
//    //    {
//    //        if (this.Count == 0)
//    //        {
//    //            _bounds = Rectangle.Empty;
//    //        }
//    //        else
//    //        {
//    //            _bounds = this[0].Bounds;
//    //            for (int i = 1; i < this.Count; i++)
//    //            {
//    //                _bounds = Rectangle.Union(_bounds, this[i].Bounds);
//    //            }
//    //        }
//    //    }

//    //    private void Item_BoundsChanged(object sender, EventArgs e)
//    //    {
//    //        RecomputeBounds();
//    //    }

//    //    private void ShapeCollection_ItemAdded(object sender, ItemEventArgs<Shape> e)
//    //    {
//    //        e.Item.BoundsChanged += Item_BoundsChanged;
//    //        RecomputeBounds();
//    //    }

//    //    private void ShapeCollection_ItemRemoved(object sender, ItemEventArgs<Shape> e)
//    //    {
//    //        RecomputeBounds();
//    //        e.Item.BoundsChanged -= Item_BoundsChanged;
//    //    }
//    //}

//}
