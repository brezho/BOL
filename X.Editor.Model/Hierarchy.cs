using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public class InterpretationResult
    {
        public Exception Exception { get; set; }
        public string Result { get; set; }
    }
    public delegate InterpretationResult Interpreter(string commandText);
    public abstract class Hierarchy : HierarchyNode
    {
        public event EventHandler<HierarchyNode> SelectedNodeChanged;
        public event EventHandler<HierarchyNode> ActivationRequested;
        protected Interpreter Interpreter { get; set; }

        HierarchyNode _selectedNode;
        public HierarchyNode SelectedNode
        {
            get { return _selectedNode; }
            private set
            {
                _selectedNode = value;
                if (SelectedNodeChanged != null) SelectedNodeChanged(this, value);
            }
        }

        public override Hierarchy Root { get { return this; } }

        public void Remove(long p)
        {
            var it = GetNode(p);
            if (it != null) it.Parent.Remove(it);
        }

        public void SetSelected(HierarchyNode item)
        {
            SelectedNode = item;
        }

        public void Activate(HierarchyNode item)
        {
            if (ActivationRequested != null) ActivationRequested(this, item);
        }
        public InterpretationResult InterpretCommand(string commandText)
        {
            return OnInterpretCommand(commandText);
        }

        protected virtual InterpretationResult OnInterpretCommand(string commandText)
        {
            if (Interpreter != null)
            {
                try
                {
                    return Interpreter(commandText);
                }
                catch (Exception x)
                {
                    return new InterpretationResult { Exception = x, Result = "Interpreter threw an exception" };
                }
            }
            return new InterpretationResult { Result = "No command interpreter defined in hierarchy" };
        }


        public override bool HandleUserInput(UserInput input)
        {
            if (SelectedNode != null)
            {
                var ancestors = SelectedNode.AncestorsAndSelf().Where(x => x != this).ToArray();
                foreach (var node in ancestors)
                {
                    if (node.HandleUserInput(input)) return true;
                }
            }
            return false;
        }
    }
}
