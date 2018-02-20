using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SchemaBuilder.Model
{
    public interface IAutoInstanciate { }

    [AttributeUsage(AttributeTargets.Property)]
    public class XDoNotInstanciateAttribute : System.Attribute { }

    public delegate object XObjectMethodMissingDelegate(XObject instance, string memberName, params object[] args);

    [Serializable]
    [DataContract]
    public class XObject : DynamicObject, INotifyPropertyChanged, IHaveXRules, IAutoInstanciate
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NonSerialized]
        bool _isDirty;

        [NonSerialized]
        Hashtable dynamicProperties;

        [System.Xml.Serialization.XmlIgnore]
        public XObjectMethodMissingDelegate MissingMethodHandler { get; set; }

        public XObject()
        {
            dynamicProperties = new Hashtable();

            DynamicsCache
                .GetPropertiesAccessors(GetType())
                .Where(axx => axx.PropertyName != "Item")
                .ForEach(axx =>
                {
                    var val = GetDefaultPropertyValue(axx);
                    dynamicProperties[axx.Property.Name] = val;
                    axx.Setter.Invoke(this, val);
                });
        }

        protected virtual object GetDefaultPropertyValue(DynamicsCache.PropertyAccessor accessor)
        {
            if (accessor.Property.PropertyType.IsConcrete()
                && accessor.Property.PropertyType.Match(typeof(IAutoInstanciate))
                && !accessor.Property.GetCustomAttributes(typeof(XDoNotInstanciateAttribute), true).Any())
            {
                var value = accessor.Property.PropertyType.Hype().GetOne();
                return value;
            }
            return accessor.Property.PropertyType.Hype().GetDefaultValue();
        }

        void NotifyPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }
        public bool HasKey(string key)
        {
            return dynamicProperties.ContainsKey(key);
        }

        protected virtual internal void InternalSet(string propName, object value)
        {
            object actualValue = null;
            bool valueExists = true;
            try
            {
                // check if we have an existing value present already
                actualValue = InternalGet(propName, true);
            }
            catch (MissingMemberException)
            {
                // we don't have any actual value
                valueExists = false;
            }

            if (!valueExists || (value != actualValue))
            {
                dynamicProperties[propName] = value;
                _isDirty = true;
                NotifyPropertyChanged(propName);
            }
        }
        protected virtual internal object InternalGet(string propName, bool avoidMissingHandler = false)
        {
            if (dynamicProperties.Contains(propName))
            {
                return dynamicProperties[propName];
            }

            if (MissingMethodHandler != null && !avoidMissingHandler)
            {
                var handler = MissingMethodHandler;
                return handler(this, propName);
            }
            throw new MissingMemberException(propName);
        }

        internal object InternalInvoke(string memberName, params object[] args)
        {
            if (dynamicProperties.Contains(memberName))
            {
                var propertyValue = dynamicProperties[memberName];
                if (propertyValue is Delegate) // Case of scripting engine being fooled by a call like: x.Foo() where Foo is a property containing a delegate
                {
                    var dele = propertyValue as Delegate;
                    return dele.DynamicInvoke(args);
                }
            }

            if (memberName.StartsWith("set_"))
            {
                InternalSet(memberName.Replace("set_", ""), args[0]);
                return null;
            }

            if (memberName.StartsWith("get_"))
            {
                return InternalGet(memberName.Replace("get_", ""));
            }

            if (memberName == ("GetMetaObject")) return base.GetMetaObject((System.Linq.Expressions.Expression)args[0]);

            if (MissingMethodHandler != null)
            {
                var handler = MissingMethodHandler;
                return handler(this, memberName, args);
            }
            throw new MissingMethodException(memberName);
        }
        public object this[string key]
        {
            get
            {
                return InternalGet(key);
            }
            set
            {
                InternalSet(key, value);
            }
        }
        public T Get<T>(Expression<Func<T>> expression)
        {
            return (T)InternalGet(expression.ToPropertyName<T>());
        }
        public void Set<T>(Expression<Func<T>> expression, T value)
        {
            InternalSet(expression.ToPropertyName<T>(), value);
        }

        #region implementation of IDynamicMetaObjectProvider
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            InternalSet(binder.Name, value);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            try
            {
                result = InternalGet(binder.Name);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            try
            {
                result = InternalInvoke(binder.Name, args);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        public virtual IEnumerable<XRule> GetRules()
        {
            return new XRule[]{};
        }

        public bool IsDirty()
        {
            return _isDirty;
        }
    }
}
