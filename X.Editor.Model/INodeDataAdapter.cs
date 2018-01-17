using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public interface INodeDataAdapter
    {
        string GetDisplayName();
        IEnumerable<Property> GetProperties();
        IEnumerable<Command> GetCommands();
        string GetDefaultPropertyName();
    }
}
