using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    public static class EventHandlerExtensions
    {
        public static void Run<T>(this EventHandler<T> eventHandler, object sender, T args) where T : EventArgs
        {
            if (eventHandler == null)
            {
                return;
            }

            var handlers = eventHandler.GetInvocationList();
            foreach (EventHandler<T> handler in handlers)
            {
                handler.Invoke(sender, args);
            }
        }

        public static void Run(this EventHandler eventHandler, object sender, EventArgs args)
        {
            if (eventHandler == null)
            {
                return;
            }

            var handlers = eventHandler.GetInvocationList();
            foreach (EventHandler handler in handlers)
            {
                handler.Invoke(sender, args);
            }
        }
    }
}
