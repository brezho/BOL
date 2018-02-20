using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace X.Editor.Tools.Output
{
    public class OutputTool : ToolWindowBase
    {
        TextBox OutBox;
        public OutputTool(Main main)
            : base(main)
        {
            Text = "Output";
            OutBox = new TextBox();
            OutBox.Multiline = true;
            OutBox.Dock = DockStyle.Fill;
            OutBox.ScrollBars = ScrollBars.Vertical;
            
            Controls.Add(OutBox);
            main.Commands.Add("OUT-Clear", () => { OutBox.Text = ""; }, "Clear the output window");
        }

        public void Write(string text)
        {
            OutBox.AppendText(text);
        }
    }
}
