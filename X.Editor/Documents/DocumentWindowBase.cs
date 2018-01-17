using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using X.Editor.Model;

namespace X.Editor.Documents
{
    public class DocumentWindowBase : DockContent, IEditorContainer
    {
        public IEditorShell Shell { get; private set; }
        public HierarchyNode Node { get; private set; }
        public DocumentWindowBase(IEditorShell main, HierarchyNode item)
        {
            Shell = main;
            Node = item;
            Text = item.NodeDataAdapter.GetDisplayName();
            item.Changed += (s, a) => { Text = item.NodeDataAdapter.GetDisplayName(); };
            this.Activated += (s, a) => { Shell.Hierarchy.SetSelected(Node); };
        }
    }
}
