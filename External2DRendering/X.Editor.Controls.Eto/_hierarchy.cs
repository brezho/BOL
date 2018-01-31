using System.Windows.Forms;
using X.Editor.Controls.Utils;
using X.Editor.Model;

namespace X.Editor.Controls.Eto
{
    public class TestHierarchyProvider : IHierarchyProvider
    {
        public Hierarchy CreateHierarchy(IEditorShell shell)
        {
            return new TestHierarchy(shell);
        }
    }
    public class TestHierarchy : Hierarchy
    {
        IEditorShell shell;

        public TestHierarchy(IEditorShell shell)
        {
            this.shell = shell;
            BindRoot();
        }

        void BindRoot()
        {
            var terminalsFolder = this.AddFolder("Test");
            terminalsFolder.RegisterEditorBuilder(typeof(Control), () => new Surface() { Dock = DockStyle.Fill });
        }
    }
}
