using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchemaBuilder.UIModel
{
    public interface IView : IInitialize
    {
        XModelBase Model { get; set; }
        Control InnerControl { get; }
    }
}
