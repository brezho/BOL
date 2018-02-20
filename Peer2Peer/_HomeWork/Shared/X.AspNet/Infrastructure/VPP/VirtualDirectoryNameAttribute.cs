using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace X.AspNet.Infrastructure.VPP
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class VirtualDirectoryNameAttribute : System.Attribute
    {
        public VirtualDirectoryNameAttribute(string alias)
            : base()
        {
            VirtualDirectoryAlias = alias;
        }
        public string VirtualDirectoryAlias { get; set; }
    }
}