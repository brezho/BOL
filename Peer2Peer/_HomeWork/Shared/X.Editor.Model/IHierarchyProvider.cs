using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public interface IHierarchyProvider
    {
        Hierarchy CreateHierarchy(IEditorShell shell);
    }
}
