using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace Task4.Configuration
{
    public class RuleCollection : ConfigurationElementCollection, IEnumerable<Rule>
    {
        [ConfigurationProperty("defaultDest")]
        public string DefaultDestPath => (string)base["defaultDest"];

        protected override ConfigurationElement CreateNewElement()
        {
            return new Rule();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Rule)element).Filter;
        }

        public IEnumerator<Rule> GetEnumerator()
        {
            foreach (var rule in (IEnumerable)this)
            {
                yield return (Rule)rule;
            }
        }
    }
}