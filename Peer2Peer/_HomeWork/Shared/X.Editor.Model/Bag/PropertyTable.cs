using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Configuration.Model.Bag
{

    /// <summary>
    /// An extension of PropertyBag that manages a table of property values, in
    /// addition to firing events when property values are requested or set.
    /// </summary>
    public class PropertyTable : PropertyBag
    {
        private Hashtable propValues;

        /// <summary>
        /// Initializes a new instance of the PropertyTable class.
        /// </summary>
        public PropertyTable()
        {
            propValues = new Hashtable();
        }

        /// <summary>
        /// The property values being held.
        /// </summary>
        public Hashtable PropertyValues
        {
            get { return propValues; }
        }

        /// <summary>
        /// Gets or sets the value of the property with the specified name.
        /// <p>In C#, this property is the indexer of the PropertyTable class.</p>
        /// </summary>
        public object this[string key]
        {
            get { return propValues[key]; }
            set { propValues[key] = value; }
        }

        /// <summary>
        /// This member overrides PropertyBag.OnGetValue.
        /// </summary>
        protected internal override void OnGetValue(PropertySpecEventArgs e)
        {
            e.Value = propValues[e.Property.Name];
            base.OnGetValue(e);
        }

        /// <summary>
        /// This member overrides PropertyBag.OnSetValue.
        /// </summary>
        protected internal override void OnSetValue(PropertySpecEventArgs e)
        {
            propValues[e.Property.Name] = e.Value;
            base.OnSetValue(e);
        }
    }

}
