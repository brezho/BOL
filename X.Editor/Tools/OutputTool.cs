using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace X.Editor.Tools.Output
{
    partial class X { }
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
            OutBox.ReadOnly = true;

            Controls.Add(OutBox);
            main.Commands.Add("OUT-Clear", () => { OutBox.Text = ""; }, "Clear the output window");
        }

        public void Write(string text)
        {
            UpdateUI(() => OutBox.AppendText(text));
        }
    }
}
