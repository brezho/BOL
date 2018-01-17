using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
   public class HirarchyChangedEventArgs : EventArgs
    {
      public  Hierarchy Hierarchy { get; private set; }
        public HirarchyChangedEventArgs(Hierarchy item)
        {
            Hierarchy = item;
        }
    }
}
