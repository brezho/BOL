using System;
using System.Linq;
using System.Windows.Forms;
using TerminalConfiguration;
using TestModel.Editor.Controls;
using X.Editor.Model;

namespace TestModel.Editor
{
    public class TerminalConfigurationHierarchyProvider : IHierarchyProvider
    {
        public Hierarchy CreateHierarchy(IEditorShell shell)
        {
            return new TerminalConfigurationHierarchy(shell);
        }
    }
    public class TerminalConfigurationHierarchy : Hierarchy
    {
        IEditorShell shell;
        MasterTerminalInstanceConfiguration configuration = new MasterTerminalInstanceConfiguration();
        public TerminalConfigurationHierarchy(IEditorShell shell)
        {
            this.shell = shell;
            BindRoot();
        }

        void BindRoot()
        {
            var terminalsFolder = this.AddFolder("Terminals");

            terminalsFolder.Commands.Add("New Terminal", () => configuration.Terminals.Add(new Terminal()));

            terminalsFolder.RegisterEditorBuilder(typeof(Control), () => new NullControl() { Dock = DockStyle.Fill });
            //terminalsFolder.RegisterEditorBuilder(typeof(Control), () => new Oscilloscope());

            configuration.Terminals.ItemAdded += (s, a) =>
            {
                var terminalNode = terminalsFolder.Add(a.Item);
                terminalNode.AddProperty(x => x.Name, "Terminal name");
                terminalNode.Commands.Add("Delete", () =>
                {
                    if (shell.AskApproval("Delete Terminal", string.Format("Are you sure you want to delete terminal {0}?", a.Item.Name)))
                    {
                        configuration.Terminals.Remove(a.Item);
                    }
                });
                terminalNode.Select();

                var areasFolder = terminalNode.AddFolder("Areas");
                areasFolder.Commands.Add("New area", () => a.Item.Areas.Add(new Area()));

                a.Item.Areas.ItemAdded += (aias, aiaa) =>
                {
                    var areaNode = areasFolder.Add(aiaa.Item);
                    areaNode.AddProperty(x => x.Name, "Area name");
                    areaNode.Select();
                };
                a.Item.Areas.ItemRemoved += (aias, aiaa) =>
                {
                    var areaNode = areasFolder.Nodes().FirstOrDefault(x => x.Tag == a.Item);
                    this.Remove(areaNode.Id);
                };
            };

            configuration.Terminals.ItemRemoved += (s, a) =>
            {
                var terminalNode = terminalsFolder.Nodes().FirstOrDefault(x => x.Tag == a.Item);
                this.Remove(terminalNode.Id);
            };
        }
    }
}
