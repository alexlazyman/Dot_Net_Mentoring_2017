using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace Task4.Configuration
{
    public class WatcherCollection : ConfigurationElementCollection, IEnumerable<Watcher>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Watcher();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Watcher)element).Path;
        }

        public IEnumerator<Watcher> GetEnumerator()
        {
            foreach (var watcher in (IEnumerable)this)
            {
                yield return (Watcher)watcher;
            }
        }
    }
}