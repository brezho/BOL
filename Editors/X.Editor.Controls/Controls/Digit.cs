using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using X.Editor.Controls.Utils;

namespace X.Editor.Controls.Controls
{
    public partial class X { }

    public class MultiDigits : Control
    {
        int _value = 133457;

        public int Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    UpdateChildren();
                }
            }
        }
        Digit[] _kids;
        public MultiDigits(int digitsCount)
        {
            _kids = new Digit[digitsCount];
            for (int i = 0; i < digitsCount; i++)
            {
                var d = new Digit();
                _kids[i] = d;
                if (i > 0)
                {
                    d.Location = new Point(_kids[i - 1].Location.X + _kids[i - 1].Width, 0);
                }
                this.Controls.Add(d);
            }
            if (digitsCount > 0) Size = new Size(_kids[digitsCount - 1].Bounds.Right, _kids[digitsCount - 1].Bounds.Bottom);

            UpdateChildren();
        }
        void UpdateChildren()
        {
            int divider = 1;
            for (int i = _kids.Length; i > 0; i--)
            {
                _kids[i-1].Value = _value / divider;
                divider *= 10;
            }
        }
    }
    public class Digit : Control
    {
        public int Value
        {
            get { return _value; }
            set
            {
                var old = _value;
                _value = value % 10;
                if(_value != old)Invalidate();
            }
        }
        int _value = 3;

        int _thickNess = 6;
        int _barLength = 30;

        Point A;
        Point B;
        Point C;
        Point D;
        Point E;
        Point F;

        /*
         
         0                   
  A    *****    B     
      *     *             
    5 *     * 1 
      *  6  *  
  F    *****    C
      *     *  
    4 *     * 2
      *     *  
  E    *****    D
         3  
         
 */



        static readonly int[][] _lights = new[] {
            new[] { 1, 1, 1, 1, 1, 1, 0 },
            new[] { 0, 1, 1, 0, 0, 0, 0 },
            new[] { 1, 1, 0, 1, 1, 0, 1 },
            new[] { 1, 1, 1, 1, 0, 0, 1 },
            new[] { 0, 1, 1, 0, 0, 1, 1 },
            new[] { 1, 0, 1, 1, 0, 1, 1 },
            new[] { 1, 0, 1, 1, 1, 1, 1 },
            new[] { 1, 1, 1, 0, 0, 0, 0 },
            new[] { 1, 1, 1, 1, 1, 1, 1 },
            new[] { 1, 1, 1, 1, 0, 1, 1 }
        };

        public Digit()
        {
            ComputeBars();
            Size = new Size(D).Grow(2 * _thickNess, 2 * _thickNess);
        }

        void ComputeBars()
        {
            var inSquareShape = new Rectangle(new Point(_thickNess * 2, _thickNess * 2), new Size(_barLength, _barLength));

            var topSquare = inSquareShape.Grow(KnownPoint.Center, _thickNess, _thickNess);
            var bottomSquare = topSquare.Translate(0, _thickNess + _barLength);

            A = new Point(topSquare.Left, topSquare.Top);
            B = new Point(topSquare.Right, topSquare.Top);
            C = new Point(topSquare.Right, topSquare.Bottom);
            D = new Point(bottomSquare.Right, bottomSquare.Bottom);
            E = new Point(bottomSquare.Left, bottomSquare.Bottom);
            F = new Point(bottomSquare.Left, topSquare.Bottom);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var color = Color.FromArgb(30, 43, 10);


            var off = new Pen(color);
            off.StartCap = LineCap.Triangle;
            off.EndCap = LineCap.Triangle;
            off.Width = _thickNess;

            var on = new Pen(Brushes.Green);
            on.StartCap = LineCap.Triangle;
            on.EndCap = LineCap.Triangle;
            on.Width = _thickNess;

            var pens = new Pen[] { off, on };
            var _litBars = _lights[Value];

            var shift = _thickNess / 2;
            var halfShift = _thickNess / 4;

            var A1 = A.Translate(halfShift, -shift);
            var A2 = A.Translate(-shift, halfShift);

            var B1 = B.Translate(-halfShift, -shift);
            var B2 = B.Translate(shift, halfShift);

            var C1 = C.Translate(shift, -shift);
            var C2 = C.Translate(shift, shift);
            var C3 = C.Translate(-shift, 0);

            var D1 = D.Translate(shift, -halfShift);
            var D2 = D.Translate(-halfShift, shift);

            var E1 = E.Translate(halfShift, shift);
            var E2 = E.Translate(-shift, -halfShift);

            var F1 = F.Translate(-shift, shift);
            var F2 = F.Translate(-shift, -shift);
            var F3 = F.Translate(shift, 0);

            var curPen = pens[_litBars[0]];
            e.Graphics.DrawLine(curPen, A1, B1);

            curPen = pens[_litBars[1]];
            e.Graphics.DrawLine(curPen, B2, C1);

            curPen = pens[_litBars[2]];
            e.Graphics.DrawLine(curPen, C2, D1);

            curPen = pens[_litBars[3]];
            e.Graphics.DrawLine(curPen, D2, E1);

            curPen = pens[_litBars[4]];
            e.Graphics.DrawLine(curPen, E2, F1);

            curPen = pens[_litBars[5]];
            e.Graphics.DrawLine(curPen, F2, A2);

            curPen = pens[_litBars[6]];
            e.Graphics.DrawLine(curPen, F3, C3);

            base.OnPaint(e);
        }
    }
}
