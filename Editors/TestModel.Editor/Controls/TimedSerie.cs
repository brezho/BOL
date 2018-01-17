using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestModel.Editor.Controls
{
    public class TimedSerie<T> : IPublisher<T>
    {
        T Last = default(T);
        private IPublisher<T> _valuesSubscription;

        public event EventHandler<T> OnNext;

        public IPublisher<T> ValuesSource
        {
            set
            {
                if (_valuesSubscription != null) _valuesSubscription.OnNext -= _valuesSubscription_OnNext;
                _valuesSubscription = value;
                _valuesSubscription.OnNext += _valuesSubscription_OnNext;
            }
        }

        private void _valuesSubscription_OnNext(object sender, T e)
        {
            Last = e;
        }
        public TimedSerie(int frequency = 50) : this(TimeSpan.FromMilliseconds(1000 / frequency)) { }

        public TimedSerie(TimeSpan reportingInterval)
        {
            Task.Factory.StartNew(() =>
            {
                var sw = new Stopwatch();

                while (true)
                {
                    if (OnNext != null) OnNext(this, Last);
                    var sleepTime = (int)reportingInterval.TotalMilliseconds - (int)sw.ElapsedMilliseconds;
                    if (sleepTime > 0) Thread.Sleep(sleepTime);
                    sw.Reset();
                }
            });
        }
    }
}
