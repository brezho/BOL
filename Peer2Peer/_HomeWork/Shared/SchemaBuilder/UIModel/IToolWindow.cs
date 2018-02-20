using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SchemaBuilder.UIModel
{
    public interface IToolWindow : IInitialize
    {
        string ToolName { get; }
        Control Control { get; }
        DockState DockState { get; }
    }
}
