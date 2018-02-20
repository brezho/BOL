using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Helpers;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{

    public partial class HierarchyNode : IComponent
    {

        public event EventHandler Disposed = (s, a) => { };
        public void Dispose() { }

        public ISite Site
        {
            get
            {
                return new HierarchyNodeVerbsSite(this);
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }


    class HierarchyNodeVerbsSite : ISite, IMenuCommandService
    {
        HierarchyNode _targetNode;
        public HierarchyNodeVerbsSite(HierarchyNode target)
        {
            _targetNode = target;
        }

        public IComponent Component
        {
            get { return _targetNode; }
        }

        public IContainer Container
        {
            get { return null; }
        }

        public bool DesignMode
        {
            get { return true; }
        }

        public string Name
        {
            get
            {
                return "HierarchyNodeVerbsSite";
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IMenuCommandService))
                return this;
            return null;
        }

        public void AddCommand(MenuCommand command)
        {
            throw new NotImplementedException();
        }

        public void AddVerb(DesignerVerb verb)
        {
            throw new NotImplementedException();
        }

        public MenuCommand FindCommand(CommandID commandID)
        {
            throw new NotImplementedException();
        }

        public bool GlobalInvoke(CommandID commandID)
        {
            throw new NotImplementedException();
        }

        public void RemoveCommand(MenuCommand command)
        {
            throw new NotImplementedException();
        }

        public void RemoveVerb(DesignerVerb verb)
        {
            throw new NotImplementedException();
        }

        public void ShowContextMenu(CommandID menuID, int x, int y)
        {
            throw new NotImplementedException();
        }

        public DesignerVerbCollection Verbs
        {
            get
            {
                DesignerVerbCollection Verbs = new DesignerVerbCollection();
                foreach (var command in _targetNode.Commands)
                {
                    Verbs.Add(new DesignerVerb(command.Text, (snd, args) => command.Invoke()));
                }
                return Verbs;
            }
        }
    }

}