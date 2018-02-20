using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Helpers;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Model
{
    public partial class HierarchyNode
    {
        public Property CreateBoundedPropertyFor<T, U>(T targetObject, Expression<Func<T, U>> expression, string name = null, bool isReadonly = false, string description = null, string category = null)
        {
            var member = (PropertyInfo)(expression.Body as MemberExpression).Member;
            Func<object> onGet = () => targetObject.Hype().GetValue(member.Name);
            Action<object> onSet = isReadonly ? (Action<object>)null : v => targetObject.Hype().SetValue(member.Name, v);
            name = name ?? member.Name;
            var property = Properties.Add(new Property { Name = name, DataType = member.PropertyType, OnGet = onGet, OnSet = onSet , Category = category });
            if (isReadonly)
            {
                property.Attributes.Add(new ReadOnlyAttribute(true));
            }
            return property;
        }
    }

    public partial class HierarchyNode<T>
    {
        public Property AddProperty<U>(Expression<Func<T, U>> expression, string name = null, string description = null, string category = null)
        {
            return CreateBoundedPropertyFor(Tag, expression, name, false, description, category);
        }
        public Property AddReadOnlyProperty<U>(Expression<Func<T, U>> expression, string name = null, string description = null, string category = null)
        {
            return CreateBoundedPropertyFor(Tag, expression, name, true, description, category);
        }
    }

    public class PropertiesList : NotifyingList<Property>
    {

    }

    public class Property
    {
        public Property()
        {
            Attributes = new List<Attribute>();
        }
        public Func<object> OnGet { get; set; }
        public Action<object> OnSet { get; set; }

        public Func<HierarchyNode, IEnumerable<string>> PossibleValuesDelegate { get; set; }

        public event EventHandler<EventArgs> PropertyChanged = (s, a) => { };
        internal object GetPropertyValue()
        {
            return (OnGet != null) ? OnGet() : null;
        }
        internal void SetPropertyValue(object value)
        {
            if (OnSet != null)
            {
                OnSet(value);
                PropertyChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a collection of additional Attributes for this property.  This can
        /// be used to specify attributes beyond those supported intrinsically by the
        /// Property class, such as ReadOnly and Browsable.
        /// </summary>
        public List<Attribute> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the category name of this property.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the type converter type for this property.
        /// </summary>
        /// 
        Type _converterType;
        public Type ConverterType
        {
            get
            {
                if (PossibleValuesDelegate != null) return typeof(X.Editor.Model.Converters.EnumConverter);
                return _converterType;
            }
            set { _converterType = value; }
        }

        /// <summary>
        /// Gets or sets the default value of this property.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the help text description of this property.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified name of the editor type for
        /// this property.
        /// </summary>
        public string AssemblyQualifiedNameOfEditorType { get; set; }

        /// <summary>
        /// Gets or sets the name of this property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of this property.
        /// </summary>
        public Type DataType { get; set; }
    }
}
