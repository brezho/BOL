using System;
using System.Collections.Generic;
using System.Helpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public class Command
    {
        public event EventHandler<EventArgs> Commanded = (s, a) => { };
        CommandsList Parent;
        static readonly Action<HierarchyNode, Command> DoNothing = (n, c) => { };
        internal Command(CommandsList parent, HierarchyNode node, string text, Action<HierarchyNode, Command> onInvoke = null, string description = null)
        {
            Parent = parent;
            Text = text;
            Description = description;

            if (onInvoke != null)
            {
                Commanded += (cs, ca) => { onInvoke(node, this); };
            }
        }
        public HierarchyNode TargetNode { get { return Parent.Node; } }
        public string Text { get; set; }
        public string Description { get; set; }
        public void Invoke()
        {
            Commanded(this, EventArgs.Empty);
        }
    }
    public class CommandsList : NotifyingList<Command>
    {
        internal HierarchyNode Node { get; private set; }

        internal CommandsList(HierarchyNode owner)
        {
            Node = owner;
        }
        public CommandsList()
        {
        }

        public void Remove(string commandText)
        {
            var cmd = this.Where(x => x.Text == commandText).FirstOrDefault();
            if (cmd != null) base.Remove(cmd);
        }
        public Command Add(string text, Action onInvoke = null, string description = null)
        {
            return this.Add(text, (n, c) => onInvoke(), description);
        }
        public Command Add(string text, Action<HierarchyNode> onInvoke = null, string description = null)
        {
            return this.Add(text, (n, c) => onInvoke(n), description);
        }
        public Command Add(string text, Action<HierarchyNode, Command> onInvoke = null, string description = null)
        {
            return base.Add(new Command(this, Node, text, onInvoke, description));
        }
    }
}
