using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Editor.Model;

namespace Automate.Editor
{
    public class HierarchyProvider : IHierarchyProvider
    {
        public Hierarchy CreateHierarchy(IEditorShell shell)
        {
            return new EditorRoot(shell);
        }
    }
}
