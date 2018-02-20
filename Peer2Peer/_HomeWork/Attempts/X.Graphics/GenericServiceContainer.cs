using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace X.Graphics
{
    class GenericServiceContainer : ServiceContainer
    {
        public void AddService<T>(T service) where T : class
        {
            base.AddService(typeof(T), service);
        }

        public T GetService<T>() where T : class
        {
            return (T)base.GetService(typeof(T));
        }
    }
}
