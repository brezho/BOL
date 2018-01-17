using System;
using System.Collections.Generic;
using System.Helpers;
using System.Text;

namespace Automate
{
    public class Flow : NotifyingObject
    {
        public NotifyingList<Location> Locations { get; private set; }

        public Flow()
        {
            Locations = new NotifyingList<Location>();
        }
    }
}
