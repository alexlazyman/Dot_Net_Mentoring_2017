using System.Configuration;

namespace Task4.Configuration
{
    public class WatchManagerConfig : ConfigurationSection
    {
        [ConfigurationProperty("watchers")]
        public WatcherCollection Watchers => (WatcherCollection)this["watchers"];

        [ConfigurationProperty("rules")]
        public RuleCollection Rules => (RuleCollection)this["rules"];

        public static WatchManagerConfig GetConfiguration()
        {
            return (WatchManagerConfig)ConfigurationManager.GetSection("watchManager");
        }
    }

}