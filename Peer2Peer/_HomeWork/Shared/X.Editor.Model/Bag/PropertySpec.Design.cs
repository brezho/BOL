using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Configuration.Model.Bag
{

    public class PropertyEnumStringConverter : StringConverter // bugbug = make private
    {
        override public bool GetStandardValuesExclusive(ITypeDescriptorContext x) { return true; }
        override public bool GetStandardValuesSupported(ITypeDescriptorContext x) { return true; }
        override public StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            CustomDescriptor.PropertySpecDescriptor psd = (CustomDescriptor.PropertySpecDescriptor)context.PropertyDescriptor;
            return new StandardValuesCollection(psd.EnumValues);
        }
    }


    public class PropertyBagTypeDescriptionProvider : TypeDescriptionProvider
    {
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return base.GetTypeDescriptor(objectType, instance);
        }
    }

    public class CustomDescriptor : ICustomTypeDescriptor
    {
        PropertyBag _owner;
        public CustomDescriptor(PropertyBag owner)
        {
            _owner = owner;
        }

        #region ICustomTypeDescriptor explicit interface definitions
        // Most of the functions required by the ICustomTypeDescriptor are
        // merely pssed on to the default TypeDescriptor for this type,
        // which will do something appropriate.  The exceptions are noted
        // below.
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

            PropertySpec propertySpec = null;
            if (_owner.DefaultPropertyName != null)
            {
                propertySpec = _owner.Properties.FirstOrDefault(x=>x.Name.Equals(_owner.DefaultPropertyName, StringComparison.InvariantCultureIgnoreCase));
            }

            if (propertySpec != null)
                return new PropertySpecDescriptor(propertySpec, _owner, propertySpec.Name, null);
            else
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

            foreach (PropertySpec property in _owner.Properties)
            {
                ArrayList attrs = new ArrayList();

                // If a category, description, editor, or type converter are specified
                // in the PropertySpec, create attributes to define that relationship.
                if (property.Category != null)
                    attrs.Add(new CategoryAttribute(property.Category));

                if (property.Description != null)
                    attrs.Add(new DescriptionAttribute(property.Description));

                if (property.EditorTypeName != null)
                    attrs.Add(new EditorAttribute(property.EditorTypeName, typeof(UITypeEditor)));

                if (property.ConverterType != null)
                    attrs.Add(new TypeConverterAttribute(property.ConverterType.FullName));

                // Additionally, append the custom attributes associated with the
                // PropertySpec, if any.
                if (property.Attributes != null) attrs.AddRange(property.Attributes);

                Attribute[] attrArray = (Attribute[])attrs.ToArray(typeof(Attribute));

                // Create a new property descriptor for the property item, and add
                // it to the list.
                PropertySpecDescriptor pd = new PropertySpecDescriptor(property, _owner, property.Name, attrArray);
                props.Add(pd);
            }

            // Convert the list of PropertyDescriptors to a collection that the
            // ICustomTypeDescriptor can use, and return it.
            PropertyDescriptor[] propArray = (PropertyDescriptor[])props.ToArray(typeof(PropertyDescriptor));

            return new PropertyDescriptorCollection(propArray);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return _owner;
        }
        #endregion

        public class PropertySpecDescriptor : PropertyDescriptor
        {
            private PropertyBag bag;
            private PropertySpec item;

            public PropertySpecDescriptor(PropertySpec item, PropertyBag bag, string name, Attribute[] attrs)
                :
                base(name, attrs)
            {
                this.bag = bag;
                this.item = item;
            }

            public string[] EnumValues
            {
                get { return item.EnumValues; }
            }

            public override Type ComponentType
            {
                get { return item.GetType(); }
            }

            public override bool IsReadOnly
            {
                get { return (Attributes.Matches(ReadOnlyAttribute.Yes)); }
            }

            public override Type PropertyType
            {
                get { return item.DataType; }
            }

            public override bool CanResetValue(object component)
            {
                if (item.DefaultValue == null)
                    return false;
                else
                    return !GetValue(component).Equals(item.DefaultValue);
            }

            public override object GetValue(object component)
            {
                // Have the property bag raise an event to get the current value
                // of the property.

                PropertySpecEventArgs e = new PropertySpecEventArgs(item, null);
                bag.OnGetValue(e);
                return e.Value;
            }

            public override void ResetValue(object component)
            {
                SetValue(component, item.DefaultValue);
            }

            public override void SetValue(object component, object value)
            {
                // Have the property bag raise an event to set the current value
                // of the property.

                PropertySpecEventArgs e = new PropertySpecEventArgs(item, value);
                bag.OnSetValue(e);
            }

            public override bool ShouldSerializeValue(object component)
            {
                object val = GetValue(component);

                if (item.DefaultValue == null && val == null)
                    return false;
                else
                    return !val.Equals(item.DefaultValue);
            }
        }


    }
}
