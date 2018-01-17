using System;
using System.Collections.Generic;
using System.Helpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public partial class HierarchyNode
    {
        Dictionary<Type, Func<IEditor>> editorBuilders = new Dictionary<Type, Func<IEditor>>();
        public void RegisterEditorBuilder(Type containerType, Func<IEditor> editorBuilder)
        {
            editorBuilders[containerType] = editorBuilder;
        }

        public IEditor GetEditor(Type type)
        {
            Func<IEditor> builder;
            if (editorBuilders.ContainsKey(type))
            {
                builder = editorBuilders[type];
            }
            else
            {
                builder = editorBuilders.Where(x => x.Key.Match(type)).Select(x => x.Value).FirstOrDefault();
            }

            if (builder != null)
            {
                return builder();
            }

            return null;
        }
    }
}
