//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading;

//namespace X.Editor.Model2
//{

//    public static class Safer
//    {
//        public static EventHandler<T> MakeSafe<T>(this EventHandler<T> handler)
//        {
//            var context = SynchronizationContext.Current;
//            EventHandler<T> res = (s, a) => context.Post(st => handler(s, a), null);
//            return res;
//        }
//    }

//    public class SafeEvent2<T> where T : EventArgs
//    {
//        event EventHandler<T> _internal;

//        public event EventHandler<T> Happened
//        {
//            add
//            {
//                var context = SynchronizationContext.Current;
//                _internal += (s, a) => InvokeDelegate(context, value, s, a);
//            }
//            remove
//            {
//                var context = SynchronizationContext.Current;
//                _internal -= (s, a) => InvokeDelegate(context, value, s, a);
//            }
//        }

//        void InvokeDelegate(SynchronizationContext context, MulticastDelegate @delegate, object sender, T args)
//        {
//            context.Post(state => @delegate.DynamicInvoke(sender, args), null);
//        }


//        public void Raise(object sender, T eventArgs)
//        {
//            if (_internal != null)
//            {
//                _internal(sender, eventArgs);
//            }
//        }
//    }
//}
