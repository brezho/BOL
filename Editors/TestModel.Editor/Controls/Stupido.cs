using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestModel.Editor.Controls
{
    public class Stupido : UserControl
    {
        public Stupido()
        {
            BackColor = Color.White;
            
            Size = new System.Drawing.Size(50, 50);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }
}
