using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    [TypeConverter(typeof(PropertySorter))]
    public partial class HierarchyNode : ICustomTypeDescriptor
    {
        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            // This function searches the property list for the property
            // with the same name as the DefaultProperty specified, and
            // returns a property descriptor for it.  If no property is
            // found that matches DefaultProperty, a null reference is
            // returned instead.

            //Property propertySpec = null;
            //if (this.NodeDataAdapter.DefaultPropertyName != null)
            //{
            //    propertySpec = this.NodeDataAdapter.GetProperties().FirstOrDefault(x => x.Name.Equals(this.NodeDataAdapter.DefaultPropertyName, StringComparison.InvariantCultureIgnoreCase));
            //}

            //if (propertySpec != null)
            //    return new PropertySpecDescriptor(propertySpec, this.NodeDataAdapter, propertySpec.Name, null);
            //else
                return null;
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            // Rather than passing this function on to the default TypeDescriptor,
            // which would return the actual properties of PropertyBag, I construct
            // a list here that contains property descriptors for the elements of the
            // Properties list in the bag.

            ArrayList props = new ArrayList();

            foreach (Property property in this.Properties)
            {
                property.PropertyChanged += (s, a) => { this.NotifyChanged(new HierarchyNodePropertyChangedEventArgs(this, property.Name)); };

                ArrayList attrs = new ArrayList();

                // order properties the way they were added 
                attrs.Add(new PropertyOrderAttribute(Properties.IndexOf(property)));



                // If a category, description, editor, or type converter are specified
                // in the PropertySpec, create attributes to define that relationship.
                if (property.Category != null)
                    attrs.Add(new CategoryAttribute(property.Category));

                if (property.Description != null)
                    attrs.Add(new DescriptionAttribute(property.Description));

                if (property.AssemblyQualifiedNameOfEditorType != null)
                    attrs.Add(new EditorAttribute(property.AssemblyQualifiedNameOfEditorType, typeof(UITypeEditor)));

                if (property.ConverterType != null)
                    attrs.Add(new TypeConverterAttribute(property.ConverterType.FullName));

                // Additionally, append the custom attributes associated with the
                // PropertySpec, if any.
                if (property.Attributes != null) attrs.AddRange(property.Attributes);

                Attribute[] attrArray = (Attribute[])attrs.ToArray(typeof(Attribute));

                // Create a new property descriptor for the property item, and add
                // it to the list.
                PropertySpecDescriptor pd = new PropertySpecDescriptor(property, this, property.Name, attrArray);
                props.Add(pd);
            }

            // Convert the list of PropertyDescriptors to a collection that the
            // ICustomTypeDescriptor can use, and return it.
            PropertyDescriptor[] propArray = (PropertyDescriptor[])props.ToArray(typeof(PropertyDescriptor));

            return new PropertyDescriptorCollection(propArray);
        }

    }


    public class PropertySpecDescriptor : PropertyDescriptor
    {
        private HierarchyNode node;
        private Property property;

        public PropertySpecDescriptor(Property item, HierarchyNode node, string name, Attribute[] attrs)
            :
            base(name, attrs)
        {
            this.node = node;
            this.property = item;
        }

        public string[] PossibleValues
        {
            get { return property.PossibleValuesDelegate != null ? property.PossibleValuesDelegate(node).ToArray() : null; }
        }

        public override Type ComponentType
        {
            get { return property.GetType(); }
        }

        public override bool IsReadOnly
        {
            get { return (Attributes.Matches(ReadOnlyAttribute.Yes)); }
        }

        public override Type PropertyType
        {
            get { return property.DataType; }
        }

        public override bool CanResetValue(object component)
        {
            if (property.DefaultValue == null)
                return false;
            else
                return !GetValue(component).Equals(property.DefaultValue);
        }

        public override object GetValue(object component)
        {
            return property.GetPropertyValue();
        }

        public override void ResetValue(object component)
        {
            SetValue(component, property.DefaultValue);
        }

        public override void SetValue(object component, object value)
        {
            property.SetPropertyValue(value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            object val = GetValue(component);

            if (property.DefaultValue == null && val == null)
                return false;
            else
                return !val.Equals(property.DefaultValue);
        }
    }

    class PropertySorter : ExpandableObjectConverter
    {
        #region Methods
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            //
            // This override returns a list of properties in order
            //
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value, attributes);
            ArrayList orderedProperties = new ArrayList();
            foreach (PropertyDescriptor pd in pdc)
            {
                Attribute attribute = pd.Attributes[typeof(PropertyOrderAttribute)];
                if (attribute != null)
                {
                    //
                    // If the attribute is found, then create an pair object to hold it
                    //
                    PropertyOrderAttribute poa = (PropertyOrderAttribute)attribute;
                    orderedProperties.Add(new PropertyOrderPair(pd.Name, poa.Order));
                }
                else
                {
                    //
                    // If no order attribute is specifed then given it an order of 0
                    //
                    orderedProperties.Add(new PropertyOrderPair(pd.Name, 0));
                }
            }
            //
            // Perform the actual order using the value PropertyOrderPair classes
            // implementation of IComparable to sort
            //
            orderedProperties.Sort();
            //
            // Build a string list of the ordered names
            //
            ArrayList propertyNames = new ArrayList();
            foreach (PropertyOrderPair pop in orderedProperties)
            {
                propertyNames.Add(pop.Name);
            }
            //
            // Pass in the ordered list for the PropertyDescriptorCollection to sort by
            //
            return pdc.Sort((string[])propertyNames.ToArray(typeof(string)));
        }
        #endregion
        class PropertyOrderPair : IComparable
        {
            private int _order;
            private string _name;
            public string Name
            {
                get
                {
                    return _name;
                }
            }

            public PropertyOrderPair(string name, int order)
            {
                _order = order;
                _name = name;
            }

            public int CompareTo(object obj)
            {
                //
                // Sort the pair objects by ordering by order value
                // Equal values get the same rank
                //
                int otherOrder = ((PropertyOrderPair)obj)._order;
                if (otherOrder == _order)
                {
                    //
                    // If order not specified, sort by name
                    //
                    string otherName = ((PropertyOrderPair)obj)._name;
                    return string.Compare(_name, otherName);
                }
                else if (otherOrder > _order)
                {
                    return -1;
                }
                return 1;
            }
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    class PropertyOrderAttribute : Attribute
    {
        //
        // Simple attribute to allow the order of a property to be specified
        //
        private int _order;
        public PropertyOrderAttribute(int order)
        {
            _order = order;
        }

        public int Order
        {
            get
            {
                return _order;
            }
        }
    }

}
