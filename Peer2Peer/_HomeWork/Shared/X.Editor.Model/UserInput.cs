using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public class UserInput
    {
        public bool Control { get; set; }
        public bool Alt { get; set; }
        public bool Shift { get; set; }
        public string Key { get; set; }

        public override string ToString()
        {
            return
                (Control ? "Ctrl + " : "")
                + (Alt ? "Alt + " : "")
                + (Shift ? "Shift + " : "")
                + Key ?? "<null>";
        }
    }
}
