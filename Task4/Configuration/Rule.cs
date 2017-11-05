using System.Configuration;

namespace Task4.Configuration
{
    public class Rule : ConfigurationElement
    {
        [ConfigurationProperty("filter", IsKey = true)]
        public string Filter => (string)base["filter"];

        [ConfigurationProperty("dest")]
        public string DestPath => (string)base["dest"];
    }
}