using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationEditor
{
    public interface IShell
    {
        TService GetService<TService>();
       // Configuration.Model.SchemaDefinition Schema { get; }
    }
}
