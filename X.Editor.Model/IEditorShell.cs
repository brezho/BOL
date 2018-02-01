using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public interface IEditorShell
    {
        event EventHandler<HirarchyChangedEventArgs> HierarchyChanged;
        Hierarchy Hierarchy { get; }
        CommandsList Commands { get; }

        void Trace(string message);
        void TraceLine(string message = null);
        void Trace(string message, params object[] args);
        bool AskApproval(string title, string question);
        void ShowModal(string title, string text);
    }

    public interface IEditorContainer
    {
        IEditorShell Shell { get; }
        HierarchyNode Node { get; }
    }
    public interface IEditor
    {
        void ActivateIn(IEditorContainer editor);
    }
}
