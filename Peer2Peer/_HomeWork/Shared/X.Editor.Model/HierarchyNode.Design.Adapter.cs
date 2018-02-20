using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public partial class HierarchyNode
    {
        protected INodeDataAdapter _customAdapter;
        public INodeDataAdapter NodeDataAdapter
        {
            get
            {
                return _customAdapter ?? new InternalNodeAdapter(this);
            }
            set { _customAdapter = value; }
        }
    }

    public abstract class NodeAdapterBase<T> : INodeDataAdapter where T : HierarchyNode
    {
        protected readonly T Node;
        public NodeAdapterBase(T adaptee)
        {
            Node = adaptee;
        }
        public virtual IEnumerable<Property> GetProperties()
        {
            return Node.Properties;
        }

        public virtual IEnumerable<Command> GetCommands()
        {
            return Node.Commands;
        }

        public virtual string GetDisplayName()
        {
            //return Node.ToString();
            return Node.Tag != null ? Node.Tag.ToString() : Node.ToString();
        }

        public virtual string GetDefaultPropertyName()
        {
            return null;
        }
    }

    class InternalNodeAdapter : NodeAdapterBase<HierarchyNode>
    {
        public InternalNodeAdapter(HierarchyNode adaptee)
            : base(adaptee)
        {
        }
    }
}
