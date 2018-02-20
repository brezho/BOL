using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ConfigurationEditor
{
    public partial class Output : DockContent
    {
        RichTextBox tb;
        public Output(string text)
        {
            this.Text = text;
            tb = new RichTextBox();
            tb.Dock = DockStyle.Fill;
            this.Controls.Add(tb);
        }

        public void Write()
        { 
        
        }
    }
}
