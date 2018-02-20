using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X.Repository.Attributes
{
    /// <summary>
    /// Members marked with that attribute are not written to repository
    /// If a value exists in the repository it will be read as long as the property has a defined setter
    /// ie: Good for database calculated fields
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CalculatedAttribute : System.Attribute
    {
    }
}
