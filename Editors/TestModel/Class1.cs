using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Helpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalConfiguration
{
    public class MasterTerminalInstanceConfiguration : NotifyingObject
    {
        public string Name { get { return Get(() => Name); } set { Set(value); } }
        public NotifyingList<Terminal> Terminals { get; set; }
        public MasterTerminalInstanceConfiguration()
        {
            Name = "Master Terminal Instance";
            Terminals = new NotifyingList<Terminal>();
        }
        public override string ToString()
        {
            return Name;
        }
    }

    public class Terminal : NotifyingObject
    {
        public string Name { get { return Get(() => Name); } set { Set(value); } }
        public NotifyingList<Area> Areas { get; set; }
        public Terminal()
        {
            Areas = new NotifyingList<Area>();
            Name = "New terminal";
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class Area : NotifyingObject
    {
        public string Name { get; set; }
        public Area()
        {
            Name = "New area";
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
