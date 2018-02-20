using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X.Repository.Attributes
{
    /// <summary>
    /// Members are completely ignored by persistence model
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NonPersistentAttribute : System.Attribute
    {
    }
}
