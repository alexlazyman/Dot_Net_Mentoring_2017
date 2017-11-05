using System.Configuration;

namespace Task4.Configuration
{
    public class Watcher : ConfigurationElement
    {
        [ConfigurationProperty("path", IsKey = true)]
        public string Path => (string)base["path"];
    }
}