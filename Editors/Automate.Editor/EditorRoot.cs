using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Editor.Model;

namespace Automate.Editor
{
    public class EditorRoot : Hierarchy
    {
        Flow _editedModel;
        public EditorRoot(IEditorShell shell)
        {
            _editedModel = new Flow();

            var locationsFolder = this.AddFolder("Locations");

            locationsFolder.Commands.Add("Add directory", () =>
            {
                var location = new Location() { Type = LocationType.Directory };
                _editedModel.Locations.Add(location);
            });

            locationsFolder.Commands.Add("Add file", () =>
            {
                var location = new Location() { Type = LocationType.File };
                _editedModel.Locations.Add(location);
            });

            _editedModel.Locations.ItemAdded += (s, a) =>
            {
                var newNode = locationsFolder.Add(a.Item);
                newNode.Commands.Add("Remove", () => { _editedModel.Locations.Remove(a.Item); });

                var nameProp = newNode.AddProperty(x => x.Name);
                var typeProp = newNode.AddReadOnlyProperty(x => x.Type);

                string editorName = null;
                switch (a.Item.Type)
                {
                    case LocationType.Directory:
                        editorName = typeof(System.Windows.Forms.Design.FolderNameEditor).AssemblyQualifiedName;
                        break;
                    case LocationType.File:
                        editorName = typeof(System.Windows.Forms.Design.FileNameEditor).AssemblyQualifiedName;
                        break;
                }

                var pathProperty = newNode.AddProperty(x => x.Path);
                pathProperty.AssemblyQualifiedNameOfPropertyGridEditorType = editorName;


                newNode.Select();
            };

            _editedModel.Locations.ItemRemoved += (s, a) =>
            {
                var node = locationsFolder.Nodes().Where(x => x.Tag == a.Item).First();
                locationsFolder.Remove(node);
            };
        }
    }
}
