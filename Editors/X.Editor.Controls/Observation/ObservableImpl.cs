using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Controls.Observation
{
    public class ObserverImpl<T> : IObserver<T>
    {
        IDisposable unsubscriber;
        public virtual void OnCompleted() { }
        public virtual void OnError(Exception error) { }
        public virtual void OnNext(T value) { }
        public virtual void Subscribe(IObservable<T> provider)
        {
            unsubscriber = provider.Subscribe(this);
        }
    }
    public class ObservableImpl<T> : IObservable<T>
    {
        List<IObserver<T>> observers;

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!observers.Contains(observer)) observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        class Unsubscriber : IDisposable
        {
            List<IObserver<T>> _observers;
            IObserver<T> _observer;

            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (!(_observer == null)) _observers.Remove(_observer);
            }
        }
    }
}
