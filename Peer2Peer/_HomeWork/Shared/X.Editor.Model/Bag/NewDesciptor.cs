//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.Design;
//using System.Drawing.Design;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace X.Configuration.Model.Bag
//{
//    [TypeConverter(typeof(PropertySorter))]
//    public class HierarchyNodeDescriptor : IComponent, ICustomTypeDescriptor
//    {
//        [Browsable(false)]
//        public ISite Site { get { return new DesignerVerbsSite(this._targetNode); } set { } }

//        public event EventHandler Disposed = (a, s) => { };

//        HierarchyNode _targetNode;
//        public HierarchyNodeDescriptor(HierarchyNode target)
//        {
//            _targetNode = target;
//        }


//        public void Dispose() { }

//        class DesignerVerbsSite : ISite, IMenuCommandService
//        {
//            HierarchyNode _targetNode;
//            public DesignerVerbsSite(HierarchyNode taget)
//            {
//                _targetNode = taget;
//            }

//            public IComponent Component
//            {
//                get { return _targetNode; }
//            }

//            public IContainer Container
//            {
//                get { return null; }
//            }

//            public bool DesignMode
//            {
//                get { return true; }
//            }

//            public string Name
//            {
//                get
//                {
//                    return "sad";
//                }
//                set
//                {
//                    throw new NotImplementedException();
//                }
//            }
//            public object GetService(Type serviceType)
//            {
//                if (serviceType == typeof(IMenuCommandService))
//                    return this;
//                return null;
//            }

//            public void AddCommand(MenuCommand command)
//            {
//                throw new NotImplementedException();
//            }

//            public void AddVerb(DesignerVerb verb)
//            {
//                throw new NotImplementedException();
//            }

//            public MenuCommand FindCommand(CommandID commandID)
//            {
//                throw new NotImplementedException();
//            }

//            public bool GlobalInvoke(CommandID commandID)
//            {
//                throw new NotImplementedException();
//            }

//            public void RemoveCommand(MenuCommand command)
//            {
//                throw new NotImplementedException();
//            }

//            public void RemoveVerb(DesignerVerb verb)
//            {
//                throw new NotImplementedException();
//            }

//            public void ShowContextMenu(CommandID menuID, int x, int y)
//            {
//                throw new NotImplementedException();
//            }

//            public DesignerVerbCollection Verbs
//            {
//                get
//                {
//                    DesignerVerbCollection Verbs = new DesignerVerbCollection();
//                    foreach(var command in _targetNode.NodeDataAdapter.GetCommands())
//                    {
//                        Verbs.Add(new DesignerVerb(command.Text, (snd, args) => command.Invoke()));   
//                    }
//                    return Verbs;
//                }
//            }
//        }

//        class PropertySpecDescriptor : PropertyDescriptor
//        {
//            HierarchyNodeDescriptor _descriptor;
//            Property _spec;


//            public PropertySpecDescriptor(HierarchyNodeDescriptor descriptor, Property spec)
//                : base(spec.Name, spec.Attributes.ToArray())
//            {
//                _descriptor = descriptor;
//                this._spec = spec;
//            }

//            public override Type ComponentType
//            {
//                get { return _descriptor._targetNode.GetType(); }// get { return typeof(T); } // Item.GetType(); }

//                //get { return typeof(HierarchyNodeDescriptor); }// get { return typeof(T); } // Item.GetType(); }
//            }

//            public override bool IsReadOnly
//            {
//                get { return (Attributes.Matches(ReadOnlyAttribute.Yes)); }
//            }

//            public override Type PropertyType
//            {
//                get { return _spec.DataType; }
//            }

//            public override bool CanResetValue(object component)
//            {
//                if (_spec.DefaultValue == null)
//                    return false;
//                else
//                    return !GetValue(component).Equals(_spec.DefaultValue);
//            }

//            public override object GetValue(object component)
//            {
//                return _spec.OnGet();
//                //return Getter(_descriptor._target);
//            }

//            public override void ResetValue(object component)
//            {
//                SetValue(component, _spec.DefaultValue);
//            }

//            public override void SetValue(object component, object value)
//            {
//                _spec.OnSet(value);
//                //if (Setter != null) Setter(_descriptor._target, value);
//            }

//            public override bool ShouldSerializeValue(object component)
//            {
//                object val = GetValue(component);

//                if (_spec.DefaultValue == null && val == null) return false;
//                if (val == null) return true;
//                else
//                    return !val.Equals(_spec.DefaultValue);
//            }
//        }

//        //ICustomTypeDescriptor explicit interface definitions
//        // Most of the functions required by the ICustomTypeDescriptor are
//        // merely pssed on to the default TypeDescriptor for this type,
//        // which will do something appropriate.  The exceptions are noted
//        // below.
//        AttributeCollection ICustomTypeDescriptor.GetAttributes()
//        {
//            return TypeDescriptor.GetAttributes(this, true);
//        }

//        string ICustomTypeDescriptor.GetClassName()
//        {
//            return TypeDescriptor.GetClassName(this, true);
//        }

//        string ICustomTypeDescriptor.GetComponentName()
//        {
//            return TypeDescriptor.GetComponentName(this, true);
//        }

//        TypeConverter ICustomTypeDescriptor.GetConverter()
//        {
//            return TypeDescriptor.GetConverter(this, true);
//        }

//        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
//        {
//            return TypeDescriptor.GetDefaultEvent(this, true);
//        }

//        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
//        {
//            //This function searches the property list for the property
//            //with the same name as the DefaultProperty specified, and
//            //returns a property descriptor for it.  If no property is
//            //found that matches DefaultProperty, a null reference is
//            //returned instead.

//            //PropertySpec propertySpec = _target.Properties.FirstOrDefault(x=>x.Name == _spec.DefaultValue);
//            //if (_spec.DefaultProperty != null)
//            //{
//            //    int index = properties.IndexOf(defaultProperty);
//            //    propertySpec = properties[index];
//            //}

//            //if (propertySpec != null)
//            //    return new PropertySpecDescriptor(propertySpec, this, propertySpec.Name, null);
//            //else
//            return null;
//        }

//        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
//        {
//            return TypeDescriptor.GetEditor(this, editorBaseType, true);
//        }

//        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
//        {
//            return TypeDescriptor.GetEvents(this, true);
//        }

//        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
//        {
//            return TypeDescriptor.GetEvents(this, attributes, true);
//        }

//        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
//        {
//            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
//        }

//        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
//        {
//            List<PropertyDescriptor> Properties = new List<PropertyDescriptor>();

//            foreach (var spec in _targetNode.NodeDataAdapter.GetProperties())
//            {
//                if (spec.Attributes == null) spec.Attributes = new List<Attribute>();

//                if (spec.Category != null) spec.Attributes.Add(new CategoryAttribute(spec.Category));
//                if (spec.Description != null) spec.Attributes.Add(new DescriptionAttribute(spec.Description));
//                if (spec.EditorTypeName != null) spec.Attributes.Add(new EditorAttribute(spec.EditorTypeName, typeof(UITypeEditor)));
//                if (spec.ConverterType != null) spec.Attributes.Add(new TypeConverterAttribute(spec.ConverterType));

//                spec.Attributes.Add(new PropertyOrderAttribute(Properties.Count));

//                var desc = new PropertySpecDescriptor(this, spec);
//                Properties.Add(desc);

//            }

//            return new PropertyDescriptorCollection(Properties.ToArray());
//        }

//        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
//        {
//        //    return this;
//            return _targetNode;
//        }
//    }

//}
