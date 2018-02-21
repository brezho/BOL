using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using X.Editor.Model;

namespace X.Editor.Tools
{
    partial class X { }
    public abstract class ToolWindowBase : DockContent
    {
        public IEditorShell Shell { get; private set; }
        SynchronizationContext SyncContext { get; set; }

        public ToolWindowBase(IEditorShell main)
        {
            SyncContext = SynchronizationContext.Current;
            Shell = main;
            CloseButtonVisible = false;
        }

        protected void UpdateUI(Action action)
        {
            SyncContext.Post(st => action(), null);
        }
    }
}
