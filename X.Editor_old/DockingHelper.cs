using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using X.Editor.Documents;

namespace X.Editor
{
    public static class DockingHelper
    {
        public static IEnumerable<DocumentWindowBase> GetOpenDocumentWindows(this DockPanel panel)
        {
            foreach (var p in panel.Panes)
            {
                foreach (var c in p.Contents)
                {
                    if (c is DocumentWindowBase) yield return c as DocumentWindowBase;
                }
            }
        }
    }
}
