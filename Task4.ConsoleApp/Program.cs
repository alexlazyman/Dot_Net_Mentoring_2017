using System;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Threading;
using Task4.Configuration;

namespace Task4.ConsoleApp
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                var culture = new CultureInfo(ConfigurationManager.AppSettings["Culture"]);

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }
            catch
            {
                // Ignored
            }


            var config = WatchManagerConfig.GetConfiguration();
            var logger = new ConsoleLogger();

            IWatchManager watchManager = new WatchManager(config, logger);

            watchManager.EnableWatching();

            while (true)
            {
                Console.ReadKey();
            }
        }
    }
}
