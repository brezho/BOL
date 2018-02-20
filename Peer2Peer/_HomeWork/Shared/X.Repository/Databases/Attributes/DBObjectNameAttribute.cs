using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X.Repository.Databases.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class DBObjectNameAttribute : System.Attribute
    {
        public string Name { get; set; }
        public DBObjectNameAttribute(string name)
        {
            Name = name;
        }
    }
}
