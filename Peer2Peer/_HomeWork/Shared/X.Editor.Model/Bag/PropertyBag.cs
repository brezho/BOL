using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Configuration.Model.Bag
{

    /// <summary>
    /// Represents the method that will handle the GetValue and SetValue events of the
    /// PropertyBag class.
    /// </summary>
    public delegate void PropertySpecEventHandler(object sender, PropertySpecEventArgs e);

    public class PropertySpecEventArgs : EventArgs
    {
        public PropertySpec Property { get; private set; }
        public object Value { get; set; }
        public PropertySpecEventArgs(PropertySpec property, object val)
        {
            this.Property = property;
            this.Value = val;
        }
    }

    /// <summary>
    /// Represents a collection of custom properties that can be selected into a
    /// PropertyGrid to provide functionality beyond that of the simple reflection
    /// normally used to query an object's properties.
    /// </summary>

    [TypeDescriptionProvider(typeof(PropertyBagTypeDescriptionProvider))]
    public class PropertyBag
    {
        /// <summary>
        /// Gets or sets the name of the default property in the collection.
        /// </summary>
        public string DefaultPropertyName { get; set; }

        /// <summary>
        /// Gets the collection of properties contained within this PropertyBag.
        /// </summary>
        public List<PropertySpec> Properties { get; private set; }

        /// <summary>
        /// Initializes a new instance of the PropertyBag class.
        /// </summary>
        public PropertyBag()
        {
            Properties = new List<PropertySpec>();
        }

        /// <summary>
        /// Occurs when a PropertyGrid requests the value of a property.
        /// </summary>
        public event PropertySpecEventHandler GetValue;

        /// <summary>
        /// Occurs when the user changes the value of a property in a PropertyGrid.
        /// </summary>
        public event PropertySpecEventHandler SetValue;

        /// <summary>
        /// Raises the GetValue event.
        /// </summary>
        /// <param name="e">A PropertySpecEventArgs that contains the event data.</param>
        protected internal virtual void OnGetValue(PropertySpecEventArgs e)
        {
            if (GetValue != null) GetValue(this, e);
        }

        /// <summary>
        /// Raises the SetValue event.
        /// </summary>
        /// <param name="e">A PropertySpecEventArgs that contains the event data.</param>
        protected internal virtual void OnSetValue(PropertySpecEventArgs e)
        {
            if (SetValue != null) SetValue(this, e);
        }
    }
}
