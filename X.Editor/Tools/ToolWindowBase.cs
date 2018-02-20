using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using X.Editor.Model;

namespace X.Editor.Tools
{
    partial class X { }
    public abstract class ToolWindowBase : DockContent
    {
        public IEditorShell Shell { get; private set; }
        public ToolWindowBase(IEditorShell main)
        {
            Shell = main;
            CloseButtonVisible = false;
        }
    }
}
