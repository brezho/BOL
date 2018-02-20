using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace X.Editor.Model2
{

    public class SafeEvent2<T> where T : EventArgs
    {
        event EventHandler<T> _internal;

        public event EventHandler<T> Happened
        {
            add
            {
                var context = SynchronizationContext.Current;
                _internal += (s, a) => InvokeDelegate(context, value, s, a);
            }
            remove
            {
                var context = SynchronizationContext.Current;
                _internal -= (s, a) => InvokeDelegate(context, value, s, a);
            }
        }

        void InvokeDelegate(SynchronizationContext context, MulticastDelegate @delegate, object sender, T args)
        {
            context.Post(state => @delegate.DynamicInvoke(sender, args), null);
        }


        public void Raise(object sender, T eventArgs)
        {
            if (_internal != null)
            {
                _internal(sender, eventArgs);
            }
        }

        //    public object Raise(object sender, T eventArgs)
        //    {
        //        object retVal = null;

        //        if (_internal != null)
        //        {
        //            foreach (Delegate d in _internal.GetInvocationList())
        //            {
        //                var synchronizeInvoke = d.Target as ISynchronizeInvoke;
        //                if ((synchronizeInvoke != null) && synchronizeInvoke.InvokeRequired)
        //                {
        //                    retVal = synchronizeInvoke.EndInvoke(synchronizeInvoke.BeginInvoke(d, new[] { sender, eventArgs }));
        //                }
        //                else
        //                {
        //                    retVal = d.DynamicInvoke(new[] { sender, eventArgs });
        //                }
        //            }
        //        }
        //        return retVal;
        //    }
    }
}
