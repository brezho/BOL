using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace X.Editor.Controls.Controls
{
    partial class X { }
    class Digit2 : Control
    {
        int _value = 3;
        public int Value
        {
            get { return _value; }
            set
            {
                var old = _value;
                _value = value % 10;
                if (_value != old) Invalidate();
            }
        }

        public Digit2()
        {
            Size = new Size(20,40);
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            ComputeBars();
            base.OnSizeChanged(e);
        }

        private void ComputeBars()
        {
            throw new NotImplementedException();
        }
    }
}
