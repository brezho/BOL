//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace X.DataModel
//{
//    interface ILocalProp { }

//    public class MyObject : INotifyPropertyChanged
//    {
//        public MyObject()
//        {
//            DynamicsCache
//            .GetPropertiesAccessors(this.GetType())
//            .ForEach(axx =>
//            {
//                var val = GetDefaultPropertyValue(axx);
//                if (val != null && axx.Setter != null) axx.Setter.Invoke(this, val);
//            });
//            DynamicsCache
//            .GetFieldsAccessors(this.GetType())
//            .ForEach(axx =>
//            {
//                var val = GetDefaultFieldValue(axx);
//                if (val != null && axx.Setter != null) axx.Setter.Invoke(this, val);
//            });
//        }
//        protected virtual object GetDefaultPropertyValue(DynamicsCache.PropertyAccessor accessor)
//        {
//            if (accessor.Property.PropertyType.IsConcrete()
//                && accessor.Property.PropertyType.Match(typeof(IAutoInstanciate))
//                && !accessor.Property.GetCustomAttributes(typeof(XDoNotInstanciateAttribute), true).Any())
//            {
//                var value = accessor.Property.PropertyType.GetOne();
//                return value;
//            }
//            if (accessor.Property.PropertyType.IsConcrete()
//                && accessor.Property.PropertyType.Match(typeof(ILocalProp)))
//            {
//                var value = accessor.Property.PropertyType.GetOne(this);
//                return value;
//            }

//            return accessor.Property.PropertyType.GetDefault();

//        }
//        protected virtual object GetDefaultFieldValue(DynamicsCache.FieldAccessor accessor)
//        {
//            if (accessor.Field.FieldType.IsConcrete()
//                && accessor.Field.FieldType.Match(typeof(IAutoInstanciate))
//                && !accessor.Field.GetCustomAttributes(typeof(XDoNotInstanciateAttribute), true).Any())
//            {
//                var value = accessor.Field.FieldType.GetOne();
//                return value;
//            }
//            if (accessor.Field.FieldType.IsConcrete()
//                && accessor.Field.FieldType.Match(typeof(ILocalProp)))
//            {
//                var value = accessor.Field.FieldType.GetOne(this);
//                return value;
//            }
//            return accessor.Field.FieldType.GetDefault();

//        }

//        public event PropertyChangedEventHandler PropertyChanged;
//    }

//    public abstract class XValueType<T> : ILocalProp
//    {
//        MyObject _owner;
//        public XValueType(MyObject owner)
//        {
//            _owner = owner;
//        }

//        T Value { get; set; }
//        public void Set(T val)
//        {
//            Value = val;
//        }
//        public T Get()
//        {
//            return Value;
//        }
//    }

//    public class XString : XValueType<string>
//    {
//        XString(MyObject owner) : base(owner) { }

//    }

//    public class XInt : XValueType<int>
//    {
//        XInt(MyObject owner) : base(owner) { }
//    }
//}
