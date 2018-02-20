using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model.Converters
{
    public class EnumConverter : System.ComponentModel.StringConverter
    {
        public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            return true;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            //var editedNode = context.Instance as HierarchyNode;
            //var propertyDescriptor = context.PropertyDescriptor as PropertySpecDescriptor;

            //propertyDescriptor.SetValue(editedNode, value);
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
        {
            var editedNode = context.Instance as HierarchyNode;
            var propertyDescriptor = context.PropertyDescriptor as PropertySpecDescriptor;

            return new StandardValuesCollection(propertyDescriptor.PossibleValues);
        }

        public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context)
        {
            return true;
        }

    }
}
