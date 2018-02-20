
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchemaBuilder.Model
{
    public class ItemEventArgs<T>
    {
        T _item;
        public T Item { get { return _item; } }
        public ItemEventArgs(T item) { _item = item; }
    }

    public interface IXNotifyAdd<T>
    {
        event EventHandler<ItemEventArgs<T>> ItemAdded;
    }

    public interface IXNotify<T> : IXNotifyAdd<T>
    {
        event EventHandler<ItemEventArgs<T>> ItemRemoved;
    }

    public interface IXList<T> : IList<T>, IXNotify<T>
    {

    }

    public class XNotificationSource<T> : IXNotifyAdd<T>, IAutoInstanciate
    {
        public event EventHandler<ItemEventArgs<T>> ItemAdded = (s, a) => { };

        public virtual void Notify(T item)
        {
            ItemAdded(this, new ItemEventArgs<T>(item));
        }
    }

    public class XList<T> : ObservableCollection<T>, IXList<T>, IAutoInstanciate
    {
        public event EventHandler<ItemEventArgs<T>> ItemAdded = (s, a) => { };
        public event EventHandler<ItemEventArgs<T>> ItemRemoved = (s, a) => { };
        public XList()
        {
            CollectionChanged += (s, a) =>
            {
                switch (a.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        foreach (var it in a.NewItems)
                        {
                            ItemAdded(this, new ItemEventArgs<T>((T)it));
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        foreach (var it in a.OldItems)
                        {
                            ItemRemoved(this, new ItemEventArgs<T>((T)it));
                        }
                        break;
                }
            };
        }
    }
}

