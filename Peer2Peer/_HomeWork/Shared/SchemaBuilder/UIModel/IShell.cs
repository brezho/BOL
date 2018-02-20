using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SchemaBuilder.UIModel
{
    public interface IShell
    {
        DialogResult Ask(string caption, string message, MessageBoxButtons buttons);
    }
}
