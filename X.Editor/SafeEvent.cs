using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace X.Editor
{

    public static class SafeEvent
    {
        public static void SubscribeOnUIThread<T>(this EventHandler<T> @event, EventHandler<T> handler)
        {
            var context = SynchronizationContext.Current;
            @event += (s, a) => { context.Post(state => handler(s, a), null); };

            // EventHandler<T> newHandler = (s, a) => { context.Post(state => handler(s, a), null); };
            // Delegate.Combine(@event, newHandler);
        }
        public static void SubscribeOnUIThread<T>(MulticastDelegate @event, EventHandler<T> handler)
        {
            var context = SynchronizationContext.Current;
          //  @event += (s, a) => { context.Post(state => handler(s, a), null); };

            // EventHandler<T> newHandler = (s, a) => { context.Post(state => handler(s, a), null); };
            // Delegate.Combine(@event, newHandler);
        }
    }
}
