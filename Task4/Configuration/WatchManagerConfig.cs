using System.Configuration;

namespace Task4.Configuration
{
    public class WatchManagerConfig : ConfigurationSection
    {
        [ConfigurationProperty("copy", DefaultValue = CopyOptions.None)]
        public CopyOptions CopyOptions => (CopyOptions)this["copy"];

        [ConfigurationProperty("dateTimeFormat", DefaultValue = "HH_mm_ss")]
        public string DateTimeFormat => (string)this["dateTimeFormat"];

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