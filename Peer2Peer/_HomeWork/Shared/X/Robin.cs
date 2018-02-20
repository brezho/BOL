using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace X
{
    public interface IRobinObject : INotifyPropertyChanged
    {
        bool GetProperty(string propertyId, out object value);
        bool SetProperty(string propertyId, ref object value);
    }

    public class Robin : IRobinObject
    {
        bool IRobinObject.GetProperty(string propertyId, out object value)
        {
            if (!properties.ContainsKey(propertyId))
            {
                value = null;
                return false;
            }
            value = properties[propertyId];
            return true;
        }

        bool IRobinObject.SetProperty(string propertyId, ref object value)
        {
            object actualValue = null;
            var propExists = ((IRobinObject)this).GetProperty(propertyId, out actualValue);
            if (!propExists || !actualValue.Equals(value))
            {
                properties[propertyId] = value;
                NotifyPropertyChanged(propertyId);
                return true;
            }
            return false;
        }

        event PropertyChangedEventHandler _handler;
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { _handler += value; }
            remove { _handler -= value; }
        }
        Dictionary<string, object> properties;

        public Robin()
        {
            properties = new Dictionary<string, object>();
        }

        void NotifyPropertyChanged(string name)
        {
            var handler = _handler;
            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }

        public T As<T>()
        {
            return new Interceptor<T>(this, (object decorated, MethodBase method, object[] parameters) =>
            {
                var rob = decorated as IRobinObject;

                if (method.Name.StartsWith("get_"))
                {
                    object res = null;
                    rob.GetProperty(method.Name.RemoveStart("get_"), out res);
                    return res;
                }
                if (method.Name.StartsWith("set_")) rob.SetProperty(method.Name.RemoveStart("set_"), ref parameters[0]);
                return null;
            }).Proxy;
        }
    }
}
